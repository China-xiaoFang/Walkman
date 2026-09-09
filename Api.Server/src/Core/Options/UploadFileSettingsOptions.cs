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

namespace Fast.Core;

/// <summary>
/// 上传文件配置选项
/// </summary>
public class UploadFileSettingsOptions : IPostConfigure
{
    /// <summary>
    /// 公网域名，未配置时使用当前请求地址
    /// </summary>
    public string PublicDomain { get; set; }

    /// <summary>
    /// Logo
    /// </summary>
    public UploadFileInfoSettings Logo { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    public UploadFileInfoSettings Avatar { get; set; }

    /// <summary>
    /// 证件照
    /// </summary>
    public UploadFileInfoSettings IdPhoto { get; set; }

    /// <summary>
    /// 音频资源
    /// </summary>
    public UploadFileInfoSettings AudioAsset { get; set; }

    /// <summary>
    /// 封面
    /// </summary>
    public UploadFileInfoSettings Cover { get; set; }

    /// <summary>
    /// 富文本
    /// </summary>
    public UploadFileInfoSettings Editor { get; set; }

    /// <summary>
    /// 默认
    /// </summary>
    public UploadFileInfoSettings Default { get; set; }

    /// <inheritdoc />
    public void PostConfigure()
    {
        if (string.IsNullOrWhiteSpace(PublicDomain))
        {
            PublicDomain = null;
        }
        else
        {
            PublicDomain = PublicDomain.Trim()
                .TrimEnd('/');
        }

        Logo ??= new UploadFileInfoSettings
        {
            Path = "Upload/Logo",
            MaxSize = 2048,
            ContentType =
            [
                "image/jpg", "image/jpeg", "image/png", "image/gif", "image/bmp"
            ]
        };
        Avatar ??= new UploadFileInfoSettings
        {
            Path = "Upload/Avatar",
            MaxSize = 2048,
            ContentType =
            [
                "image/jpg", "image/jpeg", "image/png", "image/gif", "image/bmp"
            ]
        };
        IdPhoto ??= new UploadFileInfoSettings
        {
            Path = "Upload/IdPhoto",
            MaxSize = 5120,
            ContentType =
            [
                "image/jpg", "image/jpeg", "image/png"
            ]
        };
        AudioAsset ??= new UploadFileInfoSettings
        {
            Path = "Upload/AudioAsset",
            MaxSize = 10240,
            ContentType =
            [
                "audio/mpeg", "audio/wav", "audio/ogg", "audio/mp4", "audio/flac"
            ]
        };
        Cover ??= new UploadFileInfoSettings
        {
            Path = "Upload/Cover",
            MaxSize = 2048,
            ContentType =
            [
                "image/jpg", "image/jpeg", "image/png"
            ]
        };
        Editor ??= new UploadFileInfoSettings
        {
            Path = "Upload/Editor",
            MaxSize = 10240,
            UseDateFolder = true,
            ContentType =
            [ // 图片类
                "image/jpg", "image/jpeg", "image/png", "image/gif", "image/bmp",
                // 视频类
                "video/mp4", "video/mpeg", "video/quicktime", "video/x-msvideo", "video/x-ms-wmv", "video/webm",
                "video/ogg"
            ]
        };
        Default ??= new UploadFileInfoSettings
        {
            Path = "Upload/Default",
            MaxSize = 102400,
            UseTypeFolder = true,
            UseDateFolder = true,
            ContentType =
            [
                // 图片类
                "image/jpg", "image/jpeg", "image/png", "image/gif", "image/bmp",
                // 视频类
                "video/mp4", "video/mpeg", "video/quicktime", "video/x-msvideo", "video/x-ms-wmv", "video/webm",
                "video/ogg",
                // 音频类
                "audio/mpeg", "audio/wav", "audio/ogg", "audio/mp4", "audio/flac",
                // 文本类
                "text/plain",
                "text/csv",
                "text/html",
                "text/markdown",
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
                "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                // 压缩包
                "application/zip", "application/x-rar-compressed", "application/x-7z-compressed", "application/gzip"
            ]
        };
    }
}

/// <summary>
/// 上传文件信息配置
/// </summary>
public class UploadFileInfoSettings
{
    /// <summary>
    /// 路径
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// 最大大小，单位kb
    /// </summary>
    public long MaxSize { get; set; }

    /// <summary>
    /// 使用类型文件夹
    /// </summary>
    public bool UseTypeFolder { get; set; }

    /// <summary>
    /// 使用日期文件夹
    /// </summary>
    public bool UseDateFolder { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public string[] ContentType { get; set; }
}