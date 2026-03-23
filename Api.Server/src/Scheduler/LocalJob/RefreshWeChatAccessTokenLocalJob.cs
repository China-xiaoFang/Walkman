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
using Fast.Admin.Enum;
using Fast.Core;
using SKIT.FlurlHttpClient.Wechat.Api;
using SKIT.FlurlHttpClient.Wechat.Api.Models;
using SqlSugar;

namespace Fast.Scheduler.LocalJob;

/// <summary>
/// <see cref="RefreshWeChatAccessTokenLocalJob"/> 刷新微信 AccessToken 本地作业
/// </summary>
public class RefreshWeChatAccessTokenLocalJob : ISchedulerJob
{
    /// <summary>
    /// 获取本地作业
    /// </summary>
    /// <returns></returns>
    public SchedulerLocalJobInfo GetLocalJob()
    {
        return new SchedulerLocalJobInfo
        {
            JobName = "刷新微信 AccessToken 本地作业",
            JobGroup = SchedulerJobGroupEnum.System,
            BeginTime = new DateTime(1970, 01, 01),
            EndTime = null,
            TriggerType = TriggerTypeEnum.Simple,
            Cron = null,
            Week = null,
            DailyStartTime = null,
            DailyEndTime = null,
            IntervalSecond = 7200,
            RunTimes = null,
            WarnTime = null,
            RetryTimes = null,
            RetryMillisecond = null,
            MailMessage = MailMessageEnum.Error,
            Description = "刷新微信 AccessToken 本地作业，每7200秒执行一次。"
        };
    }

    /// <summary>
    /// 执行作业
    /// </summary>
    /// <param name="serviceProvider"><see cref="IServiceProvider"/> 服务提供者（请求作用域类似于，如果存在 TenantId 则自动注入 IUser 服务）</param>
    /// <param name="db"><see cref="ISqlSugarClient"/> SqlSugar上下文</param>
    /// <param name="logInfo"><see cref="SchedulerJobLocalLogInfo"/> 日志信息</param>
    /// <returns></returns>
    public async Task<string> Execute(IServiceProvider serviceProvider, ISqlSugarClient db, SchedulerJobLocalLogInfo logInfo)
    {
        // 进入方法的一瞬间记录时间
        var dateTime = DateTime.Now;

        // 获取所有微信小程序信息
        var applicationOpenIdList = await db.Queryable<ApplicationOpenIdModel>()
            .Where(wh => (wh.AppType & (AppEnvironmentEnum.MiniProgram | AppEnvironmentEnum.WeChatServiceAccount)) != 0)
            .Where(wh => !string.IsNullOrWhiteSpace(wh.OpenSecret))
            .Select(sl => new {sl.RecordId, sl.AppType, sl.OpenId, sl.OpenSecret})
            .ToListAsync();

        foreach (var item in applicationOpenIdList)
        {
            var apiClient = WechatApiClientBuilder
                .Create(new WechatApiClientOptions {AppId = item.OpenId, AppSecret = item.OpenSecret})
                .Build();
            var response = await apiClient.ExecuteCgibinStableTokenAsync(new CgibinStableTokenRequest {ForceRefresh = true});
            if (!response.IsSuccessful())
            {
                var message = $"调用刷新AccessToken接口失败。ErrorCode：{response.ErrorCode}。ErrorMessage：{response.ErrorMessage}";
                await logInfo.ErrorLog(logInfo.JobName, null, message);
                return message;
            }

            await db.Updateable<ApplicationOpenIdModel>()
                .SetColumns(_ => new ApplicationOpenIdModel
                {
                    WeChatAccessToken = response.AccessToken,
                    WeChatAccessTokenExpiresIn = response.ExpiresIn,
                    WeChatAccessTokenRefreshTime = dateTime
                })
                .Where(wh => wh.RecordId == item.RecordId)
                .ExecuteCommandAsync();
            // 删除缓存
            await ApplicationContext.DeleteApplication(item.OpenId);

            if (item.AppType == AppEnvironmentEnum.WeChatServiceAccount)
            {
                var ticketResponse = await apiClient.ExecuteCgibinTicketGetTicketAsync(new CgibinTicketGetTicketRequest
                {
                    AccessToken = response.AccessToken
                });
                if (!ticketResponse.IsSuccessful())
                {
                    var message =
                        $"调用获取Ticket接口失败。ErrorCode：{ticketResponse.ErrorCode}。ErrorMessage：{ticketResponse.ErrorMessage}";
                    await logInfo.ErrorLog(logInfo.JobName, null, message);
                    return message;
                }

                await db.Updateable<ApplicationOpenIdModel>()
                    .SetColumns(_ => new ApplicationOpenIdModel
                    {
                        WeChatJsApiTicket = ticketResponse.Ticket,
                        WeChatJsApiTicketExpiresIn = ticketResponse.ExpiresIn,
                        WeChatJsApiTicketRefreshTime = dateTime
                    })
                    .Where(wh => wh.RecordId == item.RecordId)
                    .ExecuteCommandAsync();
                // 删除缓存
                await ApplicationContext.DeleteApplication(item.OpenId);
            }
        }

        return $"刷新微信 AccessToken，共计：{applicationOpenIdList.Count}个。";
    }
}