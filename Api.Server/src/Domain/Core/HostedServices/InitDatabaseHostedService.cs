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

using System.Text;
using Fast.Admin.Entity;
using Fast.Admin.Enum;
using Fast.SqlSugar;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.Core;

/// <summary>
/// <see cref="InitDatabaseHostedService"/> 初始化 Database 托管服务
/// </summary>
[Order(101)]
public class InitDatabaseHostedService : IHostedService
{
    /// <summary>
    /// 日志
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// <see cref="InitDatabaseHostedService"/> 初始化 Database 托管服务
    /// </summary>
    /// <param name="logger"><see cref="ILogger"/> 日志</param>
    public InitDatabaseHostedService(ILogger<InitDatabaseHostedService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Start operation.</returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var db = new SqlSugarClient(SqlSugarContext.GetConnectionConfig(SqlSugarContext.ConnectionSettings));

            // 创建库
            db.DbMaintenance.CreateDatabase();

            // 查询核心表是否存在
            if (db.DbMaintenance.IsAnyTable<AccountModel>())
                return;
            // 加载Aop
            SugarEntityFilter.LoadSugarAop(FastContext.HostEnvironment.IsDevelopment(), db);

            {
                var logSb = new StringBuilder();
                logSb.Append("\u001b[40m\u001b[1m\u001b[32m");
                logSb.Append("info");
                logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                logSb.Append(": ");
                logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                logSb.Append(Environment.NewLine);
                logSb.Append("\u001b[40m\u001b[90m");
                logSb.Append("      ");
                logSb.Append("开始初始化数据库...");
                logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                Console.WriteLine(logSb.ToString());
            }

            // 获取所有不分表的Model类型
            var tableTypes = SqlSugarContext.SqlSugarEntityList.Where(wh => !wh.IsSplitTable)
                .Where(wh => wh.SugarDbType == null || (DatabaseTypeEnum) wh.SugarDbType == DatabaseTypeEnum.Admin)
                .Select(sl => sl.EntityType)
                .ToArray();
            // 获取所有分表的Model类型
            var splitTableTypes = SqlSugarContext.SqlSugarEntityList.Where(wh => wh.IsSplitTable)
                .Where(wh => wh.SugarDbType == null || (DatabaseTypeEnum) wh.SugarDbType == DatabaseTypeEnum.Admin)
                .Select(sl => sl.EntityType)
                .ToArray();

            // 创建表
            db.CodeFirst.InitTables(tableTypes);
            db.CodeFirst.SplitTables()
                .InitTables(splitTableTypes);

            var dateTime = new DateTime(2025, 01, 01);

            #region 超级管理员

            var superAdminAccountModel = new AccountModel
            {
                AccountId = CommonConst.Default.SuperAdminAccountId,
                Mobile = "15580001115",
                Password = CryptoUtil.SHA1Encrypt(CommonConst.Default.AdminPassword)
                    .ToUpper(),
                NickName = "小方",
                Avatar = "https://gitee.com/FastDotnet/Fast.Admin/raw/master/Fast.png",
                Status = CommonStatusEnum.Enable,
                Sex = GenderEnum.Unknown,
                Birthday = new DateTime(1998, 01, 01),
                CreatedTime = dateTime
            };
            superAdminAccountModel = await db.Insertable(superAdminAccountModel)
                .ExecuteReturnEntityAsync();

            var adminAccountModel = new AccountModel
            {
                AccountId = YitIdHelper.NextId(),
                Mobile = "15580001111",
                Password = CryptoUtil.SHA1Encrypt(CommonConst.Default.Password)
                    .ToUpper(),
                NickName = "管理员",
                Avatar = "https://gitee.com/FastDotnet/Fast.Admin/raw/master/Fast.png",
                Status = CommonStatusEnum.Enable,
                Sex = GenderEnum.Unknown,
                Birthday = new DateTime(1998, 01, 01),
                CreatedTime = dateTime
            };
            adminAccountModel = await db.Insertable(adminAccountModel)
                .ExecuteReturnEntityAsync();

            var superAdminUserId = YitIdHelper.NextId();
            var adminUserId = YitIdHelper.NextId();
            var robotUserId = YitIdHelper.NextId();
            await db.Insertable(new List<EmployeeModel>
                {
                    new()
                    {
                        EmployeeId = superAdminUserId,
                        AccountId = superAdminAccountModel.AccountId,
                        UserType = UserTypeEnum.SuperAdmin,
                        EmployeeNo = "SuperAdmin",
                        EmployeeName = "超级管理员",
                        Mobile = "15580001115",
                        Status = EmployeeStatusEnum.Formal,
                        Sex = GenderEnum.Man,
                        IdPhoto = "https://gitee.com/FastDotnet/Fast.Admin/raw/master/Fast.png",
                        EntryDate = new DateTime(2000, 1, 1),
                        CreatedTime = dateTime
                    },
                    new()
                    {
                        EmployeeId = adminUserId,
                        AccountId = adminAccountModel.AccountId,
                        UserType = UserTypeEnum.Admin,
                        EmployeeNo = "Admin",
                        EmployeeName = "管理员",
                        Mobile = "15580001111",
                        Status = EmployeeStatusEnum.Formal,
                        Sex = GenderEnum.Man,
                        IdPhoto = "https://gitee.com/FastDotnet/Fast.Admin/raw/master/Fast.png",
                        EntryDate = new DateTime(2000, 1, 1),
                        CreatedTime = dateTime
                    },
                    new()
                    {
                        EmployeeId = robotUserId,
                        AccountId = -99,
                        UserType = UserTypeEnum.Robot,
                        EmployeeNo = "Robot",
                        EmployeeName = "机器人",
                        Mobile = "",
                        Status = EmployeeStatusEnum.Resigned,
                        Sex = GenderEnum.Unknown,
                        IdPhoto = "https://gitee.com/FastDotnet/Fast.Admin/raw/master/Fast.png",
                        EntryDate = new DateTime(2000, 1, 1),
                        CreatedTime = dateTime
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
                        Type = PasswordTypeEnum.SHA1,
                        Password = CryptoUtil.SHA1Encrypt(CommonConst.Default.AdminPassword)
                            .ToUpper(),
                        CreatedTime = dateTime
                    },
                    new()
                    {
                        AccountId = adminAccountModel.AccountId,
                        OperationType = PasswordOperationTypeEnum.Create,
                        Type = PasswordTypeEnum.SHA1,
                        Password = CryptoUtil.SHA1Encrypt(CommonConst.Default.Password)
                            .ToUpper(),
                        CreatedTime = dateTime
                    }
                })
                .ExecuteCommandAsync(cancellationToken);

            #endregion

            // 初始化公司（组织架构）
            await db.Insertable(new OrganizationModel
                {
                    OrgId = YitIdHelper.NextId(),
                    ParentId = 0,
                    ParentIds = [0],
                    ParentNames = [],
                    OrgName = "FastDotNet工作室",
                    OrgCode = "fa_hq",
                    Contacts = "超级管理员",
                    Phone = "15580001115",
                    Email = "2875616188@qq.com",
                    Sort = 1,
                    DataPublic = false,
                    Remark = null
                })
                .ExecuteCommandAsync(cancellationToken);

            // 初始化管理员角色
            await db.Insertable(new RoleModel
                {
                    RoleId = YitIdHelper.NextId(),
                    RoleType = RoleTypeEnum.Admin,
                    IsSystemMenu = true,
                    RoleName = "管理员",
                    RoleCode = "manager_role",
                    Sort = 1,
                    DataScopeType = DataScopeTypeEnum.All,
                    AssignableRoleIds = [],
                    Remark = null
                })
                .ExecuteCommandAsync(cancellationToken);

            // 配置
            await ConfigSeedData.SystemConfigSeedData(db, dateTime);

            // 系统序号规则
            await SerialSeedData.SeedData(db);

            // 菜单
            await MenuSeedData.DefaultMenuSeedData(db, dateTime);

            {
                var logSb = new StringBuilder();
                logSb.Append("\u001b[40m\u001b[1m\u001b[32m");
                logSb.Append("info");
                logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                logSb.Append(": ");
                logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                logSb.Append(Environment.NewLine);
                logSb.Append("\u001b[40m\u001b[90m");
                logSb.Append("      ");
                logSb.Append("初始化数据库成功。");
                logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                Console.WriteLine(logSb.ToString());
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Init database error...");
        }
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Stop operation.</returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}