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

using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Fast.AdminLog.Entity;
using Fast.SqlSugar;
using Fast.UnifyResult;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.Core;

/// <summary>
/// <see cref="GlobalExceptionHandler"/> 全局异常处理
/// </summary>
public class GlobalExceptionHandler : IGlobalExceptionHandler
{
    /// <summary>
    /// 日志
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// <see cref="GlobalExceptionHandler"/> 全局异常处理
    /// </summary>
    /// <param name="logger"><see cref="ILogger"/> 日志</param>
    public GlobalExceptionHandler(ILogger<IGlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>异常拦截</summary>
    /// <param name="context"><see cref="T:Microsoft.AspNetCore.Mvc.Filters.ExceptionContext" /></param>
    /// <param name="isUserFriendlyException"><see cref="T:System.Boolean" /> 是否友好异常</param>
    /// <param name="isValidationException"><see cref="T:System.Boolean" /> 是否验证异常</param>
    /// <returns></returns>
    public async Task OnExceptionAsync(ExceptionContext context, bool isUserFriendlyException, bool isValidationException)
    {
        // 行版本更新异常直接忽略
        if (context.Exception is VersionExceptions)
            return;

        var httpContext = context.HttpContext;
        var message = new StringBuilder();

        try
        {
            message.AppendLine(context.Exception.Message);
            message.AppendLine(
                $"请求Url：{httpContext.Request.Method}, {httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}");

            var deviceType = httpContext.Request.Headers[HttpHeaderConst.DeviceType]
                .ToString()
                .UrlDecode();
            var deviceId = httpContext.Request.Headers[HttpHeaderConst.DeviceId]
                .ToString()
                .UrlDecode();

            message.AppendLine($"device: {deviceType}, {deviceId}");
            if (httpContext.Items.TryGetValue($"{nameof(Fast)}.RequestParams", out var requestParams))
            {
                message.AppendLine($"请求参数: {requestParams}");
            }
            else
            {
                message.AppendLine("请求参数: 无");
            }

            if (httpContext.RequestAborted.IsCancellationRequested)
            {
                message.AppendLine("连接被客户端强制关闭。");
                // 写入警告日志
                _logger.LogWarning(message.ToString());
                return;
            }
        }
        // 客户端中途断开
        catch (ConnectionResetException)
        {
            message.AppendLine("连接被客户端强制关闭。");
            // 写入警告日志
            _logger.LogWarning(message.ToString());
            return;
        }
        catch (SocketException socketException) when (socketException.SocketErrorCode == SocketError.ConnectionReset)
        {
            message.AppendLine("连接被客户端强制关闭。");
            // 写入警告日志
            _logger.LogWarning(message.ToString());
            return;
        }
        // Kestrel 封装的管道读写抛出 IOException
        catch (IOException ioException) when (ioException.InnerException is SocketException
                                              {
                                                  SocketErrorCode: SocketError.ConnectionReset
                                              })
        {
            message.AppendLine("连接被客户端强制关闭。");
            // 写入警告日志
            _logger.LogWarning(message.ToString());
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(context.Exception, "全局异常原始错误。");
            _logger.LogError(ex, "全局异常拦截失败。");
        }

        // 判断是否为友好异常
        if (isUserFriendlyException)
        {
            // 只写入最深的一条堆栈信息
            var firstLine = context.Exception.StackTrace?.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault();

            // 如果有匹配的堆栈信息，选择第一条（最深的那一条）
            if (!string.IsNullOrWhiteSpace(firstLine))
            {
                message.AppendLine($"{firstLine}");
            }
            else
            {
                message.AppendLine("未找到堆栈信息...");
            }

            // 写入警告日志
            _logger.LogWarning(message.ToString());
        }
        // 判断是否为验证异常
        else if (isValidationException)
        {
            // 写入警告日志
            _logger.LogWarning(message.ToString());
        }
        else
        {
            var className = context.Exception.TargetSite?.DeclaringType?.FullName;
            var methodName = "";
            var groupCollection = Regex.Match(className, "<(.*?)>")
                .Groups;
            if (groupCollection.Count > 1)
                methodName = groupCollection[1].Value;

            // 获取 CenterLog 库的连接字符串配置
            var connectionConfig = SqlSugarContext.GetConnectionConfig(GlobalContext.LogConnectionSettings);

            var _user = httpContext.RequestServices.GetService<IUser>();
            var exceptionLogModel = new ExceptionLogModel
            {
                RecordId = YitIdHelper.NextId(),
                AccountId = _user?.AccountId,
                Mobile = _user?.Mobile,
                NickName = _user?.NickName,
                ClassName = context.Exception.TargetSite?.DeclaringType?.FullName,
                MethodName = methodName,
                Message = context.Exception.Message,
                Source = context.Exception.Source,
                StackTrace = context.Exception.StackTrace,
                ParamsObj = context.Exception.TargetSite?.GetParameters()
                    .Select(sl => new
                    {
                        PropertyName = sl.Name, TypeName = sl.ParameterType.Name, TypeFullName = sl.ParameterType.FullName
                    })
                    .ToList()
                    .ToJsonString(),
                DepartmentId = _user?.DepartmentId,
                DepartmentName = _user?.DepartmentName,
                CreatedUserId = _user?.EmployeeId,
                CreatedUserName = _user?.EmployeeName,
                CreatedTime = DateTime.Now
            };
            exceptionLogModel.RecordCreate(httpContext);

            _ = Task.Run(async () =>
            {
                try
                {
                    // 这里不能使用Aop
                    var db = new SqlSugarClient(connectionConfig);

                    // 异步不等待
                    await db.Insertable(exceptionLogModel)
                        .ExecuteCommandAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Global Exception 执行Sql，保存失败；{ex.Message}");
                }
            });
            // 写入错误日志
            _logger.LogError(context.Exception, message.ToString());
        }

        await Task.CompletedTask;
    }
}