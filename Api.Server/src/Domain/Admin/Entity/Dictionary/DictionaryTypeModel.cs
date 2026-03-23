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
/// <see cref="DictionaryTypeModel"/> 字典类型表Model类
/// </summary>
[SugarTable("DictionaryType", "字典类型表")]
[SugarDbType(DatabaseTypeEnum.Admin)]
[SugarIndex($"IX_{{table}}_{nameof(DictionaryKey)}", nameof(DictionaryKey), OrderByType.Asc, true)]
public class DictionaryTypeModel : BaseEntity, IUpdateVersion
{
    /// <summary>
    /// 字典Id
    /// </summary>
    [SugarColumn(ColumnDescription = "字典Id", IsPrimaryKey = true)]
    public long DictionaryId { get; set; }

    /// <summary>
    /// 服务名称
    /// </summary>
    [SugarColumn(ColumnDescription = "服务名称", Length = 50)]
    public string ServiceName { get; set; }

    /// <summary>
    /// 字典Key
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "字典Key", Length = 50)]
    public string DictionaryKey { get; set; }

    /// <summary>
    /// 字典名称
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "字典名称", Length = 50)]
    public string DictionaryName { get; set; }

    /// <summary>
    /// 字典值类型
    /// </summary>
    [SugarColumn(ColumnDescription = "字典值类型")]
    public DictionaryValueTypeEnum ValueType { get; set; }

    /// <summary>
    /// Flags枚举
    /// </summary>
    [SugarColumn(ColumnDescription = "Flags枚举")]
    public bool HasFlags { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public CommonStatusEnum Status { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 200)]
    public string Remark { get; set; }

    /// <summary>
    /// 更新版本控制字段
    /// </summary>
    [SugarColumn(ColumnDescription = "更新版本控制字段", IsEnableUpdateVersionValidation = true, CreateTableFieldSort = 998)]
    public long RowVersion { get; set; }

    /// <summary>
    /// 字典项信息
    /// </summary>
    [Navigate(NavigateType.OneToMany, nameof(DictionaryItemModel.DictionaryId), nameof(DictionaryId))]
    public List<DictionaryItemModel> DictionaryItemList { get; set; }

    /// <summary>Serves as the default hash function.</summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return DictionaryId.GetHashCode();
    }

    /// <summary>Determines whether the specified object is equal to the current object.</summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
    public override bool Equals(object obj)
    {
        if (obj is not DictionaryTypeModel oldDictionaryTypeModel)
            return false;

        return DictionaryId == oldDictionaryTypeModel.DictionaryId
               && DictionaryKey == oldDictionaryTypeModel.DictionaryKey
               && DictionaryName == oldDictionaryTypeModel.DictionaryName
               && ValueType == oldDictionaryTypeModel.ValueType
               && HasFlags == oldDictionaryTypeModel.HasFlags;
    }
}