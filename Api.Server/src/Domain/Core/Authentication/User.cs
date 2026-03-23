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

using Fast.AdminLog.Entity;
using Fast.AdminLog.Enum;
using Fast.JwtBearer;
using Fast.SqlSugar;
using Microsoft.AspNetCore.Http;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.Core;

/// <summary>
/// <see cref="User"/> 授权用户信息
/// </summary>
/// <remarks>作用域注册，保证当前请求管道中是唯一的，并且只会加载一次</remarks>
public sealed class User : AuthUserInfo, IUser, IScopedDependency
{
    /// <summary>
    /// 是否存在用户信息
    /// </summary>
    private bool _hasUserInfo { get; set; }

    /// <summary>
    /// 缓存
    /// </summary>
    private readonly ICache<AuthCCL> _authCache;

    /// <summary>
    /// 请求上下文
    /// </summary>
    private readonly HttpContext _httpContext;

    /// <summary>
    /// <see cref="User"/> 授权用户信息
    /// </summary>
    /// <param name="authCache"><see cref="ICache"/> 缓存</param>
    /// <param name="httpContextAccessor"><see cref="IHttpContextAccessor"/> 请求上下文</param>
    /// <remarks>作用域注册，保证当前请求管道中是唯一的，并且只会加载一次</remarks>
    public User(ICache<AuthCCL> authCache, IHttpContextAccessor httpContextAccessor)
    {
        _authCache = authCache;
        _httpContext = httpContextAccessor.HttpContext;
    }

    /// <summary>
    /// 设置授权用户
    /// </summary>
    /// <param name="authUserInfo"><see cref="AuthUserInfo"/> 授权用户信息</param>
    /// <param name="forceUserInfo"><see cref="bool"/> 强制覆盖用户信息，默认 <c>false</c></param>
    /// <remarks>只会赋值一次</remarks>
    public void SetAuthUser(AuthUserInfo authUserInfo, bool forceUserInfo = false)
    {
        if (_hasUserInfo && !forceUserInfo)
            return;

        if (authUserInfo == null)
        {
            throw new UnauthorizedAccessException("授权用户信息不存在！");
        }

        // 设置授权用户信息
        DeviceType = authUserInfo.DeviceType;
        DeviceId = authUserInfo.DeviceId;

        // 账号
        AccountId = authUserInfo.AccountId;
        Mobile = authUserInfo.Mobile;
        NickName = authUserInfo.NickName;
        Avatar = authUserInfo.Avatar;

        // 微信用户
        WeChatId = authUserInfo.WeChatId;
        WeChatOpenId = authUserInfo.WeChatOpenId;

        EmployeeId = authUserInfo.EmployeeId;
        EmployeeNo = authUserInfo.EmployeeNo;
        EmployeeName = authUserInfo.EmployeeName;
        DepartmentId = authUserInfo.DepartmentId;
        DepartmentName = authUserInfo.DepartmentName;
        IsSuperAdmin = authUserInfo.IsSuperAdmin;
        IsAdmin = authUserInfo.IsAdmin;
        LastLoginDevice = authUserInfo.LastLoginDevice;
        LastLoginOS = authUserInfo.LastLoginOS;
        LastLoginBrowser = authUserInfo.LastLoginBrowser;
        LastLoginProvince = authUserInfo.LastLoginProvince;
        LastLoginCity = authUserInfo.LastLoginCity;
        LastLoginIp = authUserInfo.LastLoginIp;
        LastLoginTime = authUserInfo.LastLoginTime;
        RoleIdList = authUserInfo.RoleIdList;
        RoleNameList = authUserInfo.RoleNameList;
        RoleType = authUserInfo.RoleType;
        DataScopeType = authUserInfo.DataScopeType;
        MenuCodeList = authUserInfo.MenuCodeList;
        ButtonCodeList = authUserInfo.ButtonCodeList;
        _hasUserInfo = true;
    }

    /// <summary>
    /// 从缓存中获取授权用户信息
    /// </summary>
    /// <param name="deviceType"><see cref="AppEnvironmentEnum"/> 设备类型</param>
    /// <param name="employeeNo"><see cref="string"/> 工号</param>
    /// <returns></returns>
    public async Task<AuthUserInfo> GetAuthUserInfo(AppEnvironmentEnum deviceType, string employeeNo)
    {
        // 获取缓存Key
        var cacheKey = CacheConst.GetCacheKey(CacheConst.AuthUser, deviceType.ToString(), employeeNo);

        return await _authCache.GetAsync<AuthUserInfo>(cacheKey);
    }

    /// <summary>
    /// 统一登录
    /// </summary>
    /// <param name="authUserInfo"><see cref="AuthUserInfo"/> 授权用户信息</param>
    /// <returns></returns>
    public async Task Login(AuthUserInfo authUserInfo)
    {
        if (authUserInfo == null || string.IsNullOrWhiteSpace(authUserInfo.Mobile))
        {
            throw new UnauthorizedAccessException("账号信息不存在！");
        }

        if (string.IsNullOrWhiteSpace(authUserInfo.DeviceId))
        {
            throw new UnauthorizedAccessException("未知的设备！");
        }

        if (string.IsNullOrWhiteSpace(authUserInfo.EmployeeNo))
        {
            throw new UnauthorizedAccessException("员工信息不存在！");
        }

        try
        {
            // 设置授权用户信息
            SetAuthUser(authUserInfo, true);

            var payload = new Dictionary<string, string>
            {
                {nameof(DeviceType), authUserInfo.DeviceType.ToString()},
                {nameof(DeviceId), authUserInfo.DeviceId},
                {nameof(EmployeeNo), authUserInfo.EmployeeNo},
                {nameof(LastLoginIp), authUserInfo.LastLoginIp},
                {nameof(LastLoginTime), authUserInfo.LastLoginTime.ToString("yyyy-MM-dd HH:mm:ss")}
            };

            var data = payload.ToJsonString()
                .ToBase64();

            // 生成 AccessToken
            var accessToken = JwtBearerUtil.GenerateToken(new Dictionary<string, object> {{"Data", data}});

            // 生成 RefreshToken
            var refreshToken = JwtBearerUtil.GenerateRefreshToken(accessToken);

            // 获取缓存Key
            var cacheKey = CacheConst.GetCacheKey(CacheConst.AuthUser,
                authUserInfo.DeviceType.ToString(), authUserInfo.EmployeeNo);

            // 设置缓存信息
            await _authCache.SetAsync(cacheKey, authUserInfo);

            // 设置 AccessToken
            _httpContext.Response.Headers["access-token"] = accessToken;

            // 设置 RefreshToken
            _httpContext.Response.Headers["x-access-token"] = refreshToken;

            // 设置Swagger自动登录
            _httpContext.SignInToSwagger(accessToken);
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException($"401 登录鉴权失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 客户端统一登录
    /// </summary>
    /// <param name="authUserInfo"><see cref="AuthUserInfo"/> 授权用户信息</param>
    /// <returns></returns>
    public async Task ClientLogin(AuthUserInfo authUserInfo)
    {
        if (authUserInfo == null)
        {
            throw new UnauthorizedAccessException("用户信息不存在！");
        }

        if (string.IsNullOrWhiteSpace(authUserInfo.DeviceId))
        {
            throw new UnauthorizedAccessException("未知的设备！");
        }

        if (string.IsNullOrWhiteSpace(authUserInfo.WeChatOpenId))
        {
            throw new UnauthorizedAccessException("用户信息不存在！");
        }

        try
        {
            // 设置授权用户信息
            SetAuthUser(authUserInfo, true);

            var payload = new Dictionary<string, string>
            {
                {nameof(DeviceType), authUserInfo.DeviceType.ToString()},
                {nameof(DeviceId), authUserInfo.DeviceId},
                {nameof(EmployeeNo), authUserInfo.WeChatOpenId},
                {nameof(LastLoginIp), authUserInfo.LastLoginIp},
                {nameof(LastLoginTime), authUserInfo.LastLoginTime.ToString("yyyy-MM-dd HH:mm:ss")}
            };

            var data = payload.ToJsonString()
                .ToBase64();

            // 生成 AccessToken
            var accessToken = JwtBearerUtil.GenerateToken(new Dictionary<string, object> {{"Data", data}});

            // 生成 RefreshToken
            var refreshToken = JwtBearerUtil.GenerateRefreshToken(accessToken);

            // 获取缓存Key
            var cacheKey = CacheConst.GetCacheKey(CacheConst.AuthUser, authUserInfo.DeviceType.ToString(),
                authUserInfo.WeChatOpenId);

            // 设置缓存信息
            await _authCache.SetAsync(cacheKey, authUserInfo);

            // 设置 AccessToken
            _httpContext.Response.Headers["access-token"] = accessToken;

            // 设置 RefreshToken
            _httpContext.Response.Headers["x-access-token"] = refreshToken;

            // 设置Swagger自动登录
            _httpContext.SignInToSwagger(accessToken);
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException($"401 登录鉴权失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 刷新授权信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task RefreshAuth(RefreshAuthDto input)
    {
        if (string.IsNullOrWhiteSpace(input.EmployeeNo))
        {
            throw new UnauthorizedAccessException("员工信息不存在！");
        }

        // 设置授权用户信息
        RoleIdList = input.RoleIdList;
        RoleNameList = input.RoleNameList;
        RoleType = input.RoleType;
        DataScopeType = input.DataScopeType;
        MenuCodeList = input.MenuCodeList;
        ButtonCodeList = input.ButtonCodeList;

        // 获取缓存Key
        var cacheKey = CacheConst.GetCacheKey(CacheConst.AuthUser, input.DeviceType.ToString(), input.EmployeeNo);

        // 设置缓存信息
        await _authCache.SetAsync(cacheKey, this);
    }

    /// <summary>
    /// 刷新账号信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task RefreshAccount(RefreshAccountDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Mobile))
        {
            throw new UnauthorizedAccessException("账号信息不存在！");
        }

        if (string.IsNullOrWhiteSpace(input.EmployeeNo))
        {
            throw new UnauthorizedAccessException("员工信息不存在！");
        }

        // 设置授权用户信息
        Mobile = input.Mobile;
        NickName = input.NickName;
        Avatar = input.Avatar;

        // 获取缓存Key
        var cacheKey = CacheConst.GetCacheKey(CacheConst.AuthUser, input.DeviceType.ToString(), input.EmployeeNo);

        // 设置缓存信息
        await _authCache.SetAsync(cacheKey, this);
    }

    /// <summary>
    /// 刷新微信用户信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task RefreshWeChatUser(RefreshWeChatUserDto input)
    {
        if (string.IsNullOrWhiteSpace(input.WeChatOpenId))
        {
            throw new UnauthorizedAccessException("用户信息不存在！");
        }

        // 设置授权用户信息
        Mobile = input.Mobile;
        NickName = input.NickName;
        Avatar = input.Avatar;

        // 获取缓存Key
        var cacheKey = CacheConst.GetCacheKey(CacheConst.AuthUser, input.DeviceType.ToString(), input.WeChatOpenId);

        // 设置缓存信息
        await _authCache.SetAsync(cacheKey, this);
    }

    /// <summary>
    /// 刷新职员信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task RefreshEmployee(RefreshEmployeeDto input)
    {
        if (string.IsNullOrWhiteSpace(input.EmployeeNo))
        {
            throw new UnauthorizedAccessException("员工信息不存在！");
        }

        // 设置授权用户信息
        EmployeeName = input.EmployeeName;
        DepartmentId = input.DepartmentId;
        DepartmentName = input.DepartmentName;
        RoleIdList = input.RoleIdList;
        RoleNameList = input.RoleNameList;
        RoleType = input.RoleType;
        DataScopeType = input.DataScopeType;

        // 获取缓存Key
        var cacheKey = CacheConst.GetCacheKey(CacheConst.AuthUser, input.DeviceType.ToString(), input.EmployeeNo);

        // 设置缓存信息
        await _authCache.SetAsync(cacheKey, this);
    }

    /// <summary>
    /// 统一退出登录
    /// </summary>
    /// <returns></returns>
    public async Task Logout()
    {
        /*
         * 首先确定，退出登录有两种情况，
         *  1.正常情况，点击退出登录，这个时候的Token是存在的，且没有过期的。
         *  2.401的情况下，系统调用退出登录的接口，这个时候虽然存在Token，但是Token肯定是过期的。
         */

        // 这里直接从请求头中获取 AccessToken
        var accessToken = JwtBearerUtil.GetJwtBearerToken(_httpContext);

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            // 标记过期
            JwtBearerUtil.SetExpiredToken(_httpContext, accessToken);

            try
            {
                // 读取 AccessToken，不验证
                var accessTokenIdentity = JwtBearerUtil.ReadJwtToken(accessToken);
                // 从 AccessToken 中读取 Data
                var data = accessTokenIdentity.Claims.FirstOrDefault(f => f.Type == "Data")!.Value;
                var payload = data.Base64ToString()
                    .ToObject<Dictionary<string, string>>();
                // 从 payload 中读取 DeviceType,DeviceId,AppNo,TenantNo,EmployeeNo
                if (payload.TryGetValue(nameof(DeviceType), out var deviceType)
                    && payload.TryGetValue(nameof(DeviceId), out var deviceId)
                    && payload.TryGetValue(nameof(EmployeeNo), out var employeeNo))
                {
                    // 尝试获取缓存
                    var cacheKey = CacheConst.GetCacheKey(CacheConst.AuthUser, deviceType, employeeNo);
                    var authUserInfo = await _authCache.GetAsync<AuthUserInfo>(cacheKey);
                    if (authUserInfo != null)
                    {
                        // 添加登出日志
                        var visitLogModel = new VisitLogModel
                        {
                            RecordId = YitIdHelper.NextId(),
                            AccountId = authUserInfo.AccountId,
                            Mobile = authUserInfo.Mobile,
                            NickName = authUserInfo.NickName,
                            VisitType = VisitTypeEnum.Logout,
                            DepartmentId = authUserInfo.DepartmentId,
                            DepartmentName = authUserInfo.DepartmentName,
                            CreatedUserId = authUserInfo.EmployeeId,
                            CreatedUserName = authUserInfo.EmployeeName,
                            CreatedTime = DateTime.Now
                        };
                        visitLogModel.RecordCreate(_httpContext);

                        // 获取 CenterLog 库的连接字符串配置
                        var connectionConfig = SqlSugarContext.GetConnectionConfig(GlobalContext.LogConnectionSettings);

                        // 这里不能使用Aop
                        var db = new SqlSugarClient(connectionConfig);

                        // 异步不等待
                        await db.Insertable(visitLogModel)
                            .SplitTable()
                            .ExecuteCommandAsync();

                        // 判断缓存中的设备信息是否和当前 AccessToken 中的相同
                        if (authUserInfo.DeviceId == deviceId && authUserInfo.DeviceType.ToString() == deviceType)
                        {
                            // 清除缓存用户信息
                            await _authCache.DelAsync(cacheKey);
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        // 设置Swagger退出登录
        _httpContext.SignOutToSwagger();
    }
}