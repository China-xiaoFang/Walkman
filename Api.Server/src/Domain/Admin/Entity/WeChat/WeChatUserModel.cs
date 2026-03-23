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
/// <see cref="WeChatUserModel"/> 微信用户表Model类
/// </summary>
[SugarTable("WeChatUser", "微信用户表")]
[SugarDbType(DatabaseTypeEnum.Admin)]
[SugarIndex($"IX_{{table}}_{nameof(OpenId)}", nameof(OpenId), OrderByType.Asc, nameof(PurePhoneNumber), OrderByType.Asc, true)]
public class WeChatUserModel : IUpdateVersion
{
    /// <summary>
    /// 微信用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "微信Id", IsPrimaryKey = true)]
    public long WeChatId { get; set; }

    /// <summary>
    /// 用户类型
    /// </summary>
    [SugarColumn(ColumnDescription = "用户类型")]
    public WeChatUserTypeEnum UserType { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "密码", Length = 50)]
    public string Password { get; set; }

    /// <summary>
    /// 唯一用户标识
    /// </summary>
    [SugarColumn(ColumnDescription = "唯一用户标识", Length = 28)]
    public string OpenId { get; set; }

    /// <summary>
    /// 统一用户标识
    /// </summary>
    [SugarColumn(ColumnDescription = "统一用户标识", Length = 28)]
    public string UnionId { get; set; }

    /// <summary>
    /// 用户纯手机号码
    /// </summary>
    [SugarColumn(ColumnDescription = "用户纯手机号码", ColumnDataType = "varchar(11)")]
    public string PurePhoneNumber { get; set; }

    /// <summary>
    /// 用户手机号码
    /// </summary>
    [SugarColumn(ColumnDescription = "用户手机号码", Length = 20)]
    public string PhoneNumber { get; set; }

    /// <summary>
    /// 用户手机号码区号
    /// </summary>
    [SugarColumn(ColumnDescription = "用户手机号码区号", Length = 5)]
    public string CountryCode { get; set; }

    /// <summary>
    /// 小程序登录凭证
    /// </summary>
    [SugarColumn(ColumnDescription = "小程序登录凭证", Length = 64)]
    public string SessionKey { get; set; }

    /// <summary>
    /// 微信昵称
    /// </summary>
    [SugarColumn(ColumnDescription = "微信昵称", Length = 20)]
    public string NickName { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    [SugarColumn(ColumnDescription = "头像", Length = 200)]
    public string Avatar { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [SugarColumn(ColumnDescription = "性别")]
    public GenderEnum Sex { get; set; }

    /// <summary>
    /// 国家
    /// </summary>
    [SugarColumn(ColumnDescription = "国家", Length = 20)]

    public string Country { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    [SugarColumn(ColumnDescription = "省份", Length = 20)]

    public string Province { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    [SugarColumn(ColumnDescription = "城市", Length = 20)]

    public string City { get; set; }

    /// <summary>
    /// 语言
    /// </summary>
    [SugarColumn(ColumnDescription = "语言", Length = 10)]
    public string Language { get; set; }

    /// <summary>
    /// 激活码
    /// </summary>
    [SugarColumn(ColumnDescription = "激活码", Length = 50)]
    public string ActivationCode { get; set; }

    /// <summary>
    /// 激活时间
    /// </summary>
    [SugarColumn(ColumnDescription = "激活时间")]
    public DateTime? ActivationTime { get; set; }

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
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Required, SugarColumn(ColumnDescription = "创建时间", CreateTableFieldSort = 993)]
    public DateTime? CreatedTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间", CreateTableFieldSort = 996)]
    public DateTime? UpdatedTime { get; set; }

    /// <summary>
    /// 手机号更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "手机号更新时间", CreateTableFieldSort = 996)]
    public DateTime? MobileUpdateTime { get; set; }

    /// <summary>
    /// 更新版本控制字段
    /// </summary>
    [SugarColumn(ColumnDescription = "更新版本控制字段", IsEnableUpdateVersionValidation = true, CreateTableFieldSort = 998)]
    public long RowVersion { get; set; }
}