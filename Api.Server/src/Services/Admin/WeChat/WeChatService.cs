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
using Fast.Admin.Entity;
using Fast.Admin.Service.WeChat.Dto;
using Fast.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SKIT.FlurlHttpClient.Wechat.Api;
using SKIT.FlurlHttpClient.Wechat.Api.Models;
using SKIT.FlurlHttpClient.Wechat.Api.Utilities;

namespace Fast.Admin.Service.WeChat;

/// <summary>
/// <see cref="WeChatService"/> 微信服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Center, Name = "weChat")]
public class WeChatService : IDynamicApplication
{
    private readonly IUser _user;
    private readonly ISqlSugarRepository<WeChatUserModel> _repository;

    public WeChatService(IUser user, ISqlSugarRepository<WeChatUserModel> repository)
    {
        _user = user;
        _repository = repository;
    }

    /// <summary>
    /// 获取微信用户分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取微信用户分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.WeChat.Paged)]
    public async Task<PagedResult<QueryWeChatUserPagedOutput>> QueryWeChatUserPaged(QueryWeChatUserPagedInput input)
    {
        return await _repository.Entities.WhereIF(input.UserType != null, wh => wh.UserType == input.UserType)
            .WhereIF(input.Sex != null, wh => wh.Sex == input.Sex)
            .OrderByIF(input.IsOrderBy, ob => ob.CreatedTime, OrderByType.Desc)
            .Select(sl => new QueryWeChatUserPagedOutput
            {
                WeChatId = sl.WeChatId,
                UserType = sl.UserType,
                OpenId = sl.OpenId,
                UnionId = sl.UnionId,
                PurePhoneNumber = sl.PurePhoneNumber,
                PhoneNumber = sl.PhoneNumber,
                CountryCode = sl.CountryCode,
                NickName = sl.NickName,
                Avatar = sl.Avatar,
                Sex = sl.Sex,
                Country = sl.Country,
                Province = sl.Province,
                City = sl.City,
                Language = sl.Language,
                ActivationTime = sl.ActivationTime,
                LastLoginDevice = sl.LastLoginDevice,
                LastLoginOS = sl.LastLoginOS,
                LastLoginBrowser = sl.LastLoginBrowser,
                LastLoginProvince = sl.LastLoginProvince,
                LastLoginCity = sl.LastLoginCity,
                LastLoginIp = sl.LastLoginIp,
                LastLoginTime = sl.LastLoginTime,
                MobileUpdateTime = sl.MobileUpdateTime,
                CreatedTime = sl.CreatedTime,
                UpdatedTime = sl.UpdatedTime,
                RowVersion = sl.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取微信用户详情
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取微信用户详情", HttpRequestActionEnum.Query)]
    public async Task<QueryWeChatUserDetailOutput> QueryWeChatUserDetail()
    {
        // 根据 OpenId 获取微信用户信息
        var result = await _repository.Queryable<WeChatUserModel>()
            .Where(wh => wh.WeChatId == _user.WeChatId)
            .Select(sl => new QueryWeChatUserDetailOutput
            {
                WeChatId = sl.WeChatId,
                UserType = sl.UserType,
                OpenId = sl.OpenId,
                UnionId = sl.UnionId,
                PurePhoneNumber = sl.PurePhoneNumber,
                PhoneNumber = sl.PhoneNumber,
                CountryCode = sl.CountryCode,
                NickName = sl.NickName,
                Avatar = sl.Avatar,
                Sex = sl.Sex,
                Country = sl.Country,
                Province = sl.Province,
                City = sl.City,
                Language = sl.Language,
                LastLoginDevice = sl.LastLoginDevice,
                LastLoginOS = sl.LastLoginOS,
                LastLoginBrowser = sl.LastLoginBrowser,
                LastLoginProvince = sl.LastLoginProvince,
                LastLoginCity = sl.LastLoginCity,
                LastLoginIp = sl.LastLoginIp,
                LastLoginTime = sl.LastLoginTime,
                CreatedTime = sl.CreatedTime,
                UpdatedTime = sl.UpdatedTime,
                MobileUpdateTime = sl.MobileUpdateTime,
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
    /// 编辑微信用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑微信用户", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.ClientService)]
    public async Task EditWeChatUser(EditWeChatUserInput input)
    {
        var weChatUserModel = await _repository.Queryable<WeChatUserModel>()
            .Where(wh => wh.WeChatId == _user.WeChatId)
            .SingleAsync();

        if (weChatUserModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        if (!string.IsNullOrWhiteSpace(input.PurePhoneNumber))
        {
            weChatUserModel.PurePhoneNumber = input.PurePhoneNumber;
            weChatUserModel.PhoneNumber = input.PhoneNumber;
            weChatUserModel.CountryCode = input.CountryCode;
            weChatUserModel.MobileUpdateTime = DateTime.Now;
        }

        weChatUserModel.NickName = input.NickName;
        weChatUserModel.Avatar = input.Avatar;
        weChatUserModel.Sex = input.Sex;
        weChatUserModel.Country = input.Country;
        weChatUserModel.Province = input.Province;
        weChatUserModel.City = input.City;
        weChatUserModel.RowVersion = input.RowVersion;

        await _repository.UpdateAsync(weChatUserModel);

        // 刷新缓存
        await _user.RefreshWeChatUser(new RefreshWeChatUserDto
        {
            DeviceType = _user.DeviceType,
            Mobile = weChatUserModel.PurePhoneNumber,
            NickName = weChatUserModel.NickName,
            Avatar = weChatUserModel.Avatar,
            WeChatOpenId = weChatUserModel.OpenId
        });
    }

    /// <summary>
    /// 换取微信用户身份信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("换取微信用户身份信息", HttpRequestActionEnum.Auth)]
    [AllowAnonymous]
    public async Task<WeChatCode2SessionOutput> WeChatCode2Session(WeChatCode2SessionInput input)
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

        // 解析微信Code，获取OpenId
        var response = await apiClient.ExecuteSnsJsCode2SessionAsync(new SnsJsCode2SessionRequest {JsCode = input.Code});
        if (!response.IsSuccessful())
        {
            throw new UserFriendlyException(
                $"解析Code失败，获取微信登录信息失败：ErrorCode：{response.ErrorCode}。ErrorMessage：{response.ErrorMessage}");
        }

        var result = new WeChatCode2SessionOutput
        {
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

            result.NickName = decryptData.NickName;
            result.Sex = decryptData.Gender;
            result.Country = decryptData.Country;
            result.Province = decryptData.Province;
            result.City = decryptData.City;
            result.Language = decryptData.Language;
        }

        return result;
    }

    /// <summary>
    /// 换取微信用户手机号
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("换取微信用户手机号", HttpRequestActionEnum.Auth)]
    [AllowAnonymous]
    public async Task<WeChatCode2PhoneNumberOutput> WeChatCode2PhoneNumber(WeChatCode2PhoneNumberInput input)
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

        // 换取用户手机号
        var response = await apiClient.ExecuteWxaBusinessGetUserPhoneNumberAsync(
            new WxaBusinessGetUserPhoneNumberRequest {AccessToken = applicationModel.WeChatAccessToken, Code = input.Code});

        if (!response.IsSuccessful())
        {
            throw new UserFriendlyException(
                $"解析Code失败，获取用户手机号失败：ErrorCode：{response.ErrorCode}。ErrorMessage：{response.ErrorMessage}");
        }

        return new WeChatCode2PhoneNumberOutput
        {
            PurePhoneNumber = response.PhoneInfo.PurePhoneNumber,
            PhoneNumber = response.PhoneInfo.PhoneNumber,
            CountryCode = response.PhoneInfo.CountryCode
        };
    }
}