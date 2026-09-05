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
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;

namespace Fast.Core;

/// <summary>
/// API 限流规则基类
/// </summary>
internal abstract class ApiRateLimiterPolicy : IRateLimiterPolicy<string>
{
    /// <summary>
    /// 限流分区前缀
    /// </summary>
    protected string PartitionPrefix { get; }

    private readonly int _permitLimit;
    private readonly int _windowSeconds;

    /// <summary>
    /// API 限流规则基类
    /// </summary>
    /// <param name="partitionPrefix">分区前缀</param>
    /// <param name="permitLimit">单个限流分区的请求限额</param>
    /// <param name="windowSeconds">统计窗口秒数</param>
    protected ApiRateLimiterPolicy(string partitionPrefix, int permitLimit, int windowSeconds)
    {
        PartitionPrefix = partitionPrefix;
        _permitLimit = permitLimit;
        _windowSeconds = windowSeconds;
    }

    /// <inheritdoc />
    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        var partitionKey = GetPartitionKey(httpContext);
        return RateLimitPartition.GetSlidingWindowLimiter(partitionKey,
            _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = _permitLimit,
                Window = TimeSpan.FromSeconds(_windowSeconds),
                SegmentsPerWindow = Math.Min(6, _windowSeconds),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    }

    /// <summary>
    /// 获取限流分区键
    /// </summary>
    /// <remarks>默认按照 Ip 与设备Id组合分区，供登录和未登录请求使用。</remarks>
    protected virtual string GetPartitionKey(HttpContext httpContext)
    {
        // 转发头必须先经过可信代理中间件处理，不能直接用任意X-Forwarded-For绕过限流。
        var ipAddress = httpContext.Connection.RemoteIpAddress?.MapToIPv6()
                            .ToString()
                        ?? "unknown";
        var deviceId = httpContext.Request.Headers[HttpHeaderConst.DeviceId]
            .ToString()
            .UrlDecode()
            .Trim();

        // 未提供设备Id时空字符串会生成固定摘要，确保匿名请求仍受组合限流约束；
        // 请求头由客户端控制，使用固定长度摘要作为分区键，避免超长设备Id持续占用内存
        return $"{PartitionPrefix}:ip:{ipAddress}:device:{GetFingerprint(deviceId)}";
    }

    /// <summary>
    /// 获取固定长度的限流分区指纹
    /// </summary>
    protected static string GetFingerprint(string value)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    }

    /// <inheritdoc />
    public Func<OnRejectedContext, CancellationToken, ValueTask> OnRejected => null;
}