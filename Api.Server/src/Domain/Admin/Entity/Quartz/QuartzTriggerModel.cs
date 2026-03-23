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
/// <see cref="QuartzTriggerModel"/> Quartz 触发器表Model类
/// </summary>
[SugarTable("QRTZ_TRIGGERS", "Quartz 触发器表")]
[SugarDbType(DatabaseTypeEnum.Admin)]
[SugarIndex("IDX_QRTZ_T_G_J", nameof(SchedName), OrderByType.Asc, nameof(JobGroup), OrderByType.Asc, nameof(JobName),
    OrderByType.Asc)]
[SugarIndex("IDX_QRTZ_T_C", nameof(SchedName), OrderByType.Asc, nameof(CalendarName), OrderByType.Asc)]
[SugarIndex("IDX_QRTZ_T_N_G_STATE", nameof(SchedName), OrderByType.Asc, nameof(TriggerGroup), OrderByType.Asc,
    nameof(TriggerState), OrderByType.Asc)]
[SugarIndex("IDX_QRTZ_T_STATE", nameof(SchedName), OrderByType.Asc, nameof(TriggerState), OrderByType.Asc)]
[SugarIndex("IDX_QRTZ_T_N_STATE", nameof(SchedName), OrderByType.Asc, nameof(TriggerName), OrderByType.Asc, nameof(TriggerGroup),
    OrderByType.Asc, nameof(TriggerState), OrderByType.Asc)]
[SugarIndex("IDX_QRTZ_T_NEXT_FIRE_TIME", nameof(SchedName), OrderByType.Asc, nameof(NextFireTime), OrderByType.Asc)]
[SugarIndex("IDX_QRTZ_T_NFT_ST", nameof(SchedName), OrderByType.Asc, nameof(TriggerState), OrderByType.Asc, nameof(NextFireTime),
    OrderByType.Asc)]
[SugarIndex("IDX_QRTZ_T_NFT_ST_MISFIRE", nameof(SchedName), OrderByType.Asc, nameof(MisfireInstr), OrderByType.Asc,
    nameof(NextFireTime), OrderByType.Asc, nameof(TriggerState), OrderByType.Asc)]
[SugarIndex("IDX_QRTZ_T_NFT_ST_MISFIRE_GRP", nameof(SchedName), OrderByType.Asc, nameof(MisfireInstr), OrderByType.Asc,
    nameof(NextFireTime), OrderByType.Asc, nameof(TriggerGroup), OrderByType.Asc, nameof(TriggerState), OrderByType.Asc)]
public class QuartzTriggerModel : IDatabaseEntity
{
    /// <summary>
    /// 调度器名称
    /// </summary>
    [Required]
    [SugarColumn(ColumnName = "SCHED_NAME", ColumnDescription = "调度器名称", Length = 120, IsPrimaryKey = true)]
    public string SchedName { get; set; }

    /// <summary>
    /// 触发器名称
    /// </summary>
    [Required]
    [SugarColumn(ColumnName = "TRIGGER_NAME", ColumnDescription = "触发器名称", Length = 150, IsPrimaryKey = true)]
    public string TriggerName { get; set; }

    /// <summary>
    /// 调度器分组
    /// </summary>
    [Required]
    [SugarColumn(ColumnName = "TRIGGER_GROUP", ColumnDescription = "调度器分组", Length = 150, IsPrimaryKey = true)]
    public string TriggerGroup { get; set; }

    /// <summary>
    /// 作业名称
    /// </summary>
    [Required]
    [SugarColumn(ColumnName = "JOB_NAME", ColumnDescription = "作业名称", Length = 150)]
    public string JobName { get; set; }

    /// <summary>
    /// 作业分组
    /// </summary>
    [Required]
    [SugarColumn(ColumnName = "JOB_GROUP", ColumnDescription = "作业名称", Length = 150)]
    public string JobGroup { get; set; }

    /// <summary>
    /// 触发器描述
    /// </summary>
    [SugarColumn(ColumnName = "DESCRIPTION", ColumnDescription = "触发器描述", Length = 250)]
    public string Description { get; set; }

    /// <summary>
    /// 下次触发时间（时间戳）
    /// </summary>
    [SugarColumn(ColumnName = "NEXT_FIRE_TIME", ColumnDescription = "下次触发时间（时间戳）")]
    public long? NextFireTime { get; set; }

    /// <summary>
    /// 上次触发时间（时间戳）
    /// </summary>
    [SugarColumn(ColumnName = "PREV_FIRE_TIME", ColumnDescription = "上次触发时间（时间戳）")]
    public long? PrevFireTime { get; set; }

    /// <summary>
    /// 触发优先级
    /// </summary>
    [SugarColumn(ColumnName = "PRIORITY", ColumnDescription = "触发优先级")]
    public int? Priority { get; set; }

    /// <summary>
    /// 触发器状态
    /// </summary>
    [Required]
    [SugarColumn(ColumnName = "TRIGGER_STATE", ColumnDescription = "触发器状态", Length = 16)]
    public string TriggerState { get; set; }

    /// <summary>
    /// 触发器类型
    /// </summary>
    /// <remarks>
    /// <para>CRON</para>
    /// <para>SIMPLE</para>
    /// <para>BLOB</para>
    /// </remarks>
    [Required]
    [SugarColumn(ColumnName = "TRIGGER_TYPE", ColumnDescription = "触发器类型", Length = 8)]
    public string TriggerType { get; set; }

    /// <summary>
    /// 开始时间（时间戳）
    /// </summary>
    [SugarColumn(ColumnName = "START_TIME", ColumnDescription = "开始时间（时间戳）")]
    public long StartTime { get; set; }

    /// <summary>
    /// 结束时间（时间戳）
    /// </summary>
    [SugarColumn(ColumnName = "END_TIME", ColumnDescription = "结束时间（时间戳）")]
    public long? EndTime { get; set; }

    /// <summary>
    /// 日历名称
    /// </summary>
    [SugarColumn(ColumnName = "CALENDAR_NAME", ColumnDescription = "日历名称", Length = 200)]
    public string CalendarName { get; set; }

    /// <summary>
    /// 错过触发时的处理策略
    /// </summary>
    [SugarColumn(ColumnName = "MISFIRE_INSTR", ColumnDescription = "错过触发时的处理策略")]
    public int? MisfireInstr { get; set; }

    /// <summary>
    /// 触发器数据
    /// </summary>
    [SugarColumn(ColumnName = "JOB_DATA", ColumnDescription = "触发器数据")]
    public byte[] JobData { get; set; }
}