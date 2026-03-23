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
/// <see cref="TableColumnConfigCacheModel"/> 表格列配置缓存表Model类
/// </summary>
[SugarTable("TableColumnConfigCache", "表格列配置缓存表")]
[SugarDbType(DatabaseTypeEnum.Admin)]
public class TableColumnConfigCacheModel : IDatabaseEntity
{
    /// <summary>
    /// 用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "用户Id", IsPrimaryKey = true)]
    public long UserId { get; set; }

    /// <summary>
    /// 表格Id
    /// </summary>
    [SugarColumn(ColumnDescription = "表格Id", IsPrimaryKey = true)]
    public long TableId { get; set; }

    /// <summary>
    /// 列Id
    /// </summary>
    [SugarColumn(ColumnDescription = "列Id", IsPrimaryKey = true)]
    public long ColumnId { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "名称", Length = 50)]
    public string Label { get; set; }

    /// <summary>
    /// 固定
    /// </summary>
    /// <remarks>
    /// <para>left：左侧</para>
    /// <para>right：右侧</para>
    /// </remarks>
    [SugarColumn(ColumnDescription = "固定", Length = 5)]
    public string Fixed { get; set; }

    /// <summary>
    /// 自动宽度
    /// </summary>
    [SugarColumn(ColumnDescription = "自动宽度")]
    public bool AutoWidth { get; set; }

    /// <summary>
    /// 宽度
    /// </summary>
    [SugarColumn(ColumnDescription = "宽度")]
    public int Width { get; set; }

    /// <summary>
    /// 小的宽度
    /// </summary>
    [SugarColumn(ColumnDescription = "小的宽度")]
    public int SmallWidth { get; set; }

    /// <summary>
    /// 顺序
    /// </summary>
    [SugarColumn(ColumnDescription = "顺序")]
    public int Order { get; set; }

    /// <summary>
    /// 显示
    /// </summary>
    [SugarColumn(ColumnDescription = "显示")]
    public bool Show { get; set; }

    /// <summary>
    /// 复制
    /// </summary>
    [SugarColumn(ColumnDescription = "复制")]
    public bool Copy { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public bool Sortable { get; set; }

    /// <summary>
    /// 搜索项名称
    /// </summary>
    [SugarColumn(ColumnDescription = "搜索项名称", Length = 50)]
    public string SearchLabel { get; set; }

    /// <summary>
    /// 搜索项排序
    /// </summary>
    [SugarColumn(ColumnDescription = "搜索项排序")]
    public int SearchOrder { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Required, SugarColumn(ColumnDescription = "创建时间", CreateTableFieldSort = 993)]
    public DateTime? CreatedTime { get; set; }
}