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
using Fast.Admin.Service.Complaint.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Service.Complaint;

/// <summary>
/// <see cref="ComplaintService"/> 投诉服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Admin, Name = "Complaint")]
public class ComplaintService : IDynamicApplication
{
    private readonly IUser _user;
    private readonly ISqlSugarRepository<ComplaintModel> _repository;

    public ComplaintService(IUser user, ISqlSugarRepository<ComplaintModel> repository)
    {
        _user = user;
        _repository = repository;
    }

    /// <summary>
    /// 获取投诉工单分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取投诉工单分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.Complaint.Paged)]
    public async Task<PagedResult<QueryComplaintPagedOutput>> QueryComplaintPaged(QueryComplaintPagedInput input)
    {
        return await _repository.Entities.WhereIF(input.ComplaintType != null, wh => wh.ComplaintType == input.ComplaintType)
            .OrderByIF(input.IsOrderBy, ob => ob.CreatedTime, OrderByType.Desc)
            .Select(sl => new QueryComplaintPagedOutput
            {
                ComplaintId = sl.ComplaintId,
                OpenId = sl.OpenId,
                NickName = sl.NickName,
                ComplaintType = sl.ComplaintType,
                Mobile = sl.Mobile,
                ContactPhone = sl.ContactPhone,
                ContactEmail = sl.ContactEmail,
                Description = sl.Description,
                AttachmentImages = sl.AttachmentImages,
                HandleTime = sl.HandleTime,
                HandleDescription = sl.HandleDescription,
                Remark = sl.Remark,
                CreatedTime = sl.CreatedTime,
                RowVersion = sl.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取投诉详情
    /// </summary>
    /// <param name="complaintId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取投诉详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Complaint.Detail)]
    public async Task<QueryComplaintPagedOutput> QueryComplaintDetail([Required(ErrorMessage = "投诉Id不能为空")] long? complaintId)
    {
        var result = await _repository.Entities.Where(wh => wh.ComplaintId == complaintId)
            .Select(sl => new QueryComplaintPagedOutput
            {
                ComplaintId = sl.ComplaintId,
                OpenId = sl.OpenId,
                NickName = sl.NickName,
                ComplaintType = sl.ComplaintType,
                Mobile = sl.Mobile,
                ContactPhone = sl.ContactPhone,
                ContactEmail = sl.ContactEmail,
                Description = sl.Description,
                AttachmentImages = sl.AttachmentImages,
                HandleTime = sl.HandleTime,
                HandleDescription = sl.HandleDescription,
                Remark = sl.Remark,
                CreatedTime = sl.CreatedTime,
                RowVersion = sl.RowVersion
            })
            .SingleAsync();

        if (result == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        return result;
    }

    /// <summary>
    /// 添加投诉
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加投诉", HttpRequestActionEnum.Add)]
    public async Task AddComplaint(AddComplaintInput input)
    {
        // 查询应用信息
        var applicationModel = await ApplicationContext.GetApplication(GlobalContext.Origin);

        if (applicationModel.AppType != GlobalContext.DeviceType)
        {
            throw new UserFriendlyException("应用类型不匹配！");
        }

        var complaintModel = new ComplaintModel
        {
            OpenId = applicationModel.OpenId,
            UserId = _user.EmployeeId,
            NickName = _user.NickName,
            ComplaintType = input.ComplaintType,
            Mobile = _user.Mobile,
            ContactPhone = input.ContactPhone,
            ContactEmail = input.ContactEmail,
            Description = input.Description,
            AttachmentImages = input.AttachmentImages
        };
        await _repository.InsertAsync(complaintModel);
    }

    /// <summary>
    /// 处理投诉
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("处理投诉", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Complaint.Handle)]
    public async Task HandleComplaint(HandleComplaintInput input)
    {
        var complaintModel = await _repository.SingleOrDefaultAsync(input.ComplaintId);
        if (complaintModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        complaintModel.HandleTime = DateTime.Now;
        complaintModel.HandleDescription = input.HandleDescription;
        complaintModel.Remark = input.Remark;
        complaintModel.RowVersion = input.RowVersion;

        await _repository.UpdateAsync(complaintModel);
    }
}