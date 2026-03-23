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

namespace Fast.Admin.Service.Auth.Dto;

/// <summary>
/// <see cref="GetLoginUserInfoOutput"/> 获取登录用户信息输出
/// </summary>
public class GetLoginUserInfoOutput
{
    /// <summary>
    /// 账号Id
    /// </summary>
    public long AccountId { get; set; }

    /// <summary>
    /// 手机
    /// </summary>
    public string Mobile { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    public string NickName { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// 职员Id
    /// </summary>
    public long EmployeeId { get; set; }

    /// <summary>
    /// 工号
    /// </summary>
    public string EmployeeNo { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string EmployeeName { get; set; }

    /// <summary>
    /// 部门Id
    /// </summary>
    public long? DepartmentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string DepartmentName { get; set; }

    /// <summary>
    /// 是否超级管理员
    /// </summary>
    public bool IsSuperAdmin { get; set; }

    /// <summary>
    /// 是否管理员
    /// </summary>
    public bool IsAdmin { get; set; }

    /// <summary>
    /// 角色名称集合
    /// </summary>
    public List<string> RoleNameList { get; set; } = [];

    /// <summary>
    /// 角色类型
    /// </summary>
    public RoleTypeEnum RoleType { get; set; }

    /// <summary>
    /// 数据范围类型
    /// </summary>
    public DataScopeTypeEnum DataScopeType { get; set; }

    /// <summary>
    /// 按钮编码集合
    /// </summary>
    public List<string> ButtonCodeList { get; set; } = [];

    /// <summary>
    /// 菜单集合
    /// </summary>
    public List<AuthMenuInfoDto> MenuList { get; set; } = [];
}