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
/// <see cref="ComplaintModel"/> 投诉表Model类
/// </summary>
[SugarTable("Complaint", "投诉表")]
[SugarDbType(DatabaseTypeEnum.Admin)]
public class ComplaintModel : IUpdateVersion
{
    /// <summary>
    /// 投诉Id
    /// </summary>
    [SugarColumn(ColumnDescription = "投诉Id", IsPrimaryKey = true)]
    public long ComplaintId { get; set; }

    /// <summary>
    /// 应用标识
    /// </summary>
    [SugarColumn(ColumnDescription = "应用标识", Length = 50)]
    public string OpenId { get; set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "用户Id")]
    public long UserId { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [SugarColumn(ColumnDescription = "昵称", Length = 20)]
    public string NickName { get; set; }

    /// <summary>
    /// 投诉类型
    /// </summary>
    [SugarColumn(ColumnDescription = "投诉类型")]
    public ComplaintTypeEnum ComplaintType { get; set; }

    /// <summary>
    /// 手机
    /// </summary>
    [SugarColumn(ColumnDescription = "手机", ColumnDataType = "varchar(11)")]
    public string Mobile { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "联系电话", Length = 20)]
    public string ContactPhone { get; set; }

    /// <summary>
    /// 联系邮箱
    /// </summary>
    [SugarColumn(ColumnDescription = "联系邮箱", Length = 50)]
    public string ContactEmail { get; set; }

    /// <summary>
    /// 投诉描述
    /// </summary>
    [SugarColumn(ColumnDescription = "投诉描述", Length = 200)]
    public string Description { get; set; }

    /// <summary>
    /// 附件图片
    /// </summary>
    [SugarColumn(ColumnDescription = "附件图片", ColumnDataType = StaticConfig.CodeFirst_BigString, IsJson = true)]
    public List<string> AttachmentImages { get; set; }

    /// <summary>
    /// 处理时间
    /// </summary>
    [SugarColumn(ColumnDescription = "处理时间", CreateTableFieldSort = 996)]
    public DateTime? HandleTime { get; set; }

    /// <summary>
    /// 处理描述
    /// </summary>
    [SugarColumn(ColumnDescription = "处理描述", Length = 200)]
    public string HandleDescription { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 200)]
    public string Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Required, SugarColumn(ColumnDescription = "创建时间", CreateTableFieldSort = 993)]
    public DateTime? CreatedTime { get; set; }

    /// <summary>
    /// 更新版本控制字段
    /// </summary>
    [SugarColumn(ColumnDescription = "更新版本控制字段", IsEnableUpdateVersionValidation = true, CreateTableFieldSort = 998)]
    public long RowVersion { get; set; }
}