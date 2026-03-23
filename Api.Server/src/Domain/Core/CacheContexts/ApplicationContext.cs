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

using Fast.Admin.Entity;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Fast.Core;

/// <summary>
/// <see cref="ApplicationContext"/> 应用上下文
/// </summary>
[SuppressSniffer]
public class ApplicationContext
{
    /// <summary>
    /// 缓存
    /// </summary>
    internal static ICache<CenterCCL> centerCache = FastContext.GetService<ICache<CenterCCL>>();

    /// <summary>
    /// 日志
    /// </summary>
    internal static ILogger logger = FastContext.GetService<ILogger<ApplicationContext>>();

    /// <summary>
    /// 获取应用
    /// </summary>
    /// <param name="openId"><see cref="string"/> 应用标识</param>
    /// <param name="throwError"><see cref="bool"/> 抛出错误</param>
    /// <returns></returns>
    public static ApplicationOpenIdModel GetApplicationSync(string openId, bool throwError = true)
    {
        if (string.IsNullOrWhiteSpace(openId))
        {
            throw new UserFriendlyException("应用标识不能为空！");
        }

        // 优先从 HttpContext.Items 中获取
        if (FastContext.HttpContext?.Items.TryGetValue($"{nameof(Fast)}.{nameof(ApplicationOpenIdModel.OpenId)}.{openId}",
                out var obj)
            == true
            && obj is ApplicationOpenIdModel applicationOpenIdModel)
        {
            return applicationOpenIdModel;
        }

        var cacheKey = CacheConst.GetCacheKey(CacheConst.Center.App, openId);

        applicationOpenIdModel = centerCache.GetAndSet(cacheKey, () =>
        {
            var repository = FastContext.GetService<ISqlSugarClient>();

            var result = repository.Queryable<ApplicationOpenIdModel>()
                .Where(wh => wh.OpenId == openId)
                .Single();

            if (result == null && throwError)
            {
                var message = $"未能找到对应应用【{openId}】信息！";
                logger.LogError($"OpenId：{openId}；{message}");
                throw new UserFriendlyException(message);
            }

            return result;
        });

        if (FastContext.HttpContext != null)
        {
            // 放入 HttpContext.Items 中
            FastContext.HttpContext.Items[$"{nameof(Fast)}.{nameof(ApplicationOpenIdModel.OpenId)}.{openId}"] =
                applicationOpenIdModel;
        }

        return applicationOpenIdModel;
    }

    /// <summary>
    /// 获取应用
    /// </summary>
    /// <param name="openId"><see cref="string"/> 应用标识</param>
    /// <param name="throwError"><see cref="bool"/> 抛出错误</param>
    /// <returns></returns>
    public static async Task<ApplicationOpenIdModel> GetApplication(string openId, bool throwError = true)
    {
        if (string.IsNullOrWhiteSpace(openId))
        {
            throw new UserFriendlyException("应用标识不能为空！");
        }

        // 优先从 HttpContext.Items 中获取
        if (FastContext.HttpContext?.Items.TryGetValue($"{nameof(Fast)}.{nameof(ApplicationOpenIdModel.OpenId)}.{openId}",
                out var obj)
            == true
            && obj is ApplicationOpenIdModel applicationOpenIdModel)
        {
            return applicationOpenIdModel;
        }

        var cacheKey = CacheConst.GetCacheKey(CacheConst.Center.App, openId);

        applicationOpenIdModel = await centerCache.GetAndSetAsync(cacheKey, async () =>
        {
            var repository = FastContext.GetService<ISqlSugarClient>();

            var result = await repository.Queryable<ApplicationOpenIdModel>()
                .Where(wh => wh.OpenId == openId)
                .SingleAsync();

            if (result == null && throwError)
            {
                var message = $"未能找到对应应用【{openId}】信息！";
                logger.LogError($"OpenId：{openId}；{message}");
                throw new UserFriendlyException(message);
            }

            return result;
        });

        if (FastContext.HttpContext != null)
        {
            // 放入 HttpContext.Items 中
            FastContext.HttpContext.Items[$"{nameof(Fast)}.{nameof(ApplicationOpenIdModel.OpenId)}.{openId}"] =
                applicationOpenIdModel;
        }

        return applicationOpenIdModel;
    }

    /// <summary>
    /// 删除应用
    /// </summary>
    /// <param name="openId"><see cref="string"/> 应用标识</param>
    /// <returns></returns>
    public static async Task DeleteApplication(string openId)
    {
        if (string.IsNullOrWhiteSpace(openId))
        {
            throw new UserFriendlyException("应用标识不能为空！");
        }

        if (FastContext.HttpContext != null)
        {
            // 删除 HttpContext.Items 中的
            if (FastContext.HttpContext.Items.ContainsKey($"{nameof(Fast)}.{nameof(ApplicationOpenIdModel.OpenId)}.{openId}"))
            {
                FastContext.HttpContext.Items.Remove($"{nameof(Fast)}.{nameof(ApplicationOpenIdModel.OpenId)}.{openId}");
            }
        }

        var cacheKey = CacheConst.GetCacheKey(CacheConst.Center.App, openId);

        await centerCache.DelAsync(cacheKey);
    }

    /// <summary>
    /// 删除所有应用
    /// </summary>
    /// <returns></returns>
    public static async Task DeleteAllApplication()
    {
        if (FastContext.HttpContext != null)
        {
            // 清空 HttpContext.Items 中的
            var keys = FastContext.HttpContext.Items.Keys.Where(wh =>
                    wh is string key && key.StartsWith($"{nameof(Fast)}.{nameof(ApplicationOpenIdModel.OpenId)}."))
                .ToList();
            foreach (var key in keys)
            {
                FastContext.HttpContext.Items.Remove(key);
            }
        }

        var cacheKey = CacheConst.GetCacheKey(CacheConst.Center.App, "*");
        await centerCache.DelByPatternAsync(cacheKey);
    }
}