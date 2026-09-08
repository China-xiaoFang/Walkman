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
using Fast.Admin.Domain;
using Fast.Admin.Service.Employee.Dto;
using Fast.AdminLog.Domain;
using Fast.Center.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yitter.IdGenerator;

namespace Fast.Admin.Service.Employee;

public partial class EmployeeService
{
    /// <summary>
    /// 职员更改状态
    /// </summary>
    [HttpPost]
    [ApiInfo("职员更改状态", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Employee.Status)]
    public async Task ChangeStatus(ChangeStatusInput input)
    {
        if (input.Status == EmployeeStatusEnum.Resigned)
        {
            throw new UserFriendlyException("禁止直接修改为离职状态！");
        }

        var employeeModel = await GetEmployeeWithinDataScope(input.EmployeeId);

        if (employeeModel.ResignDate != null)
        {
            employeeModel.ResignDate = null;
            employeeModel.ResignReason = null;
        }

        employeeModel.Status = input.Status;
        employeeModel.RowVersion = input.RowVersion;

        await _repository.UpdateAsync(employeeModel);

        // 操作日志
        await LogContext.OperateLog(new OperateLogDto
        {
            Title = "更改职员状态",
            OperateType = OperateLogTypeEnum.Organization,
            BizId = employeeModel.EmployeeId,
            BizNo = employeeModel.EmployeeNo,
            Description = $"职员：{employeeModel.EmployeeName}，状态 -> {employeeModel.Status.GetDescription()}"
        });
    }

    /// <summary>
    /// 职员离职
    /// </summary>
    [HttpPost]
    [ApiInfo("职员离职", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Employee.Status)]
    public async Task EmployeeResigned(EmployeeResignedInput input)
    {
        var employeeModel = await GetEmployeeWithinDataScope(input.EmployeeId);

        if (employeeModel.EmployeeId == _user.EmployeeId)
        {
            throw new UserFriendlyException("禁止将当前登录职员设为离职！");
        }

        if (employeeModel.Status == EmployeeStatusEnum.Resigned)
        {
            throw new UserFriendlyException("该职员已离职，请勿重复操作！");
        }

        // 开启事务
        await _repository.Ado.BeginTranAsync();
        await _centerRepository.Ado.BeginTranAsync();
        try
        {
            var tenantUserModel = await _centerRepository.Queryable<TenantUserModel>()
                .InSingleAsync(employeeModel.EmployeeId);
            if (tenantUserModel != null)
            {
                tenantUserModel.Status = CommonStatusEnum.Disable;
                await _centerRepository.Updateable(tenantUserModel)
                    .ExecuteCommandAsync();
            }

            employeeModel.Status = EmployeeStatusEnum.Resigned;
            employeeModel.ResignDate = input.ResignDate;
            employeeModel.ResignReason = input.ResignReason;
            employeeModel.RowVersion = input.RowVersion;

            await _repository.UpdateAsync(employeeModel);

            // 提交事务
            await _repository.Ado.CommitTranAsync();
            await _centerRepository.Ado.CommitTranAsync();
        }
        catch
        {
            // 回滚事务
            await _repository.Ado.RollbackTranAsync();
            await _centerRepository.Ado.RollbackTranAsync();
            throw;
        }

        await _user.RevokeEmployee(_user.TenantNo, employeeModel.EmployeeNo);
        await ForceEmployeeOffline(employeeModel.EmployeeId, "职员已离职");

        // 操作日志
        await LogContext.OperateLog(new OperateLogDto
        {
            Title = "职员离职",
            OperateType = OperateLogTypeEnum.Organization,
            BizId = employeeModel.EmployeeId,
            BizNo = employeeModel.EmployeeNo,
            Description = $"职员：{employeeModel.EmployeeName}，离职 -> {employeeModel.ResignDate:yyyy-MM-dd HH:mm:ss}"
        });
    }

    /// <summary>
    /// 绑定登录账号
    /// </summary>
    [HttpPost]
    [ApiInfo("绑定登录账号", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Employee.Edit)]
    public async Task BindLoginAccount(BindLoginAccountInput input)
    {
        if (!new Regex(RegexConst.Mobile).IsMatch(input.Mobile))
        {
            throw new UserFriendlyException("手机号码不正确！");
        }

        var employeeModel = await GetEmployeeWithinDataScope(input.EmployeeId);

        if (employeeModel.Status == EmployeeStatusEnum.Resigned)
        {
            throw new UserFriendlyException("禁止为已离职的职员绑定登录账号！");
        }

        if (await _centerRepository.Queryable<TenantUserModel>()
                .AnyAsync(a => a.EmployeeId == employeeModel.EmployeeId))
        {
            throw new UserFriendlyException("已存在登录账号！");
        }

        var employeeOrgModel = await _repository.Queryable<EmployeeOrgModel>()
            .SingleAsync(s => s.EmployeeId == employeeModel.EmployeeId && s.IsPrimary);

        if (string.IsNullOrWhiteSpace(employeeModel.Email))
        {
            employeeModel.Email = input.Email;
        }

        employeeModel.RowVersion = input.RowVersion;

        // 开启事务
        await _repository.Ado.BeginTranAsync();
        await _centerRepository.Ado.BeginTranAsync();
        try
        {
            var accountModel = await _centerRepository.Queryable<AccountModel>()
                .Where(wh => wh.Mobile == input.Mobile)
                .SingleAsync();
            if (accountModel == null)
            {
                if (await _centerRepository.Queryable<AccountModel>()
                        .AnyAsync(a => a.Email == input.Email))
                {
                    throw new UserFriendlyException("邮箱已存在账号信息！");
                }

                var passwordHash = CryptoUtil.HashPasswordPBKDF2SHA256(CommonConst.Default.Password);
                var accountId = YitIdHelper.NextId();
                accountModel = new AccountModel
                {
                    AccountId = accountId,
                    AccountKey = NumberUtil.IdToCodeByLong(accountId),
                    Mobile = input.Mobile,
                    Email = input.Email,
                    Password = passwordHash,
                    Status = CommonStatusEnum.Enable,
                    NickName = employeeModel.EmployeeName,
                    Avatar = employeeModel.IdPhoto
                };
                await _centerRepository.Insertable(accountModel)
                    .ExecuteCommandAsync();

                #region PasswordRecordModel

                // 初始化密码记录表
                await _centerRepository.Insertable(new List<PasswordRecordModel>
                    {
                        new()
                        {
                            AccountId = accountModel.AccountId,
                            OperationType = PasswordOperationTypeEnum.Create,
                            Type = PasswordTypeEnum.PBKDF2_SHA256,
                            Password = passwordHash
                        }
                    })
                    .ExecuteCommandAsync();

                #endregion
            }

            var tenantUserModel = new TenantUserModel
            {
                EmployeeId = employeeModel.EmployeeId,
                UserKey = NumberUtil.IdToCodeByLong(employeeModel.EmployeeId),
                AccountId = accountModel.AccountId,
                EmployeeNo = employeeModel.EmployeeNo,
                EmployeeName = employeeModel.EmployeeName,
                IdPhoto = employeeModel.IdPhoto,
                DepartmentId = employeeOrgModel?.DepartmentId,
                DepartmentName = employeeOrgModel?.DepartmentName,
                UserType = UserTypeEnum.None,
                Status = CommonStatusEnum.Enable
            };
            await _centerRepository.Insertable(tenantUserModel)
                .ExecuteCommandAsync();

            await _repository.UpdateAsync(employeeModel);

            // 提交事务
            await _repository.Ado.CommitTranAsync();
            await _centerRepository.Ado.CommitTranAsync();
        }
        catch
        {
            // 回滚事务
            await _repository.Ado.RollbackTranAsync();
            await _centerRepository.Ado.RollbackTranAsync();
            throw;
        }

        // 操作日志
        await LogContext.OperateLog(new OperateLogDto
        {
            Title = "职员绑定登录账号",
            OperateType = OperateLogTypeEnum.Organization,
            BizId = employeeModel.EmployeeId,
            BizNo = employeeModel.EmployeeNo,
            Description = $"职员：{employeeModel.EmployeeName}，手机：{input.Mobile}，邮箱：{input.Email}"
        });
    }

    /// <summary>
    /// 更改登录状态
    /// </summary>
    [HttpPost]
    [ApiInfo("更改登录状态", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Employee.Status)]
    public async Task ChangeLoginStatus(EmployeeIdInput input)
    {
        var employeeModel = await GetEmployeeWithinDataScope(input.EmployeeId);

        if (employeeModel.RowVersion != input.RowVersion)
        {
            throw new UserFriendlyException("职员信息已发生变化，请刷新后重试！");
        }

        if (employeeModel.Status == EmployeeStatusEnum.Resigned)
        {
            throw new UserFriendlyException("禁止操作已离职的职员！");
        }

        var tenantUserModel = await _centerRepository.Queryable<TenantUserModel>()
            .InSingleAsync(employeeModel.EmployeeId);
        if (tenantUserModel == null)
        {
            throw new UserFriendlyException("未绑定登录账号！");
        }

        if (_user.AccountId == tenantUserModel.AccountId)
        {
            throw new UserFriendlyException("禁止更改当前登录账号状态！");
        }

        if (input.AccountStatus != null && input.AccountStatus != tenantUserModel.Status)
        {
            throw new UserFriendlyException("登录账号状态已发生变化，请刷新后重试！");
        }

        tenantUserModel.Status = tenantUserModel.Status switch
        {
            CommonStatusEnum.Enable => CommonStatusEnum.Disable,
            CommonStatusEnum.Disable => CommonStatusEnum.Enable,
            _ => tenantUserModel.Status
        };

        await _centerRepository.Updateable(tenantUserModel)
            .ExecuteCommandAsync();

        if (tenantUserModel.Status == CommonStatusEnum.Disable)
        {
            await _user.RevokeEmployee(_user.TenantNo, employeeModel.EmployeeNo);
            await ForceEmployeeOffline(employeeModel.EmployeeId, "账号已被禁用");
        }

        // 操作日志
        await LogContext.OperateLog(new OperateLogDto
        {
            Title = "更改职员登录状态",
            OperateType = OperateLogTypeEnum.Organization,
            BizId = employeeModel.EmployeeId,
            BizNo = employeeModel.EmployeeNo,
            Description = $"职员：{employeeModel.EmployeeName}，{tenantUserModel.Status.GetDescription()}登录账号"
        });
    }

    /// <summary>
    /// 强制下线职员的全部在线会话
    /// </summary>
    private async Task ForceEmployeeOffline(long employeeId, string message)
    {
        var connectionIds = await _centerRepository.Queryable<TenantOnlineUserModel>()
            .Where(wh => wh.IsOnline)
            .Where(wh => wh.TenantId == _user.TenantId)
            .Where(wh => wh.EmployeeId == employeeId)
            .Select(sl => sl.ConnectionId)
            .ToListAsync();
        if (connectionIds.Count == 0)
            return;

        await _hubContext.Clients.Clients(connectionIds)
            .ForceOffline(new ForceOfflineOutput
            {
                IsAdmin = _user.IsSuperAdmin || _user.IsAdmin,
                NickName = _user.NickName,
                EmployeeNo = _user.EmployeeNo,
                OfflineTime = DateTime.Now,
                Message = message
            });
    }
}