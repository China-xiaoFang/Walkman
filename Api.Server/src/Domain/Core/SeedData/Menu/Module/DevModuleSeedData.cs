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
/// <see cref="MenuSeedData"/> 开发模块种子数据
/// </summary>
internal static partial class MenuSeedData
{
    /// <summary>
    /// 开发模块种子数据
    /// </summary>
    /// <param name="db"></param>
    /// <param name="dateTime"><see cref="DateTime"/> 时间</param>
    /// <returns></returns>
    private static async Task DevModuleSeedData(ISqlSugarClient db, DateTime dateTime)
    {
        // 重置菜单排序
        menuSort = 0;

        var apiCLMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = "DevApi:Catalog",
            MenuName = "Api",
            MenuTitle = "Api",
            ParentId = 0,
            ParentIds = [0],
            MenuType = MenuTypeEnum.Catalog,
            HasDesktop = true,
            DesktopIcon = "api",
            HasWeb = true,
            WebIcon = "fa-icon-Api",
            WebRouter = null,
            WebComponent = null,
            WebTab = false,
            WebKeepAlive = false,
            HasMobile = false,
            MobileIcon = "fa-icon-api",
            MobileRouter = null,
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        apiCLMenuModel = await db.Insertable(apiCLMenuModel)
            .ExecuteReturnEntityAsync();

        #region Api

        var apiMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.ApiPaged,
            MenuName = "接口管理",
            MenuTitle = "接口管理",
            ParentId = apiCLMenuModel.MenuId,
            ParentIds = [0, apiCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.IT,
            HasDesktop = true,
            DesktopRouter = null,
            DesktopIcon = "api",
            HasWeb = true,
            WebIcon = "fa-icon-Gateway",
            WebRouter = "/dev/api",
            WebComponent = "dev/api/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = false,
            MobileIcon = null,
            MobileRouter = null,
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        apiMenuModel = await db.Insertable(apiMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new ButtonModel
            {
                ButtonId = YitIdHelper.NextId(),
                MenuId = apiMenuModel.MenuId,
                ButtonCode = PermissionConst.ApiPaged,
                ButtonName = "列表",
                RoleType = RoleTypeEnum.IT,
                HasDesktop = true,
                HasWeb = true,
                HasMobile = false,
                Sort = 1,
                Status = CommonStatusEnum.Enable,
                CreatedTime = dateTime
            })
            .ExecuteCommandAsync();

        await db.Insertable(new List<MenuModel>
            {
                new()
                {
                    MenuId = YitIdHelper.NextId(),
                    MenuCode = PermissionConst.ApiSwagger,
                    MenuName = "Swagger",
                    MenuTitle = "Swagger",
                    ParentId = apiCLMenuModel.MenuId,
                    ParentIds = [0, apiCLMenuModel.MenuId],
                    MenuType = MenuTypeEnum.Internal,
                    RoleType = RoleTypeEnum.IT,
                    HasDesktop = true,
                    DesktopIcon = "api",
                    DesktopRouter = null,
                    HasWeb = true,
                    WebIcon = "fa-icon-Swagger",
                    WebRouter = null,
                    WebComponent = null,
                    WebTab = false,
                    WebKeepAlive = false,
                    HasMobile = false,
                    MobileIcon = null,
                    MobileRouter = null,
                    Link = "http://127.0.0.1:38081",
                    Visible = true,
                    Sort = menuSort,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    MenuId = YitIdHelper.NextId(),
                    MenuCode = PermissionConst.ApiKnife4j,
                    MenuName = "Knife4j",
                    MenuTitle = "Knife4j",
                    ParentId = apiCLMenuModel.MenuId,
                    ParentIds = [0, apiCLMenuModel.MenuId],
                    MenuType = MenuTypeEnum.Internal,
                    RoleType = RoleTypeEnum.IT,
                    HasDesktop = true,
                    DesktopIcon = "api",
                    DesktopRouter = null,
                    HasWeb = true,
                    WebIcon = "fa-icon-Swagger",
                    WebRouter = null,
                    WebComponent = null,
                    WebTab = false,
                    WebKeepAlive = false,
                    HasMobile = false,
                    MobileIcon = null,
                    MobileRouter = null,
                    Link = "http://127.0.0.1:38081/knife4j",
                    Visible = true,
                    Sort = menuSort,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        var devCLMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = "Dev:Catalog",
            MenuName = "开发管理",
            MenuTitle = "开发管理",
            ParentId = 0,
            ParentIds = [0],
            MenuType = MenuTypeEnum.Catalog,
            HasDesktop = true,
            DesktopIcon = "terminal",
            HasWeb = true,
            WebIcon = "fa-icon-Terminal",
            WebRouter = null,
            WebComponent = null,
            WebTab = false,
            WebKeepAlive = false,
            HasMobile = true,
            MobileIcon = "fa-icon-terminal",
            MobileRouter = null,
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        devCLMenuModel = await db.Insertable(devCLMenuModel)
            .ExecuteReturnEntityAsync();

        #region 系统配置

        var configMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.Config.Paged,
            MenuName = "系统配置",
            MenuTitle = "系统配置",
            ParentId = devCLMenuModel.MenuId,
            ParentIds = [0, devCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
            HasDesktop = true,
            DesktopIcon = "setting",
            HasWeb = true,
            WebIcon = "el-icon-Setting",
            WebRouter = "/dev/config",
            WebComponent = "dev/config/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/database.png",
            MobileRouter = "pages_dev/config/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        configMenuModel = await db.Insertable(configMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = configMenuModel.MenuId,
                    ButtonCode = PermissionConst.Config.Paged,
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
                    MenuId = configMenuModel.MenuId,
                    ButtonCode = PermissionConst.Config.Detail,
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
                    MenuId = configMenuModel.MenuId,
                    ButtonCode = PermissionConst.Config.Edit,
                    ButtonName = "编辑",
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

        #region 菜单管理

        var menuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.Menu.Paged,
            MenuName = "菜单管理",
            MenuTitle = "菜单管理",
            ParentId = devCLMenuModel.MenuId,
            ParentIds = [0, devCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
            HasDesktop = true,
            DesktopIcon = "menu",
            HasWeb = true,
            WebIcon = "fa-icon-Menu",
            WebRouter = "/dev/menu",
            WebComponent = "dev/menu/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/menu.png",
            MobileRouter = "pages_dev/menu/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        menuModel = await db.Insertable(menuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = menuModel.MenuId,
                    ButtonCode = PermissionConst.Menu.Paged,
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
                    MenuId = menuModel.MenuId,
                    ButtonCode = PermissionConst.Menu.Detail,
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
                    MenuId = menuModel.MenuId,
                    ButtonCode = PermissionConst.Menu.Add,
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
                    MenuId = menuModel.MenuId,
                    ButtonCode = PermissionConst.Menu.Edit,
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
                    MenuId = menuModel.MenuId,
                    ButtonCode = PermissionConst.Menu.Delete,
                    ButtonName = "删除",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 5,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = menuModel.MenuId,
                    ButtonCode = PermissionConst.Menu.Status,
                    ButtonName = "状态更改",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 6,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        #region 序号配置

        var serialMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.Serial.Paged,
            MenuName = "序号配置",
            MenuTitle = "序号配置",
            ParentId = devCLMenuModel.MenuId,
            ParentIds = [0, devCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
            HasDesktop = true,
            DesktopIcon = "adjust",
            HasWeb = true,
            WebIcon = "fa-icon-Adjust",
            WebRouter = "/dev/serial",
            WebComponent = "dev/serial/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/menu.png",
            MobileRouter = "pages_dev/serial/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        serialMenuModel = await db.Insertable(serialMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = serialMenuModel.MenuId,
                    ButtonCode = PermissionConst.Serial.Paged,
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
                    MenuId = serialMenuModel.MenuId,
                    ButtonCode = PermissionConst.Serial.Detail,
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
                    MenuId = serialMenuModel.MenuId,
                    ButtonCode = PermissionConst.Serial.Add,
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
                    MenuId = serialMenuModel.MenuId,
                    ButtonCode = PermissionConst.Serial.Edit,
                    ButtonName = "编辑",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 4,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        #region 数据字典

        var dictionaryMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.Dictionary.Paged,
            MenuName = "数据字典",
            MenuTitle = "数据字典",
            ParentId = devCLMenuModel.MenuId,
            ParentIds = [0, devCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
            HasDesktop = true,
            DesktopIcon = "dictionary",
            HasWeb = true,
            WebIcon = "fa-icon-Dictionary",
            WebRouter = "/dev/dictionary",
            WebComponent = "dev/dictionary/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/menu.png",
            MobileRouter = "pages_dev/dictionary/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        dictionaryMenuModel = await db.Insertable(dictionaryMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = dictionaryMenuModel.MenuId,
                    ButtonCode = PermissionConst.Dictionary.Paged,
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
                    MenuId = dictionaryMenuModel.MenuId,
                    ButtonCode = PermissionConst.Dictionary.Detail,
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
                    MenuId = dictionaryMenuModel.MenuId,
                    ButtonCode = PermissionConst.Dictionary.Add,
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
                    MenuId = dictionaryMenuModel.MenuId,
                    ButtonCode = PermissionConst.Dictionary.Edit,
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
                    MenuId = dictionaryMenuModel.MenuId,
                    ButtonCode = PermissionConst.Dictionary.Delete,
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

        #region 表格配置

        var tableConfigMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.Table.Paged,
            MenuName = "表格配置",
            MenuTitle = "表格配置",
            ParentId = devCLMenuModel.MenuId,
            ParentIds = [0, devCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
            HasDesktop = true,
            DesktopIcon = "tableConfig",
            HasWeb = true,
            WebIcon = "fa-icon-TableConfig",
            WebRouter = "/dev/tableConfig",
            WebComponent = "dev/tableConfig/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/menu.png",
            MobileRouter = "pages_dev/tableConfig/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        tableConfigMenuModel = await db.Insertable(tableConfigMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = tableConfigMenuModel.MenuId,
                    ButtonCode = PermissionConst.Table.Paged,
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
                    MenuId = tableConfigMenuModel.MenuId,
                    ButtonCode = PermissionConst.Table.Detail,
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
                    MenuId = tableConfigMenuModel.MenuId,
                    ButtonCode = PermissionConst.Table.Add,
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
                    MenuId = tableConfigMenuModel.MenuId,
                    ButtonCode = PermissionConst.Table.Edit,
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
                    MenuId = tableConfigMenuModel.MenuId,
                    ButtonCode = PermissionConst.Table.Delete,
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

        #region 调度程序

        var schedulerMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.Scheduler.Paged,
            MenuName = "调度程序",
            MenuTitle = "调度程序",
            ParentId = devCLMenuModel.MenuId,
            ParentIds = [0, devCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
            HasDesktop = true,
            DesktopIcon = "task",
            HasWeb = true,
            WebIcon = "fa-icon-Task",
            WebRouter = "/dev/scheduler",
            WebComponent = "dev/scheduler/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/menu.png",
            MobileRouter = "pages_dev/scheduler/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        schedulerMenuModel = await db.Insertable(schedulerMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = schedulerMenuModel.MenuId,
                    ButtonCode = PermissionConst.Scheduler.Paged,
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
                    MenuId = schedulerMenuModel.MenuId,
                    ButtonCode = PermissionConst.Scheduler.Detail,
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
                    MenuId = schedulerMenuModel.MenuId,
                    ButtonCode = PermissionConst.Scheduler.Add,
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
                    MenuId = schedulerMenuModel.MenuId,
                    ButtonCode = PermissionConst.Scheduler.Edit,
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
                    MenuId = schedulerMenuModel.MenuId,
                    ButtonCode = PermissionConst.Scheduler.Delete,
                    ButtonName = "删除",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 5,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = schedulerMenuModel.MenuId,
                    ButtonCode = PermissionConst.Scheduler.Start,
                    ButtonName = "启动",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 6,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = schedulerMenuModel.MenuId,
                    ButtonCode = PermissionConst.Scheduler.Stop,
                    ButtonName = "暂停",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 7,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = schedulerMenuModel.MenuId,
                    ButtonCode = PermissionConst.Scheduler.Trigger,
                    ButtonName = "执行作业",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 8,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = schedulerMenuModel.MenuId,
                    ButtonCode = PermissionConst.Scheduler.ResumeJob,
                    ButtonName = "恢复作业",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 9,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                },
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = schedulerMenuModel.MenuId,
                    ButtonCode = PermissionConst.Scheduler.StopJob,
                    ButtonName = "暂停作业",
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    HasDesktop = true,
                    HasWeb = true,
                    HasMobile = true,
                    Sort = 10,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                }
            })
            .ExecuteCommandAsync();

        #endregion

        #region 密码记录

        var passwordRecordModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.PasswordRecordPaged,
            MenuName = "密码记录",
            MenuTitle = "密码记录",
            ParentId = devCLMenuModel.MenuId,
            ParentIds = [0, devCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
            HasDesktop = true,
            DesktopIcon = "lock",
            HasWeb = true,
            WebIcon = "fa-icon-Lock",
            WebRouter = "/dev/passwordRecord",
            WebComponent = "dev/passwordRecord/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/passwordMap.png",
            MobileRouter = "pages_dev/passwordRecord/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        passwordRecordModel = await db.Insertable(passwordRecordModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new ButtonModel
            {
                ButtonId = YitIdHelper.NextId(),
                MenuId = passwordRecordModel.MenuId,
                ButtonCode = PermissionConst.PasswordRecordPaged,
                ButtonName = "列表",
                RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                HasDesktop = true,
                HasWeb = true,
                HasMobile = true,
                Sort = 1,
                Status = CommonStatusEnum.Enable,
                CreatedTime = dateTime
            })
            .ExecuteCommandAsync();

        #endregion

        var logCLMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = "DevSystemLog:Catalog",
            MenuName = "系统日志",
            MenuTitle = "系统日志",
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

        #region 异常日志

        var exceptionLogMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.ExceptionLogPaged,
            MenuName = "异常日志",
            MenuTitle = "异常日志",
            ParentId = logCLMenuModel.MenuId,
            ParentIds = [0, logCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
            HasDesktop = true,
            DesktopIcon = "menu",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/dev/exceptionLog",
            WebComponent = "dev/exceptionLog/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/user.png",
            MobileRouter = "pages_dev/exceptionLog/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        exceptionLogMenuModel = await db.Insertable(exceptionLogMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = exceptionLogMenuModel.MenuId,
                    ButtonCode = PermissionConst.ExceptionLogPaged,
                    RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
                    ButtonName = "列表",
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

        #region Sql异常日志

        var sqlExceptionLogMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.SqlExceptionLogPaged,
            MenuName = "Sql异常日志",
            MenuTitle = "Sql异常日志",
            ParentId = logCLMenuModel.MenuId,
            ParentIds = [0, logCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
            HasDesktop = true,
            DesktopIcon = "menu",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/dev/sqlExceptionLog",
            WebComponent = "dev/sqlExceptionLog/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/user.png",
            MobileRouter = "pages_dev/sqlExceptionLog/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        sqlExceptionLogMenuModel = await db.Insertable(sqlExceptionLogMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = sqlExceptionLogMenuModel.MenuId,
                    ButtonCode = PermissionConst.SqlExceptionLogPaged,
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

        #region Sql超时日志

        var sqlTimeoutLogMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.SqlTimeoutLogPaged,
            MenuName = "Sql超时日志",
            MenuTitle = "Sql超时日志",
            ParentId = logCLMenuModel.MenuId,
            ParentIds = [0, logCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
            HasDesktop = true,
            DesktopIcon = "menu",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/dev/sqlTimeoutLog",
            WebComponent = "dev/sqlTimeoutLog/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/user.png",
            MobileRouter = "pages_dev/sqlTimeoutLog/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        sqlTimeoutLogMenuModel = await db.Insertable(sqlTimeoutLogMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = sqlTimeoutLogMenuModel.MenuId,
                    ButtonCode = PermissionConst.SqlTimeoutLogPaged,
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

        #region Sql执行日志

        var sqlExecutionLogMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.SqlExecutionLogPaged,
            MenuName = "Sql执行日志",
            MenuTitle = "Sql执行日志",
            ParentId = logCLMenuModel.MenuId,
            ParentIds = [0, logCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
            HasDesktop = true,
            DesktopIcon = "menu",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/dev/sqlExecutionLog",
            WebComponent = "dev/sqlExecutionLog/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/user.png",
            MobileRouter = "pages_dev/sqlExecutionLog/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        sqlExecutionLogMenuModel = await db.Insertable(sqlExecutionLogMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = sqlExecutionLogMenuModel.MenuId,
                    ButtonCode = PermissionConst.SqlExecutionLogPaged,
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

        #region Sql差异日志

        var sqlDiffLogMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            MenuCode = PermissionConst.SqlDiffLogPaged,
            MenuName = "Sql差异日志",
            MenuTitle = "Sql差异日志",
            ParentId = logCLMenuModel.MenuId,
            ParentIds = [0, logCLMenuModel.MenuId],
            MenuType = MenuTypeEnum.Menu,
            RoleType = RoleTypeEnum.Admin | RoleTypeEnum.IT,
            HasDesktop = true,
            DesktopIcon = "menu",
            HasWeb = true,
            WebIcon = null,
            WebRouter = "/dev/sqlDiffLog",
            WebComponent = "dev/sqlDiffLog/index",
            WebTab = true,
            WebKeepAlive = true,
            HasMobile = true,
            MobileIcon = "https://image.fastdotnet.com/menu/mobile/user.png",
            MobileRouter = "pages_dev/sqlDiffLog/page/index",
            Visible = true,
            Sort = menuSort,
            Status = CommonStatusEnum.Enable,
            CreatedTime = dateTime
        };
        sqlDiffLogMenuModel = await db.Insertable(sqlDiffLogMenuModel)
            .ExecuteReturnEntityAsync();
        await db.Insertable(new List<ButtonModel>
            {
                new()
                {
                    ButtonId = YitIdHelper.NextId(),
                    MenuId = sqlDiffLogMenuModel.MenuId,
                    ButtonCode = PermissionConst.SqlDiffLogPaged,
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
    }
}