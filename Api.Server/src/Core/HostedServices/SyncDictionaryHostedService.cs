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

using System.Reflection;
using Fast.Center.Domain;
using Fast.SqlSugar;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.Core;

/// <summary>
/// 同步字典托管服务
/// </summary>
[Order(106)]
public class SyncDictionaryHostedService : IHostedService
{
    /// <summary>
    /// 缓存
    /// </summary>
    private readonly ICache<CenterCCL> _centerCache;

    /// <summary>
    /// 日志
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// 同步字典托管服务
    /// </summary>
    public SyncDictionaryHostedService(ICache<CenterCCL> centerCache, ILogger<SyncDictionaryHostedService> logger)
    {
        _centerCache = centerCache;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var dateTime = DateTime.Now;

        var serviceName = Assembly.GetEntryAssembly()!.GetName()
            .Name;
        var addDictionaryTypeList = new List<DictionaryTypeModel>();
        var addDictionaryItemList = new List<DictionaryItemModel>();
        var updateDictionaryTypeList = new List<DictionaryTypeModel>();
        var updateDictionaryItemList = new List<DictionaryItemModel>();
        var deleteDictionaryItemList = new List<DictionaryItemModel>();

        MAppContext.ConsoleWrite(console =>
        {
            console.BackgroundColor = ConsoleColor.Black;
            console.ForegroundColor = ConsoleColor.Green;
            console.Write("info");
            console.ResetColor();
            console.WriteLine($": {DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
            console.BackgroundColor = ConsoleColor.Black;
            console.ForegroundColor = ConsoleColor.DarkGray;
            console.WriteLine("      开始同步字典信息...");
        });

        try
        {
            using var db = new SqlSugarClient(SqlSugarContext.GetConnectionConfig(SqlSugarContext.ConnectionSettings));

            var dictionaryTypeList = await db.Queryable<DictionaryTypeModel>()
                .ToListAsync(cancellationToken);
            var dictionaryItemList = await db.Queryable<DictionaryItemModel>()
                .ToListAsync(cancellationToken);

            // 获取所有带 FastEnumAttribute 特性的枚举
            var enumTypes = MAppContext.EffectiveTypes.Where(wh => wh.IsEnum)
                .Select(sl => new
                {
                    Type = sl,
                    FastEnumAttribute = sl.GetCustomAttribute<FastEnumAttribute>(),
                    FlagsAttribute = sl.GetCustomAttribute<FlagsAttribute>()
                })
                .Where(wh => wh.FastEnumAttribute != null)
                .ToList();

            if (!dictionaryTypeList.Any(a => a.DictionaryKey == "BooleanEnum" && a.ServiceName == serviceName))
            {
                var dictionaryId = YitIdHelper.NextId();
                addDictionaryTypeList.Add(new DictionaryTypeModel
                {
                    DictionaryId = dictionaryId,
                    DictionaryKey = "BooleanEnum",
                    ServiceName = serviceName,
                    DictionaryName = "Boolean类型枚举",
                    ValueType = DictionaryValueTypeEnum.Boolean,
                    HasFlags = false,
                    Status = CommonStatusEnum.Enable,
                    Remark = null,
                    CreatedTime = dateTime
                });
                addDictionaryItemList.AddRange(new List<DictionaryItemModel>
                {
                    new()
                    {
                        DictionaryItemId = YitIdHelper.NextId(),
                        DictionaryId = dictionaryId,
                        Label = "是",
                        Value = "true",
                        Type = TagTypeEnum.Success,
                        Order = 1,
                        Visible = true,
                        Status = CommonStatusEnum.Enable,
                        CreatedTime = dateTime
                    },
                    new()
                    {
                        DictionaryItemId = YitIdHelper.NextId(),
                        DictionaryId = dictionaryId,
                        Label = "否",
                        Value = "false",
                        Type = TagTypeEnum.Danger,
                        Order = 2,
                        Visible = true,
                        Status = CommonStatusEnum.Enable,
                        CreatedTime = dateTime
                    }
                });
            }

            // 循环所有枚举类型
            foreach (var enumType in enumTypes)
            {
                var enumItemList = enumType.Type.EnumToList<long>();

                var dictionaryTypeInfo = dictionaryTypeList.SingleOrDefault(s => s.DictionaryKey == enumType.Type.Name);

                var dictionaryTypeModel = new DictionaryTypeModel
                {
                    DictionaryKey = enumType.Type.Name,
                    ServiceName = serviceName,
                    DictionaryName =
                        enumType.FastEnumAttribute?.ChName ?? enumType.FastEnumAttribute?.EnName ?? enumType.Type.Name,
                    ValueType = Enum.GetUnderlyingType(enumType.Type) == typeof(long)
                        ? DictionaryValueTypeEnum.Long
                        : DictionaryValueTypeEnum.Int,
                    HasFlags = enumType.FlagsAttribute != null,
                    Status = CommonStatusEnum.Enable,
                    Remark = enumType.FastEnumAttribute?.Remark,
                    UpdatedTime = dateTime
                };

                if (dictionaryTypeInfo != null)
                {
                    dictionaryTypeModel.DictionaryId = dictionaryTypeInfo.DictionaryId;
                    // 不相同才修改
                    if (!dictionaryTypeInfo.Equals(dictionaryTypeModel))
                    {
                        dictionaryTypeInfo.DictionaryName = dictionaryTypeModel.DictionaryName;
                        // 这里只会存在 long 或者 int
                        dictionaryTypeInfo.ValueType = dictionaryTypeModel.ValueType;
                        dictionaryTypeInfo.HasFlags = dictionaryTypeModel.HasFlags;
                        if (string.IsNullOrWhiteSpace(dictionaryTypeInfo.Remark))
                        {
                            dictionaryTypeInfo.Remark = dictionaryTypeModel.Remark;
                        }

                        dictionaryTypeInfo.UpdatedTime = dateTime;
                        updateDictionaryTypeList.Add(dictionaryTypeInfo);
                    }

                    deleteDictionaryItemList.AddRange(dictionaryItemList
                        .Where(wh => wh.DictionaryId == dictionaryTypeInfo.DictionaryId)
                        .Where(wh => enumItemList.All(a => a.Value.ToString() != wh.Value))
                        .ToList());

                    var orderIndex = 1;

                    foreach (var enumItem in enumItemList)
                    {
                        var fieldInfo = enumType.Type.GetField(enumItem.Name);
                        var tagType = fieldInfo.GetCustomAttribute<TagTypeAttribute>();

                        var dictionaryItemModel = new DictionaryItemModel
                        {
                            DictionaryId = dictionaryTypeInfo.DictionaryId,
                            Label = enumItem.Describe ?? enumItem.Name,
                            Value = enumItem.Value.ToString(),
                            Type = tagType?.TagType ?? TagTypeEnum.Primary,
                            Order = orderIndex,
                            Visible = true,
                            Status = CommonStatusEnum.Enable,
                            UpdatedTime = dateTime
                        };

                        var dictionaryItemInfo = dictionaryItemList
                            .Where(wh => wh.DictionaryId == dictionaryTypeInfo.DictionaryId)
                            .SingleOrDefault(s => s.Value == enumItem.Value.ToString());
                        if (dictionaryItemInfo != null)
                        {
                            dictionaryItemModel.DictionaryItemId = dictionaryItemInfo.DictionaryItemId;
                            // 不相同才修改
                            if (!dictionaryItemInfo.Equals(dictionaryItemModel))
                            {
                                dictionaryItemInfo.Label = dictionaryItemModel.Label;
                                dictionaryItemInfo.Value = dictionaryItemModel.Value;
                                dictionaryItemInfo.Type = dictionaryItemModel.Type;
                                dictionaryItemInfo.Order = dictionaryItemModel.Order;
                                dictionaryItemInfo.UpdatedTime = dateTime;
                                updateDictionaryItemList.Add(dictionaryItemInfo);
                            }
                        }
                        else
                        {
                            dictionaryItemModel.DictionaryItemId = YitIdHelper.NextId();
                            dictionaryItemModel.CreatedTime = dateTime;
                            addDictionaryItemList.Add(dictionaryItemModel);
                        }

                        orderIndex++;
                    }
                }
                else
                {
                    dictionaryTypeModel.DictionaryId = YitIdHelper.NextId();
                    dictionaryTypeModel.CreatedTime = dateTime;
                    addDictionaryTypeList.Add(dictionaryTypeModel);

                    var orderIndex = 1;

                    foreach (var enumItem in enumItemList)
                    {
                        var fieldInfo = enumType.Type.GetField(enumItem.Name);
                        var tagType = fieldInfo.GetCustomAttribute<TagTypeAttribute>();

                        var dictionaryItemModel = new DictionaryItemModel
                        {
                            DictionaryItemId = YitIdHelper.NextId(),
                            DictionaryId = dictionaryTypeModel.DictionaryId,
                            Label = enumItem.Describe ?? enumItem.Name,
                            Value = enumItem.Value.ToString(),
                            Type = tagType?.TagType ?? TagTypeEnum.Primary,
                            Order = orderIndex,
                            Visible = true,
                            Status = CommonStatusEnum.Enable,
                            CreatedTime = dateTime,
                            UpdatedTime = dateTime
                        };
                        addDictionaryItemList.Add(dictionaryItemModel);

                        orderIndex++;
                    }
                }
            }

            // 加载Aop
            SugarEntityFilter.LoadSugarAop(FastContext.HostEnvironment.IsDevelopment(), db);

            if (deleteDictionaryItemList.Count > 0)
            {
                await db.Deleteable(deleteDictionaryItemList)
                    .ExecuteCommandAsync(cancellationToken);
            }

            // 只删除当前服务的
            var deleteDictionaryTypeList = dictionaryTypeList.Where(wh => wh.ServiceName == serviceName)
                .Where(wh => !enumTypes.Select(sl => sl.Type.Name)
                                 .ToHashSet()
                                 .Contains(wh.DictionaryKey)
                             && wh.DictionaryKey != "BooleanEnum")
                .ToList();
            if (deleteDictionaryTypeList.Count > 0)
            {
                await db.Deleteable(deleteDictionaryTypeList)
                    .ExecuteCommandAsync(cancellationToken);
            }

            await db.Updateable(updateDictionaryTypeList)
                .ExecuteCommandAsync(cancellationToken);
            await db.Updateable(updateDictionaryItemList)
                .ExecuteCommandAsync(cancellationToken);
            await db.Insertable(addDictionaryTypeList)
                .ExecuteCommandAsync(cancellationToken);
            await db.Insertable(addDictionaryItemList)
                .ExecuteCommandAsync(cancellationToken);

            // 删除缓存
            await _centerCache.DelAsync(CacheConst.Center.Dictionary);

            MAppContext.ConsoleWrite(console =>
            {
                console.BackgroundColor = ConsoleColor.Black;
                console.ForegroundColor = ConsoleColor.Green;
                console.Write("info");
                console.ResetColor();
                console.WriteLine($": {DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                console.BackgroundColor = ConsoleColor.Black;
                console.ForegroundColor = ConsoleColor.DarkGray;
                console.WriteLine(
                    $"      同步字典信息成功。新增 {addDictionaryTypeList.Count}/{addDictionaryItemList.Count} 个，更新 {updateDictionaryTypeList.Count}/{updateDictionaryItemList.Count} 个，删除 {deleteDictionaryTypeList.Count}/{deleteDictionaryItemList.Count} 个。");
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sync dictionary error...");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}