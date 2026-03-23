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
using System.Text;
using Fast.Admin.Entity;
using Fast.DynamicApplication;
using Fast.JwtBearer;
using Fast.SqlSugar;
using Fast.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.Core;

/// <summary>
/// <see cref="SyncApiHostedService"/> 同步 Api 托管服务
/// </summary>
[Order(103)]
public class SyncApiHostedService : IHostedService
{
    /// <summary>
    /// 托管应用程序生命周期
    /// </summary>
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    /// <summary>
    /// 接口描述提供程序
    /// </summary>
    private readonly IApiDescriptionGroupCollectionProvider _apiDescriptionGroupCollectionProvider;

    /// <summary>
    /// Swagger 配置
    /// </summary>
    private readonly SwaggerSettingsOptions _swaggerSettings;

    /// <summary>
    /// 日志
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// <see cref="SyncApiHostedService"/> 同步 Api 托管服务
    /// </summary>
    /// <param name="hostApplicationLifetime"><see cref="IHostApplicationLifetime"/> 托管应用程序生命周期</param>
    /// <param name="apiDescriptionGroupCollectionProvider"><see cref="IApiDescriptionGroupCollectionProvider"/> 接口描述提供程序</param>
    /// <param name="options"><see cref="SwaggerSettingsOptions"/> Swagger 配置</param>
    /// <param name="logger"><see cref="ILogger"/> 日志</param>
    public SyncApiHostedService(IHostApplicationLifetime hostApplicationLifetime,
        IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider, IOptions<SwaggerSettingsOptions> options,
        ILogger<SyncApiHostedService> logger)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _apiDescriptionGroupCollectionProvider = apiDescriptionGroupCollectionProvider;
        _swaggerSettings = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Start operation.</returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // 订阅 ApplicationStarted 事件
        _hostApplicationLifetime?.ApplicationStarted.Register(() =>
        {
            _ = Task.Run(async () =>
            {
                var dateTime = DateTime.Now;

                var serviceName = Assembly.GetEntryAssembly()!.GetName()
                    .Name;
                var addApiInfoList = new List<ApiInfoModel>();
                var updateApiInfoList = new List<ApiInfoModel>();
                var apiUrlList = new List<string>();

                {
                    var logSb = new StringBuilder();
                    logSb.Append("\u001b[40m\u001b[1m\u001b[32m");
                    logSb.Append("info");
                    logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                    logSb.Append(": ");
                    logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                    logSb.Append(Environment.NewLine);
                    logSb.Append("\u001b[40m\u001b[90m");
                    logSb.Append("      ");
                    logSb.Append("开始同步接口信息...");
                    logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                    Console.WriteLine(logSb.ToString());
                }

                try
                {
                    var db = new SqlSugarClient(SqlSugarContext.GetConnectionConfig(SqlSugarContext.ConnectionSettings));

                    var apiInfoList = await db.Queryable<ApiInfoModel>()
                        .Where(wh => wh.ServiceName == serviceName)
                        .ToListAsync(cancellationToken);

                    // 循环所有接口
                    foreach (var apiDescriptionGroup in _apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items
                                 .SelectMany(sl => sl.Items)
                                 .ToList())
                    {
                        var apiDescriptionSettingsAttribute = apiDescriptionGroup.ActionDescriptor.EndpointMetadata
                            .OfType<ApiDescriptionSettingsAttribute>()
                            .FirstOrDefault();
                        if (apiDescriptionSettingsAttribute.IgnoreApi)
                            continue;

                        var allowAnonymousAttribute = apiDescriptionGroup.ActionDescriptor.EndpointMetadata
                            .OfType<AllowAnonymousAttribute>()
                            .FirstOrDefault();
                        var allowForbiddenAttribute = apiDescriptionGroup.ActionDescriptor.EndpointMetadata
                            .OfType<AllowForbiddenAttribute>()
                            .FirstOrDefault();
                        var permissionAttribute = apiDescriptionGroup.ActionDescriptor.EndpointMetadata
                            .OfType<PermissionAttribute>()
                            .FirstOrDefault();
                        var apiInfoAttribute = apiDescriptionGroup.ActionDescriptor.EndpointMetadata.OfType<ApiInfoAttribute>()
                            .FirstOrDefault();

                        // 获取分组名称
                        var groupName = apiDescriptionGroup.GroupName ?? "Default";

                        var moduleName = apiDescriptionSettingsAttribute.Name
                                         ?? (apiDescriptionGroup.ActionDescriptor as ControllerActionDescriptor).ControllerName;
                        var sort = apiDescriptionSettingsAttribute.Order;
                        var groupOpenApiInfo = _swaggerSettings.GroupOpenApiInfos.FirstOrDefault(f => f.Group == groupName);

                        var apiUrl = $"/{apiDescriptionGroup.RelativePath}";

                        // 判断原有的Url是否存在
                        var apiInfo = apiInfoList.SingleOrDefault(s => s.ApiUrl == apiUrl);

                        var method = Enum.Parse<HttpRequestMethodEnum>(apiDescriptionGroup.HttpMethod, true);
                        var action = apiInfoAttribute?.Action ?? HttpRequestActionEnum.None;
                        var hasPermission = allowForbiddenAttribute == null && permissionAttribute?.TagList?.Count > 0;

                        var apiInfoModel = new ApiInfoModel
                        {
                            ServiceName = serviceName,
                            GroupName = groupName,
                            GroupTitle = groupOpenApiInfo?.Title,
                            Version = groupOpenApiInfo?.Version,
                            Description = groupOpenApiInfo?.Description,
                            ModuleName = moduleName,
                            ApiUrl = apiUrl,
                            ApiName = apiInfoAttribute?.Name,
                            Method = method,
                            Action = action,
                            HasAuth = allowAnonymousAttribute == null,
                            HasPermission = hasPermission,
                            Tags = permissionAttribute?.TagList ?? [],
                            Sort = sort
                        };
                        apiUrlList.Add(apiInfoModel.ApiUrl);

                        if (apiInfo != null)
                        {
                            apiInfoModel.ApiId = apiInfo.ApiId;
                            // 不相同才修改
                            if (!apiInfo.Equals(apiInfoModel))
                            {
                                apiInfo.ServiceName = apiInfoModel.ServiceName;
                                apiInfo.GroupName = apiInfoModel.GroupName;
                                apiInfo.GroupTitle = apiInfoModel.GroupTitle;
                                apiInfo.Version = apiInfoModel.Version;
                                apiInfo.Description = apiInfoModel.Description;
                                apiInfo.ModuleName = apiInfoModel.ModuleName;
                                apiInfo.ApiUrl = apiInfoModel.ApiUrl;
                                apiInfo.ApiName = apiInfoModel.ApiName;
                                apiInfo.Method = apiInfoModel.Method;
                                apiInfo.Action = apiInfoModel.Action;
                                apiInfo.HasAuth = apiInfoModel.HasAuth;
                                apiInfo.HasPermission = apiInfoModel.HasPermission;
                                apiInfo.Tags = apiInfoModel.Tags;
                                apiInfo.Sort = apiInfoModel.Sort;
                                apiInfo.UpdatedTime = dateTime;
                                updateApiInfoList.Add(apiInfo);
                            }
                        }
                        else
                        {
                            apiInfoModel.ApiId = YitIdHelper.NextId();
                            apiInfoModel.CreatedTime = dateTime;
                            addApiInfoList.Add(apiInfoModel);
                        }
                    }

                    // 加载Aop
                    SugarEntityFilter.LoadSugarAop(FastContext.HostEnvironment.IsDevelopment(), db);

                    var deleteApiInfoList = apiInfoList.Where(wh => !apiUrlList.Contains(wh.ApiUrl))
                        .ToList();

                    if (deleteApiInfoList.Count > 0)
                    {
                        await db.Deleteable(deleteApiInfoList)
                            .ExecuteCommandAsync(cancellationToken);
                    }

                    await db.Updateable(updateApiInfoList)
                        .ExecuteCommandAsync(cancellationToken);
                    await db.Insertable(addApiInfoList)
                        .ExecuteCommandAsync(cancellationToken);

                    CacheContext.ApiInfoList = apiInfoList;
                    CacheContext.ApiInfoList.AddRange(addApiInfoList);

                    {
                        var logSb = new StringBuilder();
                        logSb.Append("\u001b[40m\u001b[1m\u001b[32m");
                        logSb.Append("info");
                        logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                        logSb.Append(": ");
                        logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                        logSb.Append(Environment.NewLine);
                        logSb.Append("\u001b[40m\u001b[90m");
                        logSb.Append("      ");
                        logSb.Append(
                            $"同步接口信息成功。新增 {addApiInfoList.Count} 个，更新 {updateApiInfoList.Count} 个，删除 {deleteApiInfoList.Count} 个。");
                        logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                        Console.WriteLine(logSb.ToString());
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Sync api error...");
                }
            }, cancellationToken);
        });

        await Task.CompletedTask;
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Stop operation.</returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}