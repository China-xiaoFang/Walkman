// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称"软件"）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按"原样"提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

namespace Fast.Admin.Entity;

/// <summary>
/// <see cref="QuestionOptionModel"/> 题目选项表Model类
/// </summary>
[SugarTable("QuestionOption", "题目选项表")]
[SugarDbType(DatabaseTypeEnum.Admin)]
[SugarIndex($"IX_{{table}}_{nameof(QuestionId)}", nameof(QuestionId), OrderByType.Asc)]
public class QuestionOptionModel : BaseEntity, IUpdateVersion
{
    /// <summary>
    /// 题目选项Id
    /// </summary>
    [SugarColumn(ColumnDescription = "题目选项Id", IsPrimaryKey = true)]
    public long QuestionOptionId { get; set; }

    /// <summary>
    /// 题目Id
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "题目Id")]
    public long QuestionId { get; set; }

    /// <summary>
    /// 选项标签（A/B/C/D）
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "选项标签", Length = 10)]
    public string Label { get; set; }

    /// <summary>
    /// 选项内容
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "选项内容", Length = 500)]
    public string Content { get; set; }

    /// <summary>
    /// 是否正确答案
    /// </summary>
    [SugarColumn(ColumnDescription = "是否正确答案")]
    public bool IsCorrect { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; }

    /// <summary>
    /// 更新版本控制字段
    /// </summary>
    [SugarColumn(ColumnDescription = "更新版本控制字段", IsEnableUpdateVersionValidation = true, CreateTableFieldSort = 998)]
    public long RowVersion { get; set; }
}
