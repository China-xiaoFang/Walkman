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

using Fast.Admin.Enum;
using Fast.Core;
using Fast.DynamicApplication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl.Calendar;
using Quartz.Impl.Triggers;

namespace Fast.Scheduler.Applications;

/// <summary>
/// <see cref="SchedulerApplication"/> 调度作业
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Scheduler, Name = "scheduler")]
public class SchedulerApplication : IDynamicApplication
{
    /// <summary>
    /// <see cref="ISchedulerCenter"/> 调度中心
    /// </summary>
    private readonly ISchedulerCenter _schedulerCenter;

    /// <summary>
    /// <see cref="SchedulerApplication"/> 调度作业
    /// </summary>
    /// <param name="schedulerCenter"><see cref="ISchedulerCenter"/> 调度中心</param>
    public SchedulerApplication(ISchedulerCenter schedulerCenter)
    {
        _schedulerCenter = schedulerCenter;
    }

    /// <summary>
    /// 运行并验证Cron表达式
    /// </summary>
    /// <param name="cron"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("运行并验证Cron表达式", HttpRequestActionEnum.Other)]
    public List<string> RunVerifyCron(string cron)
    {
        // 验证表达式是否正确
        if (!CronExpression.IsValidExpression(cron))
        {
            return ["请检查Cron表达式是否拼写正确！"];
        }

        var result = new List<string>();

        var cronTrigger = new CronTriggerImpl("TestName", "TestGroup", cron);
        var calendar = new BaseCalendar(TimeZoneInfo.Local);
        // 默认获取10条
        var list = TriggerUtils.ComputeFireTimes(cronTrigger, calendar, 10);

        foreach (var item in list)
        {
            result.Add(item.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        return result;
    }

    /// <summary>
    /// 获取调度器详情
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取调度器详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Scheduler.Detail)]
    public async Task<QuerySchedulerDetailOutput> QuerySchedulerDetail()
    {
        return await _schedulerCenter.QuerySchedulerDetail();
    }

    /// <summary>
    /// 启动调度器
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("启动调度器", HttpRequestActionEnum.Other)]
    [Permission(PermissionConst.Scheduler.Start)]
    public async Task StartScheduler()
    {
        await _schedulerCenter.StartScheduler();
    }

    /// <summary>
    /// 停止调度器
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("停止调度器", HttpRequestActionEnum.Other)]
    [Permission(PermissionConst.Scheduler.Stop)]
    public async Task StopScheduler()
    {
        await _schedulerCenter.StopScheduler();
    }

    /// <summary>
    /// 暂停调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("暂停调度作业", HttpRequestActionEnum.Other)]
    [Permission(PermissionConst.Scheduler.StopJob)]
    public async Task StopSchedulerJob(SchedulerJobKeyInput input)
    {
        await _schedulerCenter.StopSchedulerJob(input);
    }

    /// <summary>
    /// 恢复调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("恢复调度作业", HttpRequestActionEnum.Other)]
    [Permission(PermissionConst.Scheduler.ResumeJob)]
    public async Task ResumeSchedulerJob(SchedulerJobKeyInput input)
    {
        await _schedulerCenter.ResumeSchedulerJob(input);
    }

    /// <summary>
    /// 立即执行调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("立即执行调度作业", HttpRequestActionEnum.Other)]
    [Permission(PermissionConst.Scheduler.Trigger)]
    public async Task TriggerSchedulerJob(SchedulerJobKeyInput input)
    {
        await _schedulerCenter.TriggerSchedulerJob(input);
    }

    /// <summary>
    /// 获取调度作业日志
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取调度作业日志", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Scheduler.Detail)]
    public async Task<List<string>> QuerySchedulerJobLogs(SchedulerJobKeyInput input)
    {
        return await _schedulerCenter.QuerySchedulerJobLogs(input);
    }

    /// <summary>
    /// 获取调度作业运行次数
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取调度作业运行次数", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Scheduler.Detail)]
    public async Task<long> QuerySchedulerJobRunNumber(SchedulerJobKeyInput input)
    {
        return await _schedulerCenter.QuerySchedulerJobRunNumber(input);
    }

    /// <summary>
    /// 获取全部调度作业
    /// </summary>
    /// <param name="jobGroup"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取全部调度作业", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Scheduler.Paged)]
    public async Task<List<QueryAllSchedulerJobOutput>> QueryAllSchedulerJob(SchedulerJobGroupEnum? jobGroup = null)
    {
        return await _schedulerCenter.QueryAllSchedulerJob(jobGroup);
    }

    /// <summary>
    /// 获取调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取调度作业", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Scheduler.Detail)]
    public async Task<SchedulerJobInfo> QuerySchedulerJob(SchedulerJobKeyInput input)
    {
        return await _schedulerCenter.QuerySchedulerJob(input);
    }

    /// <summary>
    /// 添加调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加调度作业", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.Scheduler.Add)]
    public async Task AddSchedulerJob(AddSchedulerJobInput input)
    {
        await _schedulerCenter.AddSchedulerJob(input);
    }

    /// <summary>
    /// 编辑调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑调度作业", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Scheduler.Edit)]
    public async Task EditSchedulerJob(EditSchedulerJobInput input)
    {
        await _schedulerCenter.EditSchedulerJob(input);
    }

    /// <summary>
    /// 删除调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除调度作业", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.Scheduler.Delete)]
    public async Task DeleteSchedulerJob(SchedulerJobKeyInput input)
    {
        await _schedulerCenter.DeleteSchedulerJob(input);
    }

    /// <summary>
    /// 移除调度作业异常信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("移除调度作业异常信息", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.Scheduler.Delete)]
    public async Task DeleteSchedulerJobException(SchedulerJobKeyInput input)
    {
        await _schedulerCenter.DeleteSchedulerJobException(input);
    }
}