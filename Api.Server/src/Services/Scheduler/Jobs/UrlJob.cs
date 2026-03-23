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

using System.Net.Http.Headers;
using System.Web;
using Fast.Admin.Enum;
using Fast.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Fast.Scheduler;

/// <summary>
/// <see cref="UrlJob"/> Url调度作业
/// </summary>
internal class UrlJob : JobBase<SchedulerJobUrlLogInfo>
{
    public UrlJob(IServiceProvider serviceProvider, IMailService mailService, IOptions<MvcNewtonsoftJsonOptions> jsonOptions,
        ILogger<IJob> logger) : base(serviceProvider, mailService, jsonOptions, logger, new SchedulerJobUrlLogInfo())
    {
    }

    /// <summary>
    /// 执行调度作业
    /// </summary>
    /// <param name="serviceProvider"><see cref="IServiceProvider"/> 服务提供者</param>
    /// <param name="db"><see cref="ISqlSugarClient"/> SqlSugar上下文</param>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override async Task JobExecute(IServiceProvider serviceProvider, ISqlSugarClient db, IJobExecutionContext context)
    {
        // 作业类型
        var jobType = context.JobDetail.JobDataMap.GetEnum<SchedulerJobTypeEnum>(nameof(SchedulerJobInfo.JobType));

        if ((jobType & (SchedulerJobTypeEnum.IntranetUrl | SchedulerJobTypeEnum.OuterNetUrl)) == 0)
        {
            throw new UserFriendlyException("当前调度作业只支持【内网Url】【外网Url】！");
        }

        // 请求方式，默认Get请求
        var requestMethod =
            context.JobDetail.JobDataMap.GetNullableEnum<HttpRequestMethodEnum>(nameof(SchedulerJobInfo.RequestMethod))
            ?? HttpRequestMethodEnum.Get;
        _logInfo.RequestMethod = requestMethod.ToString();

        // 请求超时时间，默认不超时
        var requestTimeout = context.JobDetail.JobDataMap.GetNullableInt(nameof(SchedulerJobInfo.RequestTimeout));
        _logInfo.RequestTimeout = requestTimeout;

        // 请求参数
        var requestParams = new Dictionary<string, object>();
        var jobRequestParams =
            context.JobDetail.JobDataMap[nameof(SchedulerJobInfo.RequestParams)] as IDictionary<string, object>;
        jobRequestParams ??= new Dictionary<string, object>();
        // 请求参数处理
        foreach (var (key, value) in jobRequestParams)
        {
            requestParams.TryAdd(key, value);
        }

        _logInfo.RequestParams = $"<span class='params'>{HttpUtility.HtmlEncode(requestParams.ToJsonString())}</span>";

        // 请求头部
        var requestHeader = new Dictionary<string, string>();
        var jobRequestHeader =
            context.JobDetail.JobDataMap[nameof(SchedulerJobInfo.RequestHeader)] as IDictionary<string, string>;
        jobRequestHeader ??= new Dictionary<string, string>();
        // 请求头部处理
        foreach (var (key, value) in jobRequestHeader)
        {
            requestHeader.TryAdd(key, value);
        }

        _logInfo.RequestHeader = $"<span class='headers'>{HttpUtility.HtmlEncode(requestHeader.ToJsonString())}</span>";

        // 请求Url
        var requestUrl = context.JobDetail.JobDataMap.GetString(nameof(SchedulerJobInfo.RequestUrl));

        _logInfo.RequestUrl = $"<span class='url'>{requestUrl}</span>";

        // 响应数据
        string responseData;
        HttpResponseHeaders responseHeaders;

        // 发送请求
        switch (requestMethod)
        {
            case HttpRequestMethodEnum.Get:
                (responseData, responseHeaders) =
                    await RemoteRequestUtil.GetAsync(requestUrl, requestParams, requestHeader, timeout: requestTimeout);
                break;
            case HttpRequestMethodEnum.Post:
                (responseData, responseHeaders) =
                    await RemoteRequestUtil.PostAsync(requestUrl, requestParams, requestHeader, timeout: requestTimeout);
                break;
            case HttpRequestMethodEnum.Put:
                (responseData, responseHeaders) =
                    await RemoteRequestUtil.PutAsync(requestUrl, requestParams, requestHeader, timeout: requestTimeout);
                break;
            case HttpRequestMethodEnum.Delete:
                (responseData, responseHeaders) =
                    await RemoteRequestUtil.DeleteAsync(requestUrl, requestHeader, timeout: requestTimeout);
                break;
            default:
                throw new UserFriendlyException("请求方式不支持！");
        }

        _logInfo.Result =
            $"<span class='result'>{HttpUtility.HtmlEncode(responseData).GetSubStringWithEllipsis(1000, true)}</span>";

        var responseHeader = new Dictionary<string, string>();
        foreach (var header in responseHeaders)
        {
            responseHeader.TryAdd(header.Key, string.Join(",", header.Value));
        }

        _logInfo.ResponseHeader = responseHeader;
    }
}