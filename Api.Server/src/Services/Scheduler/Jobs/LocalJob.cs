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

using System.Web;
using Fast.Admin.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Fast.Scheduler;

/// <summary>
/// <see cref="LocalJob"/> Local调度作业
/// </summary>
internal class LocalJob : JobBase<SchedulerJobLogInfo>
{
    public LocalJob(IServiceProvider serviceProvider, ISchedulerCenter schedulerCenter, IMailService mailService,
        IOptions<MvcNewtonsoftJsonOptions> jsonOptions, ILogger<IJob> logger) : base(serviceProvider, mailService, jsonOptions,
        logger, new SchedulerJobUrlLogInfo())
    {
    }

    /// <summary>
    /// 执行调度作业
    /// </summary>
    /// <param name="serviceProvider"><see cref="IServiceProvider"/> 服务提供者</param>
    /// <param name="db"><see cref="ISqlSugarClient"/> SqlSugar上下文</param>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override async Task JobExecute(IServiceProvider serviceProvider, ISqlSugarClient db, IJobExecutionContext context)
    {
        // 作业类型
        var jobType = context.JobDetail.JobDataMap.GetEnum<SchedulerJobTypeEnum>(nameof(SchedulerJobInfo.JobType));

        if (jobType != SchedulerJobTypeEnum.Local)
        {
            throw new UserFriendlyException("当前调度作业只支持【本地】！");
        }

        var jobKey = new JobKey(context.JobDetail.Key.Name, context.JobDetail.Key.Group);

        // 尝试从缓存中获取本地调度作业的实现类
        var localSchedulerJobType = SchedulerContext.LocalSchedulerJobTypes.GetValueOrDefault(jobKey.ToString());

        if (localSchedulerJobType == null)
        {
            throw new SchedulerException("未能在缓存中找到【本地】调度作业！");
        }

        // 解析服务
        if (serviceProvider.GetService(localSchedulerJobType) is not ISchedulerJob _schedulerJob)
        {
            throw new UserFriendlyException("解析【本地】调度作业服务异常，服务不存在！");
        }

        // 执行本地调度作业
        var result = await _schedulerJob.Execute(serviceProvider, db,
            new SchedulerJobLocalLogInfo
            {
                JobName = _logInfo.JobName,
                InfoLog = InfoLog,
                WarnLog = WarnLog,
                ErrorLog = ErrorLog,
                RobotInfo = _logInfo.RobotInfo
            });

        if (!string.IsNullOrWhiteSpace(result))
        {
            _logInfo.Result =
                $"<span class='result'>{HttpUtility.HtmlEncode(result).GetSubStringWithEllipsis(1000, true)}</span>";
        }
    }
}