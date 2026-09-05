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

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fast.Core;

/// <summary>
/// 删除日志后台服务
/// </summary>
[Order(1)]
public class DeleteLogBackgroundService : BackgroundService
{
    /// <summary>
    /// 最大保留天数
    /// </summary>
    private const int MaxRetainDay = 90;

    /// <summary>
    /// 应用程序生命周期
    /// </summary>
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    /// <summary>
    /// 日志
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// 删除日志托管服务
    /// </summary>
    public DeleteLogBackgroundService(IHostApplicationLifetime hostApplicationLifetime,
        ILogger<DeleteLogBackgroundService> logger)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
    }

    /// <summary>
    /// 处理空文件夹
    /// </summary>
    /// <param name="stopDirectory">停止递归扫描的根目录</param>
    /// <param name="directoryInfo">当前扫描目录</param>
    private void HandleEmptyDirectory(string stopDirectory, DirectoryInfo directoryInfo)
    {
        while (directoryInfo != null)
        {
            // 如果和日志文件相同则停止
            if (string.Equals(directoryInfo.FullName, stopDirectory, StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            // 检查目录是否为空
            if (!directoryInfo.EnumerateFileSystemInfos()
                    .Any())
            {
                // 上级目录
                var parent = directoryInfo.Parent;
                directoryInfo.Delete();
                directoryInfo = parent;
            }
            else
            {
                break;
            }
        }
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // 日志组件会在启动阶段创建当前日志文件；等待应用完全启动并写入启动日志后再检查空文件
            if (!_hostApplicationLifetime.ApplicationStarted.IsCancellationRequested)
            {
                var applicationStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                await using var registration =
                    _hostApplicationLifetime.ApplicationStarted.Register(() => applicationStarted.TrySetResult());
                await applicationStarted.Task.WaitAsync(stoppingToken);
            }

            var logPath = Path.Combine(Environment.CurrentDirectory, "logs");

            if (Directory.Exists(logPath))
            {
                // 空文件数量
                var emptyFileNum = 0;
                var oldFileNum = 0;

                try
                {
                    // SearchPattern 由运行时按当前平台处理路径分隔符，可同时支持 Windows、Linux 和 macOS
                    var matchedFiles = Directory.EnumerateFiles(logPath, "*.log", SearchOption.AllDirectories);

                    foreach (var filePath in matchedFiles)
                    {
                        stoppingToken.ThrowIfCancellationRequested();

                        var fileInfo = new FileInfo(filePath);

                        // 删除空文件
                        if (fileInfo.Length == 0)
                        {
                            try
                            {
                                File.Delete(filePath);
                                HandleEmptyDirectory(logPath, new DirectoryInfo(Path.GetDirectoryName(filePath)));
                                emptyFileNum++;
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                        // 删除超过最大保留天数的文件
                        else if ((DateTime.Now.Date - fileInfo.LastWriteTime.Date).TotalDays > MaxRetainDay)
                        {
                            try
                            {
                                File.Delete(filePath);
                                HandleEmptyDirectory(logPath, new DirectoryInfo(Path.GetDirectoryName(filePath)));
                                oldFileNum++;
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                    }
                }
                catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogError(ex, "Delete log error...");
                }

                MAppContext.ConsoleWrite(console =>
                {
                    console.BackgroundColor = ConsoleColor.Black;
                    console.ForegroundColor = ConsoleColor.Green;
                    console.Write("info");
                    console.ResetColor();
                    console.WriteLine($": {DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                    console.BackgroundColor = ConsoleColor.Black;
                    console.ForegroundColor = ConsoleColor.DarkGray;
                    console.WriteLine($"      删除日志文件，空文件 {emptyFileNum} 个，超过最长保留{MaxRetainDay}天文件 {oldFileNum} 个。");
                });
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // 宿主停止时取消启动等待或日志扫描属于正常停机流程
        }
    }
}