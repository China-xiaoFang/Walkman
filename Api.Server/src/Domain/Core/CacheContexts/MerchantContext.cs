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
/// <see cref="MerchantContext"/> 商户号上下文
/// </summary>
[SuppressSniffer]
public class MerchantContext
{
    /// <summary>
    /// 缓存
    /// </summary>
    internal static ICache<CenterCCL> centerCache = FastContext.GetService<ICache<CenterCCL>>();

    /// <summary>
    /// 日志
    /// </summary>
    internal static ILogger logger = FastContext.GetService<ILogger<MerchantContext>>();

    /// <summary>
    /// 获取商户号
    /// </summary>
    /// <param name="merchantNo"><see cref="string"/> 商户号</param>
    /// <param name="throwError"><see cref="bool"/> 抛出错误</param>
    /// <returns></returns>
    public static MerchantModel GetMerchantSync(string merchantNo, bool throwError = true)
    {
        if (string.IsNullOrWhiteSpace(merchantNo))
        {
            throw new UserFriendlyException("商户号不能为空！");
        }

        // 优先从 HttpContext.Items 中获取
        if (FastContext.HttpContext?.Items.TryGetValue($"{nameof(Fast)}.{nameof(MerchantModel.MerchantNo)}.{merchantNo}",
                out var obj)
            == true
            && obj is MerchantModel merchantModel)
        {
            return merchantModel;
        }

        var cacheKey = CacheConst.GetCacheKey(CacheConst.Center.Merchant, merchantNo);

        merchantModel = centerCache.GetAndSet(cacheKey, () =>
        {
            var repository = FastContext.GetService<ISqlSugarClient>();

            var result = repository.Queryable<MerchantModel>()
                .Where(wh => wh.MerchantNo == merchantNo)
                .Single();

            if (result == null && throwError)
            {
                var message = $"未能找到对应商户号【{merchantNo}】信息！";
                logger.LogError($"MerchantNo：{merchantNo}；{message}");
                throw new UserFriendlyException(message);
            }

            return result;
        });

        if (FastContext.HttpContext != null)
        {
            // 放入 HttpContext.Items 中
            FastContext.HttpContext.Items[$"{nameof(Fast)}.{nameof(MerchantModel.MerchantNo)}.{merchantNo}"] = merchantModel;
        }

        return merchantModel;
    }

    /// <summary>
    /// 获取商户号
    /// </summary>
    /// <param name="merchantNo"><see cref="string"/> 商户号</param>
    /// <param name="throwError"><see cref="bool"/> 抛出错误</param>
    /// <returns></returns>
    public static async Task<MerchantModel> GetMerchant(string merchantNo, bool throwError = true)
    {
        if (string.IsNullOrWhiteSpace(merchantNo))
        {
            throw new UserFriendlyException("商户号不能为空！");
        }

        // 优先从 HttpContext.Items 中获取
        if (FastContext.HttpContext?.Items.TryGetValue($"{nameof(Fast)}.{nameof(MerchantModel.MerchantNo)}.{merchantNo}",
                out var obj)
            == true
            && obj is MerchantModel merchantModel)
        {
            return merchantModel;
        }

        var cacheKey = CacheConst.GetCacheKey(CacheConst.Center.Merchant, merchantNo);

        merchantModel = await centerCache.GetAndSetAsync(cacheKey, async () =>
        {
            var repository = FastContext.GetService<ISqlSugarClient>();

            var result = await repository.Queryable<MerchantModel>()
                .Where(wh => wh.MerchantNo == merchantNo)
                .SingleAsync();

            if (result == null && throwError)
            {
                var message = $"未能找到对应商户号【{merchantNo}】信息！";
                logger.LogError($"MerchantNo：{merchantNo}；{message}");
                throw new UserFriendlyException(message);
            }

            return result;
        });

        if (FastContext.HttpContext != null)
        {
            // 放入 HttpContext.Items 中
            FastContext.HttpContext.Items[$"{nameof(Fast)}.{nameof(MerchantModel.MerchantNo)}.{merchantNo}"] = merchantModel;
        }

        return merchantModel;
    }

    /// <summary>
    /// 删除商户号
    /// </summary>
    /// <param name="merchantNo"><see cref="string"/> 商户号</param>
    /// <returns></returns>
    public static async Task DeleteMerchant(string merchantNo)
    {
        if (string.IsNullOrWhiteSpace(merchantNo))
        {
            throw new UserFriendlyException("商户号不能为空！");
        }

        if (FastContext.HttpContext != null)
        {
            // 删除 HttpContext.Items 中的
            if (FastContext.HttpContext.Items.ContainsKey($"{nameof(Fast)}.{nameof(MerchantModel.MerchantNo)}.{merchantNo}"))
            {
                FastContext.HttpContext.Items.Remove($"{nameof(Fast)}.{nameof(MerchantModel.MerchantNo)}.{merchantNo}");
            }
        }

        var cacheKey = CacheConst.GetCacheKey(CacheConst.Center.Merchant, merchantNo);

        await centerCache.DelAsync(cacheKey);
    }

    /// <summary>
    /// 删除所有商户号
    /// </summary>
    /// <returns></returns>
    public static async Task DeleteAllMerchant()
    {
        if (FastContext.HttpContext != null)
        {
            // 清空 HttpContext.Items 中的
            var keys = FastContext.HttpContext.Items.Keys.Where(wh =>
                    wh is string key && key.StartsWith($"{nameof(Fast)}.{nameof(MerchantModel.MerchantNo)}."))
                .ToList();
            foreach (var key in keys)
            {
                FastContext.HttpContext.Items.Remove(key);
            }
        }

        var cacheKey = CacheConst.GetCacheKey(CacheConst.Center.Config, "*");
        await centerCache.DelByPatternAsync(cacheKey);
    }
}