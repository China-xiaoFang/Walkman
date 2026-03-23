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

using Dm.util;
using Fast.Admin.Entity;
using Fast.Admin.Enum;
using Fast.Admin.Service.Role.Dto;
using Fast.AdminLog.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Service.Role;

/// <summary>
/// <see cref="RoleService"/> 角色服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Admin, Name = "role")]
public class RoleService : IDynamicApplication
{
    private readonly IUser _user;
    private readonly ISqlSugarRepository<RoleModel> _repository;

    public RoleService(IUser user, ISqlSugarRepository<RoleModel> repository)
    {
        _user = user;
        _repository = repository;
    }

    /// <summary>
    /// 角色选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("角色选择器", HttpRequestActionEnum.Query)]
    public async Task<List<ElSelectorOutput<long>>> RoleSelector()
    {
        var queryable = _repository.Entities;
        if (!_user.IsSuperAdmin && !_user.IsAdmin)
        {
            var roleIds = _user.RoleIdList ?? [];
            var roleList = await _repository.Queryable<RoleModel>()
                .Where(wh => roleIds.Contains(wh.RoleId))
                .Select(sl => new {sl.AssignableRoleIds})
                .ToListAsync();
            var assignableRoleIds = roleList.Where(wh => wh.AssignableRoleIds?.Count > 0)
                .SelectMany(sl => sl.AssignableRoleIds)
                .Distinct()
                .ToList();

            // 如果没有可分配的角色，则代表可以分配全部
            queryable = queryable.WhereIF(assignableRoleIds.Count > 0, wh => assignableRoleIds.Contains(wh.RoleId));
        }

        var data = await queryable.OrderBy(ob => ob.Sort)
            .Select(sl => new {sl.RoleId, sl.RoleName, sl.RoleCode})
            .ToListAsync();

        return data.Select(sl => new ElSelectorOutput<long> {Value = sl.RoleId, Label = sl.RoleName, Data = new {sl.RoleCode}})
            .ToList();
    }

    /// <summary>
    /// 获取角色分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取角色分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.Role.Paged)]
    public async Task<PagedResult<QueryRolePagedOutput>> QueryRolePaged(QueryRolePagedInput input)
    {
        return await _repository.Entities.WhereIF(input.RoleType != null, wh => wh.RoleType == input.RoleType)
            .WhereIF(input.DataScopeType != null, wh => wh.DataScopeType == input.DataScopeType)
            .OrderByIF(input.IsOrderBy, ob => ob.Sort)
            .Select(sl => new QueryRolePagedOutput
            {
                RoleId = sl.RoleId,
                RoleType = sl.RoleType,
                IsSystemMenu = sl.IsSystemMenu,
                RoleName = sl.RoleName,
                RoleCode = sl.RoleCode,
                Sort = sl.Sort,
                DataScopeType = sl.DataScopeType,
                Remark = sl.Remark,
                DepartmentName = sl.DepartmentName,
                CreatedUserName = sl.CreatedUserName,
                CreatedTime = sl.CreatedTime,
                UpdatedUserName = sl.UpdatedUserName,
                UpdatedTime = sl.UpdatedTime,
                RowVersion = sl.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取角色详情
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取角色详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Role.Detail)]
    public async Task<QueryRoleDetailOutput> QueryRoleDetail([Required(ErrorMessage = "角色Id不能为空")] long? roleId)
    {
        var result = await _repository.Entities.Where(wh => wh.RoleId == roleId)
            .Select(sl => new QueryRoleDetailOutput
            {
                RoleId = sl.RoleId,
                RoleType = sl.RoleType,
                IsSystemMenu = sl.IsSystemMenu,
                RoleName = sl.RoleName,
                RoleCode = sl.RoleCode,
                Sort = sl.Sort,
                DataScopeType = sl.DataScopeType,
                AssignableRoleIds = sl.AssignableRoleIds,
                Remark = sl.Remark,
                DepartmentName = sl.DepartmentName,
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
    /// 添加角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加角色", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.Role.Add)]
    public async Task AddRole(AddRoleInput input)
    {
        if (await _repository.AnyAsync(a => a.RoleName == input.RoleName))
        {
            throw new UserFriendlyException("角色名称重复！");
        }

        if (await _repository.AnyAsync(a => a.RoleCode == input.RoleCode))
        {
            throw new UserFriendlyException("角色编码重复！");
        }

        var roleModel = new RoleModel
        {
            RoleType = input.RoleType,
            IsSystemMenu = input.IsSystemMenu,
            RoleName = input.RoleName,
            RoleCode = input.RoleCode,
            Sort = input.Sort,
            DataScopeType = input.DataScopeType,
            AssignableRoleIds = input.AssignableRoleIds,
            Remark = input.Remark
        };

        await _repository.InsertAsync(roleModel);

        // 操作日志
        LogContext.OperateLog(new OperateLogDto
        {
            Title = "添加角色",
            OperateType = OperateLogTypeEnum.Organization,
            BizId = roleModel.RoleId,
            BizNo = null,
            Description = $"添加角色：{roleModel.RoleName}"
        });
    }

    /// <summary>
    /// 编辑角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑角色", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Role.Edit)]
    public async Task EditRole(EditRoleInput input)
    {
        if (await _repository.AnyAsync(a => a.RoleName == input.RoleName && a.RoleId != input.RoleId))
        {
            throw new UserFriendlyException("角色名称重复！");
        }

        if (await _repository.AnyAsync(a => a.RoleCode == input.RoleCode && a.RoleId != input.RoleId))
        {
            throw new UserFriendlyException("角色编码重复！");
        }

        var roleModel = await _repository.SingleOrDefaultAsync(input.RoleId);
        if (roleModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        roleModel.RoleType = input.RoleType;
        roleModel.IsSystemMenu = input.IsSystemMenu;
        roleModel.RoleName = input.RoleName;
        roleModel.RoleCode = input.RoleCode;
        roleModel.Sort = input.Sort;
        roleModel.DataScopeType = input.DataScopeType;
        roleModel.AssignableRoleIds = input.AssignableRoleIds;
        roleModel.Remark = input.Remark;
        roleModel.RowVersion = input.RowVersion;

        await _repository.UpdateAsync(roleModel);

        await _repository.Updateable<EmployeeRoleModel>()
            .SetColumns(_ => new EmployeeRoleModel {RoleName = roleModel.RoleName})
            .Where(wh => wh.RoleId == roleModel.RoleId)
            .ExecuteCommandAsync();

        // 操作日志
        LogContext.OperateLog(new OperateLogDto
        {
            Title = "编辑角色",
            OperateType = OperateLogTypeEnum.Organization,
            BizId = roleModel.RoleId,
            BizNo = null,
            Description = $"编辑角色：{roleModel.RoleName}"
        });
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除角色", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.Role.Delete)]
    public async Task DeleteRole(RoleIdInput input)
    {
        // 检查是否有职员关联
        if (await _repository.Queryable<EmployeeRoleModel>()
                .AnyAsync(a => a.RoleId == input.RoleId))
        {
            throw new UserFriendlyException("角色存在职员关联，无法删除！");
        }

        var roleModel = await _repository.SingleOrDefaultAsync(input.RoleId);
        if (roleModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        await _repository.Ado.UseTranAsync(async () =>
        {
            // 删除角色菜单关联
            await _repository.Deleteable<RoleMenuModel>()
                .Where(wh => wh.RoleId == input.RoleId)
                .ExecuteCommandAsync();

            // 删除角色按钮关联
            await _repository.Deleteable<RoleButtonModel>()
                .Where(wh => wh.RoleId == input.RoleId)
                .ExecuteCommandAsync();

            // 删除角色
            await _repository.DeleteAsync(roleModel);
        }, ex => throw ex);

        // 操作日志
        LogContext.OperateLog(new OperateLogDto
        {
            Title = "删除角色",
            OperateType = OperateLogTypeEnum.Organization,
            BizId = roleModel.RoleId,
            BizNo = null,
            Description = $"删除角色：{roleModel.RoleName}"
        });
    }

    /// <summary>
    /// 角色授权
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("角色授权", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Role.Edit)]
    public async Task RoleAuth(RoleAuthInput input)
    {
        var roleModel = await _repository.SingleOrDefaultAsync(input.RoleId);
        if (roleModel == null)
        {
            throw new UserFriendlyException("角色不存在！");
        }

        // 验证菜单是否都存在
        var menuIds = input.MenuIds ?? [];
        if (menuIds.Any())
        {
            if (await _repository.Queryable<MenuModel>()
                    .Where(wh => menuIds.Contains(wh.MenuId))
                    .Select(sl => sl.MenuId)
                    .CountAsync()
                != menuIds.Count)
            {
                throw new UserFriendlyException("授权菜单数据不存在！");
            }
        }

        // 验证按钮是否都存在
        var buttonIds = input.ButtonIds ?? [];
        if (buttonIds.Any())
        {
            var existButtonIds = await _repository.Queryable<ButtonModel>()
                .Where(wh => buttonIds.Contains(wh.ButtonId))
                .Select(sl => sl.ButtonId)
                .ToListAsync();

            if (existButtonIds.Count != buttonIds.Count)
            {
                throw new UserFriendlyException("授权按钮数据不存在！");
            }
        }

        await _repository.Ado.UseTranAsync(async () =>
        {
            // 删除旧的菜单权限
            await _repository.Deleteable<RoleMenuModel>()
                .Where(wh => wh.RoleId == roleModel.RoleId)
                .ExecuteCommandAsync();

            // 添加新的菜单权限
            if (menuIds.Any())
            {
                await _repository.Insertable(menuIds
                        .Select(menuId => new RoleMenuModel {RoleId = roleModel.RoleId, MenuId = menuId})
                        .ToList())
                    .ExecuteCommandAsync();
            }

            // 删除旧的按钮权限
            await _repository.Deleteable<RoleButtonModel>()
                .Where(wh => wh.RoleId == roleModel.RoleId)
                .ExecuteCommandAsync();

            // 添加新的按钮权限
            if (buttonIds.Any())
            {
                await _repository.Insertable(buttonIds
                        .Select(buttonId => new RoleButtonModel {RoleId = roleModel.RoleId, ButtonId = buttonId})
                        .ToList())
                    .ExecuteCommandAsync();
            }
        }, ex => throw ex);

        // 操作日志
        LogContext.OperateLog(new OperateLogDto
        {
            Title = "角色授权",
            OperateType = OperateLogTypeEnum.Organization,
            BizId = roleModel.RoleId,
            BizNo = null,
            Description = $"角色授权：{roleModel.RoleName}"
        });
    }

    /// <summary>
    /// 获取角色授权菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取角色授权菜单", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Role.Edit)]
    public async Task<RoleAuthInput> QueryRoleAuthMenu(RoleIdInput input)
    {
        var roleModel = await _repository.SingleOrDefaultAsync(input.RoleId);
        if (roleModel == null)
        {
            throw new UserFriendlyException("角色不存在！");
        }

        var result = new RoleAuthInput
        {
            RoleId = roleModel.RoleId,
            RoleName = roleModel.RoleName,
            RowVersion = roleModel.RowVersion,
            MenuIds = await _repository.Queryable<RoleMenuModel>()
                .Where(wh => wh.RoleId == roleModel.RoleId)
                .Select(sl => sl.MenuId)
                .ToListAsync(),
            ButtonIds = await _repository.Queryable<RoleButtonModel>()
                .Where(wh => wh.RoleId == roleModel.RoleId)
                .Select(sl => sl.ButtonId)
                .ToListAsync()
        };

        return result;
    }

    /// <summary>
    /// 获取授权菜单
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取授权菜单", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Role.Edit, PermissionConst.Employee.Edit)]
    public async Task<List<ElSelectorOutput<long>>> QueryAuthMenu()
    {
        // 查询应用信息
        var applicationModel = await ApplicationContext.GetApplication(GlobalContext.Origin);

        if (applicationModel.AppType != GlobalContext.DeviceType)
        {
            throw new UserFriendlyException("应用类型不匹配！");
        }

        // 查询角色
        var roleIds = _user.RoleIdList ?? [];
        var roleList = await _repository.Queryable<RoleModel>()
            .Where(wh => roleIds.Contains(wh.RoleId))
            .Distinct()
            .ToListAsync();
        // 系统菜单角色类型
        var systemMenuRoleType = roleList.Where(wh => wh.IsSystemMenu)
            .Select(sl => sl.RoleType)
            .Aggregate(default(RoleTypeEnum), (acc, item) => acc | item);
        // 自定义菜单角色
        var customMenuRoleIds = roleList.Where(wh => !wh.IsSystemMenu)
            .Select(sl => sl.RoleId)
            .ToList();

        var menuQueryable = _repository.Queryable<MenuModel>()
            .Where(wh => wh.Status == CommonStatusEnum.Enable)
            .Where(wh => wh.MenuType != MenuTypeEnum.Catalog);

        var buttonQueryable = _repository.Queryable<ButtonModel>()
            .Where(wh => wh.Status == CommonStatusEnum.Enable);

        if (!_user.IsSuperAdmin && !_user.IsAdmin)
        {
            // 查询当前用户角色对应的菜单Id
            var roleMenuIds = await _repository.Queryable<RoleMenuModel>()
                .Where(wh => customMenuRoleIds.Contains(wh.RoleId))
                .Select(sl => sl.MenuId)
                .ToListAsync();
            menuQueryable = menuQueryable.Where(wh => (wh.RoleType & systemMenuRoleType) != 0 || roleMenuIds.Contains(wh.MenuId));

            // 查询当前用户角色对应的按钮Id
            var roleButtonIds = await _repository.Queryable<RoleButtonModel>()
                .Where(wh => customMenuRoleIds.Contains(wh.RoleId))
                .Select(sl => sl.ButtonId)
                .ToListAsync();
            buttonQueryable =
                buttonQueryable.Where(wh => (wh.RoleType & systemMenuRoleType) != 0 || roleButtonIds.Contains(wh.ButtonId));
        }

        // 查询所有菜单
        var menuList = await menuQueryable.Clone()
            .OrderBy(ob => ob.Sort)
            .Select(sl => new
            {
                sl.MenuId,
                sl.MenuName,
                sl.HasMobile,
                sl.HasWeb,
                sl.HasDesktop
            })
            .ToListAsync();

        // 查询所有按钮
        var buttonList = await buttonQueryable.Clone()
            .InnerJoin(menuQueryable.Clone(), (t1, t2) => t1.MenuId == t2.MenuId)
            .OrderBy(t1 => t1.Sort)
            .Select(t1 => new
            {
                t1.ButtonId,
                t1.MenuId,
                t1.ButtonName,
                t1.HasMobile,
                t1.HasWeb,
                t1.HasDesktop
            })
            .ToListAsync();

        var result = new List<ElSelectorOutput<long>>();

        foreach (var menuInfo in menuList.ToList())
        {
            var item = new ElSelectorOutput<long>
            {
                Value = menuInfo.MenuId,
                Label = menuInfo.MenuName,
                Data = new {menuInfo.HasMobile, menuInfo.HasWeb, menuInfo.HasDesktop},
                Children = []
            };
            foreach (var buttonInfo in buttonList.Where(wh => wh.MenuId == menuInfo.MenuId)
                         .ToList())
            {
                item.Children.Add(new ElSelectorOutput<long>
                {
                    Value = buttonInfo.ButtonId,
                    Label = buttonInfo.ButtonName,
                    Data = new {buttonInfo.HasMobile, buttonInfo.HasWeb, buttonInfo.HasDesktop}
                });
            }

            result.add(item);
        }

        return result;
    }
}