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

using Fast.Center.Domain;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.Core;

/// <summary>
/// 配置种子数据
/// </summary>
internal static class ConfigSeedData
{
    /// <summary>
    /// 配置种子数据
    /// </summary>
    public static async Task SystemConfigSeedData(ISqlSugarClient db, DateTime dateTime)
    {
        await db.Insertable(new List<ConfigModel>
            {
                new()
                {
                    ConfigId = YitIdHelper.NextId(),
                    ConfigCode = ConfigConst.SingleTenantWhenAutoLogin,
                    ConfigName = "单租户自动登录",
                    ConfigValue = "True",
                    Remark = "True：打开（如果只有一个租户，则默认当前租户自动登录）；False：关闭；",
                    CreatedTime = dateTime
                },
                new()
                {
                    ConfigId = YitIdHelper.NextId(),
                    ConfigCode = ConfigConst.SingleLogin,
                    ConfigName = "单点登录",
                    ConfigValue = "True",
                    Remark = "True：打开（多次登录只会保留最后一次登录有效）；False：关闭；",
                    CreatedTime = dateTime
                },
                new()
                {
                    ConfigId = YitIdHelper.NextId(),
                    ConfigCode = ConfigConst.LoginCaptchaOpen,
                    ConfigName = "登录验证码开关",
                    ConfigValue = "True",
                    Remark = "True：打开；False：关闭；",
                    CreatedTime = dateTime
                },
                new()
                {
                    ConfigId = YitIdHelper.NextId(),
                    ConfigCode = ConfigConst.LoginIdentityVerificationOpen,
                    ConfigName = "登录后身份验证开关",
                    ConfigValue = "False",
                    Remark = "True：打开；False：关闭；",
                    CreatedTime = dateTime
                },
                new()
                {
                    ConfigId = YitIdHelper.NextId(),
                    ConfigCode = ConfigConst.MailSmtp,
                    ConfigName = "邮件服务器地址",
                    ConfigValue = "smtp.qq.com",
                    Remark = "QQ：smtp.qq.com，网易：smtp.qq.com；",
                    CreatedTime = dateTime
                },
                new()
                {
                    ConfigId = YitIdHelper.NextId(),
                    ConfigCode = ConfigConst.MailPort,
                    ConfigName = "邮件服务器端口",
                    ConfigValue = "465",
                    Remark = "常规端口：25，加密端口：465/994；",
                    CreatedTime = dateTime
                },
                new()
                {
                    ConfigId = YitIdHelper.NextId(),
                    ConfigCode = ConfigConst.MailEmail,
                    ConfigName = "发件邮箱",
                    ConfigValue = "",
                    Remark = "发送系统邮件的邮箱地址；",
                    CreatedTime = dateTime
                },
                new()
                {
                    ConfigId = YitIdHelper.NextId(),
                    ConfigCode = ConfigConst.MailAuthCode,
                    ConfigName = "邮件授权码",
                    ConfigValue = "",
                    Remark = "发件邮箱的SMTP授权码；",
                    CreatedTime = dateTime
                },
                new()
                {
                    ConfigId = YitIdHelper.NextId(),
                    ConfigCode = ConfigConst.MailDisplayName,
                    ConfigName = "发件人名称",
                    ConfigValue = "FastDotNet",
                    Remark = "系统邮件显示的发件人名称；",
                    CreatedTime = dateTime
                },
                new()
                {
                    ConfigId = YitIdHelper.NextId(),
                    ConfigCode = ConfigConst.MailReceiveEmails,
                    ConfigName = "默认收件邮箱",
                    ConfigValue = "[]",
                    Remark = "默认收件邮箱列表，配置值使用JSON数组格式；",
                    CreatedTime = dateTime
                },
                new()
                {
                    ConfigId = YitIdHelper.NextId(),
                    ConfigCode = ConfigConst.SmsAccessKeyId,
                    ConfigName = "阿里云短信AccessKeyId",
                    ConfigValue = "",
                    Remark = "",
                    CreatedTime = dateTime
                },
                new()
                {
                    ConfigId = YitIdHelper.NextId(),
                    ConfigCode = ConfigConst.SmsAccessKeySecret,
                    ConfigName = "阿里云短信AccessKey密钥",
                    ConfigValue = "",
                    Remark = "",
                    CreatedTime = dateTime
                },
                new()
                {
                    ConfigId = YitIdHelper.NextId(),
                    ConfigCode = ConfigConst.SmsSignName,
                    ConfigName = "阿里云短信签名",
                    ConfigValue = "",
                    Remark = "",
                    CreatedTime = dateTime
                },
                new()
                {
                    ConfigId = YitIdHelper.NextId(),
                    ConfigCode = ConfigConst.SmsVerificationTemplateCode,
                    ConfigName = "阿里云短信验证码模板Code",
                    ConfigValue = "",
                    Remark = "阿里云审核通过的验证码短信模板编码，模板变量为Code；",
                    CreatedTime = dateTime
                },
                new()
                {
                    ConfigId = YitIdHelper.NextId(),
                    ConfigCode = ConfigConst.GaoDeMapKey,
                    ConfigName = "高德地图Key",
                    ConfigValue = "",
                    Remark = null,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();
    }
}