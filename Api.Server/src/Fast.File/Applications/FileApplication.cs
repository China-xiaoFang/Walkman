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

using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Fast.Center.Domain;
using Fast.Core;
using Fast.DynamicApplication;
using Fast.File.Applications.Dto;
using Fast.Runtime;
using Fast.Shared;
using Fast.SqlSugar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.File.Applications;

/// <summary>
/// 文件服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.File, Name = "file")]
public class FileApplication : IDynamicApplication
{
    /// <summary>
    /// 应用根目录
    /// </summary>
    private readonly string _rootPath;

    /// <summary>
    /// 单张图片允许的最大像素总数（宽 × 高），用于避免超大图片解码时占用过多服务器内存
    /// </summary>
    private const long MaxImagePixels = 40_000_000;

    private readonly IUser _user;
    private readonly ISqlSugarRepository<FileModel> _repository;
    private readonly UploadFileSettingsOptions _uploadFileSettingsOptions;
    private readonly HttpContext _httpContext;

    /// <summary>
    /// 图片
    /// </summary>
    private readonly HashSet<string> Images = ["image/jpg", "image/jpeg", "image/png", "image/gif", "image/bmp"];

    /// <summary>
    /// 视频
    /// </summary>
    private readonly HashSet<string> Videos =
        ["video/mp4", "video/mpeg", "video/quicktime", "video/x-msvideo", "video/x-ms-wmv", "video/webm", "video/ogg"];

    /// <summary>
    /// 音频
    /// </summary>
    private readonly HashSet<string> Audios = ["audio/mpeg", "audio/wav", "audio/ogg", "audio/mp4", "audio/flac"];

    /// <summary>
    /// 文本
    /// </summary>
    private readonly HashSet<string> Texts =
    [
        "text/plain",
        "text/csv",
        "text/html",
        "text/markdown"
    ];

    /// <summary>
    /// 文档
    /// </summary>
    private readonly HashSet<string> Documents =
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
    private readonly HashSet<string> Archives =
        ["application/zip", "application/x-rar-compressed", "application/x-7z-compressed", "application/gzip"];

    /// <summary>
    /// 图片尺寸
    /// </summary>
    private readonly Dictionary<string, int> ImageSizes = new() {{"thumb", 100}, {"small", 300}, {"normal", 600}};

    /// <summary>
    /// 文件服务
    /// </summary>
    /// <param name="hostEnvironment">宿主环境</param>
    /// <param name="user">当前登录用户</param>
    /// <param name="repository">数据仓储</param>
    /// <param name="uploadFileSettingsOptions">文件上传配置</param>
    /// <param name="httpContextAccessor">HTTP 请求上下文访问器</param>
    public FileApplication(IWebHostEnvironment hostEnvironment, IUser user, ISqlSugarRepository<FileModel> repository,
        IOptions<UploadFileSettingsOptions> uploadFileSettingsOptions, IHttpContextAccessor httpContextAccessor)
    {
        _rootPath = hostEnvironment.ContentRootPath;
        _user = user;
        _repository = repository;
        _uploadFileSettingsOptions = uploadFileSettingsOptions.Value;
        _httpContext = httpContextAccessor.HttpContext;
    }

    /// <summary>
    /// 获取文件分页列表
    /// </summary>
    [HttpPost]
    [ApiInfo("获取文件分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.FilePaged)]
    public async Task<PagedResult<QueryFilePagedOutput>> QueryFilePaged(QueryFilePagedInput input)
    {
        var tenantModel = await TenantContext.GetTenant(_user.TenantNo);

        var queryable = _repository.Entities.LeftJoin<TenantModel>((t1, t2) => t1.TenantId == t2.TenantId);

        if (tenantModel.TenantType == TenantTypeEnum.System)
        {
            queryable = queryable.ClearFilter<IBaseTEntity>()
                .WhereIF(input.TenantId != null, t1 => t1.TenantId == input.TenantId);
        }
        else if (!_user.IsAdmin)
        {
            queryable = queryable.Where(t1 => t1.CreatedUserId == _user.EmployeeId);
        }

        return await queryable.SelectMergeTable((t1, t2) => new QueryFilePagedOutput
            {
                FileId = t1.FileId,
                FileObjectName = t1.FileObjectName,
                FileOriginName = t1.FileOriginName,
                FileSuffix = t1.FileSuffix,
                FileMimeType = t1.FileMimeType,
                FileSizeKb = t1.FileSizeKb,
                FilePath = t1.FilePath,
                FileLocation = t1.FileLocation,
                FileHash = t1.FileHash,
                UploadDevice = t1.UploadDevice,
                UploadOS = t1.UploadOS,
                UploadBrowser = t1.UploadBrowser,
                UploadProvince = t1.UploadProvince,
                UploadCity = t1.UploadCity,
                UploadIp = t1.UploadIp,
                CreatedUserName = t1.CreatedUserName,
                CreatedTime = t1.CreatedTime,
                TenantName = t2.TenantName
            })
            .OrderByIF(input.IsOrderBy, ob => ob.CreatedTime, OrderByType.Desc)
            .ToPagedListAsync(input);
    }


    /// <summary>
    /// 预览文件
    /// </summary>
    [ApiDescriptionSettings(false)]
    [HttpGet("/file/{fileName}")]
    [ApiInfo("预览文件", HttpRequestActionEnum.Download)]
    [AllowAnonymous, DisabledRequestLog, DisableRateLimiting]
    public async Task<IActionResult> Preview([FromRoute, Required(ErrorMessage = "文件名称不能为空")] string fileName)
    {
        return await LocalPreview(fileName);
    }

    /// <summary>
    /// 预览文件
    /// </summary>
    /// <param name="fileName">文件名称</param>
    /// <param name="size">
    /// 图片尺寸
    /// <para><c>thumb</c>：缩略图</para>
    /// <para><c>small</c>：小图</para>
    /// <para><c>normal</c>：正常尺寸</para>
    /// </param>
    [ApiDescriptionSettings(false)]
    [HttpGet("/file/{fileName}@!{size}")]
    [ApiInfo("预览文件", HttpRequestActionEnum.Download)]
    [AllowAnonymous, DisabledRequestLog, DisableRateLimiting]
    public async Task<IActionResult> Preview([FromRoute, Required(ErrorMessage = "文件名称不能为空")] string fileName,
        [FromRoute, Required(ErrorMessage = "文件大小不能为空")] string size)
    {
        return await LocalPreview(fileName, size);
    }

    /// <summary>
    /// 将配置或数据库中的跨平台路径转换为当前操作系统使用的本地路径
    /// </summary>
    /// <param name="filePath">配置或数据库中的相对目录，兼容“/”和“\”</param>
    /// <param name="fileName">可选文件名</param>
    /// <returns>绝对配置保持原位置；相对配置基于程序根目录解析</returns>
    private string GetLocalPath(string filePath, string fileName = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new UserFriendlyException("文件存储路径不能为空！");

        var rootPath = Path.GetFullPath(_rootPath);
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
    private string GetFileLocation(string fileObjectName)
    {
        var publicDomain = _uploadFileSettingsOptions.PublicDomain;
        if (string.IsNullOrWhiteSpace(publicDomain))
        {
            publicDomain = $"{_httpContext.Request.Scheme}://{_httpContext.Request.Host}";
        }

        return $"{publicDomain}/file/{fileObjectName}";
    }

    /// <summary>
    /// 预览文件
    /// </summary>
    /// <param name="fileName">文件名称</param>
    /// <param name="size">
    /// 图片尺寸
    /// <para><c>thumb</c>：缩略图</para>
    /// <para><c>small</c>：小图</para>
    /// <para><c>normal</c>：正常尺寸</para>
    /// </param>
    /// <returns>文件预览响应</returns>
    private async Task<IActionResult> LocalPreview(string fileName, string size = null)
    {
        if (!string.IsNullOrWhiteSpace(size))
        {
            if (!ImageSizes.ContainsKey(size))
            {
                throw new UserFriendlyException("不支持的图片尺寸！");
            }

            size = $"@{size}";
        }
        else
        {
            size = "";
        }

        // 获取文件后缀
        var fileSuffix = Path.GetExtension(fileName);
        var fileIdStr = fileName[..^fileSuffix.Length];
        if (!long.TryParse(fileIdStr, out var fileId))
            throw new UserFriendlyException("文件不存在！");

        // 这里作为预览文件，必须禁用 AOP，所以直接使用 NEW 的方式
        using var db = new SqlSugarClient(SqlSugarContext.GetConnectionConfig(SqlSugarContext.ConnectionSettings));
        var fileInfoModel = await db.Queryable<FileModel>()
            .InSingleAsync(fileId);
        if (fileInfoModel == null)
            throw new UserFriendlyException("文件不存在！");

        // 匿名预览仅允许图片。文档、压缩包和 HTML 等文件必须通过租户鉴权下载
        // 防止通过可猜测的 FileId 越权读取或在同源下执行活动内容
        if (!Images.Contains(fileInfoModel.FileMimeType.ToLowerInvariant()))
        {
            throw new UserFriendlyException("该文件不支持公开预览，请登录后下载！");
        }

        _httpContext.Response.Headers.CacheControl = "public,max-age=31536000";
        _httpContext.Response.Headers.XContentTypeOptions = "nosniff";

        var localFileName = $"{fileInfoModel.FileId}{size}.{fileInfoModel.FileSuffix}";

        var localFilePath = GetLocalPath(fileInfoModel.FilePath, localFileName);
        if (!System.IO.File.Exists(localFilePath))
            throw new UserFriendlyException("文件丢失或已被删除！");

        var stream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return new FileStreamResult(stream, fileInfoModel.FileMimeType);
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    [HttpPost]
    [ApiInfo("下载文件", HttpRequestActionEnum.Download)]
    public async Task<IActionResult> Download(DownloadFileInput input)
    {
        var fileInfoModel = await _repository.Entities.InSingleAsync(input.FileId);
        if (fileInfoModel == null)
            throw new UserFriendlyException("文件不存在！");

        var filePath = GetLocalPath(fileInfoModel.FilePath, fileInfoModel.FileObjectName);
        if (!System.IO.File.Exists(filePath))
            throw new UserFriendlyException("文件丢失或已被删除！");

        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        _httpContext.Response.Headers.XContentTypeOptions = "nosniff";
        return new FileStreamResult(stream, fileInfoModel.FileMimeType) {FileDownloadName = fileInfoModel.FileOriginName};
    }

    /// <summary>
    /// 上传Logo
    /// </summary>
    [HttpPost]
    [ApiInfo("上传Logo", HttpRequestActionEnum.Upload)]
    [RequestSizeLimit(3 * 1024 * 1024)]
    public async Task<string> UploadLogo(IFormFile file)
    {
        return await LocalUploadFile(file, _uploadFileSettingsOptions.Logo);
    }

    /// <summary>
    /// 上传头像
    /// </summary>
    [HttpPost]
    [ApiInfo("上传头像", HttpRequestActionEnum.Upload)]
    [RequestSizeLimit(3 * 1024 * 1024)]
    public async Task<string> UploadAvatar(IFormFile file)
    {
        return await LocalUploadFile(file, _uploadFileSettingsOptions.Avatar);
    }

    /// <summary>
    /// 上传证件照
    /// </summary>
    [HttpPost]
    [ApiInfo("上传证件照", HttpRequestActionEnum.Upload)]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<string> UploadIdPhoto(IFormFile file)
    {
        return await LocalUploadFile(file, _uploadFileSettingsOptions.IdPhoto);
    }

    /// <summary>
    /// 上传富文本
    /// </summary>
    [HttpPost]
    [ApiInfo("上传富文本", HttpRequestActionEnum.Upload)]
    [RequestSizeLimit(11 * 1024 * 1024)]
    public async Task<string> UploadEditor(IFormFile file)
    {
        return await LocalUploadFile(file, _uploadFileSettingsOptions.Editor);
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    [HttpPost]
    [ApiInfo("上传文件", HttpRequestActionEnum.Upload)]
    [RequestSizeLimit(101 * 1024 * 1024)]
    public async Task<string> UploadFile(IFormFile file)
    {
        return await LocalUploadFile(file);
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <returns>文件访问地址</returns>
    private async Task<string> LocalUploadFile(IFormFile file, UploadFileInfoSettings fileInfoSettings = null)
    {
        if (file == null || file.Length == 0)
            throw new UserFriendlyException("上传文件不能为空！");

        fileInfoSettings ??= _uploadFileSettingsOptions.Default;

        var dateTime = DateTime.Now;

        // 文件大小
        var fileSizeKb = (file.Length + 1023L) / 1024L;
        if (fileInfoSettings.MaxSize > 0 && fileSizeKb > fileInfoSettings.MaxSize)
            throw new UserFriendlyException($"文件大小超出限制，最大允许{fileInfoSettings.MaxSize / 1024}MB。");

        // 浏览器可能提交带 Windows 或 Unix 路径的文件名，存储前只保留最后一段名称
        var fileOriginName = Path.GetFileName(file.FileName.Replace('\\', '/'));
        var fileSuffix = Path.GetExtension(fileOriginName)
            .ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(fileSuffix))
            throw new UserFriendlyException("文件没有有效后缀名!");
        if (fileSuffix.Length > 17)
            throw new UserFriendlyException("文件后缀名过长！");

        var normalizedContentType = file.ContentType?.Trim()
            .ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalizedContentType))
            throw new UserFriendlyException("文件内容类型不能为空！");

        if (fileInfoSettings.ContentType?.Any() == true)
        {
            if (!fileInfoSettings.ContentType.Contains(normalizedContentType, StringComparer.OrdinalIgnoreCase))
                throw new UserFriendlyException($"文件类型不支持，当前类型：{normalizedContentType}");
        }

        // 根据当前已支持的 MIME 类型校验文件后缀，防止客户端声明的类型与文件名不一致
        var isFileExtensionCompatible = normalizedContentType switch
        {
            "image/jpg" or "image/jpeg" => fileSuffix is ".jpg" or ".jpeg",
            "image/png" => fileSuffix == ".png",
            "image/gif" => fileSuffix == ".gif",
            "image/bmp" => fileSuffix is ".bmp" or ".dib",
            "video/mp4" => fileSuffix is ".mp4" or ".m4v",
            "video/mpeg" => fileSuffix is ".mpeg" or ".mpg" or ".mpe",
            "video/quicktime" => fileSuffix is ".mov" or ".qt",
            "video/x-msvideo" => fileSuffix == ".avi",
            "video/x-ms-wmv" => fileSuffix == ".wmv",
            "video/webm" => fileSuffix == ".webm",
            "video/ogg" => fileSuffix is ".ogv" or ".ogg",
            "audio/mpeg" => fileSuffix is ".mp3" or ".mpga",
            "audio/wav" => fileSuffix == ".wav",
            "audio/ogg" => fileSuffix is ".ogg" or ".oga" or ".opus",
            "audio/mp4" => fileSuffix is ".m4a" or ".mp4",
            "audio/flac" => fileSuffix == ".flac",
            "text/plain" => fileSuffix == ".txt",
            "text/csv" => fileSuffix == ".csv",
            "text/html" => fileSuffix is ".html" or ".htm",
            "text/markdown" => fileSuffix is ".md" or ".markdown",
            "application/pdf" => fileSuffix == ".pdf",
            "application/msword" => fileSuffix == ".doc",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => fileSuffix == ".docx",
            "application/vnd.ms-excel" => fileSuffix == ".xls",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => fileSuffix == ".xlsx",
            "application/vnd.ms-powerpoint" => fileSuffix == ".ppt",
            "application/vnd.openxmlformats-officedocument.presentationml.presentation" => fileSuffix == ".pptx",
            "application/zip" => fileSuffix == ".zip",
            "application/x-rar-compressed" => fileSuffix == ".rar",
            "application/x-7z-compressed" => fileSuffix == ".7z",
            "application/gzip" => fileSuffix is ".gz" or ".gzip" or ".tgz",
            _ => false
        };
        if (!isFileExtensionCompatible)
            throw new UserFriendlyException("文件后缀与声明的文件类型不一致！");

        if (Images.Contains(normalizedContentType))
        {
            try
            {
                await using var formatStream = file.OpenReadStream();
                var imageFormat = await Image.DetectFormatAsync(formatStream);
                var expectedMimeType = normalizedContentType == "image/jpg" ? "image/jpeg" : normalizedContentType;
                if (imageFormat == null
                    || !string.Equals(imageFormat.DefaultMimeType, expectedMimeType, StringComparison.OrdinalIgnoreCase))
                {
                    throw new UserFriendlyException("图片内容与声明的文件类型不一致！");
                }

                await using var identifyStream = file.OpenReadStream();
                var imageInfo = await Image.IdentifyAsync(identifyStream);
                if (imageInfo == null || (long) imageInfo.Width * imageInfo.Height > MaxImagePixels)
                    throw new UserFriendlyException("图片像素尺寸超出限制！");
            }
            catch (UnknownImageFormatException)
            {
                throw new UserFriendlyException("图片内容与声明的文件类型不一致！");
            }
            catch (InvalidImageContentException)
            {
                throw new UserFriendlyException("图片内容已损坏或格式不受支持！");
            }
        }

        // 计算文件哈希
        await using var stream = file.OpenReadStream();
        var hashBytes = await SHA256.HashDataAsync(stream);
        var fileHash = Convert.ToHexStringLower(hashBytes);

        // 判断是否存在重复文件
        var existFileModel = await _repository.SingleOrDefaultAsync(s => s.FileHash == fileHash);
        if (existFileModel != null)
        {
            if (!System.IO.File.Exists(GetLocalPath(existFileModel.FilePath, existFileModel.FileObjectName)))
                throw new UserFriendlyException("相同文件的存储记录存在，但物理文件已丢失，请联系管理员处理！");
            return existFileModel.FileLocation;
        }

        var fileId = YitIdHelper.NextId();
        // 本地文件名称
        var fileObjectName = $"{fileId}{fileSuffix}";

        // 本地文件路径
        var filePath = fileInfoSettings.Path;

        if (!string.IsNullOrWhiteSpace(_user?.TenantNo))
        {
            filePath = Path.Combine(filePath, _user.TenantNo);
        }

        // 判断是否启用类型文件夹
        if (fileInfoSettings.UseTypeFolder)
        {
            if (Images.Contains(normalizedContentType))
                filePath = Path.Combine(filePath, "image");
            else if (Videos.Contains(normalizedContentType))
                filePath = Path.Combine(filePath, "video");
            else if (Audios.Contains(normalizedContentType))
                filePath = Path.Combine(filePath, "audio");
            else if (Texts.Contains(normalizedContentType))
                filePath = Path.Combine(filePath, "text");
            else if (Documents.Contains(normalizedContentType))
                filePath = Path.Combine(filePath, "document");
            else if (Archives.Contains(normalizedContentType))
                filePath = Path.Combine(filePath, "archive");
            else
                filePath = Path.Combine(filePath, "other");
        }

        // 判断是否启用时间文件夹
        if (fileInfoSettings.UseDateFolder)
        {
            filePath = Path.Combine(filePath, dateTime.ToString("yyyy/MM/dd"));
        }

        // 数据库存储统一使用“/”，保证路径记录可以在 Windows、Linux 和 macOS 之间迁移
        filePath = filePath.Replace('\\', '/');

        var fileInfoModel = new FileModel
        {
            FileId = fileId,
            FileObjectName = fileObjectName,
            FileOriginName = fileOriginName,
            FileSuffix = fileSuffix.TrimStart('.'),
            FileMimeType = normalizedContentType,
            FileSizeKb = fileSizeKb,
            FilePath = filePath,
            FileLocation = GetFileLocation(fileObjectName),
            FileHash = fileHash
        };
        // 获取设备信息
        var userAgentInfo = _httpContext.RequestUserAgentInfo();
        // 获取万网信息
        var wanNetIpInfo = await _httpContext.RemoteIpv4InfoAsync();
        fileInfoModel.UploadDevice = userAgentInfo.Device;
        fileInfoModel.UploadOS = userAgentInfo.OS;
        fileInfoModel.UploadBrowser = userAgentInfo.Browser;
        fileInfoModel.UploadProvince = wanNetIpInfo.Province;
        fileInfoModel.UploadCity = wanNetIpInfo.City;
        fileInfoModel.UploadIp = wanNetIpInfo.Ip;
        fileInfoModel.CreatedUserId = _user?.EmployeeId;
        fileInfoModel.CreatedUserName = _user?.NickName;
        fileInfoModel.CreatedTime = dateTime;

        // 本地存储
        var localFilePath = GetLocalPath(filePath);
        Directory.CreateDirectory(localFilePath);
        var localFullPath = Path.Combine(localFilePath, fileObjectName);
        await using (var fileStream = new FileStream(localFullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920,
                         FileOptions.Asynchronous | FileOptions.SequentialScan))
        {
            await file.CopyToAsync(fileStream, _httpContext.RequestAborted);
        }

        // 判断是否为图片
        if (Images.Contains(normalizedContentType))
        {
            // 异步读取原始图片
            using var image = await Image.LoadAsync(localFullPath);

            foreach (var item in ImageSizes)
            {
                var width = Math.Min(item.Value, image.Width);
                // 按原图比例计算高度
                var ratio = (float) width / image.Width;
                var height = Math.Max(1, (int) (image.Height * ratio));

                // 创建图片副本并调整大小
                using var clone = image.Clone(ctx => ctx.Resize(width, height));
                // 拼接缩略图文件名
                var thumbnailName = $"{fileId}@{item.Key}{fileSuffix}";
                var thumbnailPath = Path.Combine(localFilePath, thumbnailName);

                // 保存图片到本地，格式自动根据后缀判断
                await clone.SaveAsync(thumbnailPath);
            }
        }

        // 物理文件及缩略图全部写入成功后再保存记录，失败时只会产生冗余文件，不会产生无效数据库记录
        await _repository.InsertAsync(fileInfoModel);

        return fileInfoModel.FileLocation;
    }
}