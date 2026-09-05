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
using System.Text.RegularExpressions;
using Fast.Center.Service.Account.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Center.Service.Account;

public partial class AccountService
{
    /// <summary>
    /// 账号校验缓存Dto
    /// </summary>
    private class AccountVerificationCacheDto
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
        /// 手机号验证码过期时间
        /// </summary>
        public DateTime MobileExpiresTime { get; set; }

        /// <summary>
        /// 手机号已验证
        /// </summary>
        public bool MobileVerified { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 邮箱验证码过期时间
        /// </summary>
        public DateTime EmailExpiresTime { get; set; }

        /// <summary>
        /// 邮箱已验证
        /// </summary>
        public bool EmailVerified { get; set; }

        /// <summary>
        /// 密码Hash
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// 客户端标识
        /// </summary>
        public string ClientIdentity { get; set; }
    }

    /// <summary>
    /// 发送账号校验验证码
    /// </summary>
    [HttpPost]
    [ApiInfo("发送账号校验验证码", HttpRequestActionEnum.Auth)]
    public async Task SendAccountVerificationCode(SendAccountVerificationCodeInput input)
    {
        await EnsureApplication();
        await _captchaService.VerifyImageCaptcha(input.CaptchaKey, input.CaptchaCode);

        var accountModel = await _repository.SingleOrDefaultAsync(_user.AccountId);
        if (accountModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        // 验证账号状态
        if (accountModel.Status == CommonStatusEnum.Disable)
        {
            throw new UserFriendlyException("账号已被平台禁用！");
        }

        // 验证状态
        if (accountModel.IdentityVerification)
        {
            // 退出登录
            await _user.Logout();
            throw new UserFriendlyException("账号已校验完成，请刷新用户信息！");
        }

        var account = input.Account.Trim()
            .ToLowerInvariant();

        MessageSendChannelEnum sendChannel;
        if (Regex.IsMatch(account, RegexConst.Mobile))
        {
            sendChannel = MessageSendChannelEnum.Sms;
            if (await _repository.AnyAsync(a => a.Mobile == account && a.AccountId != _user.AccountId))
            {
                throw new UserFriendlyException("手机号已存在账号信息！");
            }
        }
        else if (Regex.IsMatch(account, RegexConst.EmailAddress))
        {
            sendChannel = MessageSendChannelEnum.Email;
            if (await _repository.AnyAsync(a => a.Email == account && a.AccountId != _user.AccountId))
            {
                throw new UserFriendlyException("邮箱已存在账号信息！");
            }
        }
        else
        {
            throw new UserFriendlyException("请输入正确的手机号或邮箱！");
        }

        // 同一个IP地址，1小时内最多允许20次
        await EnforceSendQuota($"Ip:{FastContext.HttpContext.Connection.RemoteIpAddress?.MapToIPv6()
                                         .ToString()
                                     ?? "unknown"}", (20, 3600));
        var recipient = $"Recipient:{sendChannel}:{accountModel.AccountKey}";
        // 1小时5次，24小时10次
        await EnforceSendQuota(recipient, (5, 3600), (10, 86400));

        // 不同客户端独立保存校验进度，发送与提交共用锁，避免覆盖已验证的结果。
        var clientIdentity = GlobalContext.ClientIdentity;
        var cacheKey = CacheConst.GetCacheKey(CacheConst.AccountIdentityVerification, accountModel.AccountKey, clientIdentity);
        using var codeLock = _centerCache.Client.TryLock($"{cacheKey}:Lock", 120);
        if (codeLock == null)
        {
            throw new UserFriendlyException("操作过于频繁，请稍后重试！");
        }

        var passwordHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(accountModel.Password)));
        var dto = await _centerCache.GetAsync<AccountVerificationCacheDto>(cacheKey);
        if (dto == null
            || dto.AccountId != accountModel.AccountId
            || dto.ClientIdentity != clientIdentity
            || dto.PasswordHash != passwordHash)
        {
            dto = new AccountVerificationCacheDto
            {
                AccountId = accountModel.AccountId, PasswordHash = passwordHash, ClientIdentity = clientIdentity
            };
        }

        var expiresTime = DateTime.Now.AddMinutes(5);
        switch (sendChannel)
        {
            case MessageSendChannelEnum.Email:
                await _mailService.SendVerificationCode(MailTypeEnum.Validity, account);
                dto.Email = account;
                dto.EmailExpiresTime = expiresTime;
                dto.EmailVerified = false;
                break;
            case MessageSendChannelEnum.Sms:
                await _smsService.SendVerificationCode(SmsTypeEnum.Validity, account);
                dto.Mobile = account;
                dto.MobileExpiresTime = expiresTime;
                dto.MobileVerified = false;
                break;
        }

        // 重发只重置当前通道，另一通道的校验结果仍受其原始过期时间限制。
        await _centerCache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// 账号校验
    /// </summary>
    [HttpPost]
    [ApiInfo("账号校验", HttpRequestActionEnum.Auth)]
    public async Task AccountVerification(AccountVerificationInput input)
    {
        await EnsureApplication();

        var mobile = input.Mobile.Trim();
        var email = input.Email.Trim()
            .ToLowerInvariant();
        var accountModel = await _repository.SingleOrDefaultAsync(_user.AccountId);
        if (accountModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        // 验证账号状态
        if (accountModel.Status == CommonStatusEnum.Disable)
        {
            throw new UserFriendlyException("账号已被平台禁用！");
        }

        // 验证状态
        if (accountModel.IdentityVerification)
        {
            throw new UserFriendlyException("账号已校验完成，请勿重复校验！");
        }

        if (await _repository.AnyAsync(a => a.Mobile == mobile && a.AccountId != _user.AccountId))
        {
            throw new UserFriendlyException("手机号已存在账号信息！");
        }

        if (await _repository.AnyAsync(a => a.Email == email && a.AccountId != _user.AccountId))
        {
            throw new UserFriendlyException("邮箱已存在账号信息！");
        }

        // 获取缓存Key
        var cacheKey = CacheConst.GetCacheKey(CacheConst.AccountIdentityVerification, accountModel.AccountKey,
            GlobalContext.ClientIdentity);
        using var codeLock = _centerCache.Client.TryLock($"{cacheKey}:Lock", 120);
        if (codeLock == null)
        {
            throw new UserFriendlyException("操作过于频繁，请稍后重试！");
        }

        var dto = await _centerCache.GetAsync<AccountVerificationCacheDto>(cacheKey);
        if (dto == null || dto.AccountId != accountModel.AccountId || dto.ClientIdentity != GlobalContext.ClientIdentity)
        {
            throw new UserFriendlyException("验证码无效或已过期！");
        }

        if (!string.Equals(Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(accountModel.Password))), dto.PasswordHash))
        {
            throw new UserFriendlyException("验证码无效或已过期！");
        }


        // TODO：因为穷买不起短信，下面注释的代码是为了兼容默认短信验证码 123456 的，后续购买后可删除。
        if (
            //dto.Mobile != mobile ||
            dto.Email != email
            //|| dto.MobileExpiresTime <= DateTime.Now
            || dto.EmailExpiresTime <= DateTime.Now)
        {
            throw new UserFriendlyException("验证码无效或已过期！");
        }

        // 分别保存已验证的结果，邮箱输错或后续更新失败时，不会要求重发已通过的短信验证码。
        if (!dto.MobileVerified)
        {
            if (dto.Mobile == null && input.MobileVerificationCode == "123456")
            {
            }
            else
            {
                await _smsService.VerifyVerificationCode(SmsTypeEnum.Validity, mobile, input.MobileVerificationCode);
                dto.MobileVerified = true;
                await _centerCache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));
            }
        }

        if (!dto.EmailVerified)
        {
            await _mailService.VerifyVerificationCode(MailTypeEnum.Validity, email, input.EmailVerificationCode);
            dto.EmailVerified = true;
            await _centerCache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));
        }

        // 更新手机号，邮箱，校验标识
        //accountModel.Mobile = mobile;
        accountModel.Email = email;
        accountModel.IdentityVerification = true;

        await _repository.Updateable(accountModel)
            .UpdateColumns(e => new {e.Mobile, e.Email, e.IdentityVerification})
            .ExecuteCommandWithOptLockAsync(true);

        await _centerCache.DelAsync(cacheKey);

        // 退出登录
        await _user.Logout();
        await _user.RevokeAccount(accountModel.AccountId);
        await AccountForceOffline(accountModel.AccountId, "账号已修改，请重新登录");
    }

    /// <summary>
    /// 发送编辑账号验证码
    /// </summary>
    [HttpPost]
    [ApiInfo("发送编辑账号验证码", HttpRequestActionEnum.Auth)]
    public async Task SendEditAccountVerificationCode(SendAccountVerificationCodeInput input)
    {
        await EnsureApplication();
        await _captchaService.VerifyImageCaptcha(input.CaptchaKey, input.CaptchaCode);

        var accountModel = await _repository.SingleOrDefaultAsync(_user.AccountId);
        if (accountModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        // 验证账号状态
        if (accountModel.Status == CommonStatusEnum.Disable)
        {
            throw new UserFriendlyException("账号已被平台禁用！");
        }

        var account = input.Account.Trim()
            .ToLowerInvariant();
        MessageSendChannelEnum sendChannel;
        if (Regex.IsMatch(account, RegexConst.Mobile))
        {
            sendChannel = MessageSendChannelEnum.Sms;
            if (accountModel.Mobile == account)
            {
                throw new UserFriendlyException("手机号未发生变化！");
            }

            if (await _repository.AnyAsync(a => a.Mobile == account && a.AccountId != _user.AccountId))
            {
                throw new UserFriendlyException("手机号已存在账号信息！");
            }
        }
        else if (Regex.IsMatch(account, RegexConst.EmailAddress))
        {
            sendChannel = MessageSendChannelEnum.Email;
            if (string.Equals(accountModel.Email, account, StringComparison.OrdinalIgnoreCase))
            {
                throw new UserFriendlyException("邮箱未发生变化！");
            }

            if (await _repository.AnyAsync(a => a.Email == account && a.AccountId != _user.AccountId))
            {
                throw new UserFriendlyException("邮箱已存在账号信息！");
            }
        }
        else
        {
            throw new UserFriendlyException("请输入正确的手机号或邮箱！");
        }

        // 同一个IP地址，1小时内最多允许20次
        await EnforceSendQuota($"Ip:{FastContext.HttpContext.Connection.RemoteIpAddress?.MapToIPv6()
                                         .ToString()
                                     ?? "unknown"}", (20, 3600));
        var recipient = $"EditAccount:{sendChannel}:{accountModel.AccountKey}";
        // 1小时5次，24小时10次
        await EnforceSendQuota(recipient, (5, 3600), (10, 86400));

        // 不同客户端独立保存校验进度，发送与提交共用锁，避免覆盖已验证的结果。
        var clientIdentity = GlobalContext.ClientIdentity;
        var cacheKey = CacheConst.GetCacheKey(CacheConst.AccountIdentityVerification, accountModel.AccountKey, clientIdentity);
        using var codeLock = _centerCache.Client.TryLock($"{cacheKey}:Lock", 120);
        if (codeLock == null)
        {
            throw new UserFriendlyException("操作过于频繁，请稍后重试！");
        }

        var passwordHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(accountModel.Password)));
        var dto = await _centerCache.GetAsync<AccountVerificationCacheDto>(cacheKey);
        if (dto == null
            || dto.AccountId != accountModel.AccountId
            || dto.ClientIdentity != clientIdentity
            || dto.PasswordHash != passwordHash)
        {
            dto = new AccountVerificationCacheDto
            {
                AccountId = accountModel.AccountId, PasswordHash = passwordHash, ClientIdentity = clientIdentity
            };
        }

        var expiresTime = DateTime.Now.AddMinutes(5);
        switch (sendChannel)
        {
            case MessageSendChannelEnum.Email:
                await _mailService.SendVerificationCode(MailTypeEnum.Validity, account);
                dto.Email = account;
                dto.EmailExpiresTime = expiresTime;
                dto.EmailVerified = false;
                break;
            case MessageSendChannelEnum.Sms:
                await _smsService.SendVerificationCode(SmsTypeEnum.Validity, account);
                dto.Mobile = account;
                dto.MobileExpiresTime = expiresTime;
                dto.MobileVerified = false;
                break;
        }

        // 重发只重置当前通道，另一通道的校验结果仍受其原始过期时间限制。
        await _centerCache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));
    }
}