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
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.Core;

/// <summary>
/// 开发工具种子数据
/// </summary>
internal static partial class MenuSeedData
{
    private static async Task SeedDevTools(ISqlSugarClient db, ApplicationModel applicationModel, DateTime dateTime)
    {
        var devCLMenuModel = new MenuModel
        {
            MenuId = YitIdHelper.NextId(),
            Edition = EditionEnum.Internal,
            AppId = applicationModel.AppId,
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

        await SeedDevConfig(db, applicationModel, dateTime, devCLMenuModel);
        await SeedDevMenu(db, applicationModel, dateTime, devCLMenuModel);
        await SeedDevSerial(db, applicationModel, dateTime, devCLMenuModel);
        await SeedDevDictionary(db, applicationModel, dateTime, devCLMenuModel);
        await SeedDevTable(db, applicationModel, dateTime, devCLMenuModel);
        await SeedDevScheduler(db, applicationModel, dateTime, devCLMenuModel);
        await SeedDevPasswordRecord(db, applicationModel, dateTime, devCLMenuModel);
        await SeedDevMessageSendRecord(db, applicationModel, dateTime, devCLMenuModel);
    }
}