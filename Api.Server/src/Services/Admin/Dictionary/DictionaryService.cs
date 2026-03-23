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
using Fast.Admin.Service.Dictionary.Dto;
using Fast.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yitter.IdGenerator;

namespace Fast.Admin.Service.Dictionary;

/// <summary>
/// <see cref="DictionaryService"/> 字典服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Center, Name = "Dictionary", Order = 995)]
public class DictionaryService : IDynamicApplication
{
    private readonly ICache<CenterCCL> _centerCache;
    private readonly ISqlSugarRepository<DictionaryTypeModel> _typeRepository;
    private readonly ISqlSugarRepository<DictionaryItemModel> _itemRepository;

    public DictionaryService(ICache<CenterCCL> centerCache, ISqlSugarRepository<DictionaryTypeModel> typeRepository,
        ISqlSugarRepository<DictionaryItemModel> itemRepository)
    {
        _centerCache = centerCache;
        _typeRepository = typeRepository;
        _itemRepository = itemRepository;
    }

    /// <summary>
    /// 获取字典
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取字典", HttpRequestActionEnum.Query)]
    [AllowAnonymous, DisabledRequestLog]
    public async Task<Dictionary<string, List<FaTableEnumColumnCtx>>> QueryDictionary()
    {
        var dictionaryTypeList = await _centerCache.GetAndSetAsync(CacheConst.Center.Dictionary, async () =>
        {
            return await _typeRepository.Entities.Includes(e => e.DictionaryItemList)
                .Where(wh => wh.Status == CommonStatusEnum.Enable)
                .Select(sl => new
                {
                    sl.DictionaryKey,
                    sl.ValueType,
                    DictionaryItemList = sl.DictionaryItemList.OrderBy(ob => ob.Order)
                        .Select(iSl => new
                        {
                            iSl.Label,
                            iSl.Value,
                            iSl.Type,
                            iSl.Order,
                            iSl.Tips,
                            iSl.Visible,
                            iSl.Status
                        })
                        .ToList()
                })
                .ToListAsync();
        });

        return dictionaryTypeList.ToDictionary(sl => sl.DictionaryKey, sl => sl.DictionaryItemList.Select(iSl =>
            {
                object localValue = sl.ValueType switch
                {
                    DictionaryValueTypeEnum.Int => int.Parse(iSl.Value),
                    DictionaryValueTypeEnum.Long => long.Parse(iSl.Value),
                    DictionaryValueTypeEnum.Boolean => iSl.Value.Equals("true", StringComparison.OrdinalIgnoreCase),
                    _ => iSl.Value
                };

                return new FaTableEnumColumnCtx
                {
                    Label = iSl.Label,
                    Value = localValue,
                    Show = iSl.Visible,
                    Disabled = iSl.Status == CommonStatusEnum.Disable,
                    Tips = iSl.Tips,
                    Type = iSl.Type.GetDescription()
                };
            })
            .ToList());
    }

    /// <summary>
    /// 字典分页选择器
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("字典分页选择器", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.Dictionary.Paged)]
    public async Task<PagedResult<ElSelectorOutput<long>>> SelectorPaged(PagedInput input)
    {
        var pagedData = await _typeRepository.Entities.WhereIF(!string.IsNullOrWhiteSpace(input.SearchValue),
                wh => wh.DictionaryName.Contains(input.SearchValue))
            .OrderBy(ob => ob.DictionaryName)
            .Select(sl => new {sl.DictionaryName, sl.DictionaryId, sl.ValueType})
            .ToPagedListAsync(input);

        return pagedData.ToPagedData(sl => new ElSelectorOutput<long>
        {
            Label = sl.DictionaryName, Value = sl.DictionaryId, Data = new {sl.ValueType}
        });
    }

    /// <summary>
    /// 获取字典分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取字典分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.Dictionary.Paged)]
    public async Task<PagedResult<QueryDictionaryPagedOutput>> QueryDictionaryPaged(PagedInput input)
    {
        return await _typeRepository.Entities.OrderByIF(input.IsOrderBy, ob => ob.CreatedTime, OrderByType.Desc)
            .Select(sl => new QueryDictionaryPagedOutput
            {
                DictionaryId = sl.DictionaryId,
                DictionaryKey = sl.DictionaryKey,
                DictionaryName = sl.DictionaryName,
                ValueType = sl.ValueType,
                HasFlags = sl.HasFlags,
                Status = sl.Status,
                Remark = sl.Remark,
                DepartmentName = sl.DepartmentName,
                CreatedUserName = sl.CreatedUserName,
                CreatedTime = sl.CreatedTime,
                UpdatedUserName = sl.UpdatedUserName,
                UpdatedTime = sl.UpdatedTime,
                RowVersion = sl.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取字典详情
    /// </summary>
    /// <param name="dictionaryId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取字典详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Dictionary.Detail)]
    public async Task<QueryDictionaryDetailOutput> QueryDictionaryDetail([Required(ErrorMessage = "字典Id不能为空")] long? dictionaryId)
    {
        var result = await _typeRepository.Entities.Includes(e => e.DictionaryItemList)
            .Where(wh => wh.DictionaryId == dictionaryId)
            .Select(sl => new QueryDictionaryDetailOutput
            {
                DictionaryId = sl.DictionaryId,
                DictionaryKey = sl.DictionaryKey,
                DictionaryName = sl.DictionaryName,
                ValueType = sl.ValueType,
                HasFlags = sl.HasFlags,
                Status = sl.Status,
                Remark = sl.Remark,
                DepartmentName = sl.DepartmentName,
                CreatedUserName = sl.CreatedUserName,
                CreatedTime = sl.CreatedTime,
                UpdatedUserName = sl.UpdatedUserName,
                UpdatedTime = sl.UpdatedTime,
                RowVersion = sl.RowVersion,
                DictionaryItemList = sl.DictionaryItemList.OrderBy(ob => ob.Order)
                    .Select(iSl => new EditDictionaryItemInput
                    {
                        DictionaryItemId = iSl.DictionaryItemId,
                        Label = iSl.Label,
                        Value = iSl.Value,
                        Type = iSl.Type,
                        Order = iSl.Order,
                        Tips = iSl.Tips,
                        Visible = iSl.Visible,
                        Status = iSl.Status
                    })
                    .ToList()
            })
            .SingleAsync();

        if (result == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        return result;
    }

    /// <summary>
    /// 添加字典
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加字典", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.Dictionary.Add)]
    public async Task AddDictionary(AddDictionaryInput input)
    {
        // 判断key是否重复
        if (await _typeRepository.AnyAsync(a => a.DictionaryKey == input.DictionaryKey))
        {
            throw new UserFriendlyException("字典Key已存在！");
        }

        var dictionaryTypeModel = new DictionaryTypeModel
        {
            DictionaryId = YitIdHelper.NextId(),
            DictionaryKey = input.DictionaryKey,
            DictionaryName = input.DictionaryName,
            ValueType = input.ValueType,
            HasFlags = false,
            Status = CommonStatusEnum.Enable,
            Remark = input.Remark
        };

        var dictionaryItemList = new List<DictionaryItemModel>();
        foreach (var item in input.DictionaryItemList)
        {
            dictionaryItemList.Add(new DictionaryItemModel
            {
                DictionaryId = dictionaryTypeModel.DictionaryId,
                Label = item.Label,
                Value = item.Value,
                Type = item.Type,
                Order = item.Order,
                Tips = item.Tips,
                Visible = item.Visible,
                Status = CommonStatusEnum.Enable
            });
        }

        await _typeRepository.Ado.UseTranAsync(async () =>
        {
            await _typeRepository.InsertAsync(dictionaryTypeModel);
            await _itemRepository.InsertAsync(dictionaryItemList);
        }, ex => throw ex);

        // 删除缓存
        await _centerCache.DelAsync(CacheConst.Center.Dictionary);
    }

    /// <summary>
    /// 编辑字典
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑字典", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Dictionary.Edit)]
    public async Task EditDictionary(EditDictionaryInput input)
    {
        var dictionaryTypeModel = await _typeRepository.Entities.Includes(e => e.DictionaryItemList)
            .InSingleAsync(input.DictionaryId);
        if (dictionaryTypeModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        dictionaryTypeModel.DictionaryName = input.DictionaryName;
        dictionaryTypeModel.ValueType = input.ValueType;
        dictionaryTypeModel.Status = input.Status;
        dictionaryTypeModel.Remark = input.Remark;
        dictionaryTypeModel.RowVersion = input.RowVersion;

        var oldDictionaryItemList = dictionaryTypeModel.DictionaryItemList ?? [];
        var newDictionaryItemList = input.DictionaryItemList ?? [];

        // 新增的项
        var addDictionaryItemList = newDictionaryItemList.Where(wh => wh.DictionaryItemId == null)
            .Select(sl => new DictionaryItemModel
            {
                DictionaryId = dictionaryTypeModel.DictionaryId,
                Label = sl.Label,
                Value = sl.Value,
                Type = sl.Type,
                Order = sl.Order,
                Tips = sl.Tips,
                Visible = sl.Visible,
                Status = CommonStatusEnum.Enable
            })
            .ToList();

        // 更新的项
        var updateDictionaryItemList = newDictionaryItemList
            .Where(wh => oldDictionaryItemList.Any(a => a.DictionaryItemId == wh.DictionaryItemId))
            .Select(sl =>
            {
                var dictionaryItemModel = oldDictionaryItemList.First(f => f.DictionaryItemId == sl.DictionaryItemId);
                dictionaryItemModel.Label = sl.Label;
                dictionaryItemModel.Value = sl.Value;
                dictionaryItemModel.Type = sl.Type;
                dictionaryItemModel.Order = sl.Order;
                dictionaryItemModel.Tips = sl.Tips;
                dictionaryItemModel.Visible = sl.Visible;
                dictionaryItemModel.Status = sl.Status;
                return dictionaryItemModel;
            })
            .ToList();

        // 删除的项
        var deleteDictionaryItemList = oldDictionaryItemList
            .Where(wh => newDictionaryItemList.All(a => a.DictionaryItemId != wh.DictionaryItemId))
            .ToList();

        await _typeRepository.Ado.UseTranAsync(async () =>
        {
            await _typeRepository.UpdateAsync(dictionaryTypeModel);
            await _itemRepository.DeleteAsync(deleteDictionaryItemList);
            await _itemRepository.UpdateAsync(updateDictionaryItemList);
            await _itemRepository.InsertAsync(addDictionaryItemList);
        }, ex => throw ex);

        // 删除缓存
        await _centerCache.DelAsync(CacheConst.Center.Dictionary);
    }

    /// <summary>
    /// 删除字典
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除字典", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.Dictionary.Delete)]
    public async Task DeleteDictionary(DictionaryIdInput input)
    {
        var dictionaryTypeModel = await _typeRepository.Entities.Includes(e => e.DictionaryItemList)
            .InSingleAsync(input.DictionaryId);
        if (dictionaryTypeModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        await _typeRepository.Ado.UseTranAsync(async () =>
        {
            await _itemRepository.DeleteAsync(dictionaryTypeModel.DictionaryItemList);
            await _typeRepository.DeleteAsync(dictionaryTypeModel);
        }, ex => throw ex);

        // 删除缓存
        await _centerCache.DelAsync(CacheConst.Center.Dictionary);
    }
}