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

using MimeKit;

namespace Fast.Core;

/// <summary>
/// 邮件服务
/// </summary>
public interface IMailService
{
    /// <summary>
    /// 获取公用邮件模板
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="msg">消息正文</param>
    /// <param name="type">
    /// <para>info</para>
    /// <para>warn</para>
    /// <para>error</para>
    /// </param>
    /// <param name="displayName">发件人显示名称，为null时读取配置</param>
    /// <returns>公用邮件模板</returns>
    Task<string> GetEmailTemplate(string title, string msg, string type = null, string displayName = null);

    /// <summary>
    /// 获取当前验证码发送的剩余等待秒数
    /// </summary>
    /// <param name="mailType">邮件类型</param>
    /// <param name="email">邮箱</param>
    Task<int> GetVerificationCodeRetryAfterSeconds(MailTypeEnum mailType, string email);

    /// <summary>
    /// 发送验证码
    /// </summary>
    /// <param name="mailType">邮件类型</param>
    /// <param name="email">邮箱</param>
    Task SendVerificationCode(MailTypeEnum mailType, string email);

    /// <summary>
    /// 验证并一次性消费验证码
    /// </summary>
    /// <param name="mailType">邮件类型</param>
    /// <param name="email">邮箱</param>
    /// <param name="verificationCode">验证码</param>
    Task VerifyVerificationCode(MailTypeEnum mailType, string email, string verificationCode);

    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="title">邮件标题</param>
    /// <param name="content">邮件正文</param>
    /// <param name="receiveEmails">收件邮箱，为null时使用默认收件人</param>
    /// <param name="smtp">发件服务器地址，为null时读取配置</param>
    /// <param name="port">发件服务器端口，为null时读取配置</param>
    /// <param name="email">发件邮箱，为null时读取配置</param>
    /// <param name="authCode">发件邮箱授权码，为null时读取配置</param>
    /// <param name="displayName">发件人显示名称，为null时读取配置</param>
    Task SendEmail(string title, string content, List<string> receiveEmails = null, string smtp = null, int? port = null,
        string email = null, string authCode = null, string displayName = null);


    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="title">邮件标题</param>
    /// <param name="content">邮件正文</param>
    /// <param name="receiveEmails">收件邮箱，为null时使用默认收件人</param>
    /// <param name="smtp">发件服务器地址，为null时读取配置</param>
    /// <param name="port">发件服务器端口，为null时读取配置</param>
    /// <param name="email">发件邮箱，为null时读取配置</param>
    /// <param name="authCode">发件邮箱授权码，为null时读取配置</param>
    /// <param name="displayName">发件人显示名称，为null时读取配置</param>
    Task SendEmail(string title, BodyBuilder content, List<string> receiveEmails = null, string smtp = null, int? port = null,
        string email = null, string authCode = null, string displayName = null);
}