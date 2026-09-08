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

using System.Net;
using System.Net.Sockets;

namespace Fast.Core;

/// <summary>
/// 元数据上下文
/// </summary>
[SuppressSniffer]
public class MetadataContext
{
    /// <summary>
    /// 元数据信息
    /// </summary>
    public static ServerMetadataInfo MetadataInfo
    {
        get
        {
            field ??= GetServerMetadata()
                .Result;

            return field;
        }
    }

    /// <summary>
    /// 获取服务器Ip地址
    /// </summary>
    /// <remarks>从Metadata Service中获取</remarks>
    /// <returns>服务器元数据；无法识别云平台时返回本机信息</returns>
    private static async Task<ServerMetadataInfo> GetServerMetadata()
    {
        // 阿里云
        try
        {
            var (instanceName, _) =
                await RemoteRequestUtil.GetAsync("http://100.100.100.200/latest/meta-data/instance-id", timeout: 1);
            var (region, _) = await RemoteRequestUtil.GetAsync("http://100.100.100.200/latest/meta-data/region-id", timeout: 1);
            var (zone, _) = await RemoteRequestUtil.GetAsync("http://100.100.100.200/latest/meta-data/zone-id", timeout: 1);
            var (innerIp, _) =
                await RemoteRequestUtil.GetAsync("http://100.100.100.200/latest/meta-data/private-ipv4", timeout: 1);
            var (publicIp, _) = await RemoteRequestUtil.GetAsync("http://100.100.100.200/latest/meta-data/eipv4", timeout: 1);
            return new ServerMetadataInfo
            {
                Provider = "AliYun",
                InstanceName = instanceName,
                Region = region,
                Zone = zone,
                InnerIp = innerIp,
                PublicIp = publicIp
            };
        }
        catch
        {
            // ignored
        }

        // 腾讯云
        try
        {
            var (instanceName, _) =
                await RemoteRequestUtil.GetAsync("http://metadata.tencentyun.com/latest/meta-data/instance-id", timeout: 1);
            var (region, _) = await RemoteRequestUtil.GetAsync("http://metadata.tencentyun.com/latest/meta-data/placement/region",
                timeout: 1);
            var (zone, _) =
                await RemoteRequestUtil.GetAsync("http://metadata.tencentyun.com/latest/meta-data/placement/zone", timeout: 1);
            var (innerIp, _) =
                await RemoteRequestUtil.GetAsync("http://metadata.tencentyun.com/latest/meta-data/local-ipv4", timeout: 1);
            var (publicIp, _) =
                await RemoteRequestUtil.GetAsync("http://metadata.tencentyun.com/latest/meta-data/public-ipv4", timeout: 1);
            return new ServerMetadataInfo
            {
                Provider = "TencentCloud",
                InstanceName = instanceName,
                Region = region,
                Zone = zone,
                InnerIp = innerIp,
                PublicIp = publicIp
            };
        }
        catch
        {
            // ignored
        }

        // 华为云
        try
        {
            var (instanceName, _) =
                await RemoteRequestUtil.GetAsync("http://169.254.169.254/latest/meta-data/instance-id", timeout: 1);
            var (region, _) = await RemoteRequestUtil.GetAsync("http://169.254.169.254/latest/meta-data/region", timeout: 1);
            var (zone, _) =
                await RemoteRequestUtil.GetAsync("http://169.254.169.254/latest/meta-data/availability-zone", timeout: 1);
            var (innerIp, _) = await RemoteRequestUtil.GetAsync("http://169.254.169.254/latest/meta-data/local-ipv4", timeout: 1);
            var (publicIp, _) =
                await RemoteRequestUtil.GetAsync("http://169.254.169.254/latest/meta-data/public-ipv4s", timeout: 1);

            return new ServerMetadataInfo
            {
                Provider = "HuaweiCloud",
                InstanceName = instanceName,
                Region = region,
                Zone = zone,
                InnerIp = innerIp,
                PublicIp = publicIp
            };
        }
        catch
        {
            // ignored
        }

        return new ServerMetadataInfo
        {
            Provider = "Local",
            InstanceName = Environment.MachineName,
            Region = "Local",
            Zone = "Local",
            InnerIp = (await Dns.GetHostEntryAsync(Dns.GetHostName())).AddressList
                      .FirstOrDefault(f => f.AddressFamily == AddressFamily.InterNetwork)
                      ?.ToString()
                      ?? "127.0.0.1"
        };
    }
}