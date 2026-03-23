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

namespace Fast.Scheduler.HostedService;

/// <summary>
/// <see cref="SchedulerHostedService"/> 调度后台托管服务
/// </summary>
public class SchedulerHostedService : IHostedService
{
    /// <summary>
    /// 托管应用程序生命周期
    /// </summary>
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    /// <summary>
    /// <see cref="ISchedulerCenter"/> 调度中心
    /// </summary>
    private readonly ISchedulerCenter _schedulerCenter;

    /// <summary>
    /// <see cref="ILogger"/> 日志
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// <see cref="SchedulerHostedService"/> 调度后台托管服务
    /// </summary>
    /// <param name="hostApplicationLifetime"><see cref="IHostApplicationLifetime"/> 托管应用程序生命周期</param>
    /// <param name="schedulerCenter"><see cref="ISchedulerCenter"/> 调度中心</param>
    /// <param name="logger"><see cref="ILogger"/> 日志</param>
    public SchedulerHostedService(IHostApplicationLifetime hostApplicationLifetime, ISchedulerCenter schedulerCenter,
        ILogger<SchedulerHostedService> logger)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _schedulerCenter = schedulerCenter;
        _logger = logger;
    }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Start operation.</returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // 订阅 ApplicationStarted 事件
        _hostApplicationLifetime?.ApplicationStarted.Register(() =>
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    // 初始化调度程序
                    await _schedulerCenter.InitializeScheduler();
                }
                catch (Exception ex)
                {
                    // 启动调度器错误
                    _logger.LogError(ex, $"Initialize scheduler error. {ex.Message}");
                }
            }, cancellationToken);
        });

        await Task.CompletedTask;
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}