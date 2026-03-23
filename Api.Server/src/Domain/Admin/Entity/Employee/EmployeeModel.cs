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
/// <see cref="EmployeeModel"/> 职员表Model类
/// </summary>
[SugarTable("Employee", "职员表")]
[SugarDbType(DatabaseTypeEnum.Admin)]
[SugarIndex($"IX_{{table}}_{nameof(EmployeeNo)}", nameof(EmployeeNo), OrderByType.Asc, true)]
[SugarIndex($"IX_{{table}}_{nameof(Mobile)}", nameof(Mobile), OrderByType.Asc, true)]
[SugarIndex($"IX_{{table}}_{nameof(AccountId)}", nameof(AccountId), OrderByType.Asc, true)]
public class EmployeeModel : IUpdateVersion
{
    /// <summary>
    /// 职员Id
    /// </summary>
    [SugarColumn(ColumnDescription = "职员Id", IsPrimaryKey = true)]
    public long EmployeeId { get; set; }

    /// <summary>
    /// 账号Id
    /// </summary>
    [SugarColumn(ColumnDescription = "账号Id")]
    public long AccountId { get; set; }

    /// <summary>
    /// 用户类型
    /// </summary>
    [SugarColumn(ColumnDescription = "用户类型")]
    public UserTypeEnum UserType { get; set; }

    /// <summary>
    /// 工号
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "工号", Length = 20)]
    public string EmployeeNo { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "姓名", Length = 20)]
    public string EmployeeName { get; set; }

    /// <summary>
    /// 手机
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "手机", ColumnDataType = "varchar(11)")]
    public string Mobile { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public EmployeeStatusEnum Status { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(ColumnDescription = "邮箱", Length = 50)]
    public string Email { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [SugarColumn(ColumnDescription = "性别")]
    public GenderEnum Sex { get; set; }

    /// <summary>
    /// 证件照
    /// </summary>
    [SugarColumn(ColumnDescription = "证件照", Length = 200)]
    public string IdPhoto { get; set; }

    /// <summary>
    /// 入职日期
    /// </summary>
    [SugarColumn(ColumnDescription = "入职日期")]
    public DateTime EntryDate { get; set; }

    /// <summary>
    /// 离职日期
    /// </summary>
    [SugarColumn(ColumnDescription = "离职日期")]
    public DateTime? ResignDate { get; set; }

    /// <summary>
    /// 离职原因
    /// </summary>
    [SugarColumn(ColumnDescription = "离职原因", Length = 200)]
    public string ResignReason { get; set; }

    /// <summary>
    /// 民族
    /// </summary>
    [SugarColumn(ColumnDescription = "民族")]
    public NationEnum? Nation { get; set; }

    /// <summary>
    /// 籍贯
    /// </summary>
    [SugarColumn(ColumnDescription = "籍贯", Length = 50)]
    public string NativePlace { get; set; }

    /// <summary>
    /// 家庭地址
    /// </summary>
    [SugarColumn(ColumnDescription = "家庭地址", Length = 200)]
    public string FamilyAddress { get; set; }

    /// <summary>
    /// 通信地址
    /// </summary>
    [SugarColumn(ColumnDescription = "通信地址", Length = 200)]
    public string MailingAddress { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    [SugarColumn(ColumnDescription = "生日")]
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// 证件类型
    /// </summary>
    [SugarColumn(ColumnDescription = "证件类型")]
    public IdTypeEnum? IdType { get; set; }

    /// <summary>
    /// 证件号码
    /// </summary>
    [SugarColumn(ColumnDescription = "证件号码", Length = 50)]
    public string IdNumber { get; set; }

    /// <summary>
    /// 文件程度
    /// </summary>
    [SugarColumn(ColumnDescription = "文件程度")]
    public EducationLevelEnum? EducationLevel { get; set; }

    /// <summary>
    /// 政治面貌
    /// </summary>
    [SugarColumn(ColumnDescription = "政治面貌")]
    public PoliticalStatusEnum? PoliticalStatus { get; set; }

    /// <summary>
    /// 毕业学院
    /// </summary>
    [SugarColumn(ColumnDescription = "毕业学院", Length = 50)]
    public string GraduationCollege { get; set; }

    /// <summary>
    /// 学历
    /// </summary>
    [SugarColumn(ColumnDescription = "学历")]
    public AcademicQualificationsEnum? AcademicQualifications { get; set; }

    /// <summary>
    /// 学制
    /// </summary>
    [SugarColumn(ColumnDescription = "学制")]
    public AcademicSystemEnum? AcademicSystem { get; set; }

    /// <summary>
    /// 学位
    /// </summary>
    [SugarColumn(ColumnDescription = "学位")]
    public DegreeEnum? Degree { get; set; }

    /// <summary>
    /// 家庭电话
    /// </summary>
    [SugarColumn(ColumnDescription = "家庭电话", Length = 20)]
    public string FamilyPhone { get; set; }

    /// <summary>
    /// 办公电话
    /// </summary>
    [SugarColumn(ColumnDescription = "办公电话", Length = 20)]
    public string OfficePhone { get; set; }

    /// <summary>
    /// 紧急联系人
    /// </summary>
    [SugarColumn(ColumnDescription = "紧急联系人", Length = 20)]
    public string EmergencyContact { get; set; }

    /// <summary>
    /// 紧急联系电话
    /// </summary>
    [SugarColumn(ColumnDescription = "紧急联系电话", Length = 20)]
    public string EmergencyPhone { get; set; }

    /// <summary>
    /// 紧急联系地址
    /// </summary>
    [SugarColumn(ColumnDescription = "紧急联系地址", Length = 200)]
    public string EmergencyAddress { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 200)]
    public string Remark { get; set; }

    /// <summary>
    /// 创建者用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "创建者用户Id", CreateTableFieldSort = 991)]
    public long? CreatedUserId { get; set; }

    /// <summary>
    /// 创建者用户名称
    /// </summary>
    [SugarColumn(ColumnDescription = "创建者用户名称", Length = 20, CreateTableFieldSort = 992)]
    public string CreatedUserName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Required, SugarColumn(ColumnDescription = "创建时间", CreateTableFieldSort = 993)]
    public DateTime? CreatedTime { get; set; }

    /// <summary>
    /// 更新者用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "更新者用户Id", CreateTableFieldSort = 994)]
    public long? UpdatedUserId { get; set; }

    /// <summary>
    /// 更新者用户名称
    /// </summary>
    [SugarColumn(ColumnDescription = "更新者用户名称", Length = 20, CreateTableFieldSort = 995)]
    public string UpdatedUserName { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间", CreateTableFieldSort = 996)]
    public DateTime? UpdatedTime { get; set; }

    /// <summary>
    /// 更新版本控制字段
    /// </summary>
    [SugarColumn(ColumnDescription = "更新版本控制字段", IsEnableUpdateVersionValidation = true, CreateTableFieldSort = 998)]
    public long RowVersion { get; set; }
}