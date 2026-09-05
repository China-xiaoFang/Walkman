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

using Fast.Center.Domain;
using Fast.Center.Service.Login.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Fast.Center.Service.Login;

public partial class LoginService
{
    /// <summary>
    /// 尝试登录
    /// </summary>
    [HttpPost("/tryLogin")]
    [ApiInfo("尝试登录", HttpRequestActionEnum.Auth)]
    [EnableRateLimiting(CommonConst.LoginApiRateLimit)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<LoginOutput> TryLogin(TryLoginInput input)
    {
        // 查询应用信息
        var applicationModel = await EnsureApplication();

        var tenantUserModel = await _repository.Queryable<TenantUserModel>()
            .ClearFilter<IBaseTEntity>()
            .Where(wh => wh.UserKey == input.UserKey)
            .SingleAsync();

        if (tenantUserModel == null)
        {
            return new LoginOutput {Status = LoginStatusEnum.NotAccount, Message = "未找到用户信息，请先授权登录！"};
        }

        if (tenantUserModel.AccountId != _user.AccountId)
            throw new UserFriendlyException("禁止切换到其他账号的租户！");

        var accountModel = await _repository.Queryable<AccountModel>()
            .Where(wh => wh.AccountId == tenantUserModel.AccountId)
            .SingleAsync();

        if (accountModel == null)
        {
            throw new UserFriendlyException("账号不存在！");
        }

        // 先撤销当前租户会话，再签发目标租户会话
        await _user.Logout();

        // 处理登录
        return await HandleLogin(applicationModel.Application, accountModel, tenantUserModel, DateTime.Now);
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    [HttpPost("/logout")]
    [ApiInfo("退出登录", HttpRequestActionEnum.Auth)]
    [AllowAnonymous]
    [EnableRateLimiting(CommonConst.LoginApiRateLimit)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task Logout()
    {
        await _user.Logout();
    }
}