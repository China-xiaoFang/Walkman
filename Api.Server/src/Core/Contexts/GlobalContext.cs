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

using System.Security.Cryptography;
using System.Text;

namespace Fast.Core;

/// <summary>
/// 系统通用上下文
/// </summary>
[SuppressSniffer]
public class GlobalContext
{
    /// <summary>
    /// 客户端标识
    /// </summary>
    public static string ClientIdentity =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(
            $"{FastContext.HttpContext.Connection.RemoteIpAddress?.MapToIPv6()
                   .ToString()
               ?? "unknown"}:{Origin}:{DeviceType}:{DeviceId}")));

    /// <summary>
    /// 来源
    /// </summary>
    public static string Origin
    {
        get
        {
            string result;
            var httpContext = FastContext.HttpContext;
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                result = httpContext.Request.Query[HttpHeaderConst.Origin]
                    .ToString()
                    .UrlDecode();
            }
            else
            {
                result = httpContext.Request.Headers[HttpHeaderConst.Origin]
                    .ToString()
                    .UrlDecode();
            }

            if (!string.IsNullOrWhiteSpace(result))
                return result;

            throw new UserFriendlyException("未知的设备信息！");
        }
    }

    /// <summary>
    /// 设备类型
    /// </summary>
    public static AppEnvironmentEnum DeviceType
    {
        get
        {
            string result;
            var httpContext = FastContext.HttpContext;
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                result = httpContext.Request.Query[HttpHeaderConst.DeviceType]
                    .ToString()
                    .UrlDecode();
            }
            else
            {
                result = httpContext.Request.Headers[HttpHeaderConst.DeviceType]
                    .ToString()
                    .UrlDecode();
            }

            if (!string.IsNullOrWhiteSpace(result) && Enum.TryParse<AppEnvironmentEnum>(result, true, out var environment))
                return environment;

            throw new UserFriendlyException("未知的设备信息！");
        }
    }

    /// <summary>
    /// 设备Id
    /// </summary>
    public static string DeviceId
    {
        get
        {
            string result;
            var httpContext = FastContext.HttpContext;
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                result = httpContext.Request.Query[HttpHeaderConst.DeviceId]
                    .ToString()
                    .UrlDecode()
                    .Trim();
            }
            else
            {
                result = httpContext.Request.Headers[HttpHeaderConst.DeviceId]
                    .ToString()
                    .UrlDecode()
                    .Trim();
            }

            if (!string.IsNullOrWhiteSpace(result))
                return result;

            throw new UserFriendlyException("未知的设备信息！");
        }
    }

    /// <summary>
    /// 是否为Web端
    /// </summary>
    public static bool IsWeb => (DeviceType & AppEnvironmentEnum.Web) != 0;

    /// <summary>
    /// 是否为桌面端
    /// </summary>
    public static bool IsDesktop => (DeviceType & AppEnvironmentEnum.Desktop) != 0;

    /// <summary>
    /// 是否为移动端
    /// </summary>
    public static bool IsMobile => (DeviceType & AppEnvironmentEnum.MobileThree) != 0;
}