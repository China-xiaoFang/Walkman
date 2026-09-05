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

using System.Globalization;
using System.Reflection;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Fast.Core;

/// <summary>
/// <see cref="IServiceCollection"/> 扩展方法
/// </summary>
public static class IServiceCollectionExtension
{
    /// <summary>
    /// 添加托管服务
    /// </summary>
    /// <returns>用于继续链式配置的服务集合</returns>
    public static IServiceCollection AddHostedService(this IServiceCollection services)
    {
        var IHostedServiceType = typeof(IHostedService);

        var hostedServiceTypes = MAppContext.EffectiveTypes.Where(wh =>
                IHostedServiceType.IsAssignableFrom(wh) && wh.IsClass && !wh.IsInterface && !wh.IsAbstract)
            // 只自动注册项目自身的托管服务；第三方托管服务必须通过对应扩展方法显式启用，
            // 避免 QuartzHostedService 等服务在核心数据库初始化前启动并连接尚未创建的数据库。
            .Where(wh => wh.Assembly.GetName()
                             .Name?.StartsWith($"{nameof(Fast)}.", StringComparison.Ordinal)
                         == true)
            .Select(sl => new
            {
                Type = sl,
                Order = sl.GetCustomAttribute<OrderAttribute>()
                            ?.Order
                        ?? 0
            })
            .OrderBy(ob => ob.Order)
            .Select(sl => sl.Type)
            .ToList();

        var addHostedService = typeof(ServiceCollectionHostedServiceExtensions).GetMethods()
            .Where(wh => wh.Name == nameof(ServiceCollectionHostedServiceExtensions.AddHostedService))
            .Where(wh => wh.IsGenericMethodDefinition)
            .First(wh => wh.GetParameters()
                             .Length
                         == 1);

        foreach (var hostedServiceType in hostedServiceTypes)
        {
            addHostedService.MakeGenericMethod(hostedServiceType)
                .Invoke(null, [services]);
        }

        return services;
    }

    /// <summary>
    /// 注册 API 限流规则
    /// </summary>
    /// <returns>用于继续链式配置的服务集合</returns>
    public static IServiceCollection AddApiRateLimit(this IServiceCollection services)
    {
        // API 限流配置验证
        services.AddConfigurableOptions<ApiRateLimitSettingsOptions>();

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var policyName = httpContext.GetEndpoint()
                    ?.Metadata.GetMetadata<EnableRateLimitingAttribute>()
                    ?.PolicyName;
                var settings = httpContext.RequestServices.GetRequiredService<IOptions<ApiRateLimitSettingsOptions>>()
                    .Value;
                // 仅使用经过可信代理中间件处理的连接地址，不能直接信任客户端伪造的转发头。
                var remoteIp = httpContext.Connection.RemoteIpAddress?.MapToIPv6()
                                   .ToString()
                               ?? "unknown";
                return policyName switch
                {
                    CommonConst.GlobalApiRateLimit => RateLimitPartition.GetSlidingWindowLimiter($"global-ip:{remoteIp}",
                        _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = settings.IpPermitLimit.GetValueOrDefault(60),
                            Window = TimeSpan.FromSeconds(settings.WindowSeconds.GetValueOrDefault(60)),
                            SegmentsPerWindow = Math.Min(6, settings.WindowSeconds.GetValueOrDefault(60)),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        }),
                    CommonConst.LoginApiRateLimit => RateLimitPartition.GetSlidingWindowLimiter($"login-ip:{remoteIp}",
                        _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = settings.LoginIpPermitLimit.GetValueOrDefault(30),
                            Window = TimeSpan.FromSeconds(settings.WindowSeconds.GetValueOrDefault(60)),
                            SegmentsPerWindow = Math.Min(6, settings.WindowSeconds.GetValueOrDefault(60)),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        }),
                    _ => RateLimitPartition.GetNoLimiter("api-ip:not-applicable")
                };
            });
            options.OnRejected = (context, _) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter = Math.Ceiling(retryAfter.TotalSeconds)
                        .ToString(CultureInfo.InvariantCulture);
                }

                return ValueTask.CompletedTask;
            };
            options.AddPolicy<string, GlobalApiRateLimiterPolicy>(CommonConst.GlobalApiRateLimit);
            options.AddPolicy<string, LoginApiRateLimiterPolicy>(CommonConst.LoginApiRateLimit);
        });

        return services;
    }
}