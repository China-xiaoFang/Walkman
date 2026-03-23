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

using Fast.Admin.Enum;

namespace Fast.Admin.Service.Employee.Dto;

/// <summary>
/// <see cref="QueryEmployeePagedOutput"/> 获取职员分页列表输出
/// </summary>
public class QueryEmployeePagedOutput
{
    /// <summary>
    /// 职员Id
    /// </summary>
    public long EmployeeId { get; set; }

    /// <summary>
    /// 工号
    /// </summary>
    [SugarSearchValue]
    public string EmployeeNo { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    [SugarSearchValue]
    public string EmployeeName { get; set; }

    /// <summary>
    /// 手机
    /// </summary>
    [SugarSearchValue]
    public string Mobile { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public EmployeeStatusEnum Status { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarSearchValue]
    public string Email { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public GenderEnum Sex { get; set; }

    /// <summary>
    /// 证件照
    /// </summary>
    public string IdPhoto { get; set; }

    /// <summary>
    /// 入职日期
    /// </summary>
    public DateTime EntryDate { get; set; }

    /// <summary>
    /// 离职日期
    /// </summary>
    public DateTime? ResignDate { get; set; }

    /// <summary>
    /// 民族
    /// </summary>
    public NationEnum? Nation { get; set; }

    /// <summary>
    /// 籍贯
    /// </summary>
    public string NativePlace { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateTime? Birthday { get; set; }

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
    /// 备注
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 创建者用户名称
    /// </summary>
    public string CreatedUserName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreatedTime { get; set; }

    /// <summary>
    /// 更新者用户名称
    /// </summary>
    public string UpdatedUserName { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedTime { get; set; }

    /// <summary>
    /// 更新版本控制字段
    /// </summary>
    public long RowVersion { get; set; }

    /// <summary>
    /// 机构Id
    /// </summary>
    public long? OrgId { get; set; }

    /// <summary>
    /// 机构名称
    /// </summary>
    public string OrgName { get; set; }

    /// <summary>
    /// 机构名称
    /// </summary>
    [SugarColumn(IsJson = true)]
    public List<string> OrgNames { get; set; }

    /// <summary>
    /// 部门Id
    /// </summary>
    public long? DepartmentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string DepartmentName { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    [SugarColumn(IsJson = true)]
    public List<string> DepartmentNames { get; set; }

    /// <summary>
    /// 职位Id
    /// </summary>
    public long? PositionId { get; set; }

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
    public bool? IsPrincipal { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleNames { get; set; }

    /// <summary>
    /// 账号状态
    /// </summary>
    public CommonStatusEnum AccountStatus { get; set; }

    /// <summary>
    /// 账号手机
    /// </summary>
    public string AccountMobile { get; set; }

    /// <summary>
    /// 账号昵称
    /// </summary>
    public string AccountNickName { get; set; }

    /// <summary>
    /// 最后登录时间
    /// </summary>
    public DateTime? LastLoginTime { get; set; }
}