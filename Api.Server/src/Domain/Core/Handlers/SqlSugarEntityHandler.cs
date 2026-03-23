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

using Fast.AdminLog.Entity;
using Fast.AdminLog.Enum;
using Fast.SqlSugar;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.Core;

/// <summary>
/// <see cref="SqlSugarEntityHandler"/> Sugar实体处理
/// </summary>
public class SqlSugarEntityHandler : ISqlSugarEntityHandler
{
    /// <summary>
    /// 授权用户
    /// </summary>
    private readonly IUser _user;

    /// <summary>
    /// 请求上下文
    /// </summary>
    private readonly HttpContext _httpContext;

    /// <summary>
    /// 日志
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// <see cref="SqlSugarEntityHandler"/> Sugar实体处理
    /// </summary>
    /// <param name="user"><see cref="IUser"/> 授权用户</param>
    /// <param name="httpContextAccessor"><see cref="IHttpContextAccessor"/> 请求上下文访问器</param>
    /// <param name="logger"><see cref="ILogger"/> 日志</param>
    public SqlSugarEntityHandler(IUser user, IHttpContextAccessor httpContextAccessor, ILogger<ISqlSugarEntityHandler> logger)
    {
        _user = user;
        _httpContext = httpContextAccessor.HttpContext;
        _logger = logger;
    }

    /// <summary>根据实体类型获取连接字符串</summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="sqlSugarClient"><see cref="T:ISqlSugarClient" /> 默认库SqlSugar客户端</param>
    /// <param name="sugarDbType">实体类头部的 <see cref="T:Fast.SqlSugar.SugarDbTypeAttribute" /> 特性，如果不存在可能为空</param>
    /// <param name="entityType"><see cref="T:System.Type" /> 实体类型</param>
    /// <returns></returns>
    public async Task<ConnectionSettingsOptions> GetConnectionSettings<TEntity>(ISqlSugarClient sqlSugarClient,
        SugarDbTypeAttribute sugarDbType, Type entityType)
    {
        var databaseTypeStr = sugarDbType.Type?.ToString();
        if (string.IsNullOrWhiteSpace(databaseTypeStr))
            return null;

        var databaseType = Enum.Parse<DatabaseTypeEnum>(databaseTypeStr, true);

        switch (databaseType)
        {
            case DatabaseTypeEnum.Admin:
                return SqlSugarContext.ConnectionSettings;
            case DatabaseTypeEnum.AdminLog:
                return GlobalContext.LogConnectionSettings;
            default:
                throw new SqlSugarException("未知的 Database 类型！");
        }
    }

    /// <summary>执行Sql</summary>
    /// <param name="rawSql"><see cref="T:System.String" /> 原始Sql语句</param>
    /// <param name="parameters"><see cref="T:SugarParameter" /> Sql参数</param>
    /// <param name="executeTime"><see cref="T:System.TimeSpan" /> 执行时间</param>
    /// <param name="handlerSql"><see cref="T:System.String" /> 参数化处理后的Sql语句</param>
    /// <returns></returns>
    public async Task ExecuteAsync(string rawSql, SugarParameter[] parameters, TimeSpan executeTime, string handlerSql)
    {
        // 获取 CenterLog 库的连接字符串配置
        var connectionConfig = SqlSugarContext.GetConnectionConfig(GlobalContext.LogConnectionSettings);

        // 组装数据
        var sqlExecutionLogModel = new SqlExecutionLogModel
        {
            RecordId = YitIdHelper.NextId(),
            AccountId = _user.AccountId,
            Mobile = _user.Mobile,
            NickName = _user.NickName,
            ExecuteSeconds = executeTime.TotalSeconds,
            PureSql = handlerSql,
            DepartmentId = _user.DepartmentId,
            DepartmentName = _user.DepartmentName,
            CreatedUserId = _user.EmployeeId,
            CreatedUserName = _user.EmployeeName,
            CreatedTime = DateTime.Now
        };
        sqlExecutionLogModel.RecordCreate(_httpContext);

        _ = Task.Run(async () =>
        {
            try
            {
                // 这里不能使用Aop
                var db = new SqlSugarClient(connectionConfig);

                // 异步不等待
                await db.Insertable(sqlExecutionLogModel)
                    .SplitTable()
                    .ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"SqlSugar Aop 执行Sql，保存失败；{ex.Message}");
            }
        });

        await Task.CompletedTask;
    }

    /// <summary>执行Sql超时</summary>
    /// <param name="fileName"><see cref="T:System.String" /> 文件名称</param>
    /// <param name="fileLine"><see cref="T:System.Int32" /> 文件行数</param>
    /// <param name="methodName"><see cref="T:System.String" /> 方法名称</param>
    /// <param name="rawSql"><see cref="T:System.String" /> 未处理的Sql语句</param>
    /// <param name="parameters"><see cref="T:SugarParameter" /> Sql参数</param>
    /// <param name="executeTime"><see cref="T:System.TimeSpan" /> 执行时间</param>
    /// <param name="handlerSql"><see cref="T:System.String" /> 参数化处理后的Sql语句</param>
    /// <param name="message"><see cref="T:System.String" /></param>
    /// <returns></returns>
    public async Task ExecuteTimeoutAsync(string fileName, int fileLine, string methodName, string rawSql,
        SugarParameter[] parameters, TimeSpan executeTime, string handlerSql, string message)
    {
        // 获取 CenterLog 库的连接字符串配置
        var connectionConfig = SqlSugarContext.GetConnectionConfig(GlobalContext.LogConnectionSettings);

        // 组装数据
        var sqlTimeoutLogModel = new SqlTimeoutLogModel
        {
            RecordId = YitIdHelper.NextId(),
            AccountId = _user.AccountId,
            Mobile = _user.Mobile,
            NickName = _user.NickName,
            FileName = fileName,
            FileLine = fileLine,
            MethodName = methodName,
            TimeoutSeconds = executeTime.TotalSeconds,
            PureSql = handlerSql,
            DepartmentId = _user.DepartmentId,
            DepartmentName = _user.DepartmentName,
            CreatedUserId = _user.EmployeeId,
            CreatedUserName = _user.EmployeeName,
            CreatedTime = DateTime.Now
        };
        sqlTimeoutLogModel.RecordCreate(_httpContext);

        _ = Task.Run(async () =>
        {
            try
            {
                // 这里不能使用Aop
                var db = new SqlSugarClient(connectionConfig);

                // 异步不等待
                await db.Insertable(sqlTimeoutLogModel)
                    .ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"SqlSugar Aop 执行Sql超时，保存失败；{ex.Message}");
            }
        });

        await Task.CompletedTask;
    }

    /// <summary>执行Sql差异</summary>
    /// <param name="diffType"><see cref="T:DiffType" /> 差异类型</param>
    /// <param name="tableName"><see cref="T:System.String" /> 表名称</param>
    /// <param name="tableDescription"><see cref="T:System.String" /> 表描述</param>
    /// <param name="businessData"><see cref="object"/> 业务数据</param>
    /// <param name="beforeColumnList"><see cref="T:System.String" /> 执行前列信息</param>
    /// <param name="afterColumnList"><see cref="T:System.String" /> 执行后列信息</param>
    /// <param name="rawSql"><see cref="T:System.String" /> 原始Sql语句</param>
    /// <param name="parameters"><see cref="T:SugarParameter" /> Sql参数</param>
    /// <param name="executeTime"><see cref="T:System.TimeSpan" /> 执行时间</param>
    /// <param name="handlerSql"><see cref="T:System.String" /> 参数化处理后的Sql语句</param>
    /// <returns></returns>
    public async Task ExecuteDiffLogAsync(DiffType diffType, string tableName, string tableDescription, object businessData,
        List<List<DiffLogColumnInfo>> beforeColumnList, List<List<DiffLogColumnInfo>> afterColumnList, string rawSql,
        SugarParameter[] parameters, TimeSpan? executeTime, string handlerSql)
    {
        // 获取 CenterLog 库的连接字符串配置
        var connectionConfig = SqlSugarContext.GetConnectionConfig(GlobalContext.LogConnectionSettings);

        var diffLogType = diffType switch
        {
            DiffType.insert => DiffLogTypeEnum.Insert,
            DiffType.update => DiffLogTypeEnum.Update,
            DiffType.delete => DiffLogTypeEnum.Delete,
            _ => DiffLogTypeEnum.Unknown
        };

        // 组装数据
        var sqlDiffLogModel = new SqlDiffLogModel
        {
            RecordId = YitIdHelper.NextId(),
            AccountId = _user.AccountId,
            Mobile = _user.Mobile,
            NickName = _user.NickName,
            DiffType = diffLogType,
            TableName = tableName,
            TableDescription = tableDescription,
            BeforeColumnList = beforeColumnList,
            AfterColumnList = afterColumnList,
            ExecuteSeconds = executeTime?.TotalSeconds,
            PureSql = handlerSql,
            DepartmentId = _user.DepartmentId,
            DepartmentName = _user.DepartmentName,
            CreatedUserId = _user.EmployeeId,
            CreatedUserName = _user.EmployeeName,
            CreatedTime = DateTime.Now
        };
        sqlDiffLogModel.RecordCreate(_httpContext);

        _ = Task.Run(async () =>
        {
            try
            {
                // 这里不能使用Aop
                var db = new SqlSugarClient(connectionConfig);

                // 异步不等待
                await db.Insertable(sqlDiffLogModel)
                    .SplitTable()
                    .ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"SqlSugar Aop 执行Sql差异，保存失败；{ex.Message}");
            }
        });

        await Task.CompletedTask;
    }

    /// <summary>执行Sql错误</summary>
    /// <param name="fileName"><see cref="T:System.String" /> 文件名称</param>
    /// <param name="fileLine"><see cref="T:System.Int32" /> 文件行数</param>
    /// <param name="methodName"><see cref="T:System.String" /> 方法名称</param>
    /// <param name="rawSql"><see cref="T:System.String" /> 原始Sql语句</param>
    /// <param name="parameters"><see cref="T:SugarParameter" /> Sql参数</param>
    /// <param name="handlerSql"><see cref="T:System.String" /> 参数化处理后的Sql语句</param>
    /// <param name="exception"><see cref="T:SqlSugarException" /> 异常信息</param>
    /// <returns></returns>
    public async Task ExecuteErrorAsync(string fileName, int fileLine, string methodName, string rawSql,
        SugarParameter[] parameters, string handlerSql, SqlSugarException exception)
    {
        // 获取 CenterLog 库的连接字符串配置
        var connectionConfig = SqlSugarContext.GetConnectionConfig(GlobalContext.LogConnectionSettings);

        // 组装数据
        var sqlExceptionLogModel = new SqlExceptionLogModel
        {
            RecordId = YitIdHelper.NextId(),
            AccountId = _user.AccountId,
            Mobile = _user.Mobile,
            NickName = _user.NickName,
            FileName = fileName,
            FileLine = fileLine,
            MethodName = methodName,
            Message = exception.Message,
            Source = exception.Source,
            StackTrace = exception.StackTrace,
            PureSql = handlerSql,
            DepartmentId = _user.DepartmentId,
            DepartmentName = _user.DepartmentName,
            CreatedUserId = _user.EmployeeId,
            CreatedUserName = _user.EmployeeName,
            CreatedTime = DateTime.Now
        };
        sqlExceptionLogModel.RecordCreate(_httpContext);

        _ = Task.Run(async () =>
        {
            try
            {
                // 这里不能使用Aop
                var db = new SqlSugarClient(connectionConfig);

                // 异步不等待
                await db.Insertable(sqlExceptionLogModel)
                    .ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"SqlSugar Aop 执行Sql错误，保存失败；{ex.Message}");
            }
        });

        await Task.CompletedTask;
    }

    /// <summary>是否为超级管理员</summary>
    /// <returns></returns>
    public bool IsSuperAdmin()
    {
        return _user.IsSuperAdmin;
    }

    /// <summary>是否为管理员</summary>
    /// <returns></returns>
    public bool IsAdmin()
    {
        return _user.IsAdmin;
    }

    /// <summary>指派租户Id</summary>
    /// <returns></returns>
    public long? AssignTenantId()
    {
        return null;
    }

    /// <summary>指派部门Id</summary>
    /// <returns></returns>
    public long? AssignDepartmentId()
    {
        return _user.DepartmentId;
    }

    /// <summary>指派部门名称</summary>
    /// <returns></returns>
    public string AssignDepartmentName()
    {
        return _user.DepartmentName;
    }

    /// <summary>指派用户Id</summary>
    /// <returns></returns>
    public long? AssignUserId()
    {
        return _user.EmployeeId;
    }

    /// <summary>指派用户名称</summary>
    /// <returns></returns>
    public string AssignUserName()
    {
        return string.IsNullOrWhiteSpace(_user.EmployeeName) ? _user.NickName : _user.EmployeeName;
    }
}