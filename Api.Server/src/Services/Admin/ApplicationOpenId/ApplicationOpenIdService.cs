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
using Fast.Admin.Service.ApplicationOpenId.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SKIT.FlurlHttpClient.Wechat.Api;
using SKIT.FlurlHttpClient.Wechat.Api.Models;

namespace Fast.Admin.Service.ApplicationOpenId;

/// <summary>
/// <see cref="ApplicationOpenIdService"/> 应用标识服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Admin, Name = "applicationOpenId")]
public class ApplicationOpenIdService : IDynamicApplication
{
    private readonly IUser _user;
    private readonly ISqlSugarRepository<ApplicationOpenIdModel> _repository;

    public ApplicationOpenIdService(IUser user, ISqlSugarRepository<ApplicationOpenIdModel> repository)
    {
        _user = user;
        _repository = repository;
    }

    /// <summary>
    /// 获取应用标识分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取应用标识分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.AppOpenId.Paged)]
    public async Task<PagedResult<QueryApplicationOpenIdPagedOutput>> QueryApplicationOpenIdPaged(
        QueryApplicationOpenIdPagedInput input)
    {
        return await _repository.Entities.WhereIF(input.AppType != null, wh => wh.AppType == input.AppType)
            .WhereIF(input.EnvironmentType != null, wh => wh.EnvironmentType == input.EnvironmentType)
            .OrderByIF(input.IsOrderBy, ob => ob.CreatedTime, OrderByType.Desc)
            .Select(sl => new QueryApplicationOpenIdPagedOutput
            {
                RecordId = sl.RecordId,
                OpenId = sl.OpenId,
                AppType = sl.AppType,
                EnvironmentType = sl.EnvironmentType,
                WeChatMerchantNo = sl.WeChatMerchantNo,
                AlipayMerchantNo = sl.AlipayMerchantNo,
                WeChatAccessTokenRefreshTime = sl.WeChatAccessTokenRefreshTime,
                WeChatJsApiTicketRefreshTime = sl.WeChatJsApiTicketRefreshTime,
                Remark = sl.Remark,
                DepartmentName = sl.DepartmentName,
                CreatedUserName = sl.CreatedUserName,
                CreatedTime = sl.CreatedTime,
                UpdatedUserName = sl.UpdatedUserName,
                UpdatedTime = sl.UpdatedTime,
                RowVersion = sl.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取应用标识详情
    /// </summary>
    /// <param name="recordId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取应用标识详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.AppOpenId.Detail)]
    public async Task<QueryApplicationOpenIdDetailOutput> QueryApplicationOpenIdDetail(
        [Required(ErrorMessage = "记录Id不能为空")] long? recordId)
    {
        var result = await _repository.Entities.Where(wh => wh.RecordId == recordId)
            .Select(sl => new QueryApplicationOpenIdDetailOutput
            {
                RecordId = sl.RecordId,
                OpenId = sl.OpenId,
                AppType = sl.AppType,
                OpenSecret = sl.OpenSecret,
                EnvironmentType = sl.EnvironmentType,
                WeChatMerchantId = sl.WeChatMerchantId,
                WeChatMerchantNo = sl.WeChatMerchantNo,
                AlipayMerchantId = sl.AlipayMerchantId,
                AlipayMerchantNo = sl.AlipayMerchantNo,
                WeChatAccessTokenRefreshTime = sl.WeChatAccessTokenRefreshTime,
                WeChatJsApiTicketRefreshTime = sl.WeChatJsApiTicketRefreshTime,
                Remark = sl.Remark,
                DepartmentName = sl.DepartmentName,
                CreatedUserName = sl.CreatedUserName,
                CreatedTime = sl.CreatedTime,
                UpdatedUserName = sl.UpdatedUserName,
                UpdatedTime = sl.UpdatedTime,
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
    /// 添加应用标识
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加应用标识", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.AppOpenId.Add)]
    public async Task AddApplicationOpenId(AddApplicationOpenIdInput input)
    {
        if (await _repository.AnyAsync(a => a.OpenId == input.OpenId))
        {
            throw new UserFriendlyException("应用标识重复！");
        }

        var applicationOpenIdModel = new ApplicationOpenIdModel
        {
            OpenId = input.OpenId,
            AppType = input.AppType,
            OpenSecret = input.OpenSecret,
            EnvironmentType = input.EnvironmentType,
            WeChatMerchantId = input.WeChatMerchantId,
            WeChatMerchantNo = input.WeChatMerchantNo,
            AlipayMerchantId = input.AlipayMerchantId,
            AlipayMerchantNo = input.AlipayMerchantNo,
            Remark = input.Remark
        };

        if (!string.IsNullOrWhiteSpace(input.OpenSecret))
        {
            var apiClient = WechatApiClientBuilder
                .Create(new WechatApiClientOptions {AppId = input.OpenId, AppSecret = input.OpenSecret})
                .Build();
            var response = await apiClient.ExecuteCgibinStableTokenAsync(new CgibinStableTokenRequest());
            if (!response.IsSuccessful())
            {
                throw new UserFriendlyException(
                    $"调用刷新AccessToken接口失败。ErrorCode：{response.ErrorCode}。ErrorMessage：{response.ErrorMessage}");
            }

            applicationOpenIdModel.WeChatAccessToken = response.AccessToken;
            applicationOpenIdModel.WeChatAccessTokenExpiresIn = response.ExpiresIn;
            applicationOpenIdModel.WeChatAccessTokenRefreshTime = DateTime.Now;

            if (input.AppType == AppEnvironmentEnum.WeChatServiceAccount)
            {
                var ticketResponse = await apiClient.ExecuteCgibinTicketGetTicketAsync(new CgibinTicketGetTicketRequest
                {
                    AccessToken = response.AccessToken
                });
                if (!ticketResponse.IsSuccessful())
                {
                    throw new UserFriendlyException(
                        $"调用获取Ticket接口失败。ErrorCode：{ticketResponse.ErrorCode}。ErrorMessage：{ticketResponse.ErrorMessage}");
                }

                applicationOpenIdModel.WeChatJsApiTicket = ticketResponse.Ticket;
                applicationOpenIdModel.WeChatJsApiTicketExpiresIn = ticketResponse.ExpiresIn;
                applicationOpenIdModel.WeChatJsApiTicketRefreshTime = DateTime.Now;
            }
        }

        await _repository.InsertAsync(applicationOpenIdModel);
        // 删除缓存
        await ApplicationContext.DeleteApplication(applicationOpenIdModel.OpenId);
    }

    /// <summary>
    /// 编辑应用标识
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑应用标识", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.AppOpenId.Edit)]
    public async Task EditApplicationOpenId(EditApplicationOpenIdInput input)
    {
        if (await _repository.AnyAsync(a => a.OpenId == input.OpenId && a.RecordId != input.RecordId))
        {
            throw new UserFriendlyException("应用标识重复！");
        }

        var applicationOpenIdModel = await _repository.SingleOrDefaultAsync(input.RecordId);
        if (applicationOpenIdModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        applicationOpenIdModel.OpenId = input.OpenId;
        applicationOpenIdModel.AppType = input.AppType;
        applicationOpenIdModel.OpenSecret = input.OpenSecret;
        applicationOpenIdModel.EnvironmentType = input.EnvironmentType;
        applicationOpenIdModel.WeChatMerchantId = input.WeChatMerchantId;
        applicationOpenIdModel.WeChatMerchantNo = input.WeChatMerchantNo;
        applicationOpenIdModel.AlipayMerchantId = input.AlipayMerchantId;
        applicationOpenIdModel.AlipayMerchantNo = input.AlipayMerchantNo;
        applicationOpenIdModel.Remark = input.Remark;
        applicationOpenIdModel.RowVersion = input.RowVersion;

        if (!string.IsNullOrWhiteSpace(input.OpenSecret))
        {
            var apiClient = WechatApiClientBuilder
                .Create(new WechatApiClientOptions {AppId = input.OpenId, AppSecret = input.OpenSecret})
                .Build();
            var response = await apiClient.ExecuteCgibinStableTokenAsync(new CgibinStableTokenRequest());
            if (!response.IsSuccessful())
            {
                throw new UserFriendlyException(
                    $"调用刷新AccessToken接口失败。ErrorCode：{response.ErrorCode}。ErrorMessage：{response.ErrorMessage}");
            }

            applicationOpenIdModel.WeChatAccessToken = response.AccessToken;
            applicationOpenIdModel.WeChatAccessTokenExpiresIn = response.ExpiresIn;
            applicationOpenIdModel.WeChatAccessTokenRefreshTime = DateTime.Now;

            if (input.AppType == AppEnvironmentEnum.WeChatServiceAccount)
            {
                var ticketResponse = await apiClient.ExecuteCgibinTicketGetTicketAsync(new CgibinTicketGetTicketRequest
                {
                    AccessToken = response.AccessToken
                });
                if (!ticketResponse.IsSuccessful())
                {
                    throw new UserFriendlyException(
                        $"调用获取Ticket接口失败。ErrorCode：{ticketResponse.ErrorCode}。ErrorMessage：{ticketResponse.ErrorMessage}");
                }

                applicationOpenIdModel.WeChatJsApiTicket = ticketResponse.Ticket;
                applicationOpenIdModel.WeChatJsApiTicketExpiresIn = ticketResponse.ExpiresIn;
                applicationOpenIdModel.WeChatJsApiTicketRefreshTime = DateTime.Now;
            }
        }

        await _repository.UpdateAsync(applicationOpenIdModel);
        // 删除缓存
        await ApplicationContext.DeleteApplication(applicationOpenIdModel.OpenId);
    }

    /// <summary>
    /// 删除应用标识
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除应用标识", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.AppOpenId.Delete)]
    public async Task DeleteApplicationOpenId(RecordIdInput input)
    {
        var applicationOpenIdModel = await _repository.SingleOrDefaultAsync(input.RecordId);
        if (applicationOpenIdModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        await _repository.DeleteAsync(applicationOpenIdModel);
        // 删除缓存
        await ApplicationContext.DeleteApplication(applicationOpenIdModel.OpenId);
    }
}