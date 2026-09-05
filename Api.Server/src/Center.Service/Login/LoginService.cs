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

using System.Security.Cryptography;
using System.Text;
using Fast.Cache;
using Fast.Center.Domain;
using Fast.Center.Service.Login.Dto;
using Fast.CenterLog.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Yitter.IdGenerator;

namespace Fast.Center.Service.Login;

/// <summary>
/// 登录服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Auth, Name = "login")]
public partial class LoginService : IDynamicApplication
{
    private readonly IUser _user;
    private readonly ICache<CenterCCL> _centerCache;
    private readonly ICaptchaService _captchaService;
    private readonly HttpContext _httpContext;
    private readonly ISqlSugarClient _repository;

    public LoginService(IUser user, IHttpContextAccessor httpContextAccessor, ICache<CenterCCL> centerCache,
        ICaptchaService captchaService, ISqlSugarClient repository)
    {
        _user = user;
        _httpContext = httpContextAccessor.HttpContext;
        _centerCache = centerCache;
        _captchaService = captchaService;
        _repository = repository;
    }

    /// <summary>
    /// 判断登录图片验证码是否启用
    /// </summary>
    private async Task<bool> IsLoginCaptchaEnabled()
    {
        return bool.Parse(await ConfigContext.GetConfig(ConfigConst.LoginCaptchaOpen));
    }

    /// <summary>
    /// 生成HashCode
    /// </summary>
    private string GenerateHashCode(string code)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(code ?? string.Empty)));
    }

    /// <summary>
    /// 租户登录凭证缓存Dto
    /// </summary>
    private class TenantLoginTicketCacheDto
    {
        /// <summary>
        /// 账号Id
        /// </summary>
        public long AccountId { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 设备标识
        /// </summary>
        public string ClientIdentity { get; set; }

        /// <summary>
        /// 密码Hash
        /// </summary>
        public string PasswordHash { get; set; }
    }

    /// <summary>
    /// 生成临时租户登录凭证
    /// </summary>
    private async Task<string> GetTenantLoginTicket(AccountModel accountModel)
    {
        var loginTicket = Guid.NewGuid()
            .ToString("N");
        var cacheKey = CacheConst.GetCacheKey(CacheConst.TenantLoginTicket, loginTicket);
        await _centerCache.SetAsync(cacheKey,
            new TenantLoginTicketCacheDto
            {
                AccountId = accountModel.AccountId,
                Mobile = accountModel.Mobile,
                Email = accountModel.Email,
                ClientIdentity = GlobalContext.ClientIdentity,
                PasswordHash = GenerateHashCode(accountModel.Password)
            }, TimeSpan.FromMinutes(5));

        return loginTicket;
    }

    /// <summary>
    /// 确保应用安全
    /// </summary>
    private async Task<ApplicationOpenIdModel> EnsureApplication()
    {
        // 查询应用信息
        var applicationModel = await ApplicationContext.GetApplication(GlobalContext.Origin);

        if (applicationModel.AppType != GlobalContext.DeviceType)
        {
            throw new UserFriendlyException("应用类型不匹配！");
        }

        return applicationModel;
    }

    /// <summary>
    /// 验证并一次性消费租户登录凭证
    /// </summary>
    private async Task VerifyTenantLoginTicket(string loginTicket, AccountModel account)
    {
        if (!Guid.TryParseExact(loginTicket, "N", out _))
        {
            throw new UserFriendlyException("登录凭据已失效，请返回重新登录！");
        }

        var cacheKey = CacheConst.GetCacheKey(CacheConst.TenantLoginTicket, loginTicket);
        using var codeLock = _centerCache.Client.TryLock($"{cacheKey}:Lock", 30);
        if (codeLock == null)
        {
            throw new UserFriendlyException("操作过于频繁，请稍后重试！");
        }

        var cacheDto = await _centerCache.GetAsync<TenantLoginTicketCacheDto>(cacheKey);
        if (cacheDto == null
            || cacheDto.AccountId != account.AccountId
            || cacheDto.Mobile != account.Mobile
            || cacheDto.Email != account.Email
            || cacheDto.ClientIdentity != GlobalContext.ClientIdentity
            || cacheDto.PasswordHash != GenerateHashCode(account.Password))
        {
            throw new UserFriendlyException("登录凭据已失效，请返回重新登录！");
        }

        await _centerCache.DelAsync(cacheKey);
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    /// <param name="accountModel">账号信息</param>
    /// <param name="password">待验证的原始密码</param>
    /// <param name="dateTime">操作时间</param>
    private async Task VerifyPassword(AccountModel accountModel, string password, DateTime dateTime)
    {
        if (accountModel.Status == CommonStatusEnum.Disable)
            throw new UserFriendlyException("账号已被平台禁用！");

        if (string.IsNullOrWhiteSpace(accountModel.Password))
        {
            throw new UserFriendlyException("未设定密码，请重置密码后重试！");
        }

        if (accountModel.LockEndTime != null && accountModel.LockEndTime > dateTime)
        {
            var unLockTimeSpan = accountModel.LockEndTime.Value - dateTime;
            throw new UserFriendlyException($"账号已被锁定，请 {unLockTimeSpan.ToDescription()} 后再重试！");
        }

        /*
         * 连续错误3次，锁定1分钟
         * 连续错误5次，锁定5分钟
         * 连续错误10次，锁定账号
         * 登录成功后清除锁定信息
         */
        if (!CryptoUtil.VerifyPasswordPBKDF2SHA256(password, accountModel.Password))
        {
            accountModel.PasswordErrorTime ??= 0;
            // 错误次数+1
            accountModel.PasswordErrorTime++;

            switch (accountModel.PasswordErrorTime)
            {
                // 错误3次，锁定1分钟
                case 3:
                    accountModel.LockStartTime ??= dateTime;
                    accountModel.LockEndTime = accountModel.LockStartTime.Value.AddMinutes(1);
                    break;
                // 错误5次，锁定5分钟
                case 5:
                    accountModel.LockStartTime ??= dateTime;
                    accountModel.LockEndTime = dateTime.AddMinutes(5);
                    break;
                // 判断是否连续错误10次以上
                case >= 10:
                    // 错误10次，直接禁用账号
                    accountModel.Status = CommonStatusEnum.Disable;
                    break;
            }

            // 采用条件更新，避免并发问题
            await _repository.Updateable(accountModel)
                .UpdateColumns(e => new {e.PasswordErrorTime, e.LockStartTime, e.LockEndTime, e.Status})
                .ExecuteCommandAsync();
            if (accountModel.Status == CommonStatusEnum.Disable)
            {
                await _user.RevokeAccount(accountModel.AccountId);
                throw new UserFriendlyException("密码连续输入错误10次，账号已被禁用，请联系管理员！");
            }

            throw new UserFriendlyException("密码不正确！");
        }

        // 清除锁定信息
        if (accountModel.PasswordErrorTime != null)
        {
            accountModel.PasswordErrorTime = null;
            accountModel.LockStartTime = null;
            accountModel.LockEndTime = null;
            // 采用条件更新，避免并发问题
            await _repository.Updateable(accountModel)
                .UpdateColumns(e => new {e.PasswordErrorTime, e.LockStartTime, e.LockEndTime})
                .ExecuteCommandAsync();
        }
    }

    /// <summary>
    /// 处理登录
    /// </summary>
    /// <returns>登录结果</returns>
    private async Task<LoginOutput> HandleLogin(ApplicationModel applicationModel, AccountModel accountModel,
        TenantUserModel tenantUserModel, DateTime dateTime)
    {
        // 验证账号状态
        if (accountModel.Status == CommonStatusEnum.Disable)
        {
            throw new UserFriendlyException("账号已被平台禁用！");
        }

        if (tenantUserModel == null)
        {
            throw new UserFriendlyException("用户不存在！");
        }

        // 验证租户用户状态
        if (tenantUserModel.Status == CommonStatusEnum.Disable)
        {
            throw new UserFriendlyException("用户已被禁用！");
        }

        // 验证是否为机器人
        if (tenantUserModel.UserType == UserTypeEnum.Robot)
        {
            throw new UserFriendlyException("无效用户！");
        }

        // 查询租户
        var tenantModel = await _repository.Queryable<TenantModel>()
            .Where(wh => wh.TenantId == tenantUserModel.TenantId)
            .SingleAsync();

        if (tenantModel == null)
        {
            throw new UserFriendlyException("租户不存在！");
        }

        if (tenantModel.Status == CommonStatusEnum.Disable)
        {
            throw new UserFriendlyException("租户已被禁用！");
        }

        // 验证版本
        if (tenantModel.Edition < applicationModel.Edition)
        {
            throw new UserFriendlyException(
                $"当前租户版本【{tenantModel.Edition.GetDescription()}】不支持访问该应用，请升级至【{applicationModel.Edition.GetDescription()}】或更高版本。");
        }

        // 获取设备信息
        var userAgentInfo = _httpContext.RequestUserAgentInfo();
        // 获取万网信息
        var wanNetIpInfo = await _httpContext.RemoteIpv4InfoAsync();

        if (accountModel.FirstLoginTime == null)
        {
            accountModel.FirstLoginTenantId = tenantModel.TenantId;
            accountModel.FirstLoginDevice = userAgentInfo.Device;
            accountModel.FirstLoginOS = userAgentInfo.OS;
            accountModel.FirstLoginBrowser = userAgentInfo.Browser;
            accountModel.FirstLoginProvince = wanNetIpInfo.Province;
            accountModel.FirstLoginCity = wanNetIpInfo.City;
            accountModel.FirstLoginIp = wanNetIpInfo.Ip;
            accountModel.FirstLoginTime = dateTime;
        }

        accountModel.LastLoginTenantId = tenantModel.TenantId;
        accountModel.LastLoginDevice = userAgentInfo.Device;
        accountModel.LastLoginOS = userAgentInfo.OS;
        accountModel.LastLoginBrowser = userAgentInfo.Browser;
        accountModel.LastLoginProvince = wanNetIpInfo.Province;
        accountModel.LastLoginCity = wanNetIpInfo.City;
        accountModel.LastLoginIp = wanNetIpInfo.Ip;
        accountModel.LastLoginTime = dateTime;
        // 登录不更新错误密码信息，并且启用版本标识
        await _repository.Updateable(accountModel)
            .IgnoreColumns(it => new {it.PasswordErrorTime, it.LockStartTime, it.LockEndTime, it.Status})
            .ExecuteCommandWithOptLockAsync(true);

        // 登录后身份验证开关
        var loginIdentityVerificationOpen = bool.Parse(await ConfigContext.GetConfig(ConfigConst.LoginIdentityVerificationOpen));

        // 登录
        await _user.Login(new AuthUserInfo
        {
            DeviceType = GlobalContext.DeviceType,
            DeviceId = GlobalContext.DeviceId,
            AppNo = applicationModel.AppNo,
            AppName = applicationModel.AppName,
            AccountId = accountModel.AccountId,
            AccountKey = accountModel.AccountKey,
            Mobile = accountModel.Mobile,
            NickName = accountModel.NickName,
            Avatar = accountModel.Avatar,
            // 开启验证并且未验证
            IdentityVerification = loginIdentityVerificationOpen && !accountModel.IdentityVerification,
            TenantId = tenantModel.TenantId,
            TenantNo = tenantModel.TenantNo,
            TenantName = tenantModel.TenantName,
            TenantCode = tenantModel.TenantCode,
            IsSystemTenant = tenantModel.TenantType == TenantTypeEnum.System,
            UserKey = tenantUserModel.UserKey,
            EmployeeId = tenantUserModel.EmployeeId,
            EmployeeNo = tenantUserModel.EmployeeNo,
            EmployeeName = tenantUserModel.EmployeeName,
            DepartmentId = tenantUserModel.DepartmentId,
            DepartmentName = tenantUserModel.DepartmentName,
            IsSuperAdmin = tenantUserModel.UserType == UserTypeEnum.SuperAdmin,
            IsAdmin = tenantUserModel.UserType == UserTypeEnum.Admin,
            LastLoginDevice = accountModel.LastLoginDevice,
            LastLoginOS = accountModel.LastLoginOS,
            LastLoginBrowser = accountModel.LastLoginBrowser,
            LastLoginProvince = accountModel.LastLoginProvince,
            LastLoginCity = accountModel.LastLoginCity,
            LastLoginIp = accountModel.LastLoginIp,
            LastLoginTime = accountModel.LastLoginTime.Value
        });

        // 添加访问日志
        var visitLogModel = new VisitLogModel
        {
            RecordId = YitIdHelper.NextId(),
            AccountId = _user.AccountId,
            Mobile = _user.Mobile,
            NickName = _user.NickName,
            VisitType = VisitTypeEnum.Login,
            DepartmentId = _user.DepartmentId,
            DepartmentName = _user.DepartmentName,
            CreatedUserId = _user.EmployeeId,
            CreatedUserName = _user.EmployeeName,
            CreatedTime = DateTime.Now,
            TenantId = _user.TenantId,
            TenantName = _user.TenantName
        };
        visitLogModel.RecordCreate(_httpContext);
        await _httpContext.RequestServices.GetService<ISqlSugarRepository<VisitLogModel>>()
            .InsertAsync(visitLogModel);

        return new LoginOutput
        {
            Status = LoginStatusEnum.Success,
            Message = "登录成功",
            AccountKey = accountModel.AccountKey,
            NickName = accountModel.NickName,
            Avatar = accountModel.Avatar,
            TenantList =
            [
                new LoginTenantOutput
                {
                    UserKey = tenantUserModel.UserKey,
                    TenantName = tenantModel.TenantName,
                    ShortName = tenantModel.ShortName,
                    SpellName = tenantModel.SpellName,
                    Edition = tenantModel.Edition,
                    LogoUrl = tenantModel.LogoUrl,
                    EmployeeNo = tenantUserModel.EmployeeNo,
                    EmployeeName = tenantUserModel.EmployeeName,
                    IdPhoto = tenantUserModel.IdPhoto,
                    DepartmentId = tenantUserModel.DepartmentId,
                    DepartmentName = tenantUserModel.DepartmentName,
                    UserType = tenantUserModel.UserType,
                    Status = tenantUserModel.Status
                }
            ]
        };
    }

    /// <summary>
    /// 获取登录图片验证码
    /// </summary>
    [HttpPost("/getLoginCaptcha")]
    [ApiInfo("获取登录图片验证码", HttpRequestActionEnum.Auth)]
    [AllowAnonymous]
    [EnableRateLimiting(CommonConst.LoginApiRateLimit)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<LoginCaptchaOutput> GetLoginCaptcha(bool isForce = false)
    {
        // 查询应用信息
        await EnsureApplication();

        // 判断是否为强制启用
        if (!(isForce || await IsLoginCaptchaEnabled()))
        {
            return new LoginCaptchaOutput {Enabled = false};
        }

        var (captchaKey, captchaImage) = await _captchaService.GetImageCaptcha();
        return new LoginCaptchaOutput {Enabled = true, CaptchaKey = captchaKey, CaptchaImage = captchaImage};
    }
}