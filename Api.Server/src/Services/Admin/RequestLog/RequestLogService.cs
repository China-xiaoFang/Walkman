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

using Fast.Admin.Service.RequestLog.Dto;
using Fast.AdminLog.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Service.RequestLog;

/// <summary>
/// <see cref="RequestLogService"/> 请求日志服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Admin, Name = "requestLog")]
public class RequestLogService : IDynamicApplication
{
    private readonly IUser _user;
    private readonly ISqlSugarRepository<RequestLogModel> _repository;

    public RequestLogService(IUser user, ISqlSugarRepository<RequestLogModel> repository)
    {
        _user = user;
        _repository = repository;
    }

    /// <summary>
    /// 获取请求日志分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取请求日志分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.RequestLogPaged), DisabledRequestLog]
    public async Task<PagedResult<RequestLogModel>> QueryRequestLogPaged(QueryRequestLogPagedInput input)
    {
        if (input.SearchTimeList is not {Count: > 1})
        {
            throw new UserFriendlyException("请选择具体的时间范围！");
        }

        var queryable = _repository.Entities.WhereIF(input.AccountId != null, wh => wh.AccountId == input.AccountId)
            .WhereIF(input.IsSuccess != null, wh => wh.IsSuccess == input.IsSuccess)
            .WhereIF(input.OperationAction != null, wh => wh.OperationAction == input.OperationAction)
            .WhereIF(input.RequestMethod != null, wh => wh.RequestMethod == input.RequestMethod);

        if (!_user.IsSuperAdmin && !_user.IsAdmin)
        {
            queryable = queryable.Where(wh => wh.AccountId == _user.AccountId);
        }

        return await queryable.SplitTable()
            .OrderByIF(input.IsOrderBy, ob => ob.CreatedTime, OrderByType.Desc)
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 删除90天前的请求日志
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除90天前的请求日志", HttpRequestActionEnum.Delete)]
    public async Task DeleteRequestLog()
    {
        if (!_user.IsSuperAdmin)
        {
            throw new UserFriendlyException("非超级管理员，禁止操作！");
        }

        var expireDate = DateTime.Now.Date.AddDays(-90);

        await _repository.Deleteable<RequestLogModel>()
            .Where(wh => wh.CreatedTime < expireDate)
            .SplitTable(t => t.Where(wh => wh.Date.Date < expireDate))
            .ExecuteCommandAsync();
    }
}