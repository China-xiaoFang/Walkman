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

using Fast.Center.Entity;
using Fast.SqlSugar;
using Microsoft.Extensions.Hosting;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.Core;

/// <summary>
/// <see cref="DatabaseSeedData"/> 系统数据库种子数据
/// </summary>
public static class DatabaseSeedData
{
    /// <summary>
    /// 系统数据库种子数据
    /// </summary>
    /// <param name="db"></param>
    /// <param name="tenantId"><see cref="long"/> 租户Id</param>
    /// <param name="tenantCode"><see cref="string"/> 租户编码</param>
    /// <param name="dateTime"><see cref="DateTime"/> 时间</param>
    /// <returns></returns>
    public static async Task SystemDatabaseSeedData(ISqlSugarClient db, long tenantId, string tenantCode, DateTime dateTime)
    {
        var isDevelopment = FastContext.HostEnvironment.IsDevelopment();
        var dbType = SqlSugarContext.ConnectionSettings.DbType != null
            ? SqlSugarContext.ConnectionSettings.DbType.Value.ToSugarDbType()
            : SugarDbType.SqlServer;
        await db.Insertable(new List<MainDatabaseModel>
            {
                // 初始化日志库
                new()
                {
                    MainId = YitIdHelper.NextId(),
                    DatabaseType = DatabaseTypeEnum.CenterLog,
                    DbType = dbType,
                    PublicIp = SqlSugarContext.ConnectionSettings.ServiceIp,
                    IntranetIp = "127.0.0.1",
                    Port = SqlSugarContext.ConnectionSettings.Port ?? 1433,
                    DbName = isDevelopment ? "WMCenter_Log_Dev" : "WMCenter_Log",
                    DbUser = SqlSugarContext.ConnectionSettings.DbUser,
                    DbPwd = SqlSugarContext.ConnectionSettings.DbPwd,
                    CommandTimeOut = SqlSugarContext.ConnectionSettings.CommandTimeOut!.Value,
                    SugarSqlExecMaxSeconds = SqlSugarContext.ConnectionSettings.SugarSqlExecMaxSeconds!.Value,
                    DiffLog = false,
                    DisableAop = true,
                    IsInitialized = true,
                    CreatedTime = dateTime,
                    TenantId = tenantId
                },
                // 初始化网关系统库
                new()
                {
                    MainId = YitIdHelper.NextId(),
                    DatabaseType = DatabaseTypeEnum.Gateway,
                    DbType = dbType,
                    PublicIp = SqlSugarContext.ConnectionSettings.ServiceIp,
                    IntranetIp = "127.0.0.1",
                    Port = SqlSugarContext.ConnectionSettings.Port ?? 1433,
                    DbName = isDevelopment ? "WMGateway_Dev" : "WMGateway",
                    DbUser = SqlSugarContext.ConnectionSettings.DbUser,
                    DbPwd = SqlSugarContext.ConnectionSettings.DbPwd,
                    CommandTimeOut = SqlSugarContext.ConnectionSettings.CommandTimeOut!.Value,
                    SugarSqlExecMaxSeconds = SqlSugarContext.ConnectionSettings.SugarSqlExecMaxSeconds!.Value,
                    DiffLog = true,
                    DisableAop = false,
                    IsInitialized = false,
                    CreatedTime = dateTime,
                    TenantId = tenantId
                },
                // 初始化部署系统库
                new()
                {
                    MainId = YitIdHelper.NextId(),
                    DatabaseType = DatabaseTypeEnum.Deploy,
                    DbType = dbType,
                    PublicIp = SqlSugarContext.ConnectionSettings.ServiceIp,
                    IntranetIp = "127.0.0.1",
                    Port = SqlSugarContext.ConnectionSettings.Port ?? 1433,
                    DbName = isDevelopment ? "WMDeploy_Dev" : "WMDeploy",
                    DbUser = SqlSugarContext.ConnectionSettings.DbUser,
                    DbPwd = SqlSugarContext.ConnectionSettings.DbPwd,
                    CommandTimeOut = SqlSugarContext.ConnectionSettings.CommandTimeOut!.Value,
                    SugarSqlExecMaxSeconds = SqlSugarContext.ConnectionSettings.SugarSqlExecMaxSeconds!.Value,
                    DiffLog = true,
                    DisableAop = false,
                    IsInitialized = false,
                    CreatedTime = dateTime,
                    TenantId = tenantId
                },
                // 初始化业务库
                new()
                {
                    MainId = YitIdHelper.NextId(),
                    DatabaseType = DatabaseTypeEnum.Admin,
                    DbType = dbType,
                    PublicIp = SqlSugarContext.ConnectionSettings.ServiceIp,
                    IntranetIp = "127.0.0.1",
                    Port = SqlSugarContext.ConnectionSettings.Port ?? 1433,
                    DbName = isDevelopment ? "WMAdmin_Dev" : "WMAdmin",
                    DbUser = SqlSugarContext.ConnectionSettings.DbUser,
                    DbPwd = SqlSugarContext.ConnectionSettings.DbPwd,
                    CommandTimeOut = SqlSugarContext.ConnectionSettings.CommandTimeOut!.Value,
                    SugarSqlExecMaxSeconds = SqlSugarContext.ConnectionSettings.SugarSqlExecMaxSeconds!.Value,
                    DiffLog = true,
                    DisableAop = false,
                    IsInitialized = false,
                    CreatedTime = dateTime,
                    TenantId = tenantId
                },
                // 初始化业务日志库
                new()
                {
                    MainId = YitIdHelper.NextId(),
                    DatabaseType = DatabaseTypeEnum.AdminLog,
                    DbType = dbType,
                    PublicIp = SqlSugarContext.ConnectionSettings.ServiceIp,
                    IntranetIp = "127.0.0.1",
                    Port = SqlSugarContext.ConnectionSettings.Port ?? 1433,
                    DbName = isDevelopment ? "WMAdmin_Log_Dev" : "WMAdmin_Log",
                    DbUser = SqlSugarContext.ConnectionSettings.DbUser,
                    DbPwd = SqlSugarContext.ConnectionSettings.DbPwd,
                    CommandTimeOut = SqlSugarContext.ConnectionSettings.CommandTimeOut!.Value,
                    SugarSqlExecMaxSeconds = SqlSugarContext.ConnectionSettings.SugarSqlExecMaxSeconds!.Value,
                    DiffLog = false,
                    DisableAop = true,
                    IsInitialized = false,
                    CreatedTime = dateTime,
                    TenantId = tenantId
                }
            })
            .ExecuteCommandAsync();
    }
}