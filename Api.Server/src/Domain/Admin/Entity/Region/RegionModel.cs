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
/// <see cref="RegionModel"/> 地区表Model类
/// </summary>
[SugarTable("Region", "地区表")]
[SugarDbType(DatabaseTypeEnum.Admin)]
[SugarIndex($"IX_{{table}}_{nameof(RegionCode)}", nameof(RegionCode), OrderByType.Asc, true)]
[SugarIndex($"IX_{{table}}_{nameof(RegionName)}", nameof(RegionName), OrderByType.Asc)]
public class RegionModel : BaseEntity, IUpdateVersion
{
    /// <summary>
    /// 区域Id
    /// </summary>
    [SugarColumn(ColumnDescription = "区域Id", IsPrimaryKey = true)]
    public long RegionId { get; set; }

    /// <summary>
    /// 父级Id
    /// </summary>
    [SugarColumn(ColumnDescription = "父级Id")]
    public long ParentId { get; set; }

    /// <summary>
    /// 行政编码
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "行政编码", Length = 12)]
    public string RegionCode { get; set; }

    /// <summary>
    /// 区域名称
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "区域名称", Length = 50)]
    public string RegionName { get; set; }

    /// <summary>
    /// 区号
    /// </summary>
    /// <remarks>地级市</remarks>
    [SugarColumn(ColumnDescription = "区号", Length = 4)]
    public string AreaCode { get; set; }

    /// <summary>
    /// 邮政编码
    /// </summary>
    /// <remarks>区/县/自治县</remarks>
    [SugarColumn(ColumnDescription = "邮政编码", Length = 6)]
    public string PostalCode { get; set; }

    /// <summary>
    /// 区域层级
    /// </summary>
    [SugarColumn(ColumnDescription = "区域层级")]
    public RegionLevelEnum RegionLevel { get; set; }

    /// <summary>
    /// 纬度
    /// </summary>
    [SugarColumn(ColumnDescription = "纬度", Length = 20, DecimalDigits = 7)]
    public decimal? Latitude { get; set; }

    /// <summary>
    /// 经度
    /// </summary>
    [SugarColumn(ColumnDescription = "经度", Length = 20, DecimalDigits = 7)]
    public decimal? Longitude { get; set; }

    /// <summary>
    /// 全称
    /// </summary>
    [SugarColumn(ColumnDescription = "全称", Length = 200)]
    public string FullRegionName { get; set; }

    /// <summary>
    /// 拼音首字母
    /// </summary>
    /// <remarks>大写</remarks>
    [SugarColumn(ColumnDescription = "拼音首字母", Length = 50)]
    public string PinYin { get; set; }

    /// <summary>
    /// 拼音全拼
    /// </summary>
    [SugarColumn(ColumnDescription = "拼音全拼", Length = 200)]
    public string PinYinFull { get; set; }

    /// <summary>
    /// 更新版本控制字段
    /// </summary>
    [SugarColumn(ColumnDescription = "更新版本控制字段", IsEnableUpdateVersionValidation = true, CreateTableFieldSort = 998)]
    public long RowVersion { get; set; }
}