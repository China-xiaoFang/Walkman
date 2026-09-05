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

using System.Diagnostics;
using System.Runtime.InteropServices;
using Fast.DynamicApplication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using SqlSugar;

namespace Fast.Core;

/// <summary>
/// 健康检查
/// </summary>
[ApiDescriptionSettings(false)]
public class HealthApplication : IDynamicApplication
{
    /// <summary>
    /// SqlSugar 客户端
    /// </summary>
    private readonly ISqlSugarClient _repository;

    /// <summary>
    /// 分布式缓存
    /// </summary>
    private readonly IDistributedCache _distributedCache;

    /// <summary>
    /// 健康检查
    /// </summary>
    public HealthApplication(ISqlSugarClient repository, IDistributedCache distributedCache)
    {
        _repository = repository;
        _distributedCache = distributedCache;
    }

    /// <summary>
    /// 健康检查
    /// </summary>
    [HttpGet("/health"), HttpGet("/health/index")]
    [ApiInfo("健康检查", HttpRequestActionEnum.Other)]
    [AllowAnonymous]
    [ResponseEncipher(false)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var databaseStopwatch = Stopwatch.StartNew();
        var databaseHealthy = false;
        var databaseMessage = "数据库连接失败";
        try
        {
            _repository.Ado.CheckConnection();
            databaseHealthy = true;
            databaseMessage = "数据库连接正常";
        }
        catch
        {
            // 健康检查只返回组件状态，避免向匿名调用方暴露数据库异常详情
        }
        finally
        {
            databaseStopwatch.Stop();
        }

        var redisStopwatch = Stopwatch.StartNew();
        var redisHealthy = false;
        var redisMessage = "分布式缓存读写失败";
        var cacheKey = $"Fast:Health:{Guid.NewGuid():N}";
        var cacheValue = Guid.NewGuid()
            .ToByteArray();
        try
        {
            await _distributedCache.SetAsync(cacheKey, cacheValue,
                new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)}, cancellationToken);
            var cachedValue = await _distributedCache.GetAsync(cacheKey, cancellationToken);
            redisHealthy = cachedValue != null && cachedValue.SequenceEqual(cacheValue);
            redisMessage = redisHealthy ? "分布式缓存读写正常" : "分布式缓存读写校验失败";
        }
        catch
        {
            // 健康检查只返回组件状态，避免向匿名调用方暴露 Redis 异常详情
        }
        finally
        {
            redisStopwatch.Stop();
            try
            {
                await _distributedCache.RemoveAsync(cacheKey, CancellationToken.None);
            }
            catch
            {
                // Redis 异常已体现在健康状态中，清理探针键失败时不覆盖原始结果
            }
        }

        var isHealthy = databaseHealthy && redisHealthy;
        return new JsonResult(new
        {
            Status = isHealthy ? "Healthy" : "Unhealthy",
            // 运行时版本
            RuntimeVersion = RuntimeInformation.FrameworkDescription,
            // 当前时间
            CurrentTime = DateTime.Now,
            // 运行时间
            RunTimes = MachineUtil.GetProgramRunTimes(),
            Checks = new
            {
                Database = new
                {
                    Status = databaseHealthy ? "Healthy" : "Unhealthy",
                    Message = databaseMessage,
                    Duration = databaseStopwatch.Elapsed.TotalMilliseconds
                },
                Redis = new
                {
                    Status = redisHealthy ? "Healthy" : "Unhealthy",
                    Message = redisMessage,
                    Duration = redisStopwatch.Elapsed.TotalMilliseconds
                }
            }
        }) {StatusCode = isHealthy ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable};
    }
}