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

namespace Fast.Core;

/// <summary>
/// 缓存常量
/// </summary>
[SuppressSniffer]
public static class CacheConst
{
    /// <summary>
    /// 获取缓存Key
    /// </summary>
    /// <returns>缓存键</returns>
    public static string GetCacheKey(string cacheKey, params object[] args)
    {
        return string.Format(cacheKey, args);
    }

    /// <summary>
    /// 授权用户
    /// </summary>
    /// <remarks>{0}应用编号，{1}租户编号，{2}登录环境，{3}工号，{4}会话Id</remarks>
    public const string AuthUser = "{0}:{1}:Auth:{2}:{3}:{4}";

    /// <summary>
    /// 图片验证码
    /// </summary>
    /// <remarks>{0}验证码Key</remarks>
    public const string ImageCaptcha = "Captcha:{0}";

    /// <summary>
    /// 邮件
    /// </summary>
    /// <remarks>{0}类型，{1}邮箱</remarks>
    public const string Mail = "Mail:{0}:{1}";

    /// <summary>
    /// 短信
    /// </summary>
    /// <remarks>{0}类型，{1}手机号</remarks>
    public const string Sms = "SMS:{0}:{1}";

    /// <summary>
    /// 租户登录凭证
    /// </summary>
    /// <remarks>{0}凭据Key</remarks>
    public const string TenantLoginTicket = "Login:TenantTicket:{0}";

    /// <summary>
    /// 重置密码
    /// </summary>
    /// <remarks>{0}验证Key</remarks>
    public const string PasswordReset = "Login:PasswordReset:{0}";

    /// <summary>
    /// 账号身份验证
    /// </summary>
    /// <remarks>{0}账号Key，{1}客户端标识</remarks>
    public const string AccountIdentityVerification = "Account:IdentityVerification:{0}:{1}";

    /// <summary>
    /// 编辑账号联系方式验证
    /// </summary>
    /// <remarks>{0}账号Key，{1}客户端标识</remarks>
    public const string EditAccountVerification = "Account:EditVerification:{0}:{1}";

    /// <summary>
    /// 管理后台
    /// </summary>
    public static class Center
    {
        /// <summary>
        /// 数据库
        /// </summary>
        /// <remarks>{0}租户编号，{1}数据库名类型</remarks>
        public const string Database = "Database:{0}:{1}";

        /// <summary>
        /// 配置
        /// </summary>
        /// <remarks>{0}配置编码</remarks>
        public const string Config = "Config:{0}";

        /// <summary>
        /// 租户
        /// </summary>
        /// <remarks>{0}租户编号</remarks>
        public const string Tenant = "Tenant:{0}";

        /// <summary>
        /// 机器人
        /// </summary>
        /// <remarks>{0}租户编号</remarks>
        public const string Rabot = "Rabot:{0}";

        /// <summary>
        /// 应用
        /// </summary>
        /// <remarks>{0}应用标识</remarks>
        public const string App = "App:{0}";

        /// <summary>
        /// 商户号
        /// </summary>
        /// <remarks>{0}商户号</remarks>
        public const string Merchant = "Merchant:{0}";

        /// <summary>
        /// 字典
        /// </summary>
        public const string Dictionary = "Dictionary";

        /// <summary>
        /// 表格配置
        /// </summary>
        /// <remarks>{0}表格Key</remarks>
        public const string TableConfig = "TableConfig:{0}";

        /// <summary>
        /// 用户表格配置缓存
        /// </summary>
        /// <remarks>{0}表格Key，{1}租户编号, {2}工号</remarks>
        public const string UserTableConfigCache = "TableConfig:{0}:{1}:{2}";

        /// <summary>
        /// 地区
        /// </summary>
        public const string Region = "Region";

        /// <summary>
        /// 省份
        /// </summary>
        public const string Province = "Province";

        /// <summary>
        /// 城市
        /// </summary>
        public const string City = "City";
    }
}