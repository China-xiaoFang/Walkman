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

using Fast.JwtBearer;
using Fast.UnifyResult;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Core;

/// <summary>
/// <see cref="JwtBearerHandle"/> Jwt验证提供器
/// </summary>
public class JwtBearerHandle : IJwtBearerHandle
{
    /// <summary>授权处理</summary>
    /// <remarks>这里已经判断了 Token 是否有效，并且已经处理了自动刷新 Token。只需要处理其余的逻辑即可。如果返回 false或抛出异常，搭配 AuthorizeFailHandle 则抛出 HttpStatusCode = 401 状态码，否则走默认处理 AuthorizationHandlerContext.Fail() 会返回 HttpStatusCode = 403 状态码</remarks>
    /// <param name="context"><see cref="T:Microsoft.AspNetCore.Authorization.AuthorizationHandlerContext" /></param>
    /// <param name="httpContext"><see cref="T:Microsoft.AspNetCore.Http.HttpContext" /></param>
    /// <returns><see cref="T:System.Boolean" /></returns>
    public async Task<bool> AuthorizeHandle(AuthorizationHandlerContext context, HttpContext httpContext)
    {
        if (httpContext.User.Identity?.IsAuthenticated == false)
            return false;

        // 获取 IUser，当前请求生命周期，只会解析一次
        var _user = httpContext.RequestServices.GetService<IUser>();

        // 从 AccessToken 中读取 Data
        var data = httpContext.User.Claims.FirstOrDefault(f => f.Type == "Data")!.Value;
        var payload = data.Base64ToString()
            .ToObject<Dictionary<string, string>>();
        // 从 payload 中读取 DeviceType,AppNo,TenantNo,EmployeeNo
        if (payload.TryGetValue(nameof(AuthUserInfo.DeviceType), out var deviceType)
            && payload.TryGetValue(nameof(AuthUserInfo.EmployeeNo), out var employeeNo))
        {
            // 获取授权用户信息
            var authUserInfo = await _user.GetAuthUserInfo(Enum.Parse<AppEnvironmentEnum>(deviceType, true), employeeNo);

            if (authUserInfo == null)
                return false;

            // 判断设备信息是否和缓存中的一致
            if (GlobalContext.DeviceId != authUserInfo.DeviceId || GlobalContext.DeviceType != authUserInfo.DeviceType)
                return false;

            // 设置授权用户
            _user.SetAuthUser(authUserInfo);

            return true;
        }

        return false;
    }

    /// <summary>授权失败处理</summary>
    /// <remarks>如果返回 null，则走默认处理 AuthorizationHandlerContext.Fail()</remarks>
    /// <param name="context"><see cref="T:Microsoft.AspNetCore.Authorization.AuthorizationHandlerContext" /></param>
    /// <param name="httpContext"><see cref="T:Microsoft.AspNetCore.Http.HttpContext" /></param>
    /// <param name="exception"><see cref="T:System.Exception" /></param>
    /// <returns></returns>
    public async Task<object> AuthorizeFailHandle(AuthorizationHandlerContext context, HttpContext httpContext,
        Exception exception)
    {
        return await Task.FromResult(UnifyContext.GetRestfulResult(StatusCodes.Status401Unauthorized, false, null,
            exception?.Message ?? "401 未经授权", httpContext));
    }

    /// <summary>权限判断处理</summary>
    /// <remarks>这里只需要判断你的权限逻辑即可，如果返回 false或抛出异常 则抛出 HttpStatusCode = 403 状态码</remarks>
    /// <param name="context"><see cref="T:Microsoft.AspNetCore.Authorization.AuthorizationHandlerContext" /></param>
    /// <param name="requirement"><see cref="T:Microsoft.AspNetCore.Authorization.IAuthorizationRequirement" /></param>
    /// <param name="httpContext"><see cref="T:Microsoft.AspNetCore.Http.HttpContext" /></param>
    /// <returns></returns>
    public async Task<bool> PermissionHandle(AuthorizationHandlerContext context, IAuthorizationRequirement requirement,
        HttpContext httpContext)
    {
        // 获取 IUser
        var _user = httpContext.RequestServices.GetService<IUser>();

        // 超级管理员有所有的权限
        if (_user.IsSuperAdmin)
            return true;

        // 获取权限标识
        var permissionAttribute = httpContext.GetEndpoint()
            ?.Metadata.GetMetadata<PermissionAttribute>();

        if (permissionAttribute?.TagList == null || permissionAttribute.TagList.Count == 0)
            return true;

        // 输出权限标识
        httpContext.Response.Headers.TryAdd("Auth-Permission", string.Join(",", permissionAttribute.TagList));

        if (_user.ButtonCodeList == null || _user.ButtonCodeList?.Count == 0)
            return false;

        // 满足一个即可
        if (_user.ButtonCodeList.Intersect(permissionAttribute.TagList)
            .Any())
            return true;

        return await Task.FromResult(false);
    }

    /// <summary>权限判断失败处理</summary>
    /// <remarks>如果返回 null，则走默认处理 AuthorizationHandlerContext.Fail()</remarks>
    /// <param name="context"><see cref="T:Microsoft.AspNetCore.Authorization.AuthorizationHandlerContext" /></param>
    /// <param name="requirement"><see cref="T:Microsoft.AspNetCore.Authorization.IAuthorizationRequirement" /></param>
    /// <param name="httpContext"><see cref="T:Microsoft.AspNetCore.Http.HttpContext" /></param>
    /// <param name="exception"><see cref="T:System.Exception" /></param>
    /// <returns></returns>
    public async Task<object> PermissionFailHandle(AuthorizationHandlerContext context, IAuthorizationRequirement requirement,
        HttpContext httpContext, Exception exception)
    {
        return await Task.FromResult<object>(null);
    }
}