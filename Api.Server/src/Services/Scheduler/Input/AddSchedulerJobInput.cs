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

namespace Fast.Scheduler;

/// <summary>
/// <see cref="AddSchedulerJobInput"/> 添加调度作业输入
/// </summary>
public class AddSchedulerJobInput
{
    /// <summary>
    /// 作业名称
    /// </summary>
    [StringRequired(ErrorMessage = "作业名称不能为空")]
    public string JobName { get; set; }

    /// <summary>
    /// 作业分组
    /// </summary>
    [EnumRequired(ErrorMessage = "作业分组不能为空")]
    public SchedulerJobGroupEnum JobGroup { get; set; }

    /// <summary>
    /// 作业类型
    /// </summary>
    [EnumRequired(ErrorMessage = "作业类型不能为空")]
    public SchedulerJobTypeEnum JobType { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime BeginTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 触发器类型
    /// </summary>
    public TriggerTypeEnum TriggerType { get; set; }

    /// <summary>
    /// Cron表达式
    /// </summary>
    public string Cron { get; set; }

    /// <summary>
    /// 星期（Flag）
    /// </summary>
    public WeekEnum? Week { get; set; }

    /// <summary>
    /// 每天开始时间
    /// </summary>
    public TimeSpan? DailyStartTime { get; set; }

    /// <summary>
    /// 每天结束时间
    /// </summary>
    public TimeSpan? DailyEndTime { get; set; }

    /// <summary>
    /// 执行间隔时间，单位秒
    /// </summary>
    /// <remarks>如果有Cron，则IntervalSecond失效</remarks>
    public int? IntervalSecond { get; set; }

    /// <summary>
    /// 执行次数（默认无限循环）
    /// </summary>
    public int? RunTimes { get; set; }

    /// <summary>
    /// 警告秒数
    /// </summary>
    /// <remarks>优先级最高</remarks>
    public int? WarnTime { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int? RetryTimes { get; set; }

    /// <summary>
    /// 重试间隔，单位毫秒
    /// </summary>
    public int? RetryMillisecond { get; set; }

    /// <summary>
    /// 邮件消息
    /// </summary>
    [EnumRequired(ErrorMessage = "邮件消息不能为空", AllowZero = true, FlagEnum = true)]
    public MailMessageEnum MailMessage { get; set; } = MailMessageEnum.Error;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    #region Url

    /// <summary>
    /// 请求Url
    /// </summary>
    public string RequestUrl { get; set; }

    /// <summary>
    /// 请求方式
    /// </summary>
    public HttpRequestMethodEnum? RequestMethod { get; set; }

    /// <summary>
    /// 请求超时时间，单位秒（默认不超时）
    /// </summary>
    public int? RequestTimeout { get; set; }

    /// <summary>
    /// 请求参数
    /// </summary>
    public IDictionary<string, object> RequestParams { get; set; }

    /// <summary>
    /// 请求头部
    /// </summary>
    public IDictionary<string, string> RequestHeader { get; set; }

    #endregion
}