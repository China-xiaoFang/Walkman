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
using Fast.SqlSugar;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.Core;

/// <summary>
/// 数据库初始化托管服务
/// </summary>
[Order(102)]
public class InitDatabaseHostedService : IHostedService
{
    /// <summary>
    /// 日志
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// 初始化数据库托管服务
    /// </summary>
    public InitDatabaseHostedService(ILogger<InitDatabaseHostedService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var db = new SqlSugarClient(SqlSugarContext.GetConnectionConfig(SqlSugarContext.ConnectionSettings));

            // 创建库
            db.DbMaintenance.CreateDatabase();

            // 查询核心表是否存在
            if (db.DbMaintenance.IsAnyTable<AccountModel>())
                return;

            // 加载Aop
            SugarEntityFilter.LoadSugarAop(FastContext.HostEnvironment.IsDevelopment(), db);

            MAppContext.ConsoleWrite(console =>
            {
                console.BackgroundColor = ConsoleColor.Black;
                console.ForegroundColor = ConsoleColor.Green;
                console.Write("info");
                console.ResetColor();
                console.WriteLine($": {DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                console.BackgroundColor = ConsoleColor.Black;
                console.ForegroundColor = ConsoleColor.DarkGray;
                console.WriteLine("      开始初始化数据库...");
            });

            // 获取所有不分表的Model类型
            var tableTypes = SqlSugarContext.SqlSugarEntityList.Where(wh => !wh.IsSplitTable)
                .Where(wh => wh.SugarDbType == null || (DatabaseTypeEnum) wh.SugarDbType == DatabaseTypeEnum.Center)
                .Select(sl => sl.EntityType)
                .ToArray();
            // 获取所有分表的Model类型
            var splitTableTypes = SqlSugarContext.SqlSugarEntityList.Where(wh => wh.IsSplitTable)
                .Where(wh => wh.SugarDbType == null || (DatabaseTypeEnum) wh.SugarDbType == DatabaseTypeEnum.Center)
                .Select(sl => sl.EntityType)
                .ToArray();

            // 创建表
            db.CodeFirst.InitTables(tableTypes);
            db.CodeFirst.SplitTables()
                .InitTables(splitTableTypes);

            var dateTime = new DateTime(2025, 01, 01);
            var initialAdminPassword = CryptoUtil.HashPasswordPBKDF2SHA256(CommonConst.Default.AdminPassword);

            // 表结构创建不参与数据事务；种子数据统一进入事务，失败后下次启动可以安全重试
            await db.Ado.BeginTranAsync();
            try
            {
                // 初始化系统租户
                var systemTenantModel = new TenantModel
                {
                    TenantId = CommonConst.Default.TenantId,
                    TenantNo = CommonConst.Default.TenantNo,
                    TenantCode = "Fa",
                    Status = CommonStatusEnum.Enable,
                    TenantName = "FastDotNet工作室",
                    ShortName = "Fast",
                    SpellName = "fast dotnet gong zuo shi",
                    Edition = EditionEnum.Internal,
                    AdminAccountId = CommonConst.Default.SuperAdminAccountId,
                    AdminName = "超级管理员",
                    AdminMobile = "15580001115",
                    AdminEmail = "2875616188@qq.com",
                    AdminPhone = null,
                    RobotName = "机器人",
                    TenantType = TenantTypeEnum.System,
                    LogoUrl = CommonConst.DefaultLogo,
                    AllowDeleteData = true,
                    CreatedTime = dateTime
                };
                systemTenantModel = await db.Insertable(systemTenantModel)
                    .ExecuteReturnEntityAsync();

                #region 超级管理员

                var superAdminAccountModel = new AccountModel
                {
                    AccountId = CommonConst.Default.SuperAdminAccountId,
                    AccountKey = NumberUtil.IdToCodeByLong(CommonConst.Default.SuperAdminAccountId),
                    Mobile = "15580001115",
                    Email = "2875616188@qq.com",
                    Password = initialAdminPassword,
                    NickName = "小方",
                    Avatar = CommonConst.DefaultLogo,
                    Status = CommonStatusEnum.Enable,
                    CreatedTime = dateTime
                };
                superAdminAccountModel = await db.Insertable(superAdminAccountModel)
                    .ExecuteReturnEntityAsync();

                var superAdminUserId = YitIdHelper.NextId();
                var robotUserId = YitIdHelper.NextId();
                await db.Insertable(new List<TenantUserModel>
                    {
                        new()
                        {
                            EmployeeId = superAdminUserId,
                            UserKey = NumberUtil.IdToCodeByLong(superAdminUserId),
                            AccountId = superAdminAccountModel.AccountId,
                            EmployeeNo = "SuperAdmin",
                            EmployeeName = "超级管理员",
                            IdPhoto = CommonConst.DefaultLogo,
                            DepartmentId = null,
                            DepartmentName = null,
                            UserType = UserTypeEnum.SuperAdmin,
                            Status = CommonStatusEnum.Enable,
                            CreatedTime = dateTime,
                            TenantId = systemTenantModel.TenantId
                        },
                        new()
                        {
                            EmployeeId = robotUserId,
                            UserKey = NumberUtil.IdToCodeByLong(robotUserId),
                            AccountId = -99,
                            EmployeeNo = $"{systemTenantModel.TenantCode}_Robot",
                            EmployeeName = systemTenantModel.RobotName,
                            UserType = UserTypeEnum.Robot,
                            Status = CommonStatusEnum.Disable,
                            CreatedTime = dateTime,
                            TenantId = systemTenantModel.TenantId
                        }
                    })
                    .ExecuteCommandAsync(cancellationToken);

                #endregion

                #region PasswordRecordModel

                // 初始化密码记录表
                await db.Insertable(new List<PasswordRecordModel>
                    {
                        new()
                        {
                            AccountId = superAdminAccountModel.AccountId,
                            OperationType = PasswordOperationTypeEnum.Create,
                            Type = PasswordTypeEnum.PBKDF2_SHA256,
                            Password = initialAdminPassword,
                            CreatedTime = dateTime
                        }
                    })
                    .ExecuteCommandAsync(cancellationToken);

                #endregion

                // 系统数据库
                await DatabaseSeedData.SystemDatabaseSeedData(db, systemTenantModel.TenantId, systemTenantModel.TenantCode,
                    dateTime);

                // 配置
                await ConfigSeedData.SystemConfigSeedData(db, dateTime);

                // 系统序号规则
                await SysSerialSeedData.SeedData(db);

                // 应用
                var applicationModel = await ApplicationSeedData.SeedData(db, dateTime);

                // 菜单
                await MenuSeedData.DefaultMenuSeedData(db, applicationModel, dateTime);

                // 提交事务
                await db.Ado.CommitTranAsync();
            }
            catch
            {
                // 回滚事务
                await db.Ado.RollbackTranAsync();
                throw;
            }

            MAppContext.ConsoleWrite(console =>
            {
                console.BackgroundColor = ConsoleColor.Black;
                console.ForegroundColor = ConsoleColor.Green;
                console.Write("info");
                console.ResetColor();
                console.WriteLine($": {DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                console.BackgroundColor = ConsoleColor.Black;
                console.ForegroundColor = ConsoleColor.DarkGray;
                console.WriteLine("      初始化数据库成功。");
            });

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            // 核心库初始化失败时继续启动只会产生半可用实例，并把真实故障延迟到业务请求
            _logger.LogError(ex, "核心数据库初始化失败，应用停止启动。");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}