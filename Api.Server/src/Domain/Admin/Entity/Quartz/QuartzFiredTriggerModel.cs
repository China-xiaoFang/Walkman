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

namespace Fast.Admin.Entity;

/// <summary>
/// <see cref="QuartzFiredTriggerModel"/> Quartz 触发器快照表Model类
/// </summary>
[SugarTable("QRTZ_FIRED_TRIGGERS", "Quartz 触发器快照表")]
[SugarDbType(DatabaseTypeEnum.Admin)]
[SugarIndex("IDX_QRTZ_FT_INST_JOB_REQ_RCVRY", nameof(SchedName), OrderByType.Asc, nameof(InstanceName), OrderByType.Asc,
    nameof(RequestsRecovery), OrderByType.Asc)]
[SugarIndex("IDX_QRTZ_FT_G_J", nameof(SchedName), OrderByType.Asc, nameof(JobGroup), OrderByType.Asc, nameof(JobName),
    OrderByType.Asc)]
[SugarIndex("IDX_QRTZ_FT_G_T", nameof(SchedName), OrderByType.Asc, nameof(TriggerGroup), OrderByType.Asc, nameof(TriggerName),
    OrderByType.Asc)]
public class QuartzFiredTriggerModel : IDatabaseEntity
{
    /// <summary>
    /// 调度器名称
    /// </summary>
    [Required]
    [SugarColumn(ColumnName = "SCHED_NAME", ColumnDescription = "调度器名称", Length = 120, IsPrimaryKey = true)]
    public string SchedName { get; set; }

    /// <summary>
    /// 触发器实例唯一Id
    /// </summary>
    [Required]
    [SugarColumn(ColumnName = "ENTRY_ID", ColumnDescription = "触发器实例唯一Id", Length = 140, IsPrimaryKey = true)]
    public string EntryId { get; set; }

    /// <summary>
    /// 触发器名称
    /// </summary>
    [Required]
    [SugarColumn(ColumnName = "TRIGGER_NAME", ColumnDescription = "触发器名称", Length = 150)]
    public string TriggerName { get; set; }

    /// <summary>
    /// 调度器分组
    /// </summary>
    [Required]
    [SugarColumn(ColumnName = "TRIGGER_GROUP", ColumnDescription = "调度器分组", Length = 150)]
    public string TriggerGroup { get; set; }

    /// <summary>
    /// 实例名称
    /// </summary>
    [Required]
    [SugarColumn(ColumnName = "INSTANCE_NAME", ColumnDescription = "实例名称", Length = 150)]
    public string InstanceName { get; set; }

    /// <summary>
    /// 触发时间（毫秒）
    /// </summary>
    [SugarColumn(ColumnName = "FIRED_TIME", ColumnDescription = "触发时间（毫秒）")]
    public long FiredTime { get; set; }

    /// <summary>
    /// 调度时间（毫秒）
    /// </summary>
    [SugarColumn(ColumnName = "SCHED_TIME", ColumnDescription = "调度时间（毫秒）")]
    public long SchedTime { get; set; }

    /// <summary>
    /// 优先级
    /// </summary>
    [SugarColumn(ColumnName = "PRIORITY", ColumnDescription = "优先级")]
    public int Priority { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// <para>ACQUIRED</para>
    /// <para>EXECUTING</para>
    /// <para>COMPLETE</para>
    /// <para>BLOCKED </para>
    /// </remarks>
    [Required]
    [SugarColumn(ColumnName = "STATE", ColumnDescription = "状态", Length = 16)]
    public string State { get; set; }

    /// <summary>
    /// 作业名称
    /// </summary>
    [SugarColumn(ColumnName = "JOB_NAME", ColumnDescription = "作业名称", Length = 150)]
    public string JobName { get; set; }

    /// <summary>
    /// 作业分组
    /// </summary>
    [SugarColumn(ColumnName = "JOB_GROUP", ColumnDescription = "作业分组", Length = 150)]
    public string JobGroup { get; set; }

    /// <summary>
    /// 是否禁止并发执行
    /// </summary>
    [SugarColumn(ColumnName = "IS_NONCONCURRENT", ColumnDescription = "是否禁止并发执行")]
    public bool? IsNonConcurrent { get; set; }

    /// <summary>
    /// 是否请求恢复
    /// </summary>
    /// <remarks>true 表示当调度器崩溃或中断后允许重新恢复执行</remarks>
    [SugarColumn(ColumnName = "REQUESTS_RECOVERY", ColumnDescription = "是否请求恢复")]
    public bool? RequestsRecovery { get; set; }
}