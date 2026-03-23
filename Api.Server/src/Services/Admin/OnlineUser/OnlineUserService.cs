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
using Fast.Admin.Service.OnlineUser.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Fast.Admin.Service.OnlineUser;

/// <summary>
/// <see cref="OnlineUserService"/> 在线用户服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Admin, Name = "onlineUser")]
public class OnlineUserService : IDynamicApplication
{
    private readonly IUser _user;
    private readonly ISqlSugarRepository<OnlineUserModel> _repository;
    private readonly IHubContext<ChatHub, IChatClient> _hubContext;

    public OnlineUserService(IUser user, ISqlSugarRepository<OnlineUserModel> repository,
        IHubContext<ChatHub, IChatClient> hubContext)
    {
        _user = user;
        _repository = repository;
        _hubContext = hubContext;
    }

    /// <summary>
    /// 获取在线用户分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取在线用户分页列表", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.OnlineUser.Paged)]
    public async Task<PagedResult<OnlineUserModel>> QueryOnlineUserPaged(QueryOnlineUserPagedInput input)
    {
        return await _repository.Entities.WhereIF(input.DeviceType != null, wh => wh.DeviceType == input.DeviceType)
            .WhereIF(input.AccountId != null, wh => wh.AccountId == input.AccountId)
            .WhereIF(input.EmployeeId != null, wh => wh.EmployeeId == input.EmployeeId)
            .OrderByIF(input.IsOrderBy, ob => ob.IsOnline, OrderByType.Desc)
            .OrderByIF(input.IsOrderBy, ob => ob.LastLoginTime, OrderByType.Desc)
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 强制下线
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("强制下线", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.OnlineUser.ForceOffline)]
    public async Task ForceOffline(ForceOfflineInput input)
    {
        await _hubContext.Clients.Clients(input.ConnectionId)
            .ForceOffline(new ForceOfflineOutput
            {
                IsAdmin = _user.IsSuperAdmin || _user.IsAdmin,
                NickName = _user.NickName,
                EmployeeNo = _user.EmployeeNo,
                OfflineTime = DateTime.Now
            });
    }
}