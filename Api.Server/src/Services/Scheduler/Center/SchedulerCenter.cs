// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using System.Text;
using Fast.Admin.Enum;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl.Calendar;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;

namespace Fast.Scheduler;

/// <summary>
/// <see cref="SchedulerCenter"/> 调度中心
/// </summary>
public class SchedulerCenter : ISchedulerCenter, ISingletonDependency
{
    /// <summary>
    /// <see cref="IServiceProvider"/> 服务提供者
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// <see cref="IDependencySchedulerFactory"/> DI注入调度器工厂
    /// </summary>
    private readonly IDependencySchedulerFactory _schedulerFactory;

    /// <summary>
    /// <see cref="ILogger"/> 日志
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// <see cref="SchedulerCenter"/> 调度中心
    /// </summary>
    /// <param name="serviceProvider"><see cref="IServiceProvider"/> 服务提供者</param>
    /// <param name="schedulerFactory"><see cref="IDependencySchedulerFactory"/> DI注入调度器工厂</param>
    /// <param name="logger"><see cref="ILogger"/> 日志</param>
    public SchedulerCenter(IServiceProvider serviceProvider, IDependencySchedulerFactory schedulerFactory,
        ILogger<ISchedulerCenter> logger)
    {
        _serviceProvider = serviceProvider;
        _schedulerFactory = schedulerFactory;
        _logger = logger;
    }

    /// <summary>
    /// 异步线程锁
    /// </summary>
    private static readonly SemaphoreSlim semaphoreSlim = new(1, 1);

    /// <summary>
    /// 调度作业属性验证
    /// </summary>
    /// <param name="jobInfo"></param>
    private void SchedulerJobVerify(SchedulerJobInfo jobInfo)
    {
        if (string.IsNullOrWhiteSpace(jobInfo.JobName))
        {
            throw new UserFriendlyException("调度作业名称不能为空！");
        }

        // 结束时间不能小于开始时间
        if (jobInfo.EndTime != null && jobInfo.EndTime <= jobInfo.BeginTime)
        {
            throw new UserFriendlyException("结束时间不能小于等于开始时间！");
        }

        // 触发器类型判断
        switch (jobInfo.TriggerType)
        {
            case TriggerTypeEnum.Cron:
                // Cron 默认 星期，每天开始时间，每天结束时间，执行间隔时间，执行次数 为空
                jobInfo.Week = null;
                jobInfo.DailyStartTime = null;
                jobInfo.DailyEndTime = null;
                jobInfo.IntervalSecond = null;
                jobInfo.RunTimes = null;

                if (string.IsNullOrWhiteSpace(jobInfo.Cron))
                {
                    throw new UserFriendlyException("Cron表达式不能为空！");
                }

                // 验证表达式是否正确
                if (!CronExpression.IsValidExpression(jobInfo.Cron))
                {
                    throw new UserFriendlyException("Cron表达式不正确！");
                }

                if (jobInfo.Cron == "* * * * * ?")
                {
                    throw new UserFriendlyException("不允许频繁执行调度作业！");
                }

                var cronTrigger = new CronTriggerImpl("TestName", "TestGroup", jobInfo.Cron);
                var calendar = new BaseCalendar(TimeZoneInfo.Local);
                // 获取两条就好
                var list = TriggerUtils.ComputeFireTimes(cronTrigger, calendar, 2);
                var diffTime = list[1] - list[0];
                if (diffTime.TotalSeconds < 30)
                {
                    throw new UserFriendlyException("不允许低于30秒内循环执行调度作业！");
                }

                break;
            case TriggerTypeEnum.Daily:
                // Daily 默认Cron为空
                jobInfo.Cron = null;

                if (jobInfo.DailyStartTime == null)
                {
                    throw new UserFriendlyException("每天开始时间不能为空！");
                }

                if (jobInfo.DailyEndTime == null)
                {
                    throw new UserFriendlyException("每天结束时间不能为空！");
                }

                if (jobInfo.DailyEndTime <= jobInfo.DailyStartTime)
                {
                    throw new UserFriendlyException("每天结束时间不能小于等于开始时间！");
                }

                if (jobInfo.Week == null || jobInfo.Week == WeekEnum.None)
                {
                    throw new UserFriendlyException("具体星期几不能为空！");
                }

                if (jobInfo.IntervalSecond == null)
                {
                    throw new UserFriendlyException("执行间隔时间不能为空！");
                }

                // 判断执行间隔时间，不能低于30秒
                if (jobInfo.IntervalSecond < 30)
                {
                    throw new UserFriendlyException("不允许低于30秒内循环执行调度作业！");
                }

                break;
            case TriggerTypeEnum.Simple:
                // Simple 默认 Cron，星期，每天开始时间，每天结束时间 为空
                jobInfo.Cron = null;
                jobInfo.Week = null;
                jobInfo.DailyStartTime = null;
                jobInfo.DailyEndTime = null;

                if (jobInfo.IntervalSecond == null)
                {
                    throw new UserFriendlyException("执行间隔时间不能为空！");
                }

                // 判断执行间隔时间，不能低于30秒
                if (jobInfo.IntervalSecond < 30)
                {
                    throw new UserFriendlyException("不允许低于30秒内循环执行调度作业！");
                }

                break;
            default:
                throw new UserFriendlyException("不支持的触发器类型！");
        }

        // 调度作业类型判断
        switch (jobInfo.JobType)
        {
            case SchedulerJobTypeEnum.Local:
                break;
            case SchedulerJobTypeEnum.IntranetUrl:
            case SchedulerJobTypeEnum.OuterNetUrl:
                if (string.IsNullOrWhiteSpace(jobInfo.RequestUrl))
                {
                    throw new UserFriendlyException("请求Url不能为空！");
                }

                if (!Uri.TryCreate(jobInfo.RequestUrl, UriKind.Absolute, out _))
                {
                    throw new UserFriendlyException("请求Url不正确！");
                }

                if (jobInfo.RequestMethod == null)
                {
                    throw new UserFriendlyException("请求方式不能为空！");
                }

                break;
            default:
                throw new UserFriendlyException("不支持的调度作业类型！");
        }
    }

    /// <summary>
    /// 添加调度作业
    /// </summary>
    /// <param name="scheduler"></param>
    /// <param name="jobKey"></param>
    /// <param name="jobInfo"></param>
    /// <returns></returns>
    private async Task AddSchedulerJob(IScheduler scheduler, JobKey jobKey, SchedulerJobInfo jobInfo)
    {
        // 作业配置
        IJobConfigurator jobConfigurator;

        // 调度作业类型判断
        switch (jobInfo.JobType)
        {
            case SchedulerJobTypeEnum.Local:
                jobConfigurator = JobBuilder.Create<LocalJob>();

                break;
            case SchedulerJobTypeEnum.IntranetUrl:
            case SchedulerJobTypeEnum.OuterNetUrl:
                jobConfigurator = JobBuilder.Create<UrlJob>();

                break;
            default:
                throw new UserFriendlyException("不支持的调度作业类型！");
        }

        // 作业数据
        var jobDataMap = new JobDataMap(jobInfo.ToDictionary(true));

        // 构建作业
        var jobDetail = jobConfigurator.WithIdentity(jobKey)
            // 作业数据
            .SetJobData(jobDataMap)
            // 描述
            .WithDescription(jobInfo.Description)
            // 持久化，表示该作业没有触发器也能存在
            .StoreDurably()
            // 这里注释，是因为 JobBase 已经增加了特性。
            //// 持久化保留作业
            //.PersistJobDataAfterExecution()
            //// 不允许并发执行
            //.DisallowConcurrentExecution()
            // 请求恢复，当应用发生故障的时候，是否重新执行
            .RequestRecovery()
            .Build();

        // 构建触发器
        var triggerBuilder = TriggerBuilder.Create()
            .WithIdentity(jobKey.Name, jobKey.Group)
            // 开始时间
            .StartAt(jobInfo.BeginTime)
            // 结束时间
            //.EndAt(jobInfo.EndTime)
            // 作业名称
            .ForJob(jobDetail);

        // 触发器类型判断
        switch (jobInfo.TriggerType)
        {
            case TriggerTypeEnum.Cron:
                // 设置 Cron 类型的触发器
                triggerBuilder
                    // 指定 Cron 表达式
                    .WithCronSchedule(jobInfo.Cron, builder =>
                    {
                        builder
                            // 错过立即执行，剩余按计划
                            .WithMisfireHandlingInstructionFireAndProceed();
                    });

                break;
            case TriggerTypeEnum.Daily:
                // 设置 Daily 类型的触发器
                triggerBuilder.WithDailyTimeIntervalSchedule(builder =>
                {
                    builder
                        // 执行日期（星期）
                        .OnDaysOfTheWeek(jobInfo.Week!.Value.ToDayOfWeeks())
                        // 执行开始时间
                        .StartingDailyAt(TimeOfDay.HourMinuteAndSecondOfDay(jobInfo.DailyStartTime!.Value.Hours,
                            jobInfo.DailyStartTime.Value.Minutes, jobInfo.DailyStartTime.Value.Seconds))
                        // 执行结束时间
                        .EndingDailyAt(TimeOfDay.HourMinuteAndSecondOfDay(jobInfo.DailyEndTime!.Value.Hours,
                            jobInfo.DailyEndTime.Value.Minutes, jobInfo.DailyEndTime.Value.Seconds))
                        // 执行时间间隔，单位秒
                        .WithIntervalInSeconds(Math.Abs(jobInfo.IntervalSecond!.Value))
                        // 错过立即执行，剩余按计划
                        .WithMisfireHandlingInstructionFireAndProceed();

                    // 判断执行次数是否存在
                    if (jobInfo.RunTimes is > 0)
                    {
                        builder
                            // 执行次数，默认从0开始
                            .EndingDailyAfterCount(jobInfo.RunTimes.Value);
                    }
                });

                break;
            case TriggerTypeEnum.Simple:
                // 判断执行次数是否存在
                if (jobInfo.RunTimes == null || jobInfo.RunTimes <= 0)
                {
                    // 设置 Simple 类型的触发器（无限循环）
                    triggerBuilder.WithSimpleSchedule(builder =>
                    {
                        builder
                            // 执行时间间隔，单位秒
                            .WithIntervalInSeconds(Math.Abs(jobInfo.IntervalSecond!.Value))
                            // 错过立即执行，重新排计划
                            .WithMisfireHandlingInstructionFireNow()
                            // 无限循环
                            .RepeatForever();
                    });
                }
                else
                {
                    // 设置 Simple 类型的触发器
                    triggerBuilder.WithSimpleSchedule(builder =>
                    {
                        builder
                            // 执行时间间隔，单位秒
                            .WithIntervalInSeconds(Math.Abs(jobInfo.IntervalSecond!.Value))
                            // 错过立即执行，重新排计划
                            .WithMisfireHandlingInstructionFireNow()
                            // 执行次数，默认从0开始
                            .WithRepeatCount(jobInfo.RunTimes.Value);
                    });
                }

                break;
            default:
                throw new UserFriendlyException("不支持的触发器类型！");
        }

        // 触发器
        var trigger = triggerBuilder.Build();

        // 添加作业
        await scheduler.ScheduleJob(jobDetail, trigger);
    }

    /// <summary>
    /// 添加调度作业
    /// </summary>
    /// <param name="jobInfo"></param>
    /// <returns></returns>
    private async Task AddSchedulerJob(SchedulerJobInfo jobInfo)
    {
        // 属性验证
        SchedulerJobVerify(jobInfo);

        // 获取调度器
        var scheduler = await _schedulerFactory.GetScheduler();

        // 获取调度作业Key
        var jobKey = new JobKey(jobInfo.JobName, jobInfo.JobGroup.ToString());

        // 检查作业是否存在
        if (await scheduler.CheckExists(jobKey))
        {
            throw new UserFriendlyException("调度作业已存在！");
        }

        // 添加调度作业
        await AddSchedulerJob(scheduler, jobKey, jobInfo);
    }

    /// <summary>
    /// 添加本地作业
    /// </summary>
    /// <param name="localJob"></param>
    /// <returns></returns>
    private async Task AddLocalJob(SchedulerLocalJobInfo localJob)
    {
        // 获取调度器
        var scheduler = await _schedulerFactory.GetScheduler();

        // 作业Key
        var jobKey = new JobKey(localJob.JobName, localJob.JobGroup.ToString());

        var runNumber = 0L;
        string exception = null;
        List<string> logs = null;

        // 判断调度作业是否存在
        if (await scheduler.CheckExists(jobKey))
        {
            // 获取作业详情
            var jobDetail = await scheduler.GetJobDetail(jobKey);

            // 暂停旧的调度作业
            await scheduler.PauseJob(jobKey);
            // 删除旧的调度作业
            await scheduler.DeleteJob(jobKey);

            runNumber = jobDetail!.JobDataMap.GetLong(nameof(SchedulerJobInfo.RunNumber));
            exception = jobDetail.JobDataMap.GetString(nameof(SchedulerJobInfo.Exception));
            logs = jobDetail.JobDataMap[nameof(SchedulerJobInfo.Logs)] as List<string>;
        }

        // 添加本地作业
        await AddSchedulerJob(new SchedulerJobInfo
        {
            JobName = localJob.JobName,
            JobGroup = localJob.JobGroup,
            JobType = SchedulerJobTypeEnum.Local,
            BeginTime = localJob.BeginTime,
            EndTime = localJob.EndTime,
            TriggerType = localJob.TriggerType,
            Cron = localJob.Cron,
            Week = localJob.Week,
            DailyStartTime = localJob.DailyStartTime,
            DailyEndTime = localJob.DailyEndTime,
            IntervalSecond = localJob.IntervalSecond,
            RunTimes = localJob.RunTimes,
            WarnTime = localJob.WarnTime,
            RetryTimes = localJob.RetryTimes,
            RetryMillisecond = localJob.RetryMillisecond,
            MailMessage = localJob.MailMessage,
            Description = localJob.Description,
            RunNumber = runNumber,
            Exception = exception,
            Logs = logs
        });
    }

    /// <summary>
    /// 初始化调度程序
    /// </summary>
    /// <returns></returns>
    public async Task InitializeScheduler()
    {
        // 获取锁
        await semaphoreSlim.WaitAsync()
            .ConfigureAwait(false);
        try
        {
            if (SchedulerContext.Initialized)
            {
                throw new UserFriendlyException("不能多次调用 'InitializeSchedule' 进行初始化调度程序！");
            }

            // 在初始化的时候，创建所有租户调度器
            var db = new SqlSugarClient(SqlSugarContext.GetConnectionConfig(SqlSugarContext.ConnectionSettings));
            // 加载Aop
            SugarEntityFilter.LoadSugarAop(FastContext.HostEnvironment.IsDevelopment(), db);

            var allLocalJobList = new List<SchedulerLocalJobInfo>();
            var ISchedulerJobType = typeof(ISchedulerJob);
            var schedulerJobTypes = MAppContext.EffectiveTypes.Where(wh =>
                    ISchedulerJobType.IsAssignableFrom(wh) && wh.IsClass && !wh.IsInterface && !wh.IsAbstract)
                .ToList();
            foreach (var schedulerJobType in schedulerJobTypes)
            {
                if (ActivatorUtilities.CreateInstance(_serviceProvider, schedulerJobType) is ISchedulerJob schedulerJobInstance)
                {
                    var localJobEntity = schedulerJobInstance.GetLocalJob();
                    // 放入缓存集合中
                    SchedulerContext.LocalSchedulerJobList.Add(localJobEntity);

                    var jobKey = new JobKey(localJobEntity.JobName, localJobEntity.JobGroup.ToString());

                    // 放入缓存集合中
                    SchedulerContext.LocalSchedulerJobTypes.TryAdd(jobKey.ToString(), schedulerJobType);

                    allLocalJobList.Add(localJobEntity);
                }
            }

            foreach (var localJobEntity in allLocalJobList)
            {
                // 添加本地作业
                await AddLocalJob(localJobEntity);
            }

            // 启动核心调度器
            await StartScheduler();

            // 调度程序初始化完成
            SchedulerContext.Initialized = true;

            _logger.LogInformation("Initialize schedule.");
        }
        finally
        {
            // 释放锁
            semaphoreSlim.Release();
        }
    }

    /// <summary>
    /// 获取调度器详情
    /// </summary>
    /// <returns></returns>
    public async Task<QuerySchedulerDetailOutput> QuerySchedulerDetail()
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var metaData = await scheduler.GetMetaData();

        return new QuerySchedulerDetailOutput
        {
            IsStarted = !metaData.InStandbyMode,
            QuartzVersion = metaData.Version,
            SchedulerStatus = metaData.Shutdown ? "Shutdown (关闭)" :
                metaData.InStandbyMode ? "Standby (待机)" :
                metaData.Started ? "Started (启动)" : "Unknown (未知)",
            SchedulerShutdown = metaData.Shutdown,
            SchedulerInStandbyMode = metaData.InStandbyMode,
            SchedulerStarted = metaData.Started,
            SchedulerInstanceId = metaData.SchedulerInstanceId,
            SchedulerName = metaData.SchedulerName,
            SchedulerRemote = metaData.SchedulerRemote,
            SchedulerType = metaData.SchedulerType.FullName,
            JobStoreType = metaData.JobStoreType.FullName,
            SupportsPersistence = metaData.JobStoreSupportsPersistence,
            Clustered = metaData.JobStoreClustered,
            ThreadPoolSize = metaData.ThreadPoolSize,
            ThreadPoolType = metaData.ThreadPoolType.FullName,
            JobExecutedNumber = metaData.NumberOfJobsExecuted,
            RunTimes = metaData.RunningSince != null
                ? DateTimeOffset.Now.Subtract(metaData.RunningSince.Value)
                    .ToString("dd\\ \\天\\ hh\\ \\时\\ mm\\ \\分\\ ss\\ \\秒")
                : "--",
            JobCountNumber = (await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup())).Count,
            TriggerCountNumber = (await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup())).Count,
            JobExecuteNumber = (await scheduler.GetCurrentlyExecutingJobs()).Count
        };
    }

    /// <summary>
    /// 启动调度器
    /// </summary>
    /// <returns></returns>
    public async Task<bool> StartScheduler()
    {
        // 获取调度器
        var scheduler = await _schedulerFactory.GetScheduler();

        // 判断调度器是否处于待机模式
        if (scheduler.InStandbyMode)
        {
            // 启动调度器
            await scheduler.Start();
        }

        return !scheduler.InStandbyMode;
    }

    /// <summary>
    /// 停止调度器
    /// </summary>
    /// <returns></returns>
    public async Task<bool> StopScheduler()
    {
        // 获取调度器
        var scheduler = await _schedulerFactory.GetScheduler();

        // 判断调度器是否已经关闭
        if (!scheduler.InStandbyMode)
        {
            // 等待任务运行完成
            await scheduler.Standby();
        }

        return scheduler.InStandbyMode;
    }

    /// <summary>
    /// 暂停调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task StopSchedulerJob(SchedulerJobKeyInput input)
    {
        // 获取调度器
        var scheduler = await _schedulerFactory.GetScheduler();

        await scheduler.PauseJob(new JobKey(input.JobName, input.JobGroup.ToString()));
    }

    /// <summary>
    /// 恢复调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task ResumeSchedulerJob(SchedulerJobKeyInput input)
    {
        // 获取调度器
        var scheduler = await _schedulerFactory.GetScheduler();

        var jobKey = new JobKey(input.JobName, input.JobGroup.ToString());

        // 检查作业是否存在
        if (!await scheduler.CheckExists(jobKey))
        {
            throw new UserFriendlyException("调度作业不存在！");
        }

        // 获取作业详情
        var jobDetail = await scheduler.GetJobDetail(jobKey);

        var endTime = jobDetail!.JobDataMap.GetNullableDateTime(nameof(SchedulerJobInfo.EndTime));
        // 判断作业是否已经过期
        if (endTime != null && endTime <= DateTime.Now)
        {
            throw new UserFriendlyException("调度作业已过期！");
        }

        // 恢复作业
        await scheduler.ResumeJob(jobKey);
    }

    /// <summary>
    /// 立即执行调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task TriggerSchedulerJob(SchedulerJobKeyInput input)
    {
        // 获取调度器
        var scheduler = await _schedulerFactory.GetScheduler();

        await scheduler.TriggerJob(new JobKey(input.JobName, input.JobGroup.ToString()));
    }

    /// <summary>
    /// 是否存在调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<bool> ExistsSchedulerJob(SchedulerJobKeyInput input)
    {
        // 获取调度器
        var scheduler = await _schedulerFactory.GetScheduler();

        return await scheduler.CheckExists(new JobKey(input.JobName, input.JobGroup.ToString()));
    }

    /// <summary>
    /// 获取调度作业日志
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<List<string>> QuerySchedulerJobLogs(SchedulerJobKeyInput input)
    {
        // 获取调度器
        var scheduler = await _schedulerFactory.GetScheduler();

        var jobKey = new JobKey(input.JobName, input.JobGroup.ToString());

        // 检查作业是否存在
        if (!await scheduler.CheckExists(jobKey))
        {
            throw new UserFriendlyException("调度作业不存在！");
        }

        // 获取作业详情
        var jobDetail = await scheduler.GetJobDetail(jobKey);

        var logs = jobDetail!.JobDataMap[nameof(SchedulerJobInfo.Logs)] as List<string> ?? [];
        // 倒序排序
        logs.Reverse();
        return logs;
    }

    /// <summary>
    /// 获取调度作业运行次数
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<long> QuerySchedulerJobRunNumber(SchedulerJobKeyInput input)
    {
        // 获取调度器
        var scheduler = await _schedulerFactory.GetScheduler();

        var jobKey = new JobKey(input.JobName, input.JobGroup.ToString());

        // 检查作业是否存在
        if (!await scheduler.CheckExists(jobKey))
        {
            throw new UserFriendlyException("调度作业不存在！");
        }

        // 获取作业详情
        var jobDetail = await scheduler.GetJobDetail(jobKey);

        return jobDetail!.JobDataMap.GetLong(nameof(SchedulerJobInfo.RunNumber));
    }

    /// <summary>
    /// 获取全部调度作业
    /// </summary>
    /// <param name="jobGroup"></param>
    /// <returns></returns>
    public async Task<List<QueryAllSchedulerJobOutput>> QueryAllSchedulerJob(SchedulerJobGroupEnum? jobGroup = null)
    {
        var result = new List<QueryAllSchedulerJobOutput>();

        // 获取调度器
        var scheduler = await _schedulerFactory.GetScheduler();

        // 获取调度器所有作业分组名称
        var jobGroupNames = await scheduler.GetJobGroupNames();

        jobGroupNames = jobGroupNames
            // 传参筛选
            .Where(jobGroup != null, wh => wh.Equals(jobGroup.ToString(), StringComparison.OrdinalIgnoreCase))
            //（分组名称排序）
            .OrderBy(ob => ob)
            .ToList();

        // 循环作业分组名称
        foreach (var jobGroupName in jobGroupNames)
        {
            var jobGroupInfo = new QueryAllSchedulerJobOutput
            {
                JobGroup = Enum.Parse<SchedulerJobGroupEnum>(jobGroupName), JobInfoList = []
            };

            // 获取当前分组的所有调度作业
            var jobKeyList = await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(jobGroupName));

            // 循环调度作业Key（作业名称排序）
            foreach (var jobKey in jobKeyList.OrderBy(ob => ob.Name))
            {
                // 获取作业详情
                var jobDetail = await scheduler.GetJobDetail(jobKey);

                // 获取触发器
                var triggers = await scheduler.GetTriggersOfJob(jobKey);
                var trigger = triggers.AsEnumerable()
                    .LastOrDefault();

                if (trigger == null)
                {
                    throw new UserFriendlyException("调度作业对应的触发器不存在！");
                }

                // 从触发器获取时间间隔
                string interval;

                // 触发器类型
                TriggerTypeEnum triggerType;
                if (trigger is CronTriggerImpl cronTrigger)
                {
                    triggerType = TriggerTypeEnum.Cron;
                    // 从触发器获取 Cron表达式
                    interval = cronTrigger.CronExpressionString;
                }
                else if (trigger is DailyTimeIntervalTriggerImpl dailyTimeIntervalTrigger)
                {
                    triggerType = TriggerTypeEnum.Daily;
                    // 从触发器获取 执行间隔时间（默认就是秒）
                    interval = TimeSpan.FromSeconds(dailyTimeIntervalTrigger.RepeatInterval)
                        .ToString();
                }
                else if (trigger is SimpleTriggerImpl simpleTrigger)
                {
                    triggerType = TriggerTypeEnum.Simple;
                    // 从触发器获取 执行间隔时间
                    interval = simpleTrigger.RepeatInterval.ToString();
                }
                else
                {
                    throw new UserFriendlyException("触发器类型不正确！");
                }

                jobGroupInfo.JobInfoList.Add(new QueryAllSchedulerJobOutput.SchedulerJobInfoDto
                {
                    JobName = jobKey.Name,
                    PreviousFireTime = trigger.GetPreviousFireTimeUtc()
                        ?.LocalDateTime,
                    NextFireTime = trigger.GetNextFireTimeUtc()
                        ?.LocalDateTime,
                    JobType = jobDetail!.JobDataMap.GetEnum<SchedulerJobTypeEnum>(nameof(SchedulerJobInfo.JobType)),
                    BeginTime = trigger.StartTimeUtc.LocalDateTime,
                    EndTime = trigger.EndTimeUtc?.LocalDateTime
                              ?? jobDetail.JobDataMap.GetNullableDateTime(nameof(SchedulerJobInfo.EndTime)),
                    TriggerType = triggerType,
                    Interval = interval,
                    Description = jobDetail.Description,
                    // 触发器状态
                    TriggerState = await scheduler.GetTriggerState(trigger.Key),
                    RequestUrl = jobDetail.JobDataMap.GetString(nameof(SchedulerJobInfo.RequestUrl)),
                    RequestMethod =
                        jobDetail.JobDataMap.GetNullableEnum<HttpRequestMethodEnum>(nameof(SchedulerJobInfo.RequestMethod)),
                    RunNumber = jobDetail.JobDataMap.GetLong(nameof(SchedulerJobInfo.RunNumber)),
                    Exception = jobDetail.JobDataMap.GetString(nameof(SchedulerJobInfo.Exception))
                });
            }

            result.Add(jobGroupInfo);
        }

        return result;
    }

    /// <summary>
    /// 获取调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<SchedulerJobInfo> QuerySchedulerJob(SchedulerJobKeyInput input)
    {
        // 获取调度器
        var scheduler = await _schedulerFactory.GetScheduler();

        var jobKey = new JobKey(input.JobName, input.JobGroup.ToString());

        // 检查作业是否存在
        if (!await scheduler.CheckExists(jobKey))
        {
            throw new UserFriendlyException("调度作业不存在！");
        }

        // 获取作业详情
        var jobDetail = await scheduler.GetJobDetail(jobKey);

        // 获取触发器
        var triggers = await scheduler.GetTriggersOfJob(jobKey);
        var trigger = triggers.AsEnumerable()
            .LastOrDefault();

        if (trigger == null)
        {
            throw new UserFriendlyException("调度作业对应的触发器不存在！");
        }

        // 从触发器获取 Cron表达式
        string cron = null;
        // 从触发器获取 星期
        List<DayOfWeek> weeks = null;
        // 从触发器获取 每天开始时间
        TimeSpan? dailyStartTime = null;
        // 从触发器获取 每天结束时间
        TimeSpan? dailyEndTime = null;
        // 从触发器获取 执行间隔时间
        double? intervalSecond = null;
        // 从触发器获取 执行次数
        int? runTimes = null;

        // 触发器类型
        TriggerTypeEnum triggerType;
        if (trigger is CronTriggerImpl cronTrigger)
        {
            triggerType = TriggerTypeEnum.Cron;
            cron = cronTrigger.CronExpressionString;
        }
        else if (trigger is DailyTimeIntervalTriggerImpl dailyTimeIntervalTrigger)
        {
            triggerType = TriggerTypeEnum.Daily;
            weeks = dailyTimeIntervalTrigger.DaysOfWeek.ToList();
            dailyStartTime = new TimeSpan(dailyTimeIntervalTrigger.StartTimeOfDay.Hour,
                dailyTimeIntervalTrigger.StartTimeOfDay.Minute, dailyTimeIntervalTrigger.StartTimeOfDay.Second);
            dailyEndTime = new TimeSpan(dailyTimeIntervalTrigger.EndTimeOfDay.Hour, dailyTimeIntervalTrigger.EndTimeOfDay.Minute,
                dailyTimeIntervalTrigger.EndTimeOfDay.Second);
            intervalSecond = dailyTimeIntervalTrigger.RepeatInterval;
            runTimes = dailyTimeIntervalTrigger.RepeatCount == -1 ? null : dailyTimeIntervalTrigger.RepeatCount;
        }
        else if (trigger is SimpleTriggerImpl simpleTrigger)
        {
            triggerType = TriggerTypeEnum.Simple;
            intervalSecond = simpleTrigger.RepeatInterval.TotalSeconds;
            runTimes = simpleTrigger.RepeatCount == -1 ? null : simpleTrigger.RepeatCount;
        }
        else
        {
            throw new UserFriendlyException("触发器类型不正确！");
        }

        var result = new SchedulerJobInfo
        {
            JobName = jobKey.Name,
            JobGroup = Enum.Parse<SchedulerJobGroupEnum>(jobKey.Group),
            JobType = jobDetail!.JobDataMap.GetEnum<SchedulerJobTypeEnum>(nameof(SchedulerJobInfo.JobType)),
            BeginTime = trigger.StartTimeUtc.LocalDateTime,
            EndTime =
                trigger.EndTimeUtc?.LocalDateTime ?? jobDetail.JobDataMap.GetNullableDateTime(nameof(SchedulerJobInfo.EndTime)),
            TriggerType = triggerType,
            Cron = cron,
            Week = weeks?.ToWeekEnum(),
            DailyStartTime = dailyStartTime,
            DailyEndTime = dailyEndTime,
            IntervalSecond = intervalSecond != null ? Convert.ToInt32(intervalSecond.Value) : null,
            RunTimes = runTimes,
            WarnTime = jobDetail.JobDataMap.GetNullableInt(nameof(SchedulerJobInfo.WarnTime)),
            RetryTimes = jobDetail.JobDataMap.GetNullableInt(nameof(SchedulerJobInfo.RetryTimes)),
            RetryMillisecond = jobDetail.JobDataMap.GetNullableInt(nameof(SchedulerJobInfo.RetryMillisecond)),
            MailMessage = jobDetail.JobDataMap.GetEnum<MailMessageEnum>(nameof(SchedulerJobInfo.MailMessage)),
            Description = jobDetail.Description,
            // 触发器状态
            TriggerState = await scheduler.GetTriggerState(trigger.Key),
            RequestUrl = jobDetail.JobDataMap.GetString(nameof(SchedulerJobInfo.RequestUrl)),
            RequestMethod = jobDetail.JobDataMap.GetNullableEnum<HttpRequestMethodEnum>(nameof(SchedulerJobInfo.RequestMethod)),
            RequestParams = jobDetail.JobDataMap[nameof(SchedulerJobInfo.RequestParams)] as IDictionary<string, object>,
            RequestHeader = jobDetail.JobDataMap[nameof(SchedulerJobInfo.RequestHeader)] as IDictionary<string, string>,
            RequestTimeout = jobDetail.JobDataMap.GetNullableInt(nameof(SchedulerJobInfo.RequestTimeout)),
            RunNumber = jobDetail.JobDataMap.GetLong(nameof(SchedulerJobInfo.RunNumber)),
            Exception = jobDetail.JobDataMap.GetString(nameof(SchedulerJobInfo.Exception)),
            Logs = jobDetail.JobDataMap[nameof(SchedulerJobInfo.Logs)] as List<string>
        };

        return result;
    }

    /// <summary>
    /// 添加调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task AddSchedulerJob(AddSchedulerJobInput input)
    {
        await AddSchedulerJob(new SchedulerJobInfo
        {
            JobName = input.JobName,
            JobGroup = input.JobGroup,
            JobType = input.JobType,
            BeginTime = input.BeginTime,
            EndTime = input.EndTime,
            TriggerType = input.TriggerType,
            Cron = input.Cron,
            Week = input.Week,
            DailyStartTime = input.DailyStartTime,
            DailyEndTime = input.DailyEndTime,
            IntervalSecond = input.IntervalSecond,
            RunTimes = input.RunTimes,
            WarnTime = input.WarnTime,
            RetryTimes = input.RetryTimes,
            RetryMillisecond = input.RetryMillisecond,
            MailMessage = input.MailMessage,
            Description = input.Description,
            RequestUrl = input.RequestUrl,
            RequestMethod = input.RequestMethod,
            RequestTimeout = input.RequestTimeout,
            RequestParams = input.RequestParams,
            RequestHeader = input.RequestHeader
        });
    }

    /// <summary>
    /// 编辑调度作业
    /// </summary>
    /// <remarks>注：这里更新作业会导致触发器的执行记录被清空。所以会导致更新后可能会立即执行一次。</remarks>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task EditSchedulerJob(EditSchedulerJobInput input)
    {
        if (string.IsNullOrWhiteSpace(input.OldJobName))
        {
            throw new UserFriendlyException("旧的调度作业名称不能为空！");
        }

        // 获取调度器
        var scheduler = await _schedulerFactory.GetScheduler();

        // 获取旧的调度作业Key
        var oldJobKey = new JobKey(input.OldJobName, input.OldJobGroup.ToString());

        // 获取旧作业
        var oldJobDetail = await scheduler.GetJobDetail(oldJobKey);

        // 获取旧的调度作业触发器
        var oldTriggers = await scheduler.GetTriggersOfJob(oldJobKey);
        var oldTrigger = oldTriggers.AsEnumerable()
            .LastOrDefault();

        if (oldTrigger == null)
        {
            throw new UserFriendlyException("调度作业对应的触发器不存在！");
        }

        // 组装新的添加实体
        var newJobEntity = new SchedulerJobInfo
        {
            JobName = input.JobName,
            JobGroup = input.JobGroup,
            JobType = input.JobType,
            BeginTime = input.BeginTime,
            EndTime = input.EndTime,
            TriggerType = input.TriggerType,
            Cron = input.Cron,
            Week = input.Week,
            DailyStartTime = input.DailyStartTime,
            DailyEndTime = input.DailyEndTime,
            IntervalSecond = input.IntervalSecond,
            RunTimes = input.RunTimes,
            WarnTime = input.WarnTime,
            RetryTimes = input.RetryTimes,
            RetryMillisecond = input.RetryMillisecond,
            MailMessage = input.MailMessage,
            Description = input.Description,
            RequestUrl = input.RequestUrl,
            RequestMethod = input.RequestMethod,
            RequestTimeout = input.RequestTimeout,
            RequestParams = input.RequestParams,
            RequestHeader = input.RequestHeader,
            // 触发器状态
            TriggerState = await scheduler.GetTriggerState(oldTrigger.Key),
            RunNumber = oldJobDetail.JobDataMap.GetLong(nameof(SchedulerJobInfo.RunNumber)),
            Exception = oldJobDetail.JobDataMap.GetString(nameof(SchedulerJobInfo.Exception)),
            Logs = oldJobDetail.JobDataMap[nameof(SchedulerJobInfo.Logs)] as List<string>
        };

        // 属性验证
        SchedulerJobVerify(newJobEntity);

        // 创建新的调度作业Key
        var newJobKey = new JobKey(newJobEntity.JobName, newJobEntity.JobGroup.ToString());

        // 本地作业禁止修改 JobKey
        if (newJobEntity.JobType == SchedulerJobTypeEnum.Local)
        {
            if (!newJobKey.Equals(oldJobKey))
            {
                throw new UserFriendlyException("本地作业禁止修改调度作业名称和分组！");
            }
        }

        // 判断是否等于旧的调度作业Key
        if (!newJobKey.Equals(oldJobKey))
        {
            // 检查作业是否存在
            if (await scheduler.CheckExists(newJobKey))
            {
                throw new UserFriendlyException("调度作业已存在！");
            }
        }

        // 暂停旧的调度作业
        await scheduler.PauseJob(oldJobKey);
        // 删除旧的调度作业
        await scheduler.DeleteJob(oldJobKey);

        // 添加新的调度作业
        await AddSchedulerJob(scheduler, newJobKey, newJobEntity);
    }

    /// <summary>
    /// 删除调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task DeleteSchedulerJob(SchedulerJobKeyInput input)
    {
        // 获取调度器
        var scheduler = await _schedulerFactory.GetScheduler();

        var jobKey = new JobKey(input.JobName, input.JobGroup.ToString());

        // 获取作业详情
        var jobDetail = await scheduler.GetJobDetail(jobKey);

        var jobType = jobDetail!.JobDataMap.GetEnum<SchedulerJobTypeEnum>(nameof(SchedulerJobInfo.JobType));
        if (jobType == SchedulerJobTypeEnum.Local)
        {
            throw new UserFriendlyException("禁止删除本地作业！");
        }

        // 先暂停
        await scheduler.PauseJob(jobKey);
        await scheduler.DeleteJob(jobKey);
    }

    /// <summary>
    /// 移除调度作业异常信息
    /// </summary>
    /// <remarks>因为只能在 IJob 持久化操作 JobDataMap，所以这里直接暴力操作数据库</remarks>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task DeleteSchedulerJobException(SchedulerJobKeyInput input)
    {
        // 获取调度器
        var scheduler = await _schedulerFactory.GetScheduler();

        var jobKey = new JobKey(input.JobName, input.JobGroup.ToString());

        // 检查作业是否存在
        if (!await scheduler.CheckExists(jobKey))
        {
            throw new UserFriendlyException("调度作业不存在！");
        }

        // 获取选项
        var options = _serviceProvider.GetService<IOptions<QuartzOptions>>();

        // 获取数据表前缀
        var tablePrefix = options.Value.GetValueOrDefault(
            $"{StdSchedulerFactory.PropertyJobStorePrefix}.{StdSchedulerFactory.PropertyTablePrefix}",
            AdoConstants.DefaultTablePrefix);

        var db = new SqlSugarClient(SqlSugarContext.GetConnectionConfig(SqlSugarContext.ConnectionSettings));
        // 加载Aop
        SugarEntityFilter.LoadSugarAop(FastContext.HostEnvironment.IsDevelopment(), db);

        var whereConditionalModel = new List<IConditionalModel>
        {
            new ConditionalModel
            {
                FieldName = AdoConstants.ColumnSchedulerName,
                ConditionalType = ConditionalType.Equal,
                FieldValue = scheduler.SchedulerName
            },
            new ConditionalModel
            {
                FieldName = AdoConstants.ColumnJobName, ConditionalType = ConditionalType.Equal, FieldValue = jobKey.Name
            },
            new ConditionalModel
            {
                FieldName = AdoConstants.ColumnJobGroup,
                ConditionalType = ConditionalType.Equal,
                FieldValue = jobKey.Group
            }
        };
        var selectModels = new List<SelectModel>
        {
            new() {FieldName = AdoConstants.ColumnJobDataMap, AsName = nameof(QueryableJobDataResult.JobData)}
        };
        var queryResult = await db.Queryable<QueryableJobDataResult>()
            .AS($"{tablePrefix}{AdoConstants.TableJobDetails}")
            .Where(whereConditionalModel)
            .Select(selectModels)
            .SingleAsync();

        if (queryResult.JobData != null)
        {
            var jobData = JObject.Parse(Encoding.UTF8.GetString(queryResult.JobData.ToArray()));
            // 移除异常日志
            if (jobData.ContainsKey(nameof(SchedulerJobInfo.Exception)))
            {
                jobData[nameof(SchedulerJobInfo.Exception)] = string.Empty;
            }

            // 执行更新
            var modelDict = new Dictionary<string, object>
            {
                {AdoConstants.ColumnSchedulerName, scheduler.SchedulerName},
                {AdoConstants.ColumnJobName, jobKey.Name},
                {AdoConstants.ColumnJobGroup, jobKey.Group},
                {AdoConstants.ColumnJobDataMap, Encoding.UTF8.GetBytes(jobData.ToString())}
            };
            await db.Updateable(modelDict)
                .AS($"{tablePrefix}{AdoConstants.TableJobDetails}")
                .WhereColumns(AdoConstants.ColumnSchedulerName, AdoConstants.ColumnJobName, AdoConstants.ColumnJobGroup)
                .ExecuteCommandAsync();
        }
    }

    private protected class QueryableJobDataResult
    {
        public byte[] JobData { get; set; }
    }
}