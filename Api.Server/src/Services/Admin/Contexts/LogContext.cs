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
using Fast.Logging;
using Microsoft.Extensions.DependencyInjection;
using Yitter.IdGenerator;

namespace Fast.Admin.Service;

/// <summary>
/// <see cref="LogContext"/> 日志上下文
/// </summary>
[SuppressSniffer]
public class LogContext
{
    /// <summary>
    /// 添加操作日志（异步）
    /// <para>注：这里即便有问题也不会抛出错误，只会记录日志</para>
    /// </summary>
    /// <param name="logDto"></param>
    public static async void OperateLog(OperateLogDto logDto)
    {
        try
        {
            var _user = FastContext.HttpContext.RequestServices.GetService<IUser>();
            // 获取 AdminLog 库的连接字符串配置
            var connectionConfig = SqlSugarContext.GetConnectionConfig(GlobalContext.LogConnectionSettings);

            // 组装数据
            var operateLogModel = new OperateLogModel
            {
                RecordId = YitIdHelper.NextId(),
                EmployeeNo = _user.EmployeeNo,
                Mobile = _user.Mobile,
                Title = logDto.Title?.GetNVarcharMaxLen(50, true),
                OperateType = logDto.OperateType,
                BizId = logDto.BizId,
                BizNo = logDto.BizNo,
                Description = logDto.Description?.GetNVarcharMaxLen(500, true),
                DepartmentId = _user.DepartmentId,
                DepartmentName = _user.DepartmentName,
                CreatedUserId = _user.EmployeeId,
                CreatedUserName = _user.EmployeeName,
                CreatedTime = DateTime.Now
            };
            operateLogModel.RecordCreate(FastContext.HttpContext);

            _ = Task.Run(async () =>
            {
                try
                {
                    // 这里不能使用Aop
                    var db = new SqlSugarClient(connectionConfig);

                    // 异步不等待
                    await db.Insertable(operateLogModel)
                        .SplitTable()
                        .ExecuteCommandAsync();
                }
                catch (Exception ex)
                {
                    Log.Error($"添加操作日志（OperateLog），执行 Sql 错误：{ex.Message}", ex);
                }
            });
        }
        catch (Exception ex)
        {
            Log.Error("添加操作日志（OperateLog）错误！", ex);
        }
    }
}

public class OperateLogDto
{
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    public OperateLogTypeEnum OperateType { get; set; }

    /// <summary>
    /// 业务Id
    /// </summary>
    public long? BizId { get; set; }

    /// <summary>
    /// 业务编码
    /// </summary>
    public string BizNo { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }
}