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
/// <see cref="DictionaryItemModel"/> 字典项表Model类
/// </summary>
[SugarTable("DictionaryItem", "字典项表")]
[SugarDbType(DatabaseTypeEnum.Admin)]
public class DictionaryItemModel : BaseEntity
{
    /// <summary>
    /// 字典项Id
    /// </summary>
    [SugarColumn(ColumnDescription = "字典项Id", IsPrimaryKey = true)]
    public long DictionaryItemId { get; set; }

    /// <summary>
    /// 字典Id
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "字典Id")]
    public long DictionaryId { get; set; }

    /// <summary>
    /// 字典项名称
    /// </summary>
    [SugarColumn(ColumnDescription = "字典项名称", Length = 50)]
    public string Label { get; set; }

    /// <summary>
    /// 字典项值
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "字典项值", Length = 50)]
    public string Value { get; set; }

    /// <summary>
    /// 标签类型
    /// </summary>
    [SugarColumn(ColumnDescription = "标签类型")]
    public TagTypeEnum Type { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>从小到大</remarks>
    [Required]
    [SugarColumn(ColumnDescription = "排序")]
    public int Order { get; set; }

    /// <summary>
    /// 提示
    /// </summary>
    [SugarColumn(ColumnDescription = "提示", Length = 100)]
    public string Tips { get; set; }

    /// <summary>
    /// 是否显示
    /// </summary>
    [SugarColumn(ColumnDescription = "是否显示")]
    public bool Visible { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public CommonStatusEnum Status { get; set; }

    /// <summary>Serves as the default hash function.</summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return DictionaryItemId.GetHashCode();
    }

    /// <summary>Determines whether the specified object is equal to the current object.</summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
    public override bool Equals(object obj)
    {
        if (obj is not DictionaryItemModel oldDictionaryItemModel)
            return false;

        return DictionaryItemId == oldDictionaryItemModel.DictionaryItemId
               && DictionaryId == oldDictionaryItemModel.DictionaryId
               && Label == oldDictionaryItemModel.Label
               && Value == oldDictionaryItemModel.Value
               && Type == oldDictionaryItemModel.Type
               && Order == oldDictionaryItemModel.Order;
    }
}