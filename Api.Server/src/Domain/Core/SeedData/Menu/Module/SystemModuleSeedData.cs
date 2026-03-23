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
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.Core;

/// <summary>
/// <see cref="MenuSeedData"/> 系统模块种子数据
/// </summary>
internal static partial class MenuSeedData
{
    /// <summary>
    /// 系统模块种子数据
    /// </summary>
    /// <param name="db"></param>
    /// <param name="dateTime"><see cref="DateTime"/> 时间</param>
    /// <returns></returns>
    private static async Task SystemModuleSeedData(ISqlSugarClient db, DateTime dateTime)
    {
        // 重置菜单排序
        menuSort = 1000;

        #region 系统监控

        await db.Insertable(new MenuModel
            {
                MenuId = YitIdHelper.NextId(),
                MenuCode = PermissionConst.SystemMonitor,
                MenuName = "系统监控",
                MenuTitle = "系统监控",
                ParentId = 0,
                ParentIds = [0],
                MenuType = MenuTypeEnum.Menu,
                RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                HasDesktop = true,
                DesktopIcon = "test",
                HasWeb = true,
                WebIcon = "fa-icon-Test",
                WebRouter = "/system/systemMonitor",
                WebComponent = "system/systemMonitor/index",
                WebTab = true,
                WebKeepAlive = true,
                HasMobile = true,
                MobileIcon = "https://image.fastdotnet.com/menu/mobile/database.png",
                MobileRouter = "pages_dev/systemMonitor/page/index",
                Visible = true,
                Sort = menuSort,
                Status = CommonStatusEnum.Enable,
                CreatedTime = dateTime
            })
            .ExecuteCommandAsync();

        #endregion

        #region 文件存储

        var fileMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.FilePaged,
            MenuName = "文件存储",
            MenuTitle = "文件存储",
            ParentId = 0,
            ParentIds = [0],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR | RoleTypeEnum.Finance,
            HasDesktop = true,
            DesktopIcon = "desktop",
            HasWeb = true,
            WebIcon = "el-icon-FolderOpened",
            WebRouter = "/system/file",
            WebComponent = "system/file/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/app.png",
            MobileRouter = "pages_system/file/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        fileMenuModel = await db.Insertable(fileMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = fileMenuModel.MenuId,
                    ButtonCode = PermissionConst.FilePaged,
                    ButtonName = "列表",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR | RoleTypeEnum.Finance,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 1,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        #region 账号管理

        var accountMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.Account.Paged,
            MenuName = "账号管理",
            MenuTitle = "账号管理",
            ParentId = 0,
            ParentIds = [0],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
            HasDesktop = true,
            DesktopIcon = "idCard",
            HasWeb = true,
            WebIcon = "fa-icon-IdCard",
            WebRouter = "/system/account",
            WebComponent = "system/account/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/user.png",
            MobileRouter = "pages_system/account/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        accountMenuModel = await db.Insertable(accountMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = accountMenuModel.MenuId,
                    ButtonCode = PermissionConst.Account.Paged,
                    ButtonName = "列表",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 1,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = accountMenuModel.MenuId,
                    ButtonCode = PermissionConst.Account.Detail,
                    ButtonName = "详情",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 2,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = accountMenuModel.MenuId,
                    ButtonCode = PermissionConst.Account.Unlock,
                    ButtonName = "解除锁定",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 3,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = accountMenuModel.MenuId,
                    ButtonCode = PermissionConst.Account.ResetPassword,
                    ButtonName = "重置密码",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 4,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = accountMenuModel.MenuId,
                    ButtonCode = PermissionConst.Account.Status,
                    ButtonName = "状态更改",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 5,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        #region 应用标识

        var appOpenIdMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.AppOpenId.Paged,
            MenuName = "应用标识",
            MenuTitle = "应用标识",
            ParentId = 0,
            ParentIds = [0],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
            HasDesktop = true,
            DesktopIcon = "terminal",
            HasWeb = true,
            WebIcon = "fa-icon-Terminal",
            WebRouter = "/system/applicationOpenId",
            WebComponent = "system/applicationOpenId/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/app.png",
            MobileRouter = "pages_system/applicationOpenId/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        appOpenIdMenuModel = await db.Insertable(appOpenIdMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = appOpenIdMenuModel.MenuId,
                    ButtonCode = PermissionConst.AppOpenId.Paged,
                    ButtonName = "列表",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 1,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = appOpenIdMenuModel.MenuId,
                    ButtonCode = PermissionConst.AppOpenId.Detail,
                    ButtonName = "详情",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 2,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = appOpenIdMenuModel.MenuId,
                    ButtonCode = PermissionConst.AppOpenId.Add,
                    ButtonName = "新增",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 3,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = appOpenIdMenuModel.MenuId,
                    ButtonCode = PermissionConst.AppOpenId.Edit,
                    ButtonName = "编辑",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 4,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = appOpenIdMenuModel.MenuId,
                    ButtonCode = PermissionConst.AppOpenId.Delete,
                    ButtonName = "删除",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 5,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        #region 组织架构

        var orgCLMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = "Organization:Catalog",
            MenuName = "组织架构",
            MenuTitle = "组织架构",
            ParentId = 0,
            ParentIds = [0],
            MenuType = MenuTypeEnum.Catalog,
            HasDesktop = true,
            DesktopIcon = "organization",
            HasWeb = true,
            WebIcon = "fa-icon-Organization",
            WebRouter = null,
            WebComponent = null,
            WebTab = false,
            WebKeepAlive = false,
            HasMobile = false,
            MobileIcon = "fa-icon-organization",
            MobileRouter = null,
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        orgCLMenuModel = await db.Insertable(orgCLMenuModel)
            .ExecuteReturnEntityAsync();

        #region 职位管理

        var positionMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.Position.Paged,
            MenuName = "职位管理",
            MenuTitle = "职位管理",
            ParentId = orgCLMenuModel.MenuId,
            ParentIds = [0, orgCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
            HasDesktop = true,
            DesktopIcon = "menu",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/system/position",
            WebComponent = "system/position/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/user.png",
            MobileRouter = "pages_system/position/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        positionMenuModel = await db.Insertable(positionMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = positionMenuModel.MenuId,
                    ButtonCode = PermissionConst.Position.Paged,
                    ButtonName = "列表",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 1,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = positionMenuModel.MenuId,
                    ButtonCode = PermissionConst.Position.Detail,
                    ButtonName = "详情",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 2,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = positionMenuModel.MenuId,
                    ButtonCode = PermissionConst.Position.Add,
                    ButtonName = "新增",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 3,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = positionMenuModel.MenuId,
                    ButtonCode = PermissionConst.Position.Edit,
                    ButtonName = "编辑",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 4,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = positionMenuModel.MenuId,
                    ButtonCode = PermissionConst.Position.Delete,
                    ButtonName = "删除",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 5,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        #region 职级管理

        var jobLevelMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.JobLevel.Paged,
            MenuName = "职级管理",
            MenuTitle = "职级管理",
            ParentId = orgCLMenuModel.MenuId,
            ParentIds = [0, orgCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
            HasDesktop = true,
            DesktopIcon = "menu",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/system/jobLevel",
            WebComponent = "system/jobLevel/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/user.png",
            MobileRouter = "pages_system/jobLevel/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        jobLevelMenuModel = await db.Insertable(jobLevelMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = jobLevelMenuModel.MenuId,
                    ButtonCode = PermissionConst.JobLevel.Paged,
                    ButtonName = "列表",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 1,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = jobLevelMenuModel.MenuId,
                    ButtonCode = PermissionConst.JobLevel.Detail,
                    ButtonName = "详情",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 2,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = jobLevelMenuModel.MenuId,
                    ButtonCode = PermissionConst.JobLevel.Add,
                    ButtonName = "新增",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 3,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = jobLevelMenuModel.MenuId,
                    ButtonCode = PermissionConst.JobLevel.Edit,
                    ButtonName = "编辑",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 4,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = jobLevelMenuModel.MenuId,
                    ButtonCode = PermissionConst.JobLevel.Delete,
                    ButtonName = "删除",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 5,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        #region 角色管理

        var roleMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.Role.Paged,
            MenuName = "角色管理",
            MenuTitle = "角色管理",
            ParentId = orgCLMenuModel.MenuId,
            ParentIds = [0, orgCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
            HasDesktop = true,
            DesktopIcon = "menu",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/system/role",
            WebComponent = "system/role/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/user.png",
            MobileRouter = "pages_system/role/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        roleMenuModel = await db.Insertable(roleMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = roleMenuModel.MenuId,
                    ButtonCode = PermissionConst.Role.Paged,
                    ButtonName = "列表",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 1,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = roleMenuModel.MenuId,
                    ButtonCode = PermissionConst.Role.Detail,
                    ButtonName = "详情",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 2,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = roleMenuModel.MenuId,
                    ButtonCode = PermissionConst.Role.Add,
                    ButtonName = "新增",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 3,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = roleMenuModel.MenuId,
                    ButtonCode = PermissionConst.Role.Edit,
                    ButtonName = "编辑",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 4,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = roleMenuModel.MenuId,
                    ButtonCode = PermissionConst.Role.Delete,
                    ButtonName = "删除",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 5,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        #region 部门管理

        var departmentMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.Department.Paged,
            MenuName = "部门管理",
            MenuTitle = "部门管理",
            ParentId = orgCLMenuModel.MenuId,
            ParentIds = [0, orgCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
            HasDesktop = true,
            DesktopIcon = "menu",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/system/department",
            WebComponent = "system/department/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/user.png",
            MobileRouter = "pages_system/department/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        departmentMenuModel = await db.Insertable(departmentMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = departmentMenuModel.MenuId,
                    ButtonCode = PermissionConst.Department.Paged,
                    ButtonName = "列表",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 1,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = departmentMenuModel.MenuId,
                    ButtonCode = PermissionConst.Department.Detail,
                    ButtonName = "详情",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 2,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = departmentMenuModel.MenuId,
                    ButtonCode = PermissionConst.Department.Add,
                    ButtonName = "新增",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 3,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = departmentMenuModel.MenuId,
                    ButtonCode = PermissionConst.Department.Edit,
                    ButtonName = "编辑",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 4,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = departmentMenuModel.MenuId,
                    ButtonCode = PermissionConst.Department.Delete,
                    ButtonName = "删除",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 5,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        #region 职员管理

        var employeeMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.Employee.Paged,
            MenuName = "职员管理",
            MenuTitle = "职员管理",
            ParentId = orgCLMenuModel.MenuId,
            ParentIds = [0, orgCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR | RoleTypeEnum.Finance,
            HasDesktop = true,
            DesktopIcon = "menu",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/system/employee",
            WebComponent = "system/employee/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/user.png",
            MobileRouter = "pages_system/employee/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        employeeMenuModel = await db.Insertable(employeeMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = employeeMenuModel.MenuId,
                    ButtonCode = PermissionConst.Employee.Paged,
                    ButtonName = "列表",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR | RoleTypeEnum.Finance,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 1,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = employeeMenuModel.MenuId,
                    ButtonCode = PermissionConst.Employee.Detail,
                    ButtonName = "详情",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR | RoleTypeEnum.Finance,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 2,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = employeeMenuModel.MenuId,
                    ButtonCode = PermissionConst.Employee.Add,
                    ButtonName = "新增",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 3,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = employeeMenuModel.MenuId,
                    ButtonCode = PermissionConst.Employee.Edit,
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    ButtonName = "编辑",
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 4,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = employeeMenuModel.MenuId,
                    ButtonCode = PermissionConst.Employee.Status,
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    ButtonName = "状态更改",
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 5,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        #region 在线用户

        var onlineUserMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.OnlineUser.Paged,
            MenuName = "在线用户",
            MenuTitle = "在线用户",
            ParentId = orgCLMenuModel.MenuId,
            ParentIds = [0, orgCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
            HasDesktop = true,
            DesktopIcon = "menu",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/system/onlineUser",
            WebComponent = "system/onlineUser/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/user.png",
            MobileRouter = "pages_system/onlineUser/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        onlineUserMenuModel = await db.Insertable(onlineUserMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = onlineUserMenuModel.MenuId,
                    ButtonCode = PermissionConst.OnlineUser.Paged,
                    ButtonName = "列表",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 1,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = onlineUserMenuModel.MenuId,
                    ButtonCode = PermissionConst.OnlineUser.ForceOffline,
                    ButtonName = "强制下线",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 2,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        #endregion

        #region 财务管理

        var financeCLMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = "Finance:Catalog",
            MenuName = "财务管理",
            MenuTitle = "财务管理",
            ParentId = 0,
            ParentIds = [0],
            MenuType = MenuTypeEnum.Catalog,
            HasDesktop = true,
            DesktopIcon = "money",
            HasWeb = true,
            WebIcon = "fa-icon-Money",
            WebRouter = null,
            WebComponent = null,
            WebTab = false,
            WebKeepAlive = false,
            HasMobile = false,
            MobileIcon = "fa-icon-money",
            MobileRouter = null,
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        financeCLMenuModel = await db.Insertable(financeCLMenuModel)
            .ExecuteReturnEntityAsync();

        #region 商户号

        var merchantMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.Merchant.Paged,
            MenuName = "商户号",
            MenuTitle = "商户号",
            ParentId = financeCLMenuModel.MenuId,
            ParentIds = [0, financeCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.Finance,
            HasDesktop = true,
            DesktopIcon = "menu",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/system/merchant",
            WebComponent = "system/merchant/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/user.png",
            MobileRouter = "pages_system/merchant/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        merchantMenuModel = await db.Insertable(merchantMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = merchantMenuModel.MenuId,
                    ButtonCode = PermissionConst.Merchant.Paged,
                    ButtonName = "列表",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.Finance,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 1,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = merchantMenuModel.MenuId,
                    ButtonCode = PermissionConst.Merchant.Detail,
                    ButtonName = "详情",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.Finance,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 2,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = merchantMenuModel.MenuId,
                    ButtonCode = PermissionConst.Merchant.Add,
                    ButtonName = "新增",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 3,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = merchantMenuModel.MenuId,
                    ButtonCode = PermissionConst.Merchant.Edit,
                    ButtonName = "编辑",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 4,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = merchantMenuModel.MenuId,
                    ButtonCode = PermissionConst.Merchant.Delete,
                    ButtonName = "删除",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 5,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        #region 支付记录

        var payRecordMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.PayRecordPaged,
            MenuName = "支付记录",
            MenuTitle = "支付记录",
            ParentId = financeCLMenuModel.MenuId,
            ParentIds = [0, financeCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.Finance,
            HasDesktop = true,
            DesktopIcon = "lock",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/system/payRecord",
            WebComponent = "system/payRecord/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/passwordMap.png",
            MobileRouter = "pages_system/payRecord/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        payRecordMenuModel = await db.Insertable(payRecordMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new ButtonModel
            {
                ButtonId = YitIdHelper.NextId(),
                MenuId = payRecordMenuModel.MenuId,
                ButtonCode = PermissionConst.PayRecordPaged,
                ButtonName = "列表",
                RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.Finance,
                HasDesktop = true,
                HasWeb = true,
                HasMobile = true,
                Sort = 1,
                Status = CommonStatusEnum.Enable,
                CreatedTime = dateTime
            })
            .ExecuteCommandAsync();

        #endregion

        #region 退款记录

        var refundRecordMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.RefundRecordPaged,
            MenuName = "退款记录",
            MenuTitle = "退款记录",
            ParentId = financeCLMenuModel.MenuId,
            ParentIds = [0, financeCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.Finance,
            HasDesktop = true,
            DesktopIcon = "lock",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/system/refundRecord",
            WebComponent = "system/refundRecord/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/passwordMap.png",
            MobileRouter = "pages_system/refundRecord/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        refundRecordMenuModel = await db.Insertable(refundRecordMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new ButtonModel
            {
                ButtonId = YitIdHelper.NextId(),
                MenuId = refundRecordMenuModel.MenuId,
                ButtonCode = PermissionConst.RefundRecordPaged,
                ButtonName = "列表",
                RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.Finance,
                HasDesktop = true,
                HasWeb = true,
                HasMobile = true,
                Sort = 1,
                Status = CommonStatusEnum.Enable,
                CreatedTime = dateTime
            })
            .ExecuteCommandAsync();

        #endregion

        #endregion

        #region 平台管理

        var platformCLMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = "Platform:Catalog",
            MenuName = "平台管理",
            MenuTitle = "平台管理",
            ParentId = 0,
            ParentIds = [0],
            MenuType = MenuTypeEnum.Catalog,
            HasDesktop = true,
            DesktopIcon = "desktop",
            HasWeb = true,
            WebIcon = "fa-icon-Desktop",
            WebRouter = null,
            WebComponent = null,
            WebTab = false,
            WebKeepAlive = false,
            HasMobile = false,
            MobileIcon = "fa-icon-desktop",
            MobileRouter = null,
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        platformCLMenuModel = await db.Insertable(platformCLMenuModel)
            .ExecuteReturnEntityAsync();

        #region 微信用户

        var wechatUserMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.WeChat.Paged,
            MenuName = "微信用户",
            MenuTitle = "微信用户",
            ParentId = platformCLMenuModel.MenuId,
            ParentIds = [0, platformCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
            HasDesktop = true,
            DesktopIcon = "menu",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/system/weChatUser",
            WebComponent = "system/weChatUser/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/user.png",
            MobileRouter = "pages_system/weChatUser/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        wechatUserMenuModel = await db.Insertable(wechatUserMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = wechatUserMenuModel.MenuId,
                    ButtonCode = PermissionConst.WeChat.Paged,
                    ButtonName = "列表",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 1,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        #region 投诉工单

        var complaintMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.Complaint.Paged,
            MenuName = "投诉工单",
            MenuTitle = "投诉工单",
            ParentId = platformCLMenuModel.MenuId,
            ParentIds = [0, platformCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
            HasDesktop = true,
            DesktopIcon = "menu",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/system/complaint",
            WebComponent = "system/complaint/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/user.png",
            MobileRouter = "pages_system/complaint/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        complaintMenuModel = await db.Insertable(complaintMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = complaintMenuModel.MenuId,
                    ButtonCode = PermissionConst.Complaint.Paged,
                    ButtonName = "列表",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 1,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = complaintMenuModel.MenuId,
                    ButtonCode = PermissionConst.Complaint.Detail,
                    ButtonName = "详情",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 2,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = complaintMenuModel.MenuId,
                    ButtonCode = PermissionConst.Complaint.Handle,
                    ButtonName = "处理",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 3,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        #endregion

        #region 日志管理

        var logCLMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = "Log:Catalog",
            MenuName = "日志管理",
            MenuTitle = "日志管理",
            ParentId = 0,
            ParentIds = [0],
            MenuType = MenuTypeEnum.Catalog,
            HasDesktop = true,
            DesktopIcon = "desktop",
            HasWeb = true,
            WebIcon = "el-icon-Odometer",
            WebRouter = null,
            WebComponent = null,
            WebTab = false,
            WebKeepAlive = false,
            HasMobile = false,
            MobileIcon = "fa-icon-Odometer",
            MobileRouter = null,
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        logCLMenuModel = await db.Insertable(logCLMenuModel)
            .ExecuteReturnEntityAsync();

        #region 访问日志

        var visitLogMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.VisitLogPaged,
            MenuName = "访问日志",
            MenuTitle = "访问日志",
            ParentId = logCLMenuModel.MenuId,
            ParentIds = [0, logCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
            HasDesktop = true,
            DesktopIcon = "menu",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/system/visitLog",
            WebComponent = "system/visitLog/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/user.png",
            MobileRouter = "pages_system/visitLog/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        visitLogMenuModel = await db.Insertable(visitLogMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = visitLogMenuModel.MenuId,
                    ButtonCode = PermissionConst.VisitLogPaged,
                    ButtonName = "列表",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 1,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        #region 操作日志

        var operateLogMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.OperateLogPaged,
            MenuName = "操作日志",
            MenuTitle = "操作日志",
            ParentId = logCLMenuModel.MenuId,
            ParentIds = [0, logCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR | RoleTypeEnum.Finance,
            HasDesktop = true,
            DesktopIcon = "menu",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/system/operateLog",
            WebComponent = "system/operateLog/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/user.png",
            MobileRouter = "pages_system/operateLog/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        operateLogMenuModel = await db.Insertable(operateLogMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = operateLogMenuModel.MenuId,
                    ButtonCode = PermissionConst.OperateLogPaged,
                    ButtonName = "列表",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT | RoleTypeEnum.HR | RoleTypeEnum.Finance,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 1,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        #region 请求日志

        var requestLogMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.RequestLogPaged,
            MenuName = "请求日志",
            MenuTitle = "请求日志",
            ParentId = logCLMenuModel.MenuId,
            ParentIds = [0, logCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
            HasDesktop = true,
            DesktopIcon = "menu",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/system/requestLog",
            WebComponent = "system/requestLog/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/user.png",
            MobileRouter = "pages_system/requestLog/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        requestLogMenuModel = await db.Insertable(requestLogMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = requestLogMenuModel.MenuId,
                    ButtonCode = PermissionConst.RequestLogPaged,
                    ButtonName = "列表",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 1,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        #endregion
    }
}