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

using System.Text.RegularExpressions;
using Fast.Center.Domain;
using Fast.Center.Service.Login.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Fast.Center.Service.Login;

public partial class LoginService
{
    /// <summary>
    /// 登录
    /// </summary>
    [HttpPost("/login")]
    [ApiInfo("登录", HttpRequestActionEnum.Auth)]
    [AllowAnonymous]
    [EnableRateLimiting(CommonConst.LoginApiRateLimit)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<LoginOutput> Login(LoginInput input)
    {
        // 查询应用信息
        var applicationModel = await EnsureApplication();

        // 目前只有 Web 端启用了图片验证码
        if (GlobalContext.IsWeb && await IsLoginCaptchaEnabled())
        {
            await _captchaService.VerifyImageCaptcha(input.CaptchaKey, input.CaptchaCode);
        }

        // 判断账号是否为手机号
        var isMobile = new Regex(RegexConst.Mobile).IsMatch(input.Account);

        AccountModel accountModel = null;
        List<TenantUserModel> tenantUserList = [];

        if (isMobile)
        {
            // 根据手机号，查询账号
            accountModel = await _repository.Queryable<AccountModel>()
                .Where(wh => wh.Mobile == input.Account)
                .SingleAsync();

            if (accountModel != null)
            {
                tenantUserList = await _repository.Queryable<TenantUserModel>()
                    .ClearFilter<IBaseTEntity>()
                    .Where(wh => wh.AccountId == accountModel.AccountId)
                    .ToListAsync();
            }
        }
        else
        {
            // 根据账号或登录工号查询租户用户信息
            var tenantUserModel = await _repository.Queryable<TenantUserModel>()
                .ClearFilter<IBaseTEntity>()
                .Where(wh => wh.EmployeeNo == input.Account)
                .SingleAsync();

            if (tenantUserModel != null)
            {
                // 查询账号
                accountModel = await _repository.Queryable<AccountModel>()
                    .Where(wh => wh.AccountId == tenantUserModel.AccountId)
                    .SingleAsync();
                tenantUserList.Add(tenantUserModel);
            }
        }

        if (accountModel == null)
        {
            throw new UserFriendlyException("账号不存在！");
        }

        var dateTime = DateTime.Now;

        // 验证密码
        await VerifyPassword(accountModel, input.Password, dateTime);

        if (tenantUserList.Count == 0)
        {
            throw new UserFriendlyException("账号未绑定任何租户！");
        }

        // 单租户自动登录
        var autoLogin = bool.Parse(await ConfigContext.GetConfig(ConfigConst.SingleTenantWhenAutoLogin));

        // 单租户自动登录
        if (tenantUserList.Count == 1 && autoLogin)
        {
            // 处理登录
            return await HandleLogin(applicationModel.Application, accountModel, tenantUserList.Single(), dateTime);
        }

        var tenantIds = tenantUserList.Select(sl => sl.TenantId)
            .Distinct()
            .ToList();
        var tenantList = await _repository.Queryable<TenantModel>()
            .Where(wh => tenantIds.Contains(wh.TenantId))
            .ToListAsync();

        var resultTenantList = new List<LoginTenantOutput>();

        foreach (var tenantUserModel in tenantUserList)
        {
            var tenantModel = tenantList.Single(s => s.TenantId == tenantUserModel.TenantId);
            resultTenantList.Add(new LoginTenantOutput
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
            });
        }

        // 多个账号，或未开启单租户自动登录
        return new LoginOutput
        {
            Status = LoginStatusEnum.SelectTenant,
            Message = "请选择租户登录",
            LoginTicket = await GetTenantLoginTicket(accountModel),
            AccountKey = accountModel.AccountKey,
            NickName = accountModel.NickName,
            Avatar = accountModel.Avatar,
            TenantList = resultTenantList
        };
    }

    /// <summary>
    /// 获取登录用户
    /// </summary>
    [HttpGet("/queryLoginUser")]
    [ApiInfo("获取登录用户", HttpRequestActionEnum.Query)]
    [EnableRateLimiting(CommonConst.LoginApiRateLimit)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<List<LoginTenantOutput>> QueryLoginUser()
    {
        return await _repository.Queryable<AccountModel>()
            .InnerJoin<TenantUserModel>((t1, t2) => t1.AccountId == t2.AccountId)
            .InnerJoin<TenantModel>((t1, t2, t3) => t2.TenantId == t3.TenantId)
            .ClearFilter<IBaseTEntity>()
            .Where(t1 => t1.AccountId == _user.AccountId)
            .Select((t1, t2, t3) => new LoginTenantOutput
            {
                UserKey = t2.UserKey,
                TenantName = t3.TenantName,
                ShortName = t3.ShortName,
                SpellName = t3.SpellName,
                Edition = t3.Edition,
                LogoUrl = t3.LogoUrl,
                EmployeeNo = t2.EmployeeNo,
                EmployeeName = t2.EmployeeName,
                IdPhoto = t2.IdPhoto,
                DepartmentId = t2.DepartmentId,
                DepartmentName = t2.DepartmentName,
                UserType = t2.UserType,
                Status = t2.Status
            })
            .ToListAsync();
    }

    /// <summary>
    /// 租户登录
    /// </summary>
    [HttpPost("/tenantLogin")]
    [ApiInfo("租户登录", HttpRequestActionEnum.Auth)]
    [AllowAnonymous]
    [EnableRateLimiting(CommonConst.LoginApiRateLimit)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<LoginOutput> TenantLogin(TenantLoginInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Password) && string.IsNullOrWhiteSpace(input.LoginTicket))
            throw new UserFriendlyException("密码不能为空！");

        // 查询应用信息
        var applicationModel = await EnsureApplication();

        // 目前只有 Web 端启用了图片验证码，这里登录凭证为空的情况下是存在租户直接登录的
        if (string.IsNullOrWhiteSpace(input.LoginTicket) && GlobalContext.IsWeb && await IsLoginCaptchaEnabled())
        {
            await _captchaService.VerifyImageCaptcha(input.CaptchaKey, input.CaptchaCode);
        }

        // 查询租户用户
        var tenantUserModel = await _repository.Queryable<TenantUserModel>()
            .ClearFilter<IBaseTEntity>()
            .Where(wh => wh.UserKey == input.UserKey)
            .SingleAsync();

        if (tenantUserModel == null)
        {
            throw new UserFriendlyException("用户不存在！");
        }

        // 查询账号
        var accountModel = await _repository.Queryable<AccountModel>()
            .Where(wh => wh.AccountId == tenantUserModel.AccountId)
            .SingleAsync();

        if (accountModel == null)
        {
            throw new UserFriendlyException("账号不存在！");
        }

        if (!string.IsNullOrWhiteSpace(input.AccountKey)
            && !string.Equals(input.AccountKey, accountModel.AccountKey, StringComparison.Ordinal))
            throw new UserFriendlyException("账号与租户用户不匹配！");

        var dateTime = DateTime.Now;

        if (!string.IsNullOrWhiteSpace(input.LoginTicket))
        {
            // 验证登录凭证
            await VerifyTenantLoginTicket(input.LoginTicket, accountModel);
        }
        else
        {
            // 验证密码
            await VerifyPassword(accountModel, input.Password, dateTime);
        }

        // 处理登录
        return await HandleLogin(applicationModel.Application, accountModel, tenantUserModel, dateTime);
    }
}