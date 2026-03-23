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
using Fast.Admin.Service.Table.Dto;
using Fast.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Yitter.IdGenerator;

namespace Fast.Admin.Service.Table;

/// <summary>
/// <see cref="TableService"/> 表格服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Center, Name = "table", Order = 994)]
public class TableService : IDynamicApplication
{
    private readonly IUser _user;
    private readonly ICache<CenterCCL> _centerCache;
    private readonly ISqlSugarRepository<TableConfigModel> _tableRepository;
    private readonly ISqlSugarRepository<TableColumnConfigModel> _columnRepository;
    private readonly ISqlSugarRepository<TableColumnConfigCacheModel> _columnCacheRepository;

    public TableService(IUser user, ICache<CenterCCL> center, ISqlSugarRepository<TableConfigModel> tableRepository,
        ISqlSugarRepository<TableColumnConfigModel> columnRepository,
        ISqlSugarRepository<TableColumnConfigCacheModel> columnCacheRepository)
    {
        _user = user;
        _centerCache = center;
        _tableRepository = tableRepository;
        _columnRepository = columnRepository;
        _columnCacheRepository = columnCacheRepository;
    }

    /// <summary>
    /// 获取表格配置分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取表格配置分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.Table.Paged)]
    public async Task<PagedResult<QueryTableConfigPagedOutput>> QueryTableConfigPaged(PagedInput input)
    {
        return await _tableRepository.Entities.OrderByIF(input.IsOrderBy, ob => ob.CreatedTime, OrderByType.Desc)
            .Select(sl => new QueryTableConfigPagedOutput
            {
                TableId = sl.TableId,
                TableKey = sl.TableKey,
                TableName = sl.TableName,
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
    /// 获取表格配置详情
    /// </summary>
    /// <param name="tableId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取表格配置详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Table.Detail)]
    public async Task<QueryTableConfigDetailOutput> QueryTableConfigDetail([Required(ErrorMessage = "表格Id不能为空")] long? tableId)
    {
        var result = await _tableRepository.Entities.Where(wh => wh.TableId == tableId)
            .Select(sl => new QueryTableConfigDetailOutput
            {
                TableId = sl.TableId,
                TableKey = sl.TableKey,
                TableName = sl.TableName,
                Remark = sl.Remark,
                DepartmentName = sl.DepartmentName,
                CreatedUserName = sl.CreatedUserName,
                CreatedTime = sl.CreatedTime,
                UpdatedUserName = sl.UpdatedUserName,
                UpdatedTime = sl.UpdatedTime,
                RowVersion = sl.RowVersion
            })
            .SingleAsync();

        if (result == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        return result;
    }

    /// <summary>
    /// 添加表格配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加表格配置", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.Table.Add)]
    public async Task AddTableConfig(AddTableConfigInput input)
    {
        // 判断表格名称是否重复
        if (await _tableRepository.AnyAsync(a => a.TableName == input.TableName))
        {
            throw new UserFriendlyException("表格名称不能重复！");
        }

        var tableId = YitIdHelper.NextId();
        var tableConfigModel = new TableConfigModel
        {
            TableId = tableId,
            TableKey = NumberUtil.IdToCodeByLong(tableId),
            TableName = input.TableName,
            Remark = input.Remark
        };

        await _tableRepository.InsertAsync(tableConfigModel);
    }

    /// <summary>
    /// 编辑表格配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑表格配置", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Table.Edit)]
    public async Task EditTableConfig(EditTableConfigInput input)
    {
        // 判断表格名称是否重复
        if (await _tableRepository.AnyAsync(a => a.TableName == input.TableName && a.TableId != input.TableId))
        {
            throw new UserFriendlyException("表格名称不能重复！");
        }

        var tableConfigModel = await _tableRepository.SingleOrDefaultAsync(input.TableId);
        if (tableConfigModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        tableConfigModel.TableName = input.TableName;
        tableConfigModel.Remark = input.Remark;
        tableConfigModel.RowVersion = input.RowVersion;

        await _tableRepository.Updateable(tableConfigModel)
            // 避免表格同步循环问题，这里不更新时间
            .IgnoreColumns(it => new {it.UpdatedTime})
            .ExecuteCommandWithOptLockAsync(true);
    }

    /// <summary>
    /// 删除表格配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除表格配置", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.Table.Delete)]
    public async Task DeleteTableConfig(TableIdInput input)
    {
        var tableConfigModel = await _tableRepository.SingleOrDefaultAsync(input.TableId);
        if (tableConfigModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        await _tableRepository.Ado.UseTranAsync(async () =>
        {
            await _columnCacheRepository.DeleteAsync(wh => wh.TableId == tableConfigModel.TableId);
            await _columnRepository.DeleteAsync(wh => wh.TableId == tableConfigModel.TableId);
            await _tableRepository.DeleteAsync(tableConfigModel);
        }, ex => throw ex);

        // 清除缓存
        var cacheKey = CacheConst.GetCacheKey(CacheConst.Center.UserTableConfigCache, tableConfigModel.TableKey, "*", "*");
        await _centerCache.DelByPatternAsync(cacheKey);
    }

    /// <summary>
    /// 复制表格配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("复制表格配置", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Table.Edit)]
    public async Task CopyTableConfig(CopyTableConfigInput input)
    {
        // 判断表格名称是否重复
        if (await _tableRepository.AnyAsync(a => a.TableName == input.TableName))
        {
            throw new UserFriendlyException("表格名称不能重复！");
        }

        if (!await _tableRepository.AnyAsync(a => a.TableId == input.TableId))
        {
            throw new UserFriendlyException("数据不存在！");
        }

        var tableId = YitIdHelper.NextId();
        var tableConfigModel = new TableConfigModel
        {
            TableId = tableId,
            TableKey = NumberUtil.IdToCodeByLong(tableId),
            TableName = input.TableName,
            Remark = input.Remark
        };

        // 查询表格所有列
        var columnConfigList = await _columnRepository.Entities.Where(wh => wh.TableId == input.TableId)
            .OrderBy(ob => ob.Order)
            .ToListAsync();

        // 重置列Id和表格Id
        columnConfigList.ForEach(item =>
        {
            item.ColumnId = YitIdHelper.NextId();
            item.TableId = tableConfigModel.TableId;
        });

        await _tableRepository.Ado.UseTranAsync(async () =>
        {
            await _tableRepository.InsertAsync(tableConfigModel);
            await _columnRepository.InsertAsync(columnConfigList);
        }, ex => throw ex);
    }

    /// <summary>
    /// 获取表格列配置详情
    /// </summary>
    /// <param name="tableId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取表格列配置详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Table.Detail)]
    public async Task<List<FaTableColumnCtx>> QueryTableColumnConfigDetail([Required(ErrorMessage = "表格Id不能为空")] long? tableId)
    {
        return await _columnRepository.Entities.Where(wh => wh.TableId == tableId)
            .OrderBy(ob => ob.Order)
            .Select(sl => new FaTableColumnCtx
            {
                ColumnId = sl.ColumnId,
                Prop = sl.Prop,
                Label = sl.Label,
                Fixed = sl.Fixed,
                AutoWidth = sl.AutoWidth,
                Width = sl.Width,
                SmallWidth = sl.SmallWidth,
                Order = sl.Order,
                Show = sl.Show,
                Copy = sl.Copy,
                Sortable = sl.Sortable,
                SortableField = sl.SortableField,
                Type = sl.Type,
                Link = sl.Link,
                ClickEmit = sl.ClickEmit,
                Tag = sl.Tag,
                Enum = sl.Enum,
                DateFix = sl.DateFix,
                DateFormat = sl.DateFormat,
                AuthTag = sl.AuthTag,
                DataDeleteField = sl.DataDeleteField,
                Slot = sl.Slot,
                OtherConfig = sl.OtherConfig,
                PureSearch = sl.PureSearch,
                SearchEl = sl.SearchEl,
                SearchKey = sl.SearchKey,
                SearchLabel = sl.SearchLabel,
                SearchOrder = sl.SearchOrder,
                SearchSlot = sl.SearchSlot,
                SearchConfig = sl.SearchConfig
            })
            .ToListAsync();
    }

    /// <summary>
    /// 编辑表格列配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑表格列配置", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Table.Edit)]
    public async Task EditTableColumnConfig(EditTableColumnConfigInput input)
    {
        var columnIds = input.Columns.Where(wh => wh.ColumnId != null)
            .Select(sl => sl.ColumnId)
            .Distinct()
            .ToList();

        if (columnIds.Count != input.Columns.Count(c => c.ColumnId != null))
        {
            throw new UserFriendlyException("传入的列重复！");
        }

        var tableConfigModel = await _tableRepository.SingleOrDefaultAsync(input.TableId);
        if (tableConfigModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        // 查询表格配置的所有列
        var tableColumnList = await _columnRepository.Entities.Where(wh => wh.TableId == input.TableId)
            .ToListAsync();

        // 更新的
        var updateTableColumnList = input.Columns.Where(wh => wh.ColumnId != null)
            .Select(item =>
            {
                var tableColumnModel = tableColumnList.SingleOrDefault(s => s.ColumnId == item.ColumnId);
                if (tableColumnModel == null)
                {
                    throw new UserFriendlyException("数据不存在！");
                }

                tableColumnModel.Prop = item.Prop;
                tableColumnModel.Label = item.Label;
                tableColumnModel.Fixed = item.Fixed;
                tableColumnModel.AutoWidth = item.AutoWidth;
                tableColumnModel.Width = item.Width;
                tableColumnModel.SmallWidth = item.SmallWidth;
                tableColumnModel.Order = item.Order;
                tableColumnModel.Show = item.Show;
                tableColumnModel.Copy = item.Copy;
                tableColumnModel.Sortable = item.Sortable;
                tableColumnModel.SortableField = item.SortableField;
                tableColumnModel.Type = item.Type;
                tableColumnModel.Link = item.Link;
                tableColumnModel.ClickEmit = item.ClickEmit;
                tableColumnModel.Tag = item.Tag;
                tableColumnModel.Enum = item.Enum;
                tableColumnModel.DateFix = item.DateFix;
                tableColumnModel.DateFormat = item.DateFormat;
                tableColumnModel.AuthTag = item.AuthTag;
                tableColumnModel.DataDeleteField = item.DataDeleteField;
                tableColumnModel.Slot = item.Slot;
                tableColumnModel.OtherConfig = item.OtherConfig;
                tableColumnModel.PureSearch = item.PureSearch;
                tableColumnModel.SearchEl = item.SearchEl;
                tableColumnModel.SearchKey = item.SearchKey;
                tableColumnModel.SearchLabel = item.SearchLabel;
                tableColumnModel.SearchOrder = item.SearchOrder;
                tableColumnModel.SearchSlot = item.SearchSlot;
                tableColumnModel.SearchConfig = item.SearchConfig;

                return tableColumnModel;
            })
            .ToList();

        // 添加的
        var addTableColumnList = input.Columns.Where(wh => wh.ColumnId == null)
            .Select(sl => new TableColumnConfigModel
            {
                TableId = tableConfigModel.TableId,
                Prop = sl.Prop,
                Label = sl.Label,
                Fixed = sl.Fixed,
                AutoWidth = sl.AutoWidth,
                Width = sl.Width,
                SmallWidth = sl.SmallWidth,
                Order = sl.Order,
                Show = sl.Show,
                Copy = sl.Copy,
                Sortable = sl.Sortable,
                SortableField = sl.SortableField,
                Type = sl.Type,
                Link = sl.Link,
                ClickEmit = sl.ClickEmit,
                Tag = sl.Tag,
                Enum = sl.Enum,
                DateFix = sl.DateFix,
                DateFormat = sl.DateFormat,
                AuthTag = sl.AuthTag,
                DataDeleteField = sl.DataDeleteField,
                Slot = sl.Slot,
                OtherConfig = sl.OtherConfig,
                PureSearch = sl.PureSearch,
                SearchEl = sl.SearchEl,
                SearchKey = sl.SearchKey,
                SearchLabel = sl.SearchLabel,
                SearchOrder = sl.SearchOrder,
                SearchSlot = sl.SearchSlot,
                SearchConfig = sl.SearchConfig
            })
            .ToList();

        // 删除的
        var deleteTableColumnList = tableColumnList.Where(wh => !columnIds.Contains(wh.ColumnId))
            .ToList();

        tableConfigModel.RowVersion = input.RowVersion;

        await _tableRepository.Ado.UseTranAsync(async () =>
        {
            //await _tableRepository.UpdateAsync(tableConfigModel);
            await _tableRepository.Updateable(tableConfigModel)
                .ExecuteCommandAsync();
            await _columnRepository.DeleteAsync(deleteTableColumnList);
            await _columnRepository.UpdateAsync(updateTableColumnList);
            await _columnRepository.InsertAsync(addTableColumnList);
        }, ex => throw ex);

        // 删除缓存
        var cacheKey = CacheConst.GetCacheKey(CacheConst.Center.TableConfig, tableConfigModel.TableKey);
        await _centerCache.DelAsync(cacheKey);
    }

    /// <summary>
    /// 获取表格配置缓存
    /// </summary>
    /// <param name="tableKey"></param>
    /// <returns></returns>
    internal async Task<TableConfigModel> QueryTableConfigCache(string tableKey)
    {
        var cacheKey = CacheConst.GetCacheKey(CacheConst.Center.TableConfig, tableKey);

        return await _centerCache.GetAndSetAsync(cacheKey, async () =>
        {
            return await _tableRepository.Entities.Includes(e => e.TableColumnConfigList.OrderBy(ob => ob.Order)
                    .ToList())
                .Where(wh => wh.TableKey == tableKey)
                .SingleAsync();
        });
    }

    /// <summary>
    /// 获取用户表格列配置缓存
    /// </summary>
    /// <param name="tableId"></param>
    /// <param name="tableKey"></param>
    /// <returns></returns>
    internal async Task<List<TableColumnConfigCacheModel>> QueryUserTableColumnConfigCache(long tableId, string tableKey)
    {
        var cacheKey = CacheConst.GetCacheKey(CacheConst.Center.UserTableConfigCache, tableKey, _user.EmployeeNo);
        return await _centerCache.GetAndSetAsync(cacheKey, async () =>
               {
                   return await _columnCacheRepository.Entities
                       .Where(wh => wh.UserId == _user.EmployeeId && wh.TableId == tableId)
                       .OrderBy(ob => ob.Order)
                       .ToListAsync();
               })
               ?? [];
    }

    /// <summary>
    /// 获取表格列配置
    /// </summary>
    /// <param name="tableKey"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取表格列配置", HttpRequestActionEnum.Query)]
    [DisabledRequestLog]
    public async Task<QueryTableColumnConfigOutput> QueryTableColumnConfig([Required(ErrorMessage = "表格Key不能为空")] string tableKey)
    {
        var tableConfigModel = await QueryTableConfigCache(tableKey);
        if (tableConfigModel == null)
        {
            throw new UserFriendlyException("表格列配置不存在！");
        }

        var result = new QueryTableColumnConfigOutput
        {
            TableKey = tableConfigModel.TableKey,
            Columns = new List<IDictionary<string, object>>(),
            UpdatedTime = tableConfigModel.UpdatedTime,
            Change = false,
            Cache = false
        };

        // 权限判断
        if (!_user.IsSuperAdmin)
        {
            tableConfigModel.TableColumnConfigList = tableConfigModel.TableColumnConfigList.Where(wh =>
                    !wh.AuthTag.Any() || wh.AuthTag.Any(a => _user.ButtonCodeList.Contains(a)))
                .ToList();
        }

        // 循环源列数据
        foreach (var item in tableConfigModel.TableColumnConfigList)
        {
            object columnFixed = string.IsNullOrWhiteSpace(item.Fixed) ? false : item.Fixed;

            var column = new Dictionary<string, object>
            {
                {"columnId", item.ColumnId},
                {"prop", item.Prop},
                {"label", string.IsNullOrWhiteSpace(item.Label) ? null : item.Label},
                {"fixed", columnFixed},
                {"autoWidth", item.AutoWidth},
                {"width", item.Width},
                {"smallWidth", item.SmallWidth},
                {"order", item.Order},
                {"show", item.Show},
                {"copy", item.Copy},
                {"sortable", item.Sortable},
                // 如果配置原本不支持排序，则直接禁用
                {"disabledSortable", !item.Sortable},
                {"sortableField", string.IsNullOrWhiteSpace(item.SortableField) ? null : item.SortableField},
                {"type", string.IsNullOrWhiteSpace(item.Type) ? "default" : item.Type},
                {"link", item.Link},
                {"clickEmit", string.IsNullOrWhiteSpace(item.ClickEmit) ? null : item.ClickEmit},
                {"tag", item.Tag},
                {"enum", string.IsNullOrWhiteSpace(item.Enum) ? null : item.Enum},
                {"dateFix", item.DateFix},
                {"dateFormat", string.IsNullOrWhiteSpace(item.DateFormat) ? null : item.DateFormat},
                {"dataDeleteField", string.IsNullOrWhiteSpace(item.DataDeleteField) ? null : item.DataDeleteField},
                {"slot", string.IsNullOrWhiteSpace(item.Slot) ? null : item.Slot},
                {"pureSearch", item.PureSearch}
            };

            // 其他不常用配置选项
            if (item.OtherConfig?.Any() == true)
            {
                foreach (var oItem in item.OtherConfig)
                {
                    switch (oItem.Type)
                    {
                        default:
                        case ColumnAdvancedTypeEnum.String:
                            try
                            {
                                column.TryAdd(oItem.Prop, JToken.Parse(oItem.Value));
                            }
                            catch
                            {
                                column.TryAdd(oItem.Prop, oItem.Value);
                            }

                            break;
                        case ColumnAdvancedTypeEnum.Number:
                            column.TryAdd(oItem.Prop, oItem.Value.ParseToInt());
                            break;
                        case ColumnAdvancedTypeEnum.Boolean:
                            column.TryAdd(oItem.Prop, oItem.Value.ParseToBool());
                            break;
                        case ColumnAdvancedTypeEnum.Function:
                            column.TryAdd(oItem.Prop, oItem.Value);
                            break;
                    }
                }

                column.TryAdd("otherAdvancedConfig", item.OtherConfig.Select(sl => new {sl.Prop, sl.Type}));
            }

            // 搜素项
            if (!string.IsNullOrWhiteSpace(item.SearchEl))
            {
                var searchConfig = new Dictionary<string, object>
                {
                    {"el", item.SearchEl},
                    {"key", string.IsNullOrWhiteSpace(item.SearchKey) ? null : item.SearchKey},
                    {"label", string.IsNullOrWhiteSpace(item.SearchLabel) ? null : item.SearchLabel},
                    {"order", item.SearchOrder},
                    {"slot", string.IsNullOrWhiteSpace(item.SearchSlot) ? null : item.SearchSlot}
                };

                var searchPropsConfig = new Dictionary<string, object>();

                if (item.SearchConfig?.Any() == true)
                {
                    foreach (var oItem in item.SearchConfig)
                    {
                        switch (oItem.Type)
                        {
                            default:
                            case ColumnAdvancedTypeEnum.String:
                                try
                                {
                                    searchPropsConfig.TryAdd(oItem.Prop, JToken.Parse(oItem.Value));
                                }
                                catch
                                {
                                    searchPropsConfig.TryAdd(oItem.Prop, oItem.Value);
                                }

                                break;
                            case ColumnAdvancedTypeEnum.Number:
                                searchPropsConfig.TryAdd(oItem.Prop, oItem.Value.ParseToInt());
                                break;
                            case ColumnAdvancedTypeEnum.Boolean:
                                searchPropsConfig.TryAdd(oItem.Prop, oItem.Value.ParseToBool());
                                break;
                            case ColumnAdvancedTypeEnum.Function:
                                searchPropsConfig.TryAdd(oItem.Prop, oItem.Value);
                                break;
                        }
                    }

                    column.TryAdd("searchAdvancedConfig", item.SearchConfig.Select(sl => new {sl.Prop, sl.Type}));
                }

                if (searchPropsConfig.Count > 0)
                {
                    searchConfig.TryAdd("props", searchPropsConfig);
                }

                column.Add("search", searchConfig);
            }

            result.Columns.Add(column);
        }

        // 尝试获取缓存
        var tableColumnCacheList = await QueryUserTableColumnConfigCache(tableConfigModel.TableId, tableConfigModel.TableKey);

        // 判断是否存在缓存
        if (tableColumnCacheList?.Any() == true)
        {
            result.Cache = true;
            result.UpdatedTime = tableColumnCacheList.Max(m => m.CreatedTime);
            result.Change = tableConfigModel.UpdatedTime > result.UpdatedTime;

            // 深拷贝一份
            result.CacheColumns = result.Columns.Select(IDictionary<string, object> (sl) => new Dictionary<string, object>(sl))
                .ToList();

            // 循环缓存数据
            foreach (var item in tableColumnCacheList)
            {
                var columnIdx = result.CacheColumns.FindIndex(f => $"{f["columnId"]}" == item.ColumnId.ToString());

                if (columnIdx == -1)
                    continue;

                result.CacheColumns[columnIdx]["label"] = string.IsNullOrWhiteSpace(item.Label) ? null : item.Label;
                result.CacheColumns[columnIdx]["fixed"] = string.IsNullOrWhiteSpace(item.Fixed) ? false : item.Fixed;
                result.CacheColumns[columnIdx]["autoWidth"] = item.AutoWidth;
                result.CacheColumns[columnIdx]["width"] = item.Width;
                result.CacheColumns[columnIdx]["smallWidth"] = item.SmallWidth;
                result.CacheColumns[columnIdx]["order"] = item.Order;
                result.CacheColumns[columnIdx]["show"] = item.Show;
                result.CacheColumns[columnIdx]["copy"] = item.Copy;
                result.CacheColumns[columnIdx]["sortable"] = item.Sortable;

                if (result.CacheColumns[columnIdx]
                    .ContainsKey("search"))
                {
                    if (result.CacheColumns[columnIdx]["search"] is JObject searchJObject)
                    {
                        var newSearchDic = new Dictionary<string, object>();
                        foreach (var property in searchJObject.Properties())
                        {
                            newSearchDic.Add(property.Name, property.Value);
                        }

                        newSearchDic["label"] = string.IsNullOrWhiteSpace(item.SearchLabel) ? null : item.SearchLabel;
                        newSearchDic["order"] = item.SearchOrder;

                        result.CacheColumns[columnIdx]["search"] = JObject.FromObject(newSearchDic);
                    }
                }
            }

            result.CacheColumns = result.CacheColumns.OrderBy(ob => ob["order"])
                .ToList();
        }

        return result;
    }

    /// <summary>
    /// 同步用户表格配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("同步用户表格配置", HttpRequestActionEnum.Edit)]
    public async Task SyncUserTableConfig(SyncUserTableConfigInput input)
    {
        var tableConfigModel = await QueryTableConfigCache(input.TableKey);
        if (tableConfigModel == null)
        {
            throw new UserFriendlyException("表格列配置不存在！");
        }

        // 获取缓存
        var tableColumnCacheList = await QueryUserTableColumnConfigCache(tableConfigModel.TableId, tableConfigModel.TableKey);

        var columnIds = tableColumnCacheList.Select(sl => sl.ColumnId)
            .ToList();
        var sourceColumnIds = tableConfigModel.TableColumnConfigList.Select(sl => sl.ColumnId)
            .ToList();

        var dateTime = DateTime.Now;

        // 添加的
        var addTableColumnCacheList = tableConfigModel.TableColumnConfigList.Where(wh => !columnIds.Contains(wh.ColumnId))
            .Select(sl => new TableColumnConfigCacheModel
            {
                UserId = _user.EmployeeId,
                TableId = sl.TableId,
                ColumnId = sl.ColumnId,
                Label = sl.Label,
                Fixed = sl.Fixed,
                AutoWidth = sl.AutoWidth,
                Width = sl.Width,
                SmallWidth = sl.SmallWidth,
                Order = sl.Order,
                Show = sl.Show,
                Copy = sl.Copy,
                Sortable = sl.Sortable,
                SearchLabel = sl.SearchLabel,
                SearchOrder = sl.SearchOrder,
                CreatedTime = dateTime
            })
            .ToList();

        // 删除的
        var deleteTableColumnCacheList = tableColumnCacheList.Where(wh => !sourceColumnIds.Contains(wh.ColumnId))
            .ToList();

        // 更新的
        var sourceDict = tableConfigModel.TableColumnConfigList.ToDictionary(k => k.ColumnId);
        var updateTableColumnCacheList = tableColumnCacheList.Where(wh => sourceColumnIds.Contains(wh.ColumnId))
            .ToList();

        foreach (var item in updateTableColumnCacheList)
        {
            if (!sourceDict.TryGetValue(item.ColumnId, out var sourceItem))
                continue;

            item.Label = sourceItem.Label;
            item.Fixed = sourceItem.Fixed;
            item.AutoWidth = sourceItem.AutoWidth;
            item.Width = sourceItem.Width;
            item.SmallWidth = sourceItem.SmallWidth;
            item.Order = sourceItem.Order;
            item.Show = sourceItem.Show;
            item.Copy = sourceItem.Copy;
            item.Sortable = sourceItem.Sortable;
            item.SearchLabel = sourceItem.SearchLabel;
            item.SearchOrder = sourceItem.SearchOrder;
            item.CreatedTime = dateTime;
        }

        await _columnCacheRepository.Ado.UseTranAsync(async () =>
        {
            await _columnCacheRepository.DeleteAsync(deleteTableColumnCacheList);
            await _columnCacheRepository.InsertAsync(addTableColumnCacheList);
            await _columnCacheRepository.UpdateAsync(updateTableColumnCacheList);
        }, ex => throw ex);

        // 删除缓存
        var cacheKey = CacheConst.GetCacheKey(CacheConst.Center.UserTableConfigCache, tableConfigModel.TableKey,
            _user.EmployeeNo);
        await _centerCache.DelAsync(cacheKey);
    }

    /// <summary>
    /// 保存用户表格配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("保存用户表格配置", HttpRequestActionEnum.Edit)]
    public async Task SaveUserTableConfig(SaveUserTableConfigInput input)
    {
        var tableConfigModel = await QueryTableConfigCache(input.TableKey);
        if (tableConfigModel == null)
        {
            throw new UserFriendlyException("表格列配置不存在！");
        }

        // 获取缓存
        var tableColumnCacheList = await QueryUserTableColumnConfigCache(tableConfigModel.TableId, tableConfigModel.TableKey);
        var addTableColumnCacheList = new List<TableColumnConfigCacheModel>();
        var dateTime = DateTime.Now;

        // 保存的时候没有删除的
        foreach (var item in input.Columns)
        {
            var tableColumnModel = tableConfigModel.TableColumnConfigList.SingleOrDefault(s => s.ColumnId == item.ColumnId);
            if (tableColumnModel == null)
            {
                throw new UserFriendlyException("数据不存在！");
            }

            var tableColumnCacheModel = tableColumnCacheList.SingleOrDefault(s => s.ColumnId == item.ColumnId);
            if (tableColumnCacheModel == null)
            {
                tableColumnCacheModel = new TableColumnConfigCacheModel
                {
                    UserId = _user.EmployeeId,
                    TableId = tableColumnModel.TableId,
                    ColumnId = tableColumnModel.ColumnId,
                    Label = string.IsNullOrWhiteSpace(item.Label) ? tableColumnModel.Label : item.Label,
                    Fixed = string.IsNullOrWhiteSpace(item.Fixed) ? tableColumnModel.Fixed : item.Fixed,
                    AutoWidth = item.AutoWidth,
                    Width = item.Width ?? tableColumnModel.Width,
                    SmallWidth = item.SmallWidth ?? tableColumnModel.SmallWidth,
                    Order = item.Order ?? tableColumnModel.Order,
                    Show = item.Show,
                    Copy = item.Copy,
                    Sortable = item.Sortable,
                    SearchLabel =
                        string.IsNullOrWhiteSpace(item.SearchLabel) ? tableColumnModel.SearchLabel : item.SearchLabel,
                    SearchOrder = item.SearchOrder ?? tableColumnModel.SearchOrder,
                    CreatedTime = dateTime
                };
                addTableColumnCacheList.Add(tableColumnCacheModel);
            }
            else
            {
                tableColumnCacheModel.Label = string.IsNullOrWhiteSpace(item.Label) ? tableColumnModel.Label : item.Label;
                tableColumnCacheModel.Fixed = string.IsNullOrWhiteSpace(item.Fixed) ? tableColumnModel.Fixed : item.Fixed;
                tableColumnCacheModel.AutoWidth = item.AutoWidth;
                tableColumnCacheModel.Width = item.Width ?? tableColumnModel.Width;
                tableColumnCacheModel.SmallWidth = item.SmallWidth ?? tableColumnModel.SmallWidth;
                tableColumnCacheModel.Order = item.Order ?? tableColumnModel.Order;
                tableColumnCacheModel.Show = item.Show;
                tableColumnCacheModel.Copy = item.Copy;
                tableColumnCacheModel.Sortable = item.Sortable;
                tableColumnCacheModel.SearchLabel = string.IsNullOrWhiteSpace(item.SearchLabel)
                    ? tableColumnModel.SearchLabel
                    : item.SearchLabel;
                tableColumnCacheModel.SearchOrder = item.SearchOrder ?? tableColumnModel.SearchOrder;
            }
        }

        await _columnCacheRepository.Ado.UseTranAsync(async () =>
        {
            await _columnCacheRepository.UpdateAsync(tableColumnCacheList);
            await _columnCacheRepository.InsertAsync(addTableColumnCacheList);
        }, ex => throw ex);

        // 删除缓存
        var cacheKey = CacheConst.GetCacheKey(CacheConst.Center.UserTableConfigCache, tableConfigModel.TableKey,
            _user.EmployeeNo);
        await _centerCache.DelAsync(cacheKey);
    }

    /// <summary>
    /// 清除用户表格配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("清除用户表格配置", HttpRequestActionEnum.Delete)]
    public async Task ClearUserTableConfig(SyncUserTableConfigInput input)
    {
        var tableConfigModel = await QueryTableConfigCache(input.TableKey);
        if (tableConfigModel == null)
        {
            throw new UserFriendlyException("表格列配置不存在！");
        }

        await _columnCacheRepository.DeleteAsync(wh => wh.TableId == tableConfigModel.TableId);

        // 删除缓存
        var cacheKey = CacheConst.GetCacheKey(CacheConst.Center.UserTableConfigCache, tableConfigModel.TableKey,
            _user.EmployeeNo);
        await _centerCache.DelAsync(cacheKey);
    }
}