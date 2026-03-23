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
using System.Reflection;
using Fast.AdminLog.Entity;
using Fast.SqlSugar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.Core;

/// <summary>
/// <see cref="RequestActionFilter"/> 请求日志拦截
/// </summary>
public class RequestActionFilter : IAsyncActionFilter
{
    /// <summary>
    /// 日志
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// <see cref="RequestActionFilter"/> 请求日志拦截
    /// </summary>
    /// <param name="logger"><see cref="ILogger"/> 日志</param>
    public RequestActionFilter(ILogger<IAsyncActionFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called asynchronously before the action, after model binding is complete.
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext" />.</param>
    /// <param name="next">
    /// The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate" />. Invoked to execute the next action filter or the action itself.
    /// </param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that on completion indicates the filter has executed.</returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var dateTime = DateTime.Now;
        var httpContext = context.HttpContext;
        var httpRequest = httpContext.Request;
        var _user = httpContext.RequestServices.GetService<IUser>();

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var actionContext = await next();
        stopwatch.Stop();

        var actionDescriptor = actionContext.ActionDescriptor as ControllerActionDescriptor;

        // 判断是否存在禁用请求日志特性
        if (actionDescriptor?.MethodInfo.GetCustomAttribute<DisabledRequestLogAttribute>() != null)
            return;

        var apiInfoAttribute = actionDescriptor?.MethodInfo.GetCustomAttribute<ApiInfoAttribute>();
        if (!Enum.TryParse(httpRequest.Method, true, out HttpRequestMethodEnum requestMethod))
        {
            // 默认 Get 请求
            requestMethod = HttpRequestMethodEnum.Get;
        }

        // 获取 CenterLog 库的连接字符串配置
        var connectionConfig = SqlSugarContext.GetConnectionConfig(GlobalContext.LogConnectionSettings);

        var requestLogModel = new RequestLogModel
        {
            RecordId = YitIdHelper.NextId(),
            AccountId = _user?.AccountId,
            Mobile = _user?.Mobile,
            NickName = _user?.NickName,
            IsSuccess = actionContext.Exception == null,
            OperationAction = apiInfoAttribute?.Action ?? HttpRequestActionEnum.None,
            OperationName = apiInfoAttribute?.Name,
            ClassName = context.Controller.ToString(),
            MethodName = actionDescriptor?.ActionName,
            Location = httpRequest.Path,
            RequestMethod = requestMethod,
            Param = context.ActionArguments.Count < 1 ? "" : context.ActionArguments.ToJsonString(),
            Result =
                (actionContext.Result?.GetType() == typeof(JsonResult) ? actionContext.Result.ToJsonString() : null)
                ?? actionContext.Exception?.ToJsonString(),
            ElapsedTime = stopwatch.ElapsedMilliseconds,
            DepartmentId = _user?.DepartmentId,
            DepartmentName = _user?.DepartmentName,
            CreatedUserId = _user?.EmployeeId,
            CreatedUserName = _user?.EmployeeName,
            CreatedTime = dateTime
        };
        requestLogModel.RecordCreate(httpContext);

        _ = Task.Run(async () =>
        {
            try
            {
                // 这里不能使用Aop
                var db = new SqlSugarClient(connectionConfig);

                // 异步不等待
                await db.Insertable(requestLogModel)
                    .SplitTable()
                    .ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Request Action 执行Sql，保存失败；{ex.Message}");
            }
        });
    }
}