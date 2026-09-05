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

using Fast.Admin.Domain;
using Fast.Admin.Service.Auth.Dto;
using Fast.Center.Domain;
using Fast.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Service.Auth;

/// <summary>
/// 鉴权服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Auth, Name = "auth")]
public class AuthService : IDynamicApplication
{
    private readonly IUser _user;
    private readonly ISqlSugarClient _repository;
    private readonly ISqlSugarRepository<EmployeeModel> _empRepository;

    public AuthService(IUser user, ISqlSugarClient repository, ISqlSugarRepository<EmployeeModel> empRepository)
    {
        _user = user;
        _repository = repository;
        _empRepository = empRepository;
    }

    /// <summary>
    /// 获取登录用户信息
    /// </summary>
    [HttpGet("/getLoginUserInfo")]
    [ApiInfo("获取登录用户信息", HttpRequestActionEnum.Auth)]
    [AllowForbidden, DisabledRequestLog]
    public async Task<GetLoginUserInfoOutput> GetLoginUserInfo()
    {
        // 查询应用信息
        var applicationModel = await ApplicationContext.GetApplication(GlobalContext.Origin);

        if (applicationModel.AppType != GlobalContext.DeviceType)
        {
            throw new UserFriendlyException("应用类型不匹配！");
        }

        var hasDesktop = (GlobalContext.DeviceType
                          & (AppEnvironmentEnum.Windows | AppEnvironmentEnum.Mac | AppEnvironmentEnum.Linux))
                         != 0;
        var hasWeb = (GlobalContext.DeviceType & AppEnvironmentEnum.Web) != 0;
        var hasMobile = GlobalContext.IsMobile;

        // 查询租户信息
        var tenantModel = await TenantContext.GetTenant(_user.TenantNo);

        if (tenantModel.Status == CommonStatusEnum.Disable)
        {
            throw new UserFriendlyException("租户已被禁用！");
        }

        // 登录后身份验证开关
        var loginIdentityVerificationOpen = bool.Parse(await ConfigContext.GetConfig(ConfigConst.LoginIdentityVerificationOpen));

        var result = new GetLoginUserInfoOutput
        {
            AccountId = _user.AccountId,
            AccountKey = _user.AccountKey,
            Mobile = _user.Mobile,
            NickName = _user.NickName,
            Avatar = _user.Avatar,
            // 开启验证并且需要验证
            IdentityVerification = loginIdentityVerificationOpen && _user.IdentityVerification,
            TenantNo = _user.TenantNo,
            TenantName = _user.TenantName,
            ShortName = tenantModel.ShortName,
            TenantCode = _user.TenantCode,
            LogoUrl = tenantModel.LogoUrl,
            UserKey = _user.UserKey,
            EmployeeId = _user.EmployeeId,
            EmployeeNo = _user.EmployeeNo,
            EmployeeName = _user.EmployeeName,
            DepartmentId = _user.DepartmentId,
            DepartmentName = _user.DepartmentName,
            IsSuperAdmin = _user.IsSuperAdmin,
            IsAdmin = _user.IsAdmin
        };

        // 查询角色
        var roleList = await _empRepository.Queryable<EmployeeRoleModel>()
            .LeftJoin<RoleModel>((t1, t2) => t1.RoleId == t2.RoleId)
            .Where(t1 => t1.EmployeeId == _user.EmployeeId)
            .Select((t1, t2) => new
            {
                t1.RoleId,
                t1.RoleName,
                t2.RoleType,
                t2.IsSystemMenu,
                t2.DataScopeType,
                t2.DataScopeDepartmentIds
            })
            .ToListAsync();
        // 系统菜单角色类型
        var systemMenuRoleType = roleList.Where(wh => wh.IsSystemMenu)
            .Select(sl => sl.RoleType)
            .Aggregate(default(RoleTypeEnum), (acc, item) => acc | item);
        // 自定义菜单角色
        var customMenuRoleIds = roleList.Where(wh => !wh.IsSystemMenu)
            .Select(sl => sl.RoleId)
            .ToList();
        result.RoleNameList = roleList.Select(sl => sl.RoleName)
            .ToList();

        var menuQueryable = _repository.Queryable<MenuModel>()
            .Where(wh => wh.AppId == applicationModel.AppId)
            .Where(wh => wh.Status == CommonStatusEnum.Enable)
            .Where(wh => wh.MenuType != MenuTypeEnum.Catalog)
            .Where(wh => tenantModel.Edition >= wh.Edition)
            .WhereIF(hasDesktop, wh => wh.HasDesktop)
            .WhereIF(hasWeb, wh => wh.HasWeb)
            .WhereIF(hasMobile, wh => wh.HasMobile);

        if (_user.IsSuperAdmin || _user.IsAdmin)
        {
            result.RoleType = RoleTypeEnum.Admin;
            result.DataScopeType = DataScopeTypeEnum.All;
        }
        else
        {
            result.RoleType = roleList.Select(sl => sl.RoleType)
                .DefaultIfEmpty()
                .Aggregate((a, b) => a | b);
            result.DataScopeType = roleList.Any()
                ? roleList.Where(wh => wh.DataScopeType != DataScopeTypeEnum.CustomDept)
                    .Select(sl => sl.DataScopeType)
                    .DefaultIfEmpty(DataScopeTypeEnum.CustomDept)
                    .Min()
                : DataScopeTypeEnum.Self;

            // 查询当前用户角色对应的菜单Id
            var roleMenuIds = await _empRepository.Queryable<RoleMenuModel>()
                .Where(wh => customMenuRoleIds.Contains(wh.RoleId))
                .Select(sl => sl.MenuId)
                .ToListAsync();
            menuQueryable = menuQueryable.Where(wh => (wh.RoleType & systemMenuRoleType) != 0 || roleMenuIds.Contains(wh.MenuId));
        }

        // 查询所有菜单
        var menuList = await menuQueryable.Clone()
            .OrderBy(ob => ob.Sort)
            .Select(sl => new AuthMenuInfoDto
            {
                MenuId = sl.MenuId,
                MenuCode = sl.MenuCode,
                MenuName = sl.MenuName,
                MenuTitle = sl.MenuTitle,
                ParentId = sl.ParentId,
                MenuType = sl.MenuType,
                Icon = hasDesktop ? sl.DesktopIcon :
                    hasWeb ? sl.WebIcon :
                    hasMobile ? sl.MobileIcon : null,
                Router = hasDesktop ? sl.DesktopRouter :
                    hasWeb ? sl.WebRouter :
                    hasMobile ? sl.MobileRouter : null,
                Component = hasWeb ? sl.WebComponent : null,
                // ReSharper disable once SimplifyConditionalTernaryExpression
                Tab = hasWeb ? sl.WebTab : false,
                // ReSharper disable once SimplifyConditionalTernaryExpression
                KeepAlive = hasWeb ? sl.WebKeepAlive : false,
                Link = sl.Link,
                Visible = sl.Visible,
                Sort = sl.Sort
            })
            .ToListAsync();

        // 查询所有父级
        var parentMenuIds = menuList.Select(sl => sl.ParentId)
            .Distinct()
            .ToList();

        var parentMenuList = await _repository.Queryable<MenuModel>()
            .Where(wh => wh.AppId == applicationModel.AppId)
            .Where(wh => wh.Status == CommonStatusEnum.Enable)
            .Where(wh => tenantModel.Edition >= wh.Edition)
            .WhereIF(hasDesktop, wh => wh.HasDesktop)
            .WhereIF(hasWeb, wh => wh.HasWeb)
            .WhereIF(hasMobile, wh => wh.HasMobile)
            .Where(wh => parentMenuIds.Contains(wh.MenuId))
            .Select(t1 => new AuthMenuInfoDto
            {
                MenuId = t1.MenuId,
                MenuCode = t1.MenuCode,
                MenuName = t1.MenuName,
                MenuTitle = t1.MenuTitle,
                ParentId = t1.ParentId,
                MenuType = t1.MenuType,
                Icon = hasDesktop ? t1.DesktopIcon :
                    hasWeb ? t1.WebIcon :
                    hasMobile ? t1.MobileIcon : null,
                Router = hasDesktop ? t1.DesktopRouter :
                    hasWeb ? t1.WebRouter :
                    hasMobile ? t1.MobileRouter : null,
                Component = hasWeb ? t1.WebComponent : null,
                // ReSharper disable once SimplifyConditionalTernaryExpression
                Tab = hasWeb ? t1.WebTab : false,
                // ReSharper disable once SimplifyConditionalTernaryExpression
                KeepAlive = hasWeb ? t1.WebKeepAlive : false,
                Link = t1.Link,
                Visible = t1.Visible,
                Sort = t1.Sort
            })
            .ToListAsync();

        menuList.AddRange(parentMenuList);

        // 组建菜单树形
        result.MenuList = new TreeBuildUtil<AuthMenuInfoDto, long>().Build(menuList.Distinct()
            .ToList());

        if (!_user.IsSuperAdmin)
        {
            var buttonQueryable = _repository.Queryable<ButtonModel>()
                .Where(wh => wh.AppId == applicationModel.AppId)
                .Where(wh => wh.Status == CommonStatusEnum.Enable)
                .Where(wh => tenantModel.Edition >= wh.Edition)
                .WhereIF(hasDesktop, wh => wh.HasDesktop)
                .WhereIF(hasWeb, wh => wh.HasWeb)
                .WhereIF(hasMobile, wh => wh.HasMobile);
            if (!_user.IsAdmin)
            {
                // 查询当前用户角色对应的按钮Id
                var roleButtonIds = await _empRepository.Queryable<RoleButtonModel>()
                    .Where(wh => customMenuRoleIds.Contains(wh.RoleId))
                    .Select(sl => sl.ButtonId)
                    .ToListAsync();
                buttonQueryable = buttonQueryable.Where(wh =>
                    (wh.RoleType & systemMenuRoleType) != 0 || roleButtonIds.Contains(wh.ButtonId));
            }

            result.ButtonCodeList = await buttonQueryable.OrderBy(ob => ob.Sort)
                .Select(sl => sl.ButtonCode)
                .ToListAsync();
        }

        // 刷新缓存
        await _user.RefreshAuth(new RefreshAuthDto
        {
            DeviceType = _user.DeviceType,
            AppNo = _user.AppNo,
            TenantNo = _user.TenantNo,
            EmployeeNo = _user.EmployeeNo,
            RoleIdList = roleList.Select(sl => sl.RoleId)
                .ToList(),
            RoleNameList = result.RoleNameList,
            RoleType = result.RoleType,
            DataScopeType = result.DataScopeType,
            DataScopeDepartmentIdList = roleList.Where(wh => wh.DataScopeType == DataScopeTypeEnum.CustomDept)
                .Where(wh => wh.DataScopeDepartmentIds != null)
                .SelectMany(sl => sl.DataScopeDepartmentIds)
                .Distinct()
                .ToList(),
            MenuCodeList = _user.IsSuperAdmin
                ? []
                : menuList.Select(sl => sl.MenuCode)
                    .ToList(),
            ButtonCodeList = result.ButtonCodeList
        });

        return result;
    }
}