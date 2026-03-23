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
using Fast.Admin.Service.Department.Dto;
using Fast.AdminLog.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Service.Department;

/// <summary>
/// <see cref="DepartmentService"/> 部门服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Admin, Name = "department")]
public class DepartmentService : IDynamicApplication
{
    private readonly IUser _user;
    private readonly ISqlSugarRepository<DepartmentModel> _repository;

    public DepartmentService(IUser user, ISqlSugarRepository<DepartmentModel> repository)
    {
        _user = user;
        _repository = repository;
    }

    /// <summary>
    /// 部门选择器
    /// </summary>
    /// <param name="orgId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("部门选择器", HttpRequestActionEnum.Query)]
    public async Task<List<ElSelectorOutput<long>>> DepartmentSelector(long? orgId)
    {
        var queryable = _repository.Entities.WhereIF(orgId != null, wh => wh.OrgId == orgId);

        // 管理员，全部权限
        if (_user.IsSuperAdmin || _user.IsAdmin || _user.DataScopeType == DataScopeTypeEnum.All)
        {
        }
        // 本机构及以下数据
        else if (_user.DataScopeType == DataScopeTypeEnum.OrgWithChild)
        {
            queryable = queryable.Where(wh => wh.DataPublic
                                              || wh.OrgId
                                              == SqlFunc.Subqueryable<EmployeeOrgModel>()
                                                  // 主部门
                                                  .Where(e => e.EmployeeId == _user.EmployeeId && e.IsPrimary)
                                                  .Where(e => e.OrgId == wh.OrgId)
                                                  .Select(e => e.OrgId));
        }
        // 本部门及以下数据
        else if (_user.DataScopeType == DataScopeTypeEnum.DeptWithChild)
        {
            queryable = queryable.Where(wh =>
                wh.DataPublic || wh.DepartmentId == _user.DepartmentId || wh.ParentIds.Contains(_user.DepartmentId ?? 0));
        }
        // 本部门数据或仅本人数据
        else
        {
            queryable = queryable.Where(wh => wh.DepartmentId == _user.DepartmentId);
        }

        var data = await queryable.OrderBy(ob => ob.Sort)
            .Select(sl => new
            {
                sl.DepartmentId,
                sl.OrgId,
                sl.OrgName,
                sl.ParentId,
                sl.ParentName,
                sl.ParentIds,
                sl.ParentNames,
                sl.DepartmentName,
                sl.DepartmentCode,
                sl.Contacts,
                sl.Phone
            })
            .ToListAsync();

        return data.Select(sl => new ElSelectorOutput<long>
            {
                Value = sl.DepartmentId,
                Label = sl.DepartmentName,
                ParentId = sl.ParentId,
                Data = new
                {
                    sl.OrgId,
                    sl.OrgName,
                    sl.ParentName,
                    sl.ParentIds,
                    sl.ParentNames,
                    sl.DepartmentCode,
                    sl.Contacts,
                    sl.Phone
                }
            })
            .ToList()
            .Build();
    }

    /// <summary>
    /// 获取部门列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取部门列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.Department.Paged)]
    public async Task<List<QueryDepartmentPagedOutput>> QueryDepartmentPaged(QueryDepartmentPagedInput input)
    {
        var queryable = _repository.Entities.WhereIF(!string.IsNullOrWhiteSpace(input.SearchValue),
                wh => wh.DepartmentName.Contains(input.SearchValue) || wh.DepartmentCode.Contains(input.SearchValue))
            .WhereIF(input.OrgId != null, wh => wh.OrgId == input.OrgId);

        // 管理员，全部权限
        if (_user.IsSuperAdmin || _user.IsAdmin || _user.DataScopeType == DataScopeTypeEnum.All)
        {
        }
        // 本机构及以下数据
        else if (_user.DataScopeType == DataScopeTypeEnum.OrgWithChild)
        {
            queryable = queryable.Where(wh => wh.DataPublic
                                              || wh.OrgId
                                              == SqlFunc.Subqueryable<EmployeeOrgModel>()
                                                  // 主部门
                                                  .Where(e => e.EmployeeId == _user.EmployeeId && e.IsPrimary)
                                                  .Where(e => e.OrgId == wh.OrgId)
                                                  .Select(e => e.OrgId));
        }
        // 本部门及以下数据
        else if (_user.DataScopeType == DataScopeTypeEnum.DeptWithChild)
        {
            queryable = queryable.Where(wh =>
                wh.DataPublic || wh.DepartmentId == _user.DepartmentId || wh.ParentIds.Contains(_user.DepartmentId ?? 0));
        }
        // 本部门数据或仅本人数据
        else
        {
            queryable = queryable.Where(wh => wh.DepartmentId == _user.DepartmentId);
        }

        var data = await queryable.OrderByIF(input.IsOrderBy, ob => ob.Sort)
            .Select(sl => new QueryDepartmentPagedOutput
            {
                DepartmentId = sl.DepartmentId,
                OrgId = sl.OrgId,
                OrgName = sl.OrgName,
                ParentId = sl.ParentId,
                ParentName = sl.ParentName,
                ParentIds = sl.ParentIds,
                ParentNames = sl.ParentNames,
                DepartmentName = sl.DepartmentName,
                DepartmentCode = sl.DepartmentCode,
                Contacts = sl.Contacts,
                Phone = sl.Phone,
                Email = sl.Email,
                Sort = sl.Sort,
                DataPublic = sl.DataPublic,
                Remark = sl.Remark,
                CreatedUserName = sl.CreatedUserName,
                CreatedTime = sl.CreatedTime,
                UpdatedUserName = sl.UpdatedUserName,
                UpdatedTime = sl.UpdatedTime,
                RowVersion = sl.RowVersion
            })
            .SugarPaged(input)
            .ToListAsync();

        return new TreeBuildUtil<QueryDepartmentPagedOutput, long>().Build(data);
    }

    /// <summary>
    /// 获取部门详情
    /// </summary>
    /// <param name="departmentId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取部门详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Department.Detail)]
    public async Task<QueryDepartmentDetailOutput> QueryDepartmentDetail([Required(ErrorMessage = "部门Id不能为空")] long? departmentId)
    {
        var result = await _repository.Entities.Where(wh => wh.DepartmentId == departmentId)
            .Select(sl => new QueryDepartmentDetailOutput
            {
                DepartmentId = sl.DepartmentId,
                OrgId = sl.OrgId,
                OrgName = sl.OrgName,
                ParentId = sl.ParentId,
                ParentName = sl.ParentName,
                ParentIds = sl.ParentIds,
                ParentNames = sl.ParentNames,
                DepartmentName = sl.DepartmentName,
                DepartmentCode = sl.DepartmentCode,
                Contacts = sl.Contacts,
                Phone = sl.Phone,
                Email = sl.Email,
                Sort = sl.Sort,
                DataPublic = sl.DataPublic,
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

        return result;
    }

    /// <summary>
    /// 添加部门
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加部门", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.Department.Add)]
    public async Task AddDepartment(AddDepartmentInput input)
    {
        if (await _repository.AnyAsync(a => a.DepartmentName == input.DepartmentName))
        {
            throw new UserFriendlyException("部门名称重复！");
        }

        if (await _repository.AnyAsync(a => a.DepartmentCode == input.DepartmentCode))
        {
            throw new UserFriendlyException("部门编码重复！");
        }

        var organizationModel = await _repository.Queryable<OrganizationModel>()
            .SingleAsync(s => s.OrgId == input.OrgId);

        if (organizationModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        var departmentModel = new DepartmentModel
        {
            OrgId = organizationModel.OrgId,
            OrgName = organizationModel.OrgName,
            DepartmentName = input.DepartmentName,
            DepartmentCode = input.DepartmentCode,
            Contacts = input.Contacts,
            Phone = input.Phone,
            Email = input.Email,
            Sort = input.Sort,
            DataPublic = input.DataPublic,
            Remark = input.Remark
        };

        if (input.ParentId > 0)
        {
            var parentDepartment = await _repository.SingleOrDefaultAsync(s => s.DepartmentId == input.ParentId);
            if (parentDepartment == null)
            {
                throw new UserFriendlyException("数据不存在！");
            }

            departmentModel.ParentId = parentDepartment.DepartmentId;
            departmentModel.ParentName = parentDepartment.DepartmentName;
            departmentModel.ParentIds = [.. parentDepartment.ParentIds, parentDepartment.DepartmentId];
            departmentModel.ParentNames = [.. parentDepartment.ParentNames, parentDepartment.DepartmentName];
        }
        else
        {
            departmentModel.ParentId = 0;
            departmentModel.ParentName = null;
            departmentModel.ParentIds = [0];
            departmentModel.ParentNames = [];
        }

        await _repository.InsertAsync(departmentModel);

        // 操作日志
        LogContext.OperateLog(new OperateLogDto
        {
            Title = "添加部门",
            OperateType = OperateLogTypeEnum.Organization,
            BizId = departmentModel.DepartmentId,
            BizNo = null,
            Description = $"添加部门：{departmentModel.DepartmentName}"
        });
    }

    /// <summary>
    /// 编辑部门
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑部门", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Department.Edit)]
    public async Task EditDepartment(EditDepartmentInput input)
    {
        if (input.ParentId == input.DepartmentId)
        {
            throw new UserFriendlyException("不能将自己设为父部门！");
        }

        if (await _repository.AnyAsync(a => a.DepartmentName == input.DepartmentName && a.DepartmentId != input.DepartmentId))
        {
            throw new UserFriendlyException("部门名称重复！");
        }

        if (await _repository.AnyAsync(a => a.DepartmentCode == input.DepartmentCode && a.DepartmentId != input.DepartmentId))
        {
            throw new UserFriendlyException("部门编码重复！");
        }

        var departmentModel = await _repository.SingleOrDefaultAsync(input.DepartmentId);
        if (departmentModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        var organizationModel = await _repository.Queryable<OrganizationModel>()
            .SingleAsync(s => s.OrgId == input.OrgId);

        if (organizationModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        var description = $"编辑部门：{input.DepartmentName}";

        departmentModel.OrgId = organizationModel.OrgId;
        departmentModel.OrgName = organizationModel.OrgName;
        departmentModel.DepartmentName = input.DepartmentName;
        departmentModel.DepartmentCode = input.DepartmentCode;
        departmentModel.Contacts = input.Contacts;
        departmentModel.Phone = input.Phone;
        departmentModel.Email = input.Email;
        departmentModel.Sort = input.Sort;
        departmentModel.DataPublic = input.DataPublic;
        departmentModel.Remark = input.Remark;
        departmentModel.RowVersion = input.RowVersion;

        if (input.ParentId > 0)
        {
            var parentDepartment = await _repository.SingleOrDefaultAsync(s => s.DepartmentId == input.ParentId);
            if (parentDepartment == null)
            {
                throw new UserFriendlyException("数据不存在！");
            }

            if (departmentModel.ParentId != input.ParentId)
            {
                description += $"父级部门 -> {parentDepartment.DepartmentName}";
            }

            departmentModel.ParentId = parentDepartment.DepartmentId;
            departmentModel.ParentName = parentDepartment.DepartmentName;
            departmentModel.ParentIds = [.. parentDepartment.ParentIds, parentDepartment.DepartmentId];
            departmentModel.ParentNames = [.. parentDepartment.ParentNames, parentDepartment.DepartmentName];
        }
        else
        {
            if (departmentModel.ParentId != input.ParentId)
            {
                description += "删除父级部门";
            }

            departmentModel.ParentId = 0;
            departmentModel.ParentName = null;
            departmentModel.ParentIds = [0];
            departmentModel.ParentNames = [];
        }

        await _repository.UpdateAsync(departmentModel);

        // 更新所有子级
        var childrenList = await _repository.Entities.Where(wh => wh.ParentIds.Contains(departmentModel.DepartmentId))
            .ToListAsync();

        void updateChildrenName(long parentId)
        {
            var parentModel = childrenList.Single(s => s.DepartmentId == parentId);
            foreach (var item in childrenList.Where(wh => wh.ParentId == parentId))
            {
                item.ParentIds = [.. parentModel.ParentIds, parentModel.DepartmentId];
                item.ParentNames = [.. parentModel.ParentNames, parentModel.DepartmentName];
                updateChildrenName(item.DepartmentId);
            }
        }

        // 处理顶层
        foreach (var item in childrenList.Where(wh => wh.ParentId == departmentModel.DepartmentId))
        {
            item.ParentName = departmentModel.DepartmentName;
            item.ParentIds = [.. departmentModel.ParentIds, departmentModel.DepartmentId];
            item.ParentNames = [.. departmentModel.ParentNames, departmentModel.DepartmentName];

            updateChildrenName(item.DepartmentId);
        }

        await _repository.Updateable(childrenList)
            .UpdateColumns(e => new {e.ParentName, e.ParentIds, e.ParentNames})
            .ExecuteCommandAsync();

        await _repository.Updateable<EmployeeOrgModel>()
            .SetColumns(_ => new EmployeeOrgModel
            {
                DepartmentName = departmentModel.DepartmentName,
                DepartmentNames = new List<string>(departmentModel.ParentNames) {departmentModel.DepartmentName}
            })
            .Where(wh => wh.DepartmentId == departmentModel.DepartmentId)
            .ExecuteCommandAsync();

        // 操作日志
        LogContext.OperateLog(new OperateLogDto
        {
            Title = "编辑部门",
            OperateType = OperateLogTypeEnum.Organization,
            BizId = departmentModel.DepartmentId,
            BizNo = null,
            Description = description
        });
    }

    /// <summary>
    /// 删除部门
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除部门", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.Department.Delete)]
    public async Task DeleteDepartment(DepartmentIdInput input)
    {
        if (await _repository.AnyAsync(a => a.ParentId == input.DepartmentId))
        {
            throw new UserFriendlyException("部门存在子部门，无法删除！");
        }

        // 检查是否有员工关联
        if (await _repository.Queryable<EmployeeOrgModel>()
                .AnyAsync(a => a.DepartmentId == input.DepartmentId))
        {
            throw new UserFriendlyException("部门存在员工关联，无法删除！");
        }

        var departmentModel = await _repository.SingleOrDefaultAsync(input.DepartmentId);
        if (departmentModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        await _repository.DeleteAsync(departmentModel);

        // 操作日志
        LogContext.OperateLog(new OperateLogDto
        {
            Title = "删除部门",
            OperateType = OperateLogTypeEnum.Organization,
            BizId = departmentModel.DepartmentId,
            BizNo = null,
            Description = $"删除部门：{departmentModel.DepartmentName}"
        });
    }
}