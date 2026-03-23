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

using System.Text;
using System.Text.RegularExpressions;
using Fast.Admin.Entity;
using Fast.Admin.Enum;
using Fast.Admin.Service.Login.Dto;
using Fast.AdminLog.Entity;
using Fast.AdminLog.Enum;
using Fast.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SKIT.FlurlHttpClient.Wechat.Api;
using SKIT.FlurlHttpClient.Wechat.Api.Models;
using SKIT.FlurlHttpClient.Wechat.Api.Utilities;
using Yitter.IdGenerator;

namespace Fast.Admin.Service.Login;

/// <summary>
/// <see cref="LoginService"/> 登录服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Auth, Name = "login", Order = 999)]
public class LoginService : IDynamicApplication
{
    private readonly IUser _user;
    private readonly HttpContext _httpContext;
    private readonly ISqlSugarClient _repository;

    public LoginService(IUser user, IHttpContextAccessor httpContextAccessor, ISqlSugarClient repository)
    {
        _user = user;
        _httpContext = httpContextAccessor.HttpContext;
        _repository = repository;
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    /// <param name="accountModel"></param>
    /// <param name="password"></param>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    private async Task VerifyPassword(AccountModel accountModel, string password, DateTime dateTime)
    {
        if (string.IsNullOrWhiteSpace(accountModel.Password))
        {
            throw new UserFriendlyException("未设定密码，请重置密码后重试！");
        }

        /*
         * 连续错误3次，锁定1分钟
         * 连续错误5次，锁定5分钟
         * 连续错误10次，锁定账号
         * 登录成功后清除锁定信息
         */
        if (!string.Equals(password, accountModel.Password, StringComparison.OrdinalIgnoreCase))
        {
            // 判断是否存在锁定时间
            if (accountModel.LockEndTime != null && accountModel.LockEndTime > dateTime)
            {
                var unLockTimeSpan = accountModel.LockEndTime.Value - dateTime;
                throw new UserFriendlyException($"账号已被锁定，请 {unLockTimeSpan.ToDescription()} 后再重试！");
            }

            accountModel.PasswordErrorTime ??= 0;
            // 错误次数+1
            accountModel.PasswordErrorTime++;

            switch (accountModel.PasswordErrorTime)
            {
                // 错误3次，锁定1分钟
                case 3:
                    accountModel.LockStartTime ??= dateTime;
                    accountModel.LockEndTime = accountModel.LockStartTime.Value.AddMinutes(1);
                    await _repository.Updateable(accountModel)
                        .ExecuteCommandAsync();
                    break;
                // 错误5次，锁定5分钟
                case 5:
                    accountModel.LockStartTime ??= dateTime;
                    accountModel.LockEndTime = dateTime.AddMinutes(5);
                    await _repository.Updateable(accountModel)
                        .ExecuteCommandAsync();
                    break;
                // 判断是否连续错误10次以上
                case >= 10:
                    // 错误10次，直接禁用账号
                    accountModel.Status = CommonStatusEnum.Disable;
                    await _repository.Updateable(accountModel)
                        .ExecuteCommandAsync();
                    throw new UserFriendlyException("密码连续输入错误10次，账号已被禁用，请联系管理员！");
            }

            await _repository.Updateable(accountModel)
                .ExecuteCommandAsync();

            throw new UserFriendlyException("密码不正确！");
        }

        // 清除锁定信息
        if (accountModel.PasswordErrorTime != null)
        {
            accountModel.PasswordErrorTime = null;
            accountModel.LockStartTime = null;
            accountModel.LockEndTime = null;
        }
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("/login")]
    [ApiInfo("登录", HttpRequestActionEnum.Auth)]
    [AllowAnonymous]
    public async Task<LoginOutput> Login(LoginInput input)
    {
        // 判断账号是否为手机号
        var isMobile = new Regex(RegexConst.Mobile).IsMatch(input.Account);

        AccountModel accountModel = null;
        EmployeeModel employeeModel = null;

        if (isMobile)
        {
            // 根据手机号，查询账号
            accountModel = await _repository.Queryable<AccountModel>()
                .Where(wh => wh.Mobile == input.Account)
                .SingleAsync();
            employeeModel = await _repository.Queryable<EmployeeModel>()
                .Where(wh => wh.AccountId == accountModel.AccountId)
                .SingleAsync();
        }
        else
        {
            employeeModel = await _repository.Queryable<EmployeeModel>()
                .Where(wh => wh.EmployeeNo == input.Account)
                .SingleAsync();
            // 查询账号
            accountModel = await _repository.Queryable<AccountModel>()
                .Where(wh => wh.AccountId == employeeModel.AccountId)
                .SingleAsync();
        }

        if (accountModel == null)
        {
            throw new UserFriendlyException("账号不存在！");
        }

        var dateTime = DateTime.Now;

        // 验证密码
        await VerifyPassword(accountModel, input.Password, dateTime);

        // 验证账号状态
        if (accountModel.Status == CommonStatusEnum.Disable)
        {
            throw new UserFriendlyException("账号已被平台禁用！");
        }

        if (employeeModel == null)
        {
            throw new UserFriendlyException("账号未绑定任何职员！");
        }

        // 验证是否为机器人
        if (employeeModel.UserType == UserTypeEnum.Robot)
        {
            throw new UserFriendlyException("无效用户！");
        }

        // 查询职员主部门
        var employeeOrgModel = await _repository.Queryable<EmployeeOrgModel>()
            .Where(wh => wh.EmployeeId == employeeModel.EmployeeId && wh.IsPrimary)
            .SingleAsync();

        // 获取设备信息
        var userAgentInfo = _httpContext.RequestUserAgentInfo();
        // 获取Ip信息
        var ip = _httpContext.RemoteIpv4();
        // 获取万网信息
        var wanNetIpInfo = await _httpContext.RemoteIpv4InfoAsync();

        if (accountModel.FirstLoginTime == null)
        {
            accountModel.FirstLoginDevice = userAgentInfo.Device;
            accountModel.FirstLoginOS = userAgentInfo.OS;
            accountModel.FirstLoginBrowser = userAgentInfo.Browser;
            accountModel.FirstLoginProvince = wanNetIpInfo.Province;
            accountModel.FirstLoginCity = wanNetIpInfo.City;
            accountModel.FirstLoginIp = ip;
            accountModel.FirstLoginTime = dateTime;
        }

        accountModel.LastLoginDevice = userAgentInfo.Device;
        accountModel.LastLoginOS = userAgentInfo.OS;
        accountModel.LastLoginBrowser = userAgentInfo.Browser;
        accountModel.LastLoginProvince = wanNetIpInfo.Province;
        accountModel.LastLoginCity = wanNetIpInfo.City;
        accountModel.LastLoginIp = ip;
        accountModel.LastLoginTime = dateTime;
        await _repository.Updateable(accountModel)
            .ExecuteCommandAsync();

        // 登录
        await _user.Login(new AuthUserInfo
        {
            DeviceType = GlobalContext.DeviceType,
            DeviceId = GlobalContext.DeviceId,
            AccountId = accountModel.AccountId,
            Mobile = accountModel.Mobile,
            NickName = accountModel.NickName,
            Avatar = accountModel.Avatar,
            EmployeeId = employeeModel.EmployeeId,
            EmployeeNo = employeeModel.EmployeeNo,
            EmployeeName = employeeModel.EmployeeName,
            DepartmentId = employeeOrgModel?.DepartmentId,
            DepartmentName = employeeOrgModel?.DepartmentName,
            IsSuperAdmin = employeeModel.UserType == UserTypeEnum.SuperAdmin,
            IsAdmin = employeeModel.UserType == UserTypeEnum.Admin,
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
            CreatedTime = DateTime.Now
        };
        visitLogModel.RecordCreate(_httpContext);
        await _httpContext.RequestServices.GetService<ISqlSugarRepository<VisitLogModel>>()
            .InsertAsync(visitLogModel);

        return new LoginOutput
        {
            Status = LoginStatusEnum.Success, Message = "登录成功", NickName = accountModel.NickName, Avatar = accountModel.Avatar
        };
    }

    /// <summary>
    /// 客户端注册
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("/clientRegister")]
    [ApiInfo("客户端注册", HttpRequestActionEnum.Auth)]
    [AllowAnonymous]
    public async Task ClientRegister(ClientRegisterInput input)
    {
        // 查询应用信息
        var applicationModel = await ApplicationContext.GetApplication(GlobalContext.Origin);

        if (applicationModel.AppType != GlobalContext.DeviceType)
        {
            throw new UserFriendlyException("应用类型不匹配！");
        }

        // 判断手机号是否已注册
        if (await _repository.Queryable<WeChatUserModel>()
                .AnyAsync(a => a.PurePhoneNumber == input.Mobile))
        {
            throw new UserFriendlyException("该手机号已注册！");
        }

        // TODO：验证码

        var weChatUserModel = new WeChatUserModel
        {
            WeChatId = YitIdHelper.NextId(),
            UserType = GlobalContext.DeviceType switch
            {
                AppEnvironmentEnum.WeChatMiniProgram => WeChatUserTypeEnum.MiniProgram,
                AppEnvironmentEnum.WeChatOfficialAccount => WeChatUserTypeEnum.OfficialAccount,
                AppEnvironmentEnum.WeChatServiceAccount => WeChatUserTypeEnum.ServiceAccount,
                AppEnvironmentEnum.WeChatOpenPlatform => WeChatUserTypeEnum.OpenPlatform,
                AppEnvironmentEnum.WorkWeChat => WeChatUserTypeEnum.WorkWeChat,
                _ => WeChatUserTypeEnum.MiniProgram
            },
            PurePhoneNumber = input.Mobile,
            Password = input.Password,
            NickName = input.NickName,
            Avatar = input.Avatar,
            Sex = input.Sex
        };

        await _repository.Insertable(weChatUserModel)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 客户端登录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("/clientLogin")]
    [ApiInfo("客户端登录", HttpRequestActionEnum.Auth)]
    [AllowAnonymous]
    public async Task<ClientLoginOutput> ClientLogin(LoginInput input)
    {
        // 查询应用信息
        var applicationModel = await ApplicationContext.GetApplication(GlobalContext.Origin);

        if (applicationModel.AppType != GlobalContext.DeviceType)
        {
            throw new UserFriendlyException("应用类型不匹配！");
        }

        // 根据手机号获取微信用户信息
        var weChatUserModel = await _repository.Queryable<WeChatUserModel>()
            .Where(wh => wh.OpenId == input.Account)
            .SingleAsync();
        if (weChatUserModel == null)
        {
            return new ClientLoginOutput {Status = LoginStatusEnum.NotAccount, Message = "未找到用户信息，请先注册！"};
        }

        if (string.IsNullOrWhiteSpace(weChatUserModel.Password))
        {
            throw new UserFriendlyException("未设定密码，请重置密码后重试！");
        }

        if (!string.Equals(input.Password, weChatUserModel.Password, StringComparison.OrdinalIgnoreCase))
        {
            throw new UserFriendlyException("密码不正确！");
        }

        var dateTime = DateTime.Now;

        // 获取设备信息
        var userAgentInfo = _httpContext.RequestUserAgentInfo();
        // 获取Ip信息
        var ip = _httpContext.RemoteIpv4();
        // 获取万网信息
        var wanNetIpInfo = await _httpContext.RemoteIpv4InfoAsync();

        weChatUserModel.LastLoginDevice = userAgentInfo.Device;
        weChatUserModel.LastLoginOS = userAgentInfo.OS;
        weChatUserModel.LastLoginBrowser = userAgentInfo.Browser;
        weChatUserModel.LastLoginProvince = wanNetIpInfo.Province;
        weChatUserModel.LastLoginCity = wanNetIpInfo.City;
        weChatUserModel.LastLoginIp = ip;
        weChatUserModel.LastLoginTime = dateTime;
        await _repository.Updateable(weChatUserModel)
            .ExecuteCommandAsync();

        // 客户端登录
        await _user.ClientLogin(new AuthUserInfo
        {
            DeviceType = GlobalContext.DeviceType,
            DeviceId = GlobalContext.DeviceId,
            AccountId = weChatUserModel.WeChatId,
            Mobile = weChatUserModel.PurePhoneNumber,
            NickName = weChatUserModel.NickName,
            Avatar = weChatUserModel.Avatar,
            EmployeeId = weChatUserModel.WeChatId,
            EmployeeName = weChatUserModel.NickName,
            WeChatId = weChatUserModel.WeChatId,
            WeChatOpenId = weChatUserModel.OpenId,
            IsSuperAdmin = false,
            IsAdmin = false,
            LastLoginDevice = weChatUserModel.LastLoginDevice,
            LastLoginOS = weChatUserModel.LastLoginOS,
            LastLoginBrowser = weChatUserModel.LastLoginBrowser,
            LastLoginProvince = weChatUserModel.LastLoginProvince,
            LastLoginCity = weChatUserModel.LastLoginCity,
            LastLoginIp = weChatUserModel.LastLoginIp,
            LastLoginTime = weChatUserModel.LastLoginTime.Value,
            ButtonCodeList = [PermissionConst.ClientService]
        });

        return new ClientLoginOutput
        {
            Status = LoginStatusEnum.Success,
            Message = "登录成功",
            OpenId = weChatUserModel.OpenId,
            UnionId = weChatUserModel.UnionId,
            Mobile = weChatUserModel.PurePhoneNumber,
            NickName = weChatUserModel.NickName,
            Avatar = weChatUserModel.Avatar
        };
    }

    /// <summary>
    /// 微信客户端登录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("/weChatClientLogin")]
    [ApiInfo("微信客户端登录", HttpRequestActionEnum.Auth)]
    [AllowAnonymous]
    public async Task<ClientLoginOutput> WeChatClientLogin(WeChatClientLoginInput input)
    {
        // 查询应用信息
        var applicationModel = await ApplicationContext.GetApplication(GlobalContext.Origin);

        if (applicationModel.AppType != GlobalContext.DeviceType)
        {
            throw new UserFriendlyException("应用类型不匹配！");
        }

        var apiClient = WechatApiClientBuilder
            .Create(new WechatApiClientOptions {AppId = applicationModel.OpenId, AppSecret = applicationModel.OpenSecret})
            .Build();

        WeChatUserModel weChatUserModel = null;

        // 微信小程序
        if (applicationModel.AppType == AppEnvironmentEnum.WeChatMiniProgram)
        {
            // 解析微信Code，获取OpenId
            var response =
                await apiClient.ExecuteSnsJsCode2SessionAsync(new SnsJsCode2SessionRequest {JsCode = input.WeChatCode});
            if (!response.IsSuccessful())
            {
                throw new UserFriendlyException(
                    $"解析Code失败，获取微信登录信息失败：ErrorCode：{response.ErrorCode}。ErrorMessage：{response.ErrorMessage}");
            }

            // 根据 OpenId 获取微信用户信息
            weChatUserModel = await _repository.Queryable<WeChatUserModel>()
                .Where(wh => wh.OpenId == response.OpenId)
                .SingleAsync();
            if (weChatUserModel == null)
            {
                // 保存微信用户
                weChatUserModel = new WeChatUserModel
                {
                    WeChatId = YitIdHelper.NextId(),
                    UserType = GlobalContext.DeviceType switch
                    {
                        AppEnvironmentEnum.WeChatMiniProgram => WeChatUserTypeEnum.MiniProgram,
                        AppEnvironmentEnum.WeChatOfficialAccount => WeChatUserTypeEnum.OfficialAccount,
                        AppEnvironmentEnum.WeChatServiceAccount => WeChatUserTypeEnum.ServiceAccount,
                        AppEnvironmentEnum.WeChatOpenPlatform => WeChatUserTypeEnum.OpenPlatform,
                        AppEnvironmentEnum.WorkWeChat => WeChatUserTypeEnum.WorkWeChat,
                        _ => WeChatUserTypeEnum.MiniProgram
                    },
                    OpenId = response.OpenId,
                    UnionId = response.UnionId,
                    SessionKey = response.SessionKey,
                    NickName = "微信用户",
                    Avatar =
                        "https://thirdwx.qlogo.cn/mmopen/vi_32/POgEwh4mIHO4nibH0KlMECNjjGxQUq24ZEaGT4poC6icRiccVGKSyXwibcPq4BWmiaIGuG1icwxaQX6grC9VemZoJ8rg/132",
                    Sex = GenderEnum.Unknown
                };

                // 这里的 IV 和 EncryptedData 在没有授权的情况下是为空的
                if (!string.IsNullOrWhiteSpace(input.IV) || !string.IsNullOrWhiteSpace(input.EncryptedData))
                {
                    // 尝试解析加密数据
                    var decryptBytes = AESUtility.DecryptWithCBC(Convert.FromBase64String(response.SessionKey),
                        Convert.FromBase64String(input.IV), Convert.FromBase64String(input.EncryptedData));
                    var decryptStr = Encoding.Default.GetString(decryptBytes);
                    var decryptData = decryptStr.ToObject<DecryptWeChatUserInfo>();
                    if (decryptData == null)
                    {
                        throw new UserFriendlyException("解析加密用户信息失败！");
                    }

                    weChatUserModel.NickName = decryptData.NickName;
                    weChatUserModel.Sex = decryptData.Gender;
                    weChatUserModel.Country = decryptData.Country;
                    weChatUserModel.Province = decryptData.Province;
                    weChatUserModel.City = decryptData.City;
                    weChatUserModel.Language = decryptData.Language;
                }

                await _repository.Insertable(weChatUserModel)
                    .ExecuteCommandAsync();
            }

            if (!string.IsNullOrWhiteSpace(input.Code))
            {
                // 换取用户手机号
                var phoneNumberResponse = await apiClient.ExecuteWxaBusinessGetUserPhoneNumberAsync(
                    new WxaBusinessGetUserPhoneNumberRequest
                    {
                        AccessToken = applicationModel.WeChatAccessToken, Code = input.Code
                    });

                if (!phoneNumberResponse.IsSuccessful())
                {
                    throw new UserFriendlyException(
                        $"解析Code失败，获取用户手机号失败：ErrorCode：{phoneNumberResponse.ErrorCode}。ErrorMessage：{phoneNumberResponse.ErrorMessage}");
                }

                if (weChatUserModel.PurePhoneNumber != phoneNumberResponse.PhoneInfo.PurePhoneNumber)
                {
                    weChatUserModel.PurePhoneNumber = phoneNumberResponse.PhoneInfo.PurePhoneNumber;
                    weChatUserModel.PhoneNumber = phoneNumberResponse.PhoneInfo.PhoneNumber;
                    weChatUserModel.CountryCode = phoneNumberResponse.PhoneInfo.CountryCode;
                    weChatUserModel.MobileUpdateTime = DateTime.Now;
                }
            }
        }
        // 微信服务号
        else if (applicationModel.AppType == AppEnvironmentEnum.WeChatServiceAccount)
        {
            // 根据 Code 换取用户 AccessToken
            var tokenResponse =
                await apiClient.ExecuteSnsOAuth2AccessTokenAsync(new SnsOAuth2AccessTokenRequest {Code = input.WeChatCode});
            if (!tokenResponse.IsSuccessful())
            {
                return new ClientLoginOutput
                {
                    Status = LoginStatusEnum.AuthExpired,
                    Message =
                        $"解析Code失败，获取用户微信 AccessToken 失败：ErrorCode：{tokenResponse.ErrorCode}。ErrorMessage：{tokenResponse.ErrorMessage}"
                };
            }

            var response = await apiClient.ExecuteSnsUserInfoAsync(new SnsUserInfoRequest
            {
                AccessToken = tokenResponse.AccessToken, OpenId = tokenResponse.OpenId
            });
            if (!response.IsSuccessful())
            {
                throw new UserFriendlyException(
                    $"获取微信用户信息失败：ErrorCode：{response.ErrorCode}。ErrorMessage：{response.ErrorMessage}");
            }

            // 根据 OpenId 获取微信用户信息
            weChatUserModel = await _repository.Queryable<WeChatUserModel>()
                .Where(wh => wh.OpenId == response.OpenId)
                .SingleAsync();
            if (weChatUserModel == null)
            {
                // 保存微信用户
                weChatUserModel = new WeChatUserModel
                {
                    WeChatId = YitIdHelper.NextId(),
                    UserType = GlobalContext.DeviceType switch
                    {
                        AppEnvironmentEnum.WeChatMiniProgram => WeChatUserTypeEnum.MiniProgram,
                        AppEnvironmentEnum.WeChatOfficialAccount => WeChatUserTypeEnum.OfficialAccount,
                        AppEnvironmentEnum.WeChatServiceAccount => WeChatUserTypeEnum.ServiceAccount,
                        AppEnvironmentEnum.WeChatOpenPlatform => WeChatUserTypeEnum.OpenPlatform,
                        AppEnvironmentEnum.WorkWeChat => WeChatUserTypeEnum.WorkWeChat,
                        _ => WeChatUserTypeEnum.MiniProgram
                    },
                    OpenId = response.OpenId,
                    UnionId = response.UnionId,
                    NickName = response.Nickname,
                    Avatar = response.HeadImageUrl,
                    Sex = GenderEnum.Unknown
                };
                await _repository.Insertable(weChatUserModel)
                    .ExecuteCommandAsync();
            }
            else
            {
                weChatUserModel.NickName = response.Nickname;
                weChatUserModel.Avatar = response.HeadImageUrl;
            }
        }

        if (weChatUserModel == null)
        {
            throw new UserFriendlyException("暂不支持此类客户端！");
        }

        var dateTime = DateTime.Now;

        // 获取设备信息
        var userAgentInfo = _httpContext.RequestUserAgentInfo();
        // 获取Ip信息
        var ip = _httpContext.RemoteIpv4();
        // 获取万网信息
        var wanNetIpInfo = await _httpContext.RemoteIpv4InfoAsync();

        weChatUserModel.LastLoginDevice = userAgentInfo.Device;
        weChatUserModel.LastLoginOS = userAgentInfo.OS;
        weChatUserModel.LastLoginBrowser = userAgentInfo.Browser;
        weChatUserModel.LastLoginProvince = wanNetIpInfo.Province;
        weChatUserModel.LastLoginCity = wanNetIpInfo.City;
        weChatUserModel.LastLoginIp = ip;
        weChatUserModel.LastLoginTime = dateTime;
        await _repository.Updateable(weChatUserModel)
            .ExecuteCommandAsync();

        // 客户端登录
        await _user.ClientLogin(new AuthUserInfo
        {
            DeviceType = GlobalContext.DeviceType,
            DeviceId = GlobalContext.DeviceId,
            AccountId = weChatUserModel.WeChatId,
            Mobile = weChatUserModel.PurePhoneNumber,
            NickName = weChatUserModel.NickName,
            Avatar = weChatUserModel.Avatar,
            EmployeeId = weChatUserModel.WeChatId,
            EmployeeName = weChatUserModel.NickName,
            WeChatId = weChatUserModel.WeChatId,
            WeChatOpenId = weChatUserModel.OpenId,
            IsSuperAdmin = false,
            IsAdmin = false,
            LastLoginDevice = weChatUserModel.LastLoginDevice,
            LastLoginOS = weChatUserModel.LastLoginOS,
            LastLoginBrowser = weChatUserModel.LastLoginBrowser,
            LastLoginProvince = weChatUserModel.LastLoginProvince,
            LastLoginCity = weChatUserModel.LastLoginCity,
            LastLoginIp = weChatUserModel.LastLoginIp,
            LastLoginTime = weChatUserModel.LastLoginTime.Value,
            ButtonCodeList = [PermissionConst.ClientService]
        });

        return new ClientLoginOutput
        {
            Status = LoginStatusEnum.Success,
            Message = "登录成功",
            OpenId = weChatUserModel.OpenId,
            UnionId = weChatUserModel.UnionId,
            Mobile = weChatUserModel.PurePhoneNumber,
            NickName = weChatUserModel.NickName,
            Avatar = weChatUserModel.Avatar
        };
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    /// <returns></returns>
    [HttpPost("/logout")]
    [ApiInfo("退出登录", HttpRequestActionEnum.Auth)]
    [AllowAnonymous]
    public async Task Logout()
    {
        await _user.Logout();
    }
}