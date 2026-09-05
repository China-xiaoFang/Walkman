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
using Fast.Center.Service.MessageSendRecord.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Center.Service.MessageSendRecord;

/// <summary>
/// 消息发送记录服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Center, Name = "messageSendRecord")]
[PlatformOnly]
public class MessageSendRecordService : IDynamicApplication
{
    private readonly ISqlSugarRepository<MessageSendRecordModel> _repository;

    public MessageSendRecordService(ISqlSugarRepository<MessageSendRecordModel> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 获取消息发送记录分页列表
    /// </summary>
    [HttpPost]
    [ApiInfo("获取消息发送记录分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.MessageSendRecord.Paged)]
    public async Task<PagedResult<QueryMessageSendRecordPagedOutput>> QueryMessageSendRecordPaged(
        QueryMessageSendRecordPagedInput input)
    {
        return await _repository.Entities.WhereIF(input.Channel != null, wh => wh.Channel == input.Channel)
            .WhereIF(input.IsSuccess != null, wh => wh.IsSuccess == input.IsSuccess)
            .Select(sl => new QueryMessageSendRecordPagedOutput
            {
                RecordId = sl.RecordId,
                Channel = sl.Channel,
                Receiver = sl.Receiver,
                Title = sl.Title,
                IsSuccess = sl.IsSuccess,
                Device = sl.Device,
                OS = sl.OS,
                Browser = sl.Browser,
                Province = sl.Province,
                City = sl.City,
                Ip = sl.Ip,
                CreatedTime = sl.CreatedTime
            })
            .OrderByIF(input.IsOrderBy, ob => ob.CreatedTime, OrderByType.Desc)
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取消息发送记录详情
    /// </summary>
    [HttpGet]
    [ApiInfo("获取消息发送记录详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.MessageSendRecord.Detail)]
    public async Task<QueryMessageSendRecordDetailOutput> QueryMessageSendRecordDetail(
        [Required(ErrorMessage = "记录Id不能为空")] long? recordId)
    {
        var result = await _repository.Entities.Where(wh => wh.RecordId == recordId)
            .Select(sl => new QueryMessageSendRecordDetailOutput
            {
                RecordId = sl.RecordId,
                Channel = sl.Channel,
                Receiver = sl.Receiver,
                Title = sl.Title,
                RecordValue = sl.RecordValue,
                IsSuccess = sl.IsSuccess,
                Device = sl.Device,
                OS = sl.OS,
                Browser = sl.Browser,
                Province = sl.Province,
                City = sl.City,
                Ip = sl.Ip,
                CreatedTime = sl.CreatedTime
            })
            .SingleAsync();

        if (result == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        return result;
    }
}