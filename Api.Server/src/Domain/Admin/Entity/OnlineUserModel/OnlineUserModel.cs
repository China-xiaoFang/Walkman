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
/// <see cref="OnlineUserModel"/> 在线用户表Model类
/// </summary>
[SugarTable("OnlineUser", "在线用户表")]
[SugarDbType(DatabaseTypeEnum.Admin)]
[SugarIndex($"IX_{{table}}_{nameof(DeviceId)}", nameof(DeviceId), OrderByType.Asc)]
public class OnlineUserModel : IDatabaseEntity
{
    /// <summary>
    /// 连接Id
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "连接Id", Length = 32, IsPrimaryKey = true)]
    public string ConnectionId { get; set; }

    /// <summary>
    /// 设备类型
    /// </summary>
    [SugarColumn(ColumnDescription = "设备类型")]
    public AppEnvironmentEnum DeviceType { get; set; }

    /// <summary>
    /// 设备Id
    /// </summary>
    [SugarColumn(ColumnDescription = "设备Id", Length = 36)]
    public string DeviceId { get; set; }

    /// <summary>
    /// 账号Id
    /// </summary>
    [SugarColumn(ColumnDescription = "账号Id")]
    public long AccountId { get; set; }

    /// <summary>
    /// 手机
    /// </summary>
    [Required]
    [SugarSearchValue]
    [SugarColumn(ColumnDescription = "手机", ColumnDataType = "varchar(11)")]
    public string Mobile { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [SugarSearchValue]
    [SugarColumn(ColumnDescription = "昵称", Length = 20)]
    public string NickName { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    [SugarColumn(ColumnDescription = "头像", Length = 200)]
    public string Avatar { get; set; }

    /// <summary>
    /// 职员Id
    /// </summary>
    [SugarColumn(ColumnDescription = "职员Id")]
    public long EmployeeId { get; set; }

    /// <summary>
    /// 工号
    /// </summary>
    [SugarColumn(ColumnDescription = "工号", Length = 20)]
    public string EmployeeNo { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "姓名", Length = 20)]
    public string EmployeeName { get; set; }

    /// <summary>
    /// 部门Id
    /// </summary>
    [SugarColumn(ColumnDescription = "部门Id")]
    public long? DepartmentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    [SugarColumn(ColumnDescription = "部门名称", Length = 20)]
    public string DepartmentName { get; set; }

    /// <summary>
    /// 是否超级管理员
    /// </summary>
    [SugarColumn(ColumnDescription = "是否超级管理员")]
    public bool IsSuperAdmin { get; set; }

    /// <summary>
    /// 是否管理员
    /// </summary>
    [SugarColumn(ColumnDescription = "是否管理员")]
    public bool IsAdmin { get; set; }

    /// <summary>
    /// 最后登录设备
    /// </summary>
    [SugarColumn(ColumnDescription = "最后登录设备", Length = 50)]
    public string LastLoginDevice { get; set; }

    /// <summary>
    /// 最后登录操作系统（版本）
    /// </summary>
    [SugarColumn(ColumnDescription = "最后登录操作系统（版本）", Length = 50)]
    public string LastLoginOS { get; set; }

    /// <summary>
    /// 最后登录浏览器（版本）
    /// </summary>
    [SugarColumn(ColumnDescription = "最后登录浏览器（版本）", Length = 50)]
    public string LastLoginBrowser { get; set; }

    /// <summary>
    /// 最后登录省份
    /// </summary>
    [SugarColumn(ColumnDescription = "最后登录省份", Length = 20)]
    public string LastLoginProvince { get; set; }

    /// <summary>
    /// 最后登录城市
    /// </summary>
    [SugarColumn(ColumnDescription = "最后登录城市", Length = 20)]
    public string LastLoginCity { get; set; }

    /// <summary>
    /// 最后登录Ip
    /// </summary>
    [SugarColumn(ColumnDescription = "最后登录Ip", Length = 15)]
    public string LastLoginIp { get; set; }

    /// <summary>
    /// 最后登录时间
    /// </summary>
    [SugarColumn(ColumnDescription = "最后登录时间")]
    public DateTime LastLoginTime { get; set; }

    /// <summary>
    /// 是否在线
    /// </summary>
    [SugarColumn(ColumnDescription = "是否在线")]
    public bool IsOnline { get; set; }

    /// <summary>
    /// 下线时间
    /// </summary>
    [SugarColumn(ColumnDescription = "下线时间")]
    public DateTime? OfflineTime { get; set; }
}