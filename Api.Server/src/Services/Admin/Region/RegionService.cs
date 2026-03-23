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
using Fast.Admin.Enum;
using Fast.Cache;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Service.Region;

/// <summary>
/// <see cref="RegionService"/> 地区服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Center, Name = "region")]
public class RegionService : IDynamicApplication
{
    private readonly ICache<CenterCCL> _cache;
    private readonly ISqlSugarRepository<RegionModel> _repository;

    public RegionService(ICache<CenterCCL> cache, ISqlSugarRepository<RegionModel> repository)
    {
        _cache = cache;
        _repository = repository;
    }

    /// <summary>
    /// 地区选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("地区选择器", HttpRequestActionEnum.Query)]
    public async Task<List<ElSelectorOutput<long>>> RegionSelector()
    {
        return await _cache.GetAndSetAsync(CacheConst.Center.Region, async () =>
        {
            var data = await _repository.Entities.Where(wh =>
                    (wh.RegionLevel & (RegionLevelEnum.Province | RegionLevelEnum.City | RegionLevelEnum.District)) != 0)
                .OrderBy(ob => ob.RegionName)
                .Select(sl => new
                {
                    sl.RegionId,
                    sl.ParentId,
                    sl.RegionCode,
                    sl.RegionName,
                    sl.AreaCode,
                    sl.PostalCode,
                    sl.Latitude,
                    sl.Longitude,
                    sl.FullRegionName
                })
                .ToListAsync();

            return data.Select(sl => new ElSelectorOutput<long>
                {
                    Value = sl.RegionId,
                    Label = sl.RegionName,
                    ParentId = sl.ParentId,
                    Data = new
                    {
                        sl.RegionCode,
                        sl.AreaCode,
                        sl.PostalCode,
                        sl.Latitude,
                        sl.Longitude,
                        sl.FullRegionName
                    }
                })
                .ToList()
                .Build();
        });
    }

    /// <summary>
    /// 省份选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("省份选择器", HttpRequestActionEnum.Query)]
    public async Task<List<ElSelectorOutput<long>>> ProvinceSelector()
    {
        return await _cache.GetAndSetAsync(CacheConst.Center.Province, async () =>
        {
            var data = await _repository.Entities.Where(wh => wh.RegionLevel == RegionLevelEnum.Province)
                .OrderBy(ob => ob.RegionName)
                .Select(sl => new
                {
                    sl.RegionId,
                    sl.ParentId,
                    sl.RegionCode,
                    sl.RegionName,
                    sl.Latitude,
                    sl.Longitude,
                    sl.FullRegionName
                })
                .ToListAsync();

            return data.Select(sl => new ElSelectorOutput<long>
                {
                    Value = sl.RegionId,
                    Label = sl.RegionName,
                    ParentId = sl.ParentId,
                    Data = new {sl.RegionCode, sl.Latitude, sl.Longitude, sl.FullRegionName}
                })
                .ToList()
                .Build();
        });
    }

    /// <summary>
    /// 城市选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("城市选择器", HttpRequestActionEnum.Query)]
    public async Task<List<ElSelectorOutput<long>>> CitySelector()
    {
        return await _cache.GetAndSetAsync(CacheConst.Center.City, async () =>
        {
            var data = await _repository.Entities.Where(wh =>
                    (wh.RegionLevel & (RegionLevelEnum.Province | RegionLevelEnum.City)) != 0)
                .OrderBy(ob => ob.RegionName)
                .Select(sl => new
                {
                    sl.RegionId,
                    sl.ParentId,
                    sl.RegionCode,
                    sl.RegionName,
                    sl.AreaCode,
                    sl.PostalCode,
                    sl.Latitude,
                    sl.Longitude,
                    sl.FullRegionName
                })
                .ToListAsync();

            return data.Select(sl => new ElSelectorOutput<long>
                {
                    Value = sl.RegionId,
                    Label = sl.RegionName,
                    ParentId = sl.ParentId,
                    Data = new
                    {
                        sl.RegionCode,
                        sl.AreaCode,
                        sl.PostalCode,
                        sl.Latitude,
                        sl.Longitude,
                        sl.FullRegionName
                    }
                })
                .ToList()
                .Build();
        });
    }
}