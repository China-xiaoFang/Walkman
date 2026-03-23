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

using Fast.Admin.Entity;
using Fast.Admin.Enum;

namespace Fast.Admin.Service.Employee.Dto;

/// <summary>
/// <see cref="AddEmployeeInput"/> 添加职员输入
/// </summary>
public class AddEmployeeInput
{
    /// <summary>
    /// 姓名
    /// </summary>
    [StringRequired(ErrorMessage = "姓名不能为空")]
    public string EmployeeName { get; set; }

    /// <summary>
    /// 手机
    /// </summary>
    [StringRequired(ErrorMessage = "手机不能为空")]
    [RegularExpression(RegexConst.Mobile, ErrorMessage = "手机格式不正确")]
    public string Mobile { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [StringRequired(ErrorMessage = "邮箱不能为空")]
    [RegularExpression(RegexConst.EmailAddress, ErrorMessage = "邮箱格式不正确")]
    public string Email { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [EnumRequired(ErrorMessage = "性别不能为空", AllowZero = true)]
    public GenderEnum Sex { get; set; }

    /// <summary>
    /// 证件照
    /// </summary>
    public string IdPhoto { get; set; }

    /// <summary>
    /// 入职日期
    /// </summary>
    [DateTimeRequired(ErrorMessage = "入职日期不能为空")]
    public DateTime EntryDate { get; set; }

    /// <summary>
    /// 民族
    /// </summary>
    public NationEnum? Nation { get; set; }

    /// <summary>
    /// 籍贯
    /// </summary>
    public string NativePlace { get; set; }

    /// <summary>
    /// 家庭地址
    /// </summary>
    public string FamilyAddress { get; set; }

    /// <summary>
    /// 通信地址
    /// </summary>
    public string MailingAddress { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// 证件类型
    /// </summary>
    public IdTypeEnum? IdType { get; set; }

    /// <summary>
    /// 证件号码
    /// </summary>
    public string IdNumber { get; set; }

    /// <summary>
    /// 文件程度
    /// </summary>
    public EducationLevelEnum? EducationLevel { get; set; }

    /// <summary>
    /// 政治面貌
    /// </summary>
    public PoliticalStatusEnum? PoliticalStatus { get; set; }

    /// <summary>
    /// 毕业学院
    /// </summary>
    public string GraduationCollege { get; set; }

    /// <summary>
    /// 学历
    /// </summary>
    public AcademicQualificationsEnum? AcademicQualifications { get; set; }

    /// <summary>
    /// 学制
    /// </summary>
    public AcademicSystemEnum? AcademicSystem { get; set; }

    /// <summary>
    /// 学位
    /// </summary>
    public DegreeEnum? Degree { get; set; }

    /// <summary>
    /// 家庭电话
    /// </summary>
    public string FamilyPhone { get; set; }

    /// <summary>
    /// 办公电话
    /// </summary>
    public string OfficePhone { get; set; }

    /// <summary>
    /// 紧急联系人
    /// </summary>
    public string EmergencyContact { get; set; }

    /// <summary>
    /// 紧急联系电话
    /// </summary>
    public string EmergencyPhone { get; set; }

    /// <summary>
    /// 紧急联系地址
    /// </summary>
    public string EmergencyAddress { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 机构Id
    /// </summary>
    [LongRequired(ErrorMessage = "机构Id不能为空")]
    public long OrgId { get; set; }

    /// <summary>
    /// 机构名称
    /// </summary>
    public string OrgName { get; set; }

    /// <summary>
    /// 部门Id
    /// </summary>
    [LongRequired(ErrorMessage = "部门Id不能为空")]
    public long DepartmentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string DepartmentName { get; set; }

    /// <summary>
    /// 职位Id
    /// </summary>
    [LongRequired(ErrorMessage = "职位Id不能为空")]
    public long PositionId { get; set; }

    /// <summary>
    /// 职位名称
    /// </summary>
    public string PositionName { get; set; }

    /// <summary>
    /// 职级Id
    /// </summary>
    public long? JobLevelId { get; set; }

    /// <summary>
    /// 职级名称
    /// </summary>
    public string JobLevelName { get; set; }

    /// <summary>
    /// 是否为负责人
    /// </summary>
    [Required(ErrorMessage = "是否为负责人不能为空")]
    public bool IsPrincipal { get; set; }

    /// <summary>
    /// 角色信息
    /// </summary>
    public List<EmployeeRoleModel> RoleList { get; set; }
}