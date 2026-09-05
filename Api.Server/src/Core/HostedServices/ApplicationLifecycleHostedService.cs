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

using System.Net;
using Fast.Center.Domain;
using Fast.SqlSugar;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Fast.Core;

/// <summary>
/// 应用程序生命周期托管服务
/// </summary>
[Order(107)]
public class ApplicationLifecycleHostedService : IHostedLifecycleService
{
    /// <summary>
    /// 固定的收件邮箱
    /// </summary>
    private const string ReceiveEmail = "2875616188@qq.com";

    private readonly IMailService _mailService;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IServer _server;
    private readonly ILogger _logger;

    private bool _started;

    /// <summary>
    /// 应用程序生命周期托管服务
    /// </summary>
    public ApplicationLifecycleHostedService(IMailService mailService, IHostEnvironment hostEnvironment, IServer server,
        ILogger<ApplicationLifecycleHostedService> logger)
    {
        _mailService = mailService;
        _hostEnvironment = hostEnvironment;
        _server = server;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task StartingAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StartedAsync(CancellationToken cancellationToken)
    {
        _started = true;

        var addresses = _server.Features.Get<IServerAddressesFeature>()
            ?.Addresses;
        var address = addresses is {Count: > 0}
            ? string.Join("，", addresses.Select(item =>
            {
                if (item.Contains("://[::]", StringComparison.OrdinalIgnoreCase))
                    return item.Replace("://[::]", "://127.0.0.1", StringComparison.OrdinalIgnoreCase);
                if (item.Contains("://0.0.0.0", StringComparison.OrdinalIgnoreCase))
                    return item.Replace("://0.0.0.0", "://127.0.0.1", StringComparison.OrdinalIgnoreCase);
                if (item.Contains("://*", StringComparison.OrdinalIgnoreCase))
                    return item.Replace("://*", "://127.0.0.1", StringComparison.OrdinalIgnoreCase);
                return item;
            }))
            : "未知";
        await SendNotification("程序启动通知", $"{_hostEnvironment.ApplicationName} 已启动", address);
    }

    /// <inheritdoc />
    public async Task StoppingAsync(CancellationToken cancellationToken)
    {
        if (_started)
        {
            await SendNotification("程序停止通知", $"{_hostEnvironment.ApplicationName} 正在停止");
        }
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StoppedAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 发送生命周期通知
    /// </summary>
    private async Task SendNotification(string title, string status, string address = null)
    {
        try
        {
            // 独立客户端不加载 AOP，避免写日志时再次产生 SQL 日志
            using var db = new SqlSugarClient(SqlSugarContext.GetConnectionConfig(SqlSugarContext.ConnectionSettings));

            var configCodes = new List<string>
            {
                ConfigConst.MailSmtp,
                ConfigConst.MailPort,
                ConfigConst.MailEmail,
                ConfigConst.MailAuthCode,
                ConfigConst.MailDisplayName
            };

            // 直接读取数据库
            var configList = await db.Queryable<ConfigModel>()
                .Where(wh => configCodes.Contains(wh.ConfigCode))
                .ToListAsync();

            var smtp = configList.SingleOrDefault(s => s.ConfigCode == ConfigConst.MailSmtp)
                ?.ConfigValue;
            var portValue = configList.SingleOrDefault(s => s.ConfigCode == ConfigConst.MailPort)
                ?.ConfigValue;
            var email = configList.SingleOrDefault(s => s.ConfigCode == ConfigConst.MailEmail)
                ?.ConfigValue;
            var authCode = configList.SingleOrDefault(s => s.ConfigCode == ConfigConst.MailAuthCode)
                ?.ConfigValue;
            var displayName = configList.SingleOrDefault(s => s.ConfigCode == ConfigConst.MailDisplayName)
                                  ?.ConfigValue
                              ?? "FastDotNet";
            // 配置为空直接退出，避免报错
            if (string.IsNullOrWhiteSpace(smtp)
                || !int.TryParse(portValue, out var port)
                || port <= 0
                || string.IsNullOrWhiteSpace(email)
                || string.IsNullOrWhiteSpace(authCode))
            {
                return;
            }

            var content = $"""
                           <p>{WebUtility.HtmlEncode(status)}</p>
                           <p>环境：{WebUtility.HtmlEncode(_hostEnvironment.EnvironmentName)}</p>
                           <p>主机：{WebUtility.HtmlEncode(Environment.MachineName)}</p>
                           {(address != null ? $"<p>地址：{address}</p>" : string.Empty)}
                           <p>时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss zzz}</p>
                           """;
            await _mailService.SendEmail(title, await _mailService.GetEmailTemplate(title, content, displayName: displayName),
                [ReceiveEmail], smtp, port, email, authCode, displayName);
        }
        catch (Exception ex)
        {
            // 邮件配置或网络异常不能阻断程序启动与优雅停止。
            _logger.LogError(ex, "发送{Title}失败", title);
        }
    }
}