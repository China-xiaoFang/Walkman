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

using System.Security.Cryptography;
using Fast.Center.Domain;
using Fast.SqlSugar;
using Microsoft.Extensions.Options;

namespace Fast.Core;

/// <summary>
/// 文件上下文
/// </summary>
[SuppressSniffer]
public class FileContext
{
    /// <summary>
    /// 图片
    /// </summary>
    public static readonly HashSet<string> Images = ["image/jpg", "image/jpeg", "image/png", "image/gif", "image/bmp"];

    /// <summary>
    /// 视频
    /// </summary>
    public static readonly HashSet<string> Videos =
        ["video/mp4", "video/mpeg", "video/quicktime", "video/x-msvideo", "video/x-ms-wmv", "video/webm", "video/ogg"];

    /// <summary>
    /// 音频
    /// </summary>
    public static readonly HashSet<string> Audios = ["audio/mpeg", "audio/wav", "audio/ogg", "audio/mp4", "audio/flac"];

    /// <summary>
    /// 文本
    /// </summary>
    public static readonly HashSet<string> Texts =
    [
        "text/plain",
        "text/csv",
        "text/html",
        "text/markdown"
    ];

    /// <summary>
    /// 文档
    /// </summary>
    public static readonly HashSet<string> Documents =
    [
        // PDF
        "application/pdf",
        // Word
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        // Excel
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        // PowerPoint
        "application/vnd.ms-powerpoint",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation"
    ];

    /// <summary>
    /// 压缩包
    /// </summary>
    public static readonly HashSet<string> Archives =
        ["application/zip", "application/x-rar-compressed", "application/x-7z-compressed", "application/gzip"];

    /// <summary>
    /// 将配置或数据库中的跨平台路径转换为当前操作系统使用的本地路径
    /// </summary>
    /// <param name="rootPath">应用根目录</param>
    /// <param name="filePath">配置或数据库中的相对目录，兼容“/”和“\”</param>
    /// <param name="fileName">可选文件名</param>
    /// <returns>绝对配置保持原位置；相对配置基于程序根目录解析</returns>
    public static string GetLocalPath(string rootPath, string filePath, string fileName = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new UserFriendlyException("文件存储路径不能为空！");

        rootPath = Path.GetFullPath(rootPath);
        var localPath = filePath.Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
        var fullPath = string.IsNullOrEmpty(fileName)
            ? Path.GetFullPath(Path.Combine(rootPath, localPath))
            : Path.GetFullPath(Path.Combine(rootPath, localPath, fileName));

        return fullPath;
    }

    /// <summary>
    /// 获取文件访问地址
    /// </summary>
    public static string GetFileLocation(string fileObjectName, UploadFileSettingsOptions uploadFileSettingsOptions = null)
    {
        var _httpContext = FastContext.HttpContext;
        uploadFileSettingsOptions ??= FastContext.GetService<IOptions<UploadFileSettingsOptions>>()
            .Value;
        var publicDomain = uploadFileSettingsOptions.PublicDomain;
        if (string.IsNullOrWhiteSpace(publicDomain))
        {
            publicDomain = $"{_httpContext.Request.Scheme}://{_httpContext.Request.Host}";
        }

        return $"{publicDomain}/file/{fileObjectName}";
    }

    /// <summary>
    /// 创建媒体资源临时访问票据
    /// </summary>
    /// <param name="fileUrl">媒体文件地址</param>
    /// <param name="lifetimeMinutes">票据有效期，单位：分钟，限制为 15～120 分钟</param>
    /// <param name="uploadFileSettingsOptions">文件上传配置</param>
    /// <returns>媒体资源临时访问地址</returns>
    public static async Task<string> CreateMediaAssetTicket(string fileUrl, double lifetimeMinutes,
        UploadFileSettingsOptions uploadFileSettingsOptions = null)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            throw new UserFriendlyException("文件地址不能为空！");
        }

        // 限制媒体票据有效期，避免调用方传入异常值导致票据长期有效
        lifetimeMinutes = Math.Clamp(lifetimeMinutes, 15L, 120L);

        // 提取文件路径。
        // 支持完整 Url 和相对路径，并忽略 QueryString、Fragment。
        var path = fileUrl;
        if (Uri.TryCreate(fileUrl, UriKind.Absolute, out var uri))
        {
            path = uri.AbsolutePath;
        }
        else
        {
            var separatorIndex = fileUrl.IndexOfAny(['?', '#']);
            if (separatorIndex >= 0)
            {
                path = fileUrl[..separatorIndex];
            }
        }

        // 获取文件Id
        var fileName = Path.GetFileNameWithoutExtension(path.TrimEnd('/'));
        if (!long.TryParse(fileName, out var fileId))
        {
            throw new UserFriendlyException("文件地址不受支持！");
        }

        var repository = FastContext.GetService<ISqlSugarRepository<FileModel>>();
        var fileExists = await repository.Entities.AnyAsync(wh =>
            wh.FileId == fileId && (wh.FileMimeType.StartsWith("audio/") || wh.FileMimeType.StartsWith("video/")));
        if (!fileExists)
        {
            throw new UserFriendlyException("文件不存在或存储地址不受支持！");
        }

        var _user = FastContext.GetService<IUser>();
        var _cache = FastContext.GetService<ICache>();

        // 使用 256 bit 加密安全随机数生成媒体票据。
        // Hex 编码后为 64 个字符，无法通过枚举方式有效猜测。
        var token = Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(32));
        var cacheKey = CacheConst.GetCacheKey(CacheConst.MediaAssetTicket, token);
        await _cache.SetAsync(cacheKey,
            new MediaAssetTicketCacheInfo
            {
                FileId = fileId,
                AppNo = _user.AppNo,
                TenantNo = _user.TenantNo,
                DeviceType = _user.DeviceType,
                EmployeeNo = _user.EmployeeNo,
                SessionId = _user.SessionId
            }, TimeSpan.FromMinutes(lifetimeMinutes));

        var _httpContext = FastContext.HttpContext;
        uploadFileSettingsOptions ??= FastContext.GetService<IOptions<UploadFileSettingsOptions>>()
            .Value;
        var publicDomain = uploadFileSettingsOptions.PublicDomain;
        if (string.IsNullOrWhiteSpace(publicDomain))
        {
            publicDomain = $"{_httpContext.Request.Scheme}://{_httpContext.Request.Host}";
        }

        return $"{publicDomain}/file/media/{token}";
    }
}