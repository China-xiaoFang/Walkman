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
using Fast.Admin.Entity;
using Fast.Admin.Enum;
using Fast.Admin.Service.Account.Dto;
using Fast.AdminLog.Entity;
using Fast.AdminLog.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Yitter.IdGenerator;

namespace Fast.Admin.Service.Account;

/// <summary>
/// <see cref="AccountService"/> 账号服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Center, Name = "account")]
public class AccountService : IDynamicApplication
{
    private readonly IUser _user;
    private readonly ISqlSugarRepository<AccountModel> _repository;

    public AccountService(IUser user, ISqlSugarRepository<AccountModel> repository)
    {
        _user = user;
        _repository = repository;
    }

    /// <summary>
    /// 账号选择器
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("账号选择器", HttpRequestActionEnum.Query)]
    public async Task<PagedResult<ElSelectorOutput<long>>> AccountSelector(PagedInput input)
    {
        var data = await _repository.Entities.WhereIF(!string.IsNullOrWhiteSpace(input.SearchValue),
                wh => wh.Mobile.Contains(input.SearchValue) || wh.NickName.Contains(input.SearchValue))
            .OrderBy(ob => ob.Mobile)
            .Select(sl => new AccountModel
            {
                AccountId = sl.AccountId,
                Mobile = sl.Mobile,
                NickName = sl.NickName,
                Avatar = sl.Avatar,
                Sex = sl.Sex
            })
            .OrderBy(ob => ob.NickName)
            .ToPagedListAsync(input);

        return data.ToPagedData(sl => new ElSelectorOutput<long>
        {
            Value = sl.AccountId, Label = sl.Mobile, Data = new {sl.NickName, sl.Avatar, sl.Sex}
        });
    }

    /// <summary>
    /// 获取账号分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取账号分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.Account.Paged)]
    public async Task<PagedResult<QueryAccountPagedOutput>> QueryAccountPaged(QueryAccountPagedInput input)
    {
        var dateTime = DateTime.Now;

        var queryable = _repository.Queryable<AccountModel>()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Mobile), wh => wh.Mobile.Contains(input.Mobile))
            .WhereIF(input.Status != null, t1 => t1.Status == input.Status)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Phone), wh => wh.Phone.Contains(input.Phone))
            .WhereIF(input.Sex != null, t1 => t1.Sex == input.Sex)
            .WhereIF(!string.IsNullOrWhiteSpace(input.FirstLoginCity), wh => wh.FirstLoginCity.Contains(input.FirstLoginCity))
            .WhereIF(!string.IsNullOrWhiteSpace(input.FirstLoginIp), wh => wh.FirstLoginIp.Contains(input.FirstLoginIp))
            .WhereIF(!string.IsNullOrWhiteSpace(input.LastLoginCity), wh => wh.LastLoginCity.Contains(input.LastLoginCity))
            .WhereIF(!string.IsNullOrWhiteSpace(input.LastLoginIp), wh => wh.LastLoginIp.Contains(input.LastLoginIp))
            .WhereIF(input.IsLock == true, wh => wh.LockEndTime != null && wh.LockEndTime >= dateTime)
            .WhereIF(input.IsLock == false, wh => wh.LockEndTime == null || wh.LockEndTime < dateTime);

        return await queryable.SelectMergeTable(sl => new QueryAccountPagedOutput
            {
                AccountId = sl.AccountId,
                Mobile = sl.Mobile,
                Status = sl.Status,
                NickName = sl.NickName,
                Avatar = sl.Avatar,
                Sex = sl.Sex,
                Birthday = sl.Birthday,
                FirstLoginDevice = sl.FirstLoginDevice,
                FirstLoginOS = sl.FirstLoginOS,
                FirstLoginBrowser = sl.FirstLoginBrowser,
                FirstLoginProvince = sl.FirstLoginProvince,
                FirstLoginCity = sl.FirstLoginCity,
                FirstLoginIp = sl.FirstLoginIp,
                FirstLoginTime = sl.FirstLoginTime,
                LastLoginDevice = sl.LastLoginDevice,
                LastLoginOS = sl.LastLoginOS,
                LastLoginBrowser = sl.LastLoginBrowser,
                LastLoginProvince = sl.LastLoginProvince,
                LastLoginCity = sl.LastLoginCity,
                LastLoginIp = sl.LastLoginIp,
                LastLoginTime = sl.LastLoginTime,
                PasswordErrorTime = sl.PasswordErrorTime,
                LockStartTime = sl.LockStartTime,
                LockEndTime = sl.LockEndTime,
                IsLock = SqlFunc.IF(sl.LockEndTime != null && sl.LockEndTime >= dateTime)
                    .Return(true)
                    .End(false),
                CreatedTime = sl.CreatedTime,
                UpdatedTime = sl.UpdatedTime,
                RowVersion = sl.RowVersion
            })
            .OrderByIF(input.IsOrderBy, ob => ob.CreatedTime, OrderByType.Desc)
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取账号详情
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取账号详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Account.Detail)]
    public async Task<QueryAccountDetailOutput> QueryAccountDetail([Required(ErrorMessage = "账号Id不能为空")] long? accountId)
    {
        var result = await _repository.Queryable<AccountModel>()
            .Where(wh => wh.AccountId == accountId)
            .Select(sl => new QueryAccountDetailOutput
            {
                AccountId = sl.AccountId,
                Mobile = sl.Mobile,
                WeChatId = sl.WeChatId,
                Status = sl.Status,
                NickName = sl.NickName,
                Avatar = sl.Avatar,
                Phone = sl.Phone,
                Sex = sl.Sex,
                Birthday = sl.Birthday,
                FirstLoginDevice = sl.FirstLoginDevice,
                FirstLoginOS = sl.FirstLoginOS,
                FirstLoginBrowser = sl.FirstLoginBrowser,
                FirstLoginProvince = sl.FirstLoginProvince,
                FirstLoginCity = sl.FirstLoginCity,
                FirstLoginIp = sl.FirstLoginIp,
                FirstLoginTime = sl.FirstLoginTime,
                LastLoginDevice = sl.LastLoginDevice,
                LastLoginOS = sl.LastLoginOS,
                LastLoginBrowser = sl.LastLoginBrowser,
                LastLoginProvince = sl.LastLoginProvince,
                LastLoginCity = sl.LastLoginCity,
                LastLoginIp = sl.LastLoginIp,
                LastLoginTime = sl.LastLoginTime,
                PasswordErrorTime = sl.PasswordErrorTime,
                LockStartTime = sl.LockStartTime,
                LockEndTime = sl.LockEndTime,
                CreatedTime = sl.CreatedTime,
                UpdatedTime = sl.UpdatedTime,
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
    /// 获取编辑账号详情
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取编辑账号详情", HttpRequestActionEnum.Query)]
    public async Task<EditAccountInput> QueryEditAccountDetail()
    {
        var result = await _repository.Queryable<AccountModel>()
            .Where(wh => wh.AccountId == _user.AccountId)
            .Select(sl => new EditAccountInput
            {
                Mobile = sl.Mobile,
                NickName = sl.NickName,
                Avatar = sl.Avatar,
                Phone = sl.Phone,
                Sex = sl.Sex,
                Birthday = sl.Birthday,
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
    /// 编辑账号
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑账号", HttpRequestActionEnum.Edit)]
    public async Task EditAccount(EditAccountInput input)
    {
        if (!new Regex(RegexConst.Mobile).IsMatch(input.Mobile))
        {
            throw new UserFriendlyException("手机号码不正确！");
        }

        var accountModel = await _repository.SingleOrDefaultAsync(_user.AccountId);
        if (accountModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        if (await _repository.AnyAsync(a => a.Mobile == input.Mobile && a.AccountId != _user.AccountId))
        {
            throw new UserFriendlyException("手机号已存在账号信息！");
        }

        accountModel.Mobile = input.Mobile;
        accountModel.NickName = input.NickName;
        accountModel.Avatar = input.Avatar;
        accountModel.Phone = input.Phone;
        accountModel.Sex = input.Sex;
        accountModel.Birthday = input.Birthday;
        accountModel.RowVersion = input.RowVersion;

        // 同步微信用户信息
        WeChatUserModel weChatUserModel = null;
        if (accountModel.WeChatId != null)
        {
            weChatUserModel = await _repository.Queryable<WeChatUserModel>()
                .InSingleAsync(accountModel.WeChatId);

            if (weChatUserModel == null)
            {
                // 自动解绑
                accountModel.WeChatId = null;
            }
            else
            {
                weChatUserModel.NickName = input.NickName;
                weChatUserModel.Avatar = input.Avatar;
                weChatUserModel.Sex = input.Sex;
            }
        }

        await _repository.Ado.UseTranAsync(async () =>
        {
            if (weChatUserModel != null)
            {
                await _repository.Updateable(weChatUserModel)
                    .ExecuteCommandAsync();
            }

            await _repository.UpdateAsync(accountModel);
        }, ex => throw ex);

        // 刷新缓存
        await _user.RefreshAccount(new RefreshAccountDto
        {
            DeviceType = _user.DeviceType,
            Mobile = accountModel.Mobile,
            NickName = input.NickName,
            Avatar = input.Avatar,
            EmployeeNo = _user.EmployeeNo
        });
    }

    /// <summary>
    /// 账号修改密码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("账号修改密码", HttpRequestActionEnum.Edit)]
    public async Task ChangePassword(ChangePasswordInput input)
    {
        if (!string.Equals(input.NewPassword, input.ConfirmPassword, StringComparison.OrdinalIgnoreCase))
        {
            throw new UserFriendlyException("新密码和确认密码不一致！");
        }

        var accountModel = await _repository.SingleOrDefaultAsync(_user.AccountId);
        if (accountModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        if (!string.Equals(accountModel.Password, input.OldPassword, StringComparison.OrdinalIgnoreCase))
        {
            throw new UserFriendlyException("旧密码不正确！");
        }

        // 更新密码
        accountModel.Password = input.NewPassword.ToUpper();
        accountModel.RowVersion = input.RowVersion;

        var _visitLogRepository = FastContext.HttpContext.RequestServices.GetService<ISqlSugarRepository<VisitLogModel>>();

        // 添加访问日志
        var visitLogModel = new VisitLogModel
        {
            RecordId = YitIdHelper.NextId(),
            AccountId = _user.AccountId,
            Mobile = _user.Mobile,
            NickName = _user.NickName,
            VisitType = VisitTypeEnum.ChangePassword,
            DepartmentId = _user.DepartmentId,
            DepartmentName = _user.DepartmentName,
            CreatedUserId = _user.EmployeeId,
            CreatedUserName = _user.EmployeeName,
            CreatedTime = DateTime.Now
        };
        visitLogModel.RecordCreate(FastContext.HttpContext);

        await _repository.Ado.UseTranAsync(async () =>
        {
            await _repository.UpdateAsync(accountModel);
            await _repository.Insertable(new PasswordRecordModel
                {
                    AccountId = accountModel.AccountId,
                    OperationType = PasswordOperationTypeEnum.Change,
                    Type = PasswordTypeEnum.SHA1,
                    Password = accountModel.Password
                })
                .ExecuteCommandAsync();
            await _visitLogRepository.InsertAsync(visitLogModel);
        }, ex => throw ex);

        // 退出登录
        await _user.Logout();
    }

    /// <summary>
    /// 账号解除锁定
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("账号解除锁定", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Account.Unlock)]
    public async Task Unlock(AccountIdInput input)
    {
        var accountModel = await _repository.SingleOrDefaultAsync(input.AccountId);
        if (accountModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        var dateTime = DateTime.Now;

        // 判断是否存在锁定
        if (accountModel.LockEndTime == null || accountModel.LockEndTime < dateTime)
        {
            throw new UserFriendlyException("账号未锁定！");
        }

        accountModel.PasswordErrorTime = null;
        accountModel.LockStartTime = null;
        accountModel.LockEndTime = null;
        accountModel.RowVersion = input.RowVersion;

        await _repository.UpdateAsync(accountModel);
    }

    /// <summary>
    /// 账号重置密码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("账号重置密码", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Account.ResetPassword)]
    public async Task ResetPassword(AccountIdInput input)
    {
        if (_user.AccountId == input.AccountId)
        {
            throw new UserFriendlyException("禁止重置当前登录账号密码！");
        }

        var accountModel = await _repository.SingleOrDefaultAsync(input.AccountId);
        if (accountModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        // 更新密码
        accountModel.Password = CryptoUtil.SHA1Encrypt(CommonConst.Default.Password)
            .ToUpper();
        accountModel.RowVersion = input.RowVersion;

        await _repository.Ado.UseTranAsync(async () =>
        {
            await _repository.UpdateAsync(accountModel);
            await _repository.Insertable(new PasswordRecordModel
                {
                    AccountId = accountModel.AccountId,
                    OperationType = PasswordOperationTypeEnum.Reset,
                    Type = PasswordTypeEnum.SHA1,
                    Password = accountModel.Password
                })
                .ExecuteCommandAsync();
        }, ex => throw ex);
    }

    /// <summary>
    /// 账号更改状态
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("账号更改状态", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Account.Status)]
    public async Task ChangeStatus(AccountIdInput input)
    {
        if (_user.AccountId == input.AccountId)
        {
            throw new UserFriendlyException("禁止更改当前登录账号状态！");
        }

        var accountModel = await _repository.SingleOrDefaultAsync(input.AccountId);
        if (accountModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        // 更改状态
        accountModel.Status = accountModel.Status switch
        {
            CommonStatusEnum.Enable => CommonStatusEnum.Disable,
            CommonStatusEnum.Disable => CommonStatusEnum.Enable,
            _ => accountModel.Status
        };
        accountModel.RowVersion = input.RowVersion;

        await _repository.UpdateAsync(accountModel);

        if (accountModel.Status == CommonStatusEnum.Disable)
        {
            // 强制下线当前账号所有在线用户
            var _hubContext = FastContext.HttpContext.RequestServices.GetService<IHubContext<ChatHub, IChatClient>>();

            var connectionIds = await _repository.Queryable<OnlineUserModel>()
                .Where(wh => wh.IsOnline)
                .Where(wh => wh.AccountId == accountModel.AccountId)
                .Select(sl => sl.ConnectionId)
                .ToListAsync();

            await _hubContext.Clients.Clients(connectionIds)
                .ForceOffline(new ForceOfflineOutput
                {
                    IsAdmin = _user.IsSuperAdmin || _user.IsAdmin,
                    NickName = _user.NickName,
                    EmployeeNo = _user.EmployeeNo,
                    OfflineTime = DateTime.Now,
                    Message = "账号已被禁用"
                });
        }
    }
}