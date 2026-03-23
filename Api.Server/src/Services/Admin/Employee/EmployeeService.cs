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

using Fast.Admin.Entity;
using Fast.Admin.Enum;
using Fast.Admin.Service.Employee.Dto;
using Fast.AdminLog.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Yitter.IdGenerator;

namespace Fast.Admin.Service.Employee;

/// <summary>
/// <see cref="EmployeeService"/> 职员服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Admin, Name = "employee")]
public class EmployeeService : IDynamicApplication
{
    private readonly IUser _user;
    private readonly ISqlSugarRepository<EmployeeModel> _repository;

    public EmployeeService(IUser user, ISqlSugarRepository<EmployeeModel> repository)
    {
        _user = user;
        _repository = repository;
    }

    /// <summary>
    /// 职员选择器
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("职员选择器", HttpRequestActionEnum.Query)]
    public async Task<PagedResult<ElSelectorOutput<long>>> EmployeeSelector(PagedInput input)
    {
        var data = await _repository.Entities
            .LeftJoin<EmployeeOrgModel>((t1, t2) => t1.EmployeeId == t2.EmployeeId && t2.IsPrimary)
            .WhereIF(!string.IsNullOrWhiteSpace(input.SearchValue),
                t1 => t1.EmployeeNo.Contains(input.SearchValue)
                      || t1.EmployeeName.Contains(input.SearchValue)
                      || t1.Mobile.Contains(input.SearchValue))
            .Where(t1 => t1.Status != EmployeeStatusEnum.Resigned)
            .SelectMergeTable((t1, t2) => new QueryEmployeeSelectorDto
            {
                EmployeeId = t1.EmployeeId,
                DepartmentId = t2.DepartmentId,
                EmployeeNo = t1.EmployeeNo,
                EmployeeName = t1.EmployeeName,
                Mobile = t1.Mobile,
                IdPhoto = t1.IdPhoto
            })
            .OrderBy(ob => ob.EmployeeName)
            .DataScope(e => e.DepartmentId, e => e.EmployeeId)
            .ToPagedListAsync(input);

        return data.ToPagedData(sl => new ElSelectorOutput<long>
        {
            Value = sl.EmployeeId, Label = sl.EmployeeName, Data = new {sl.EmployeeNo, sl.Mobile, sl.IdPhoto}
        });
    }

    /// <summary>
    /// 获取职员分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取职员分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.Employee.Paged)]
    public async Task<PagedResult<QueryEmployeePagedOutput>> QueryEmployeePaged(QueryEmployeePagedInput input)
    {
        var result = await _repository.Entities
            .LeftJoin<EmployeeOrgModel>((t1, t2) => t1.EmployeeId == t2.EmployeeId && t2.IsPrimary)
            .LeftJoin<AccountModel>((t1, t2, t3) => t1.AccountId == t3.AccountId)
            .WhereIF(input.Status != null, t1 => t1.Status == input.Status)
            .WhereIF(input.Sex != null, t1 => t1.Sex == input.Sex)
            .WhereIF(input.Nation != null, t1 => t1.Nation == input.Nation)
            .WhereIF(!string.IsNullOrWhiteSpace(input.NativePlace), t1 => t1.NativePlace.Contains(input.NativePlace))
            .WhereIF(input.EducationLevel != null, t1 => t1.EducationLevel == input.EducationLevel)
            .WhereIF(input.PoliticalStatus != null, t1 => t1.PoliticalStatus == input.PoliticalStatus)
            .WhereIF(!string.IsNullOrWhiteSpace(input.GraduationCollege),
                t1 => t1.GraduationCollege.Contains(input.GraduationCollege))
            .WhereIF(input.AcademicQualifications != null, t1 => t1.AcademicQualifications == input.AcademicQualifications)
            .WhereIF(input.AcademicSystem != null, t1 => t1.AcademicSystem == input.AcademicSystem)
            .WhereIF(input.Degree != null, t1 => t1.Degree == input.Degree)
            .WhereIF(input.DepartmentId != null, (t1, t2) => t2.DepartmentId == input.DepartmentId)
            .SelectMergeTable((t1, t2, t3) => new QueryEmployeePagedOutput
            {
                EmployeeId = t1.EmployeeId,
                UserType = t1.UserType,
                EmployeeNo = t1.EmployeeNo,
                EmployeeName = t1.EmployeeName,
                Mobile = t1.Mobile,
                Status = t1.Status,
                Email = t1.Email,
                Sex = t1.Sex,
                IdPhoto = t1.IdPhoto,
                EntryDate = t1.EntryDate,
                ResignDate = t1.ResignDate,
                Nation = t1.Nation,
                NativePlace = t1.NativePlace,
                Birthday = t1.Birthday,
                EducationLevel = t1.EducationLevel,
                PoliticalStatus = t1.PoliticalStatus,
                GraduationCollege = t1.GraduationCollege,
                AcademicQualifications = t1.AcademicQualifications,
                AcademicSystem = t1.AcademicSystem,
                Degree = t1.Degree,
                Remark = t1.Remark,
                CreatedUserName = t1.CreatedUserName,
                CreatedTime = t1.CreatedTime,
                UpdatedUserName = t1.UpdatedUserName,
                UpdatedTime = t1.UpdatedTime,
                RowVersion = t1.RowVersion,
                OrgId = t2.OrgId,
                OrgName = t2.OrgName,
                OrgNames = t2.OrgNames,
                DepartmentId = t2.DepartmentId,
                DepartmentName = t2.DepartmentName,
                DepartmentNames = t2.DepartmentNames,
                PositionId = t2.PositionId,
                PositionName = t2.PositionName,
                JobLevelId = t2.JobLevelId,
                JobLevelName = t2.JobLevelName,
                IsPrincipal = t2.IsPrincipal,
                AccountStatus = t3.Status,
                AccountMobile = t3.Mobile,
                AccountNickName = t3.NickName,
                LastLoginTime = t3.LastLoginTime
            })
            .OrderByIF(input.IsOrderBy, ob => ob.CreatedTime, OrderByType.Desc)
            .DataScope(e => e.DepartmentId, e => e.EmployeeId)
            .ToPagedListAsync(input);

        var employeeIds = result.Rows.Select(sl => sl.EmployeeId)
            .ToList();

        var roleList = await _repository.Queryable<EmployeeRoleModel>()
            .Where(wh => employeeIds.Contains(wh.EmployeeId))
            .ToListAsync();

        foreach (var item in result.Rows)
        {
            item.RoleNames = string.Join(",", roleList.Where(wh => wh.EmployeeId == item.EmployeeId)
                .OrderBy(ob => ob.RoleName)
                .Select(sl => sl.RoleName)
                .ToList());
        }

        return result;
    }

    /// <summary>
    /// 获取职员详情
    /// </summary>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取职员详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Employee.Detail)]
    public async Task<QueryEmployeeDetailOutput> QueryEmployeeDetail([Required(ErrorMessage = "职员Id不能为空")] long? employeeId)
    {
        var result = await _repository.Entities.Where(wh => wh.EmployeeId == employeeId)
            .Select(sl => new QueryEmployeeDetailOutput
            {
                EmployeeId = sl.EmployeeId,
                EmployeeNo = sl.EmployeeNo,
                EmployeeName = sl.EmployeeName,
                Mobile = sl.Mobile,
                Status = sl.Status,
                Email = sl.Email,
                Sex = sl.Sex,
                IdPhoto = sl.IdPhoto,
                EntryDate = sl.EntryDate,
                ResignDate = sl.ResignDate,
                ResignReason = sl.ResignReason,
                Nation = sl.Nation,
                NativePlace = sl.NativePlace,
                FamilyAddress = sl.FamilyAddress,
                MailingAddress = sl.MailingAddress,
                Birthday = sl.Birthday,
                IdType = sl.IdType,
                IdNumber = sl.IdNumber,
                EducationLevel = sl.EducationLevel,
                PoliticalStatus = sl.PoliticalStatus,
                GraduationCollege = sl.GraduationCollege,
                AcademicQualifications = sl.AcademicQualifications,
                AcademicSystem = sl.AcademicSystem,
                Degree = sl.Degree,
                FamilyPhone = sl.FamilyPhone,
                OfficePhone = sl.OfficePhone,
                EmergencyContact = sl.EmergencyContact,
                EmergencyPhone = sl.EmergencyPhone,
                EmergencyAddress = sl.EmergencyAddress,
                Remark = sl.Remark,
                CreatedUserName = sl.CreatedUserName,
                CreatedTime = sl.CreatedTime,
                UpdatedUserName = sl.UpdatedUserName,
                UpdatedTime = sl.UpdatedTime,
                RowVersion = sl.RowVersion
            })
            .SingleAsync();

        if (result == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        result.OrgList = await _repository.Queryable<EmployeeOrgModel>()
            .Where(wh => wh.EmployeeId == employeeId)
            .ToListAsync();

        result.RoleList = await _repository.Queryable<EmployeeRoleModel>()
            .Where(wh => wh.EmployeeId == employeeId)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// 添加职员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加职员", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.Employee.Add)]
    public async Task AddEmployee(AddEmployeeInput input)
    {
        if (await _repository.AnyAsync(a => a.Mobile == input.Mobile))
        {
            throw new UserFriendlyException("手机号重复！");
        }

        var organizationModel = await _repository.Queryable<OrganizationModel>()
            .SingleAsync(s => s.OrgId == input.OrgId);
        if (organizationModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        var departmentModel = await _repository.Queryable<DepartmentModel>()
            .SingleAsync(s => s.DepartmentId == input.DepartmentId);
        if (departmentModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        var positionModel = await _repository.Queryable<PositionModel>()
            .SingleAsync(s => s.PositionId == input.PositionId);
        if (positionModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        var jobLevelModel = await _repository.Queryable<JobLevelModel>()
            .SingleAsync(s => s.JobLevelId == input.JobLevelId);
        if (jobLevelModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        var roleIds = input.RoleList.Select(sl => sl.RoleId)
            .ToList();
        var roleList = await _repository.Queryable<RoleModel>()
            .Where(wh => roleIds.Contains(wh.RoleId))
            .ToListAsync();
        if (roleList.Count != roleIds.Count)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        if (!_user.IsSuperAdmin && !_user.IsAdmin)
        {
            var _roleIds = _user.RoleIdList ?? [];
            var _roleList = await _repository.Queryable<RoleModel>()
                .Where(wh => _roleIds.Contains(wh.RoleId))
                .Select(sl => new {sl.AssignableRoleIds})
                .ToListAsync();
            var assignableRoleIds = _roleList.Where(wh => wh.AssignableRoleIds?.Count > 0)
                .SelectMany(sl => sl.AssignableRoleIds)
                .Distinct()
                .ToList();
            if (assignableRoleIds.Count > 0
                && roleIds.Except(assignableRoleIds)
                    .Any())
            {
                throw new UserFriendlyException("无权分配超出自身权限范围的角色！");
            }
        }

        var accountModel = await _repository.Queryable<AccountModel>()
            .Where(wh => wh.Mobile == input.Mobile)
            .SingleAsync();
        if (accountModel != null)
        {
            if (await _repository.Queryable<EmployeeModel>()
                    .AnyAsync(a => a.AccountId == accountModel.AccountId))
            {
                throw new UserFriendlyException("手机号已存在职员！");
            }
        }

        var employeeModel = new EmployeeModel
        {
            EmployeeId = YitIdHelper.NextId(),
            UserType = UserTypeEnum.None,
            EmployeeName = input.EmployeeName,
            Mobile = input.Mobile,
            // 新增默认正式员工
            Status = EmployeeStatusEnum.Formal,
            Email = input.Email,
            Sex = input.Sex,
            IdPhoto = input.IdPhoto,
            EntryDate = input.EntryDate,
            ResignDate = null,
            ResignReason = null,
            Nation = input.Nation,
            NativePlace = input.NativePlace,
            FamilyAddress = input.FamilyAddress,
            MailingAddress = input.MailingAddress,
            Birthday = input.Birthday,
            IdType = input.IdType,
            IdNumber = input.IdNumber,
            EducationLevel = input.EducationLevel,
            PoliticalStatus = input.PoliticalStatus,
            GraduationCollege = input.GraduationCollege,
            AcademicQualifications = input.AcademicQualifications,
            AcademicSystem = input.AcademicSystem,
            Degree = input.Degree,
            FamilyPhone = input.FamilyPhone,
            OfficePhone = input.OfficePhone,
            EmergencyContact = input.EmergencyContact,
            EmergencyPhone = input.EmergencyPhone,
            EmergencyAddress = input.EmergencyAddress,
            Remark = input.Remark
        };

        var employeeOrgModel = new EmployeeOrgModel
        {
            EmployeeId = employeeModel.EmployeeId,
            OrgId = organizationModel.OrgId,
            OrgName = organizationModel.OrgName,
            OrgNames = [.. organizationModel.ParentNames, organizationModel.OrgName],
            DepartmentId = departmentModel.DepartmentId,
            DepartmentName = departmentModel.DepartmentName,
            DepartmentNames = [.. departmentModel.ParentNames, departmentModel.DepartmentName],
            IsPrimary = true,
            PositionId = positionModel.PositionId,
            PositionName = positionModel.PositionName,
            JobLevelId = jobLevelModel.JobLevelId,
            JobLevelName = jobLevelModel.JobLevelName,
            IsPrincipal = input.IsPrincipal
        };

        var employeeRoleList = new List<EmployeeRoleModel>();
        foreach (var item in input.RoleList)
        {
            var roleModel = roleList.Single(s => s.RoleId == item.RoleId);
            employeeRoleList.Add(new EmployeeRoleModel
            {
                EmployeeId = employeeModel.EmployeeId, RoleId = roleModel.RoleId, RoleName = roleModel.RoleName
            });
        }

        await _repository.Ado.UseTranAsync(async () =>
        {
            if (accountModel == null)
            {
                accountModel = new AccountModel
                {
                    AccountId = YitIdHelper.NextId(),
                    Mobile = input.Mobile,
                    Password = CryptoUtil.SHA1Encrypt(CommonConst.Default.Password),
                    Status = CommonStatusEnum.Enable,
                    NickName = employeeModel.EmployeeName,
                    Avatar = employeeModel.IdPhoto,
                    Sex = GenderEnum.Unknown
                };
                await _repository.Insertable(accountModel)
                    .ExecuteCommandAsync();

                #region PasswordRecordModel

                // 初始化密码记录表
                await _repository.Insertable(new List<PasswordRecordModel>
                    {
                        new()
                        {
                            AccountId = accountModel.AccountId,
                            OperationType = PasswordOperationTypeEnum.Create,
                            Type = PasswordTypeEnum.SHA1,
                            Password = CryptoUtil.SHA1Encrypt(CommonConst.Default.Password)
                                .ToUpper()
                        }
                    })
                    .ExecuteCommandAsync();

                #endregion
            }

            employeeModel.AccountId = accountModel.AccountId;

            var employeeNo = SerialContext.GenEmployeeNo(_repository);
            employeeModel.EmployeeNo = employeeNo;
            await _repository.InsertAsync(employeeModel);

            // 如果当前职员是负责人，则清除该部门原有负责人
            if (employeeOrgModel.IsPrincipal)
            {
                await _repository.Updateable<EmployeeOrgModel>()
                    .SetColumns(_ => new EmployeeOrgModel {IsPrincipal = false})
                    .Where(wh => wh.DepartmentId == employeeOrgModel.DepartmentId)
                    .ExecuteCommandAsync();
            }

            await _repository.Insertable(employeeOrgModel)
                .ExecuteCommandAsync();

            // 删除旧的角色数据
            await _repository.Deleteable<EmployeeRoleModel>()
                .Where(wh => wh.EmployeeId == employeeModel.EmployeeId)
                .ExecuteCommandAsync();
            await _repository.Insertable(employeeRoleList)
                .ExecuteCommandAsync();
        }, ex => throw ex);

        // 操作日志
        LogContext.OperateLog(new OperateLogDto
        {
            Title = "添加职员",
            OperateType = OperateLogTypeEnum.Organization,
            BizId = employeeModel.EmployeeId,
            BizNo = employeeModel.EmployeeNo,
            Description =
                $"职员名称：{employeeModel.EmployeeName}，职员手机：{employeeModel.Mobile}，职员邮箱：{employeeModel.Email}，职员部门：{employeeOrgModel.DepartmentName}"
        });
    }

    /// <summary>
    /// 编辑本职员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑本职员", HttpRequestActionEnum.Edit)]
    public async Task EditSelfEmployee(EditEmployeeInput input)
    {
        if (await _repository.AnyAsync(a => a.Mobile == input.Mobile && a.EmployeeId != input.EmployeeId))
        {
            throw new UserFriendlyException("手机号重复！");
        }

        var employeeModel = await _repository.SingleOrDefaultAsync(_user.EmployeeId);
        if (employeeModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        employeeModel.EmployeeName = input.EmployeeName;
        employeeModel.Mobile = input.Mobile;
        employeeModel.Email = input.Email;
        employeeModel.Sex = input.Sex;
        employeeModel.IdPhoto = input.IdPhoto;
        employeeModel.Nation = input.Nation;
        employeeModel.NativePlace = input.NativePlace;
        employeeModel.FamilyAddress = input.FamilyAddress;
        employeeModel.MailingAddress = input.MailingAddress;
        employeeModel.Birthday = input.Birthday;
        employeeModel.IdType = input.IdType;
        employeeModel.IdNumber = input.IdNumber;
        employeeModel.FamilyPhone = input.FamilyPhone;
        employeeModel.OfficePhone = input.OfficePhone;
        employeeModel.EmergencyContact = input.EmergencyContact;
        employeeModel.EmergencyPhone = input.EmergencyPhone;
        employeeModel.EmergencyAddress = input.EmergencyAddress;

        await _repository.UpdateAsync(employeeModel);

        // 操作日志
        LogContext.OperateLog(new OperateLogDto
        {
            Title = "编辑本职员",
            OperateType = OperateLogTypeEnum.Organization,
            BizId = employeeModel.EmployeeId,
            BizNo = employeeModel.EmployeeNo,
            Description = $"编辑本职员：{employeeModel.EmployeeName}"
        });
    }

    /// <summary>
    /// 编辑职员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑职员", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Employee.Edit)]
    public async Task EditEmployee(EditEmployeeInput input)
    {
        if (await _repository.AnyAsync(a => a.Mobile == input.Mobile && a.EmployeeId != input.EmployeeId))
        {
            throw new UserFriendlyException("手机号重复！");
        }

        if (input.OrgList?.Count < 1)
        {
            throw new UserFriendlyException("请至少填写一个部门！");
        }

        if (input.OrgList.Count(c => c.IsPrimary) > 1)
        {
            throw new UserFriendlyException("只能存在一个主部门！");
        }

        if (input.RoleList.Select(sl => sl.RoleId)
                .Distinct()
                .Count()
            != input.RoleList.Count)
        {
            throw new UserFriendlyException("角色重复！");
        }

        var employeeModel = await _repository.SingleOrDefaultAsync(input.EmployeeId);
        if (employeeModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        if (employeeModel.Status == EmployeeStatusEnum.Resigned)
        {
            throw new UserFriendlyException("禁止修改已离职的职员资料！");
        }

        var orgIds = input.OrgList.Select(sl => sl.OrgId)
            .Distinct()
            .ToList();
        var organizationList = await _repository.Queryable<OrganizationModel>()
            .Where(wh => orgIds.Contains(wh.OrgId))
            .ToListAsync();
        if (organizationList.Count != orgIds.Count)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        var departmentIds = input.OrgList.Select(sl => sl.DepartmentId)
            .Distinct()
            .ToList();
        var departmentList = await _repository.Queryable<DepartmentModel>()
            .Where(wh => departmentIds.Contains(wh.DepartmentId))
            .ToListAsync();
        if (departmentList.Count != departmentIds.Count)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        var positionId = input.OrgList.Select(sl => sl.PositionId)
            .Distinct()
            .ToList();
        var positionList = await _repository.Queryable<PositionModel>()
            .Where(wh => positionId.Contains(wh.PositionId))
            .ToListAsync();
        if (positionList.Count != positionId.Count)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        var jobLevelId = input.OrgList.Select(sl => sl.JobLevelId)
            .Distinct()
            .ToList();
        var jobLevelList = await _repository.Queryable<JobLevelModel>()
            .Where(wh => jobLevelId.Contains(wh.JobLevelId))
            .ToListAsync();
        if (jobLevelList.Count != jobLevelId.Count)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        var roleIds = input.RoleList.Select(sl => sl.RoleId)
            .ToList();
        var roleList = await _repository.Queryable<RoleModel>()
            .Where(wh => roleIds.Contains(wh.RoleId))
            .ToListAsync();
        if (roleList.Count != roleIds.Count)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        if (!_user.IsSuperAdmin && !_user.IsAdmin)
        {
            var _roleIds = _user.RoleIdList ?? [];
            var _roleList = await _repository.Queryable<RoleModel>()
                .Where(wh => _roleIds.Contains(wh.RoleId))
                .Select(sl => new {sl.AssignableRoleIds})
                .ToListAsync();
            var assignableRoleIds = _roleList.Where(wh => wh.AssignableRoleIds?.Count > 0)
                .SelectMany(sl => sl.AssignableRoleIds)
                .Distinct()
                .ToList();
            if (assignableRoleIds.Count > 0
                && roleIds.Except(assignableRoleIds)
                    .Any())
            {
                throw new UserFriendlyException("无权分配超出自身权限范围的角色！");
            }
        }

        employeeModel.EmployeeName = input.EmployeeName;
        employeeModel.Mobile = input.Mobile;
        employeeModel.Email = input.Email;
        employeeModel.Sex = input.Sex;
        employeeModel.IdPhoto = input.IdPhoto;
        employeeModel.Nation = input.Nation;
        employeeModel.NativePlace = input.NativePlace;
        employeeModel.FamilyAddress = input.FamilyAddress;
        employeeModel.MailingAddress = input.MailingAddress;
        employeeModel.Birthday = input.Birthday;
        employeeModel.IdType = input.IdType;
        employeeModel.IdNumber = input.IdNumber;
        employeeModel.FamilyPhone = input.FamilyPhone;
        employeeModel.OfficePhone = input.OfficePhone;
        employeeModel.EmergencyContact = input.EmergencyContact;
        employeeModel.EmergencyPhone = input.EmergencyPhone;
        employeeModel.EmergencyAddress = input.EmergencyAddress;

        var employeeOrgList = new List<EmployeeOrgModel>();
        var employeeRoleList = new List<EmployeeRoleModel>();
        if (employeeModel.EmployeeId != _user.EmployeeId)
        {
            employeeModel.EntryDate = input.EntryDate;
            employeeModel.EducationLevel = input.EducationLevel;
            employeeModel.PoliticalStatus = input.PoliticalStatus;
            employeeModel.GraduationCollege = input.GraduationCollege;
            employeeModel.AcademicQualifications = input.AcademicQualifications;
            employeeModel.AcademicSystem = input.AcademicSystem;
            employeeModel.Degree = input.Degree;
            employeeModel.Remark = input.Remark;

            foreach (var item in input.OrgList)
            {
                var organizationModel = organizationList.Single(s => s.OrgId == item.OrgId);
                var departmentModel = departmentList.Single(s => s.DepartmentId == item.DepartmentId);
                var positionModel = positionList.Single(s => s.PositionId == item.PositionId);
                var jobLevelModel = jobLevelList.Single(s => s.JobLevelId == item.JobLevelId);

                employeeOrgList.Add(new EmployeeOrgModel
                {
                    EmployeeId = employeeModel.EmployeeId,
                    OrgId = organizationModel.OrgId,
                    OrgName = organizationModel.OrgName,
                    OrgNames = [.. organizationModel.ParentNames, organizationModel.OrgName],
                    DepartmentId = departmentModel.DepartmentId,
                    DepartmentName = departmentModel.DepartmentName,
                    DepartmentNames = [.. departmentModel.ParentNames, departmentModel.DepartmentName],
                    IsPrimary = item.IsPrimary,
                    PositionId = positionModel.PositionId,
                    PositionName = positionModel.PositionName,
                    JobLevelId = jobLevelModel.JobLevelId,
                    JobLevelName = jobLevelModel.JobLevelName,
                    IsPrincipal = item.IsPrincipal
                });
            }

            foreach (var item in input.RoleList)
            {
                var roleModel = roleList.Single(s => s.RoleId == item.RoleId);
                employeeRoleList.Add(new EmployeeRoleModel
                {
                    EmployeeId = employeeModel.EmployeeId, RoleId = roleModel.RoleId, RoleName = roleModel.RoleName
                });
            }
        }

        await _repository.Ado.UseTranAsync(async () =>
        {
            if (employeeModel.EmployeeId != _user.EmployeeId)
            {
                // 删除旧的部门数据
                await _repository.Deleteable<EmployeeOrgModel>()
                    .Where(wh => wh.EmployeeId == employeeModel.EmployeeId)
                    .ExecuteCommandAsync();
                // 删除旧的角色数据
                await _repository.Deleteable<EmployeeRoleModel>()
                    .Where(wh => wh.EmployeeId == employeeModel.EmployeeId)
                    .ExecuteCommandAsync();

                // 处理部门负责人
                var principalDepartmentIds = employeeOrgList.Where(wh => wh.IsPrincipal)
                    .Select(sl => sl.DepartmentId)
                    .ToList();
                if (principalDepartmentIds.Any())
                {
                    await _repository.Updateable<EmployeeOrgModel>()
                        .SetColumns(_ => new EmployeeOrgModel {IsPrincipal = false})
                        .Where(wh => principalDepartmentIds.Contains(wh.DepartmentId))
                        .ExecuteCommandAsync();
                }

                await _repository.Insertable(employeeOrgList)
                    .ExecuteCommandAsync();
                await _repository.Insertable(employeeRoleList)
                    .ExecuteCommandAsync();
            }

            await _repository.UpdateAsync(employeeModel);
        }, ex => throw ex);

        // 操作日志
        LogContext.OperateLog(new OperateLogDto
        {
            Title = "编辑职员",
            OperateType = OperateLogTypeEnum.Organization,
            BizId = employeeModel.EmployeeId,
            BizNo = employeeModel.EmployeeNo,
            Description = $"编辑职员：{employeeModel.EmployeeName}"
        });
    }

    /// <summary>
    /// 职员更改状态
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("职员更改状态", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Employee.Status)]
    public async Task ChangeStatus(ChangeStatusInput input)
    {
        if (input.Status == EmployeeStatusEnum.Resigned)
        {
            throw new UserFriendlyException("禁止直接修改为离职状态！");
        }

        var employeeModel = await _repository.SingleOrDefaultAsync(input.EmployeeId);
        if (employeeModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        if (employeeModel.ResignDate != null)
        {
            employeeModel.ResignDate = null;
            employeeModel.ResignReason = null;
        }

        employeeModel.Status = input.Status;
        employeeModel.RowVersion = input.RowVersion;

        await _repository.Ado.UseTranAsync(async () =>
        {
            var accountModel = await _repository.Queryable<AccountModel>()
                .SingleAsync(s => s.AccountId == employeeModel.AccountId);
            if (accountModel != null)
            {
                accountModel.Status = CommonStatusEnum.Enable;
                await _repository.Updateable(accountModel)
                    .ExecuteCommandAsync();
            }

            await _repository.UpdateAsync(employeeModel);
        }, ex => throw ex);

        // 操作日志
        LogContext.OperateLog(new OperateLogDto
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
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("职员离职", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Employee.Status)]
    public async Task EmployeeResigned(EmployeeResignedInput input)
    {
        var employeeModel = await _repository.SingleOrDefaultAsync(input.EmployeeId);
        if (employeeModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        employeeModel.Status = EmployeeStatusEnum.Resigned;
        employeeModel.ResignDate = input.ResignDate;
        employeeModel.ResignReason = input.ResignReason;
        employeeModel.RowVersion = input.RowVersion;

        await _repository.Ado.UseTranAsync(async () =>
        {
            var accountModel = await _repository.Queryable<AccountModel>()
                .SingleAsync(s => s.AccountId == employeeModel.AccountId);
            if (accountModel != null)
            {
                accountModel.Status = CommonStatusEnum.Disable;
                await _repository.Updateable(accountModel)
                    .ExecuteCommandAsync();

                // 强制下线在线用户
                var _hubContext = FastContext.HttpContext.RequestServices.GetService<IHubContext<ChatHub, IChatClient>>();

                var connectionId = await _repository.Queryable<OnlineUserModel>()
                    .Where(wh => wh.IsOnline)
                    .Where(wh => wh.EmployeeId == employeeModel.EmployeeId)
                    .Select(sl => sl.ConnectionId)
                    .SingleAsync();

                if (!string.IsNullOrWhiteSpace(connectionId))
                {
                    await _hubContext.Clients.Clients(connectionId)
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

            await _repository.UpdateAsync(employeeModel);
        }, ex => throw ex);

        // 操作日志
        LogContext.OperateLog(new OperateLogDto
        {
            Title = "职员离职",
            OperateType = OperateLogTypeEnum.Organization,
            BizId = employeeModel.EmployeeId,
            BizNo = employeeModel.EmployeeNo,
            Description = $"职员：{employeeModel.EmployeeName}，离职 -> {employeeModel.ResignDate:yyyy-MM-dd HH:mm:ss}"
        });
    }
}