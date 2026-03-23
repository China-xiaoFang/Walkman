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

using Fast.UnifyResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Fast.Core;

/// <summary>
/// <see cref="UnifyResponseProvider"/> 规范化响应数据提供器
/// </summary>
public class UnifyResponseProvider : IUnifyResponseProvider
{
    /// <summary>
    /// 日志
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// <see cref="UnifyResponseProvider"/> 规范化响应数据提供器
    /// </summary>
    /// <param name="logger"><see cref="ILogger"/> 日志</param>
    public UnifyResponseProvider(ILogger<IUnifyResponseProvider> logger)
    {
        _logger = logger;
    }

    /// <summary>响应异常处理</summary>
    /// <param name="context"><see cref="T:Microsoft.AspNetCore.Mvc.Filters.ExceptionContext" /></param>
    /// <param name="metadata"><see cref="T:Fast.UnifyResult.ExceptionMetadata" /> 异常元数据</param>
    /// <param name="httpContext"><see cref="T:Microsoft.AspNetCore.Http.HttpContext" /> 请求上下文</param>
    /// <returns></returns>
    public async Task<(int statusCode, string message)> ResponseExceptionAsync(ExceptionContext context,
        ExceptionMetadata metadata, HttpContext httpContext)
    {
        // 默认 500 错误
        var statusCode = StatusCodes.Status500InternalServerError;

        var message = context.Exception.Message;

        switch (context.Exception)
        {
            // 友好异常处理
            case UserFriendlyException userFriendlyException:
            {
                if (userFriendlyException.OriginErrorCode != null)
                {
                    statusCode = userFriendlyException.OriginErrorCode.ToString()
                        .ParseToInt();
                }
                else if (userFriendlyException.ErrorCode != null)
                {
                    statusCode = userFriendlyException.ErrorCode.ToString()
                        .ParseToInt();
                }
                else
                {
                    statusCode = userFriendlyException.StatusCode;
                }

                // 处理可能为0的情况
                statusCode = statusCode == 0 ? StatusCodes.Status400BadRequest : statusCode;
                break;
            }
            // SqlSugar 并发处理
            case VersionExceptions versionExceptions:
                statusCode = StatusCodes.Status400BadRequest;
                message = "数据已更改，请刷新后重试！\r\n" + versionExceptions.Message;
                break;
            // 操作异常
            case InvalidOperationException invalidOperationException:
                statusCode = StatusCodes.Status500InternalServerError;
                if (invalidOperationException.InnerException is SqlException sqlException)
                {
                    message = sqlException.Message;
                }
                else
                {
                    message = invalidOperationException.Message;
                }

                break;
        }

        return await Task.FromResult((statusCode, message));
    }

    /// <summary>响应数据验证异常处理</summary>
    /// <param name="context"><see cref="T:Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext" /></param>
    /// <param name="metadata"><see cref="T:Fast.UnifyResult.ValidationMetadata" /> 验证信息元数据</param>
    /// <param name="httpContext"><see cref="T:Microsoft.AspNetCore.Http.HttpContext" /> 请求上下文</param>
    /// <returns></returns>
    public async Task ResponseValidationExceptionAsync(ActionExecutingContext context, ValidationMetadata metadata,
        HttpContext httpContext)
    {
        await Task.CompletedTask;
    }

    /// <summary>响应数据处理</summary>
    /// <remarks>只有响应成功且为正常返回才会调用</remarks>
    /// <param name="timestamp"><see cref="T:System.Int64" /> 响应时间戳</param>
    /// <param name="data"><see cref="T:System.Object" /> 数据</param>
    /// <param name="httpContext"><see cref="T:Microsoft.AspNetCore.Http.HttpContext" /> 请求上下文</param>
    /// <returns></returns>
    public async Task<object> ResponseDataAsync(long timestamp, object data, HttpContext httpContext)
    {
        // 响应加密特性
        var responseEncipherAttribute = httpContext.GetEndpoint()
            ?.Metadata.GetMetadata<ResponseEncipherAttribute>();

        bool responseEncipher;

        if (responseEncipherAttribute == null)
        {
            // 获取配置
            var requestEncryptionStr = await ConfigContext.GetConfig(ConfigConst.RequestEncryption);
            var requestEncryption = bool.TryParse(requestEncryptionStr, out var flag) && flag;
            responseEncipher = requestEncryption;
        }
        else if (responseEncipherAttribute.Enable)
        {
            responseEncipher = true;
        }
        else
        {
            responseEncipher = false;
        }

        // 判断是否开启响应加密
        if (!responseEncipher)
            return await Task.FromResult(data);

        // 添加加密头部标识
        httpContext.Response.Headers.TryAdd($"{nameof(Fast)}-Response-Encipher", "True");

        var dataStr = data.ToJsonString();

        // 加密数据
        var encryptedStr = CryptoUtil.AESEncrypt(dataStr, timestamp.ToString(), $"FIV{timestamp}");

        return await Task.FromResult(encryptedStr);
    }
}