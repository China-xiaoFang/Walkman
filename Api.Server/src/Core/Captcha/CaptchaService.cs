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
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Fast.Core;

/// <summary>
/// <see cref="ICaptchaService"/> 默认实现
/// </summary>
public class CaptchaService : ICaptchaService, ISingletonDependency
{
    /// <summary>
    /// 缓存
    /// </summary>
    private readonly ICache<CenterCCL> _centerCache;

    /// <summary>
    /// 日志
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// 初始化验证码服务
    /// </summary>
    public CaptchaService(ICache<CenterCCL> centerCache, ILogger<IMailService> logger)
    {
        _centerCache = centerCache;
        _logger = logger;
    }

    /// <summary>
    /// 验证码缓存Dto
    /// </summary>
    private class VerificationCodeCacheDto
    {
        /// <summary>
        /// 验证码
        /// </summary>
        public string VerificationCode { get; set; }

        /// <summary>
        /// 客户端标识
        /// </summary>
        public string ClientIdentity { get; set; }
    }

    /// <inheritdoc />
    public async Task<(string captchaKey, string captchaImage)> GetImageCaptcha()
    {
        var captchaKey = Guid.NewGuid()
            .ToString("N");

        // 生成验证码
        const string codeCharacters = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";
        var dto = new VerificationCodeCacheDto
        {
            VerificationCode = new string(Enumerable.Range(0, 4)
                .Select(_ => codeCharacters[RandomNumberGenerator.GetInt32(codeCharacters.Length)])
                .ToArray()),
            ClientIdentity = GlobalContext.ClientIdentity
        };

        // 获取缓存Key
        var cacheKey = CacheConst.GetCacheKey(CacheConst.ImageCaptcha, captchaKey);
        await _centerCache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));

        // 背景颜色
        var backgroundColor = Color.ParseHex("EEF4FF");
        // 背景纹理颜色
        Color[] backgroundPatternColors =
        [
            Color.FromRgba(147, 197, 253, 40),
            Color.FromRgba(191, 219, 254, 55),
            Color.FromRgba(165, 180, 252, 35),
            Color.FromRgba(219, 234, 254, 60)
        ];
        // 验证码文字颜色
        Color[] textColors =
        [
            Color.ParseHex("1E3A8A"),
            Color.ParseHex("1D4ED8"),
            Color.ParseHex("3730A3"),
            Color.ParseHex("312E81"),
            Color.ParseHex("1E40AF")
        ];
        // 干扰线颜色
        Color[] lineColors =
        [
            Color.FromRgba(59, 130, 246, 120),
            Color.FromRgba(99, 102, 241, 105),
            Color.FromRgba(96, 165, 250, 115),
            Color.FromRgba(129, 140, 248, 95)
        ];
        // 噪点颜色
        Color[] noiseColors =
        [
            Color.FromRgba(59, 130, 246, 125),
            Color.FromRgba(96, 165, 250, 115),
            Color.FromRgba(99, 102, 241, 100),
            Color.FromRgba(30, 64, 175, 85)
        ];
        // 生成图片验证码
        using var image = ImageUtil.GenImage((canvas, font, _, _, width, height) =>
        {
            // 背景、纹理和后置干扰
            canvas.Mutate(context =>
            {
                // 设置背景
                context.BackgroundColor(backgroundColor);
                // 背景随机纹理
                for (var index = 0; index < 18; index++)
                {
                    var color = backgroundPatternColors[RandomNumberGenerator.GetInt32(backgroundPatternColors.Length)];
                    var radius = RandomNumberGenerator.GetInt32(4, 15);
                    context.Fill(color,
                        new EllipsePolygon(RandomNumberGenerator.GetInt32(width), RandomNumberGenerator.GetInt32(height),
                            radius));
                }

                // 背景噪点
                for (var index = 0; index < 45; index++)
                {
                    var color = noiseColors[RandomNumberGenerator.GetInt32(noiseColors.Length)];
                    var radius = RandomNumberGenerator.GetInt32(1, 3);
                    context.Fill(color,
                        new EllipsePolygon(RandomNumberGenerator.GetInt32(width), RandomNumberGenerator.GetInt32(height),
                            radius));
                }

                // 字符后面的直线干扰
                for (var index = 0; index < 6; index++)
                {
                    var color = lineColors[RandomNumberGenerator.GetInt32(lineColors.Length)];
                    var thickness = RandomNumberGenerator.GetInt32(8, 16) / 10F;
                    context.DrawLine(color, thickness,
                        new PointF(RandomNumberGenerator.GetInt32(width), RandomNumberGenerator.GetInt32(height)),
                        new PointF(RandomNumberGenerator.GetInt32(width), RandomNumberGenerator.GetInt32(height)));
                }

                // 字符后面的Bezier曲线
                for (var index = 0; index < 2; index++)
                {
                    var color = lineColors[RandomNumberGenerator.GetInt32(lineColors.Length)];
                    context.DrawBeziers(color, RandomNumberGenerator.GetInt32(10, 18) / 10F,
                        new PointF(0, RandomNumberGenerator.GetInt32(height)),
                        new PointF(RandomNumberGenerator.GetInt32(width / 4, width / 2), RandomNumberGenerator.GetInt32(height)),
                        new PointF(RandomNumberGenerator.GetInt32(width / 2, width * 3 / 4),
                            RandomNumberGenerator.GetInt32(height)), new PointF(width, RandomNumberGenerator.GetInt32(height)));
                }

                // 每个字符单独绘制、旋转后合成到主画布
                for (var index = 0; index < dto.VerificationCode.Length; index++)
                {
                    // 提前保存当前字符，避免Lambda捕获循环变量
                    var character = dto.VerificationCode[index];
                    var textColor = textColors[RandomNumberGenerator.GetInt32(textColors.Length)];
                    var fontSize = RandomNumberGenerator.GetInt32(25, 31);
                    var characterFont = new Font(font, fontSize);
                    // 创建单字符透明画布
                    using var characterImage = new Image<Rgba32>(42, 48, new Rgba32(0, 0, 0, 0));
                    characterImage.Mutate(characterContext =>
                    {
                        characterContext.DrawText(character.ToString(), characterFont, textColor,
                            new PointF(RandomNumberGenerator.GetInt32(5, 9), RandomNumberGenerator.GetInt32(2, 7)));
                    });
                    // 单字符随机旋转 -18° ~ 18°
                    var angle = RandomNumberGenerator.GetInt32(-18, 19);
                    characterImage.Mutate(characterContext => { characterContext.Rotate(angle); });
                    // 根据旋转后尺寸修正坐标，避免整体向右下偏移
                    var x = 1 + index * 31 + RandomNumberGenerator.GetInt32(-2, 4) - (characterImage.Width - 42) / 2;
                    var y = -2 + RandomNumberGenerator.GetInt32(-2, 5) - (characterImage.Height - 48) / 2;
                    // 直接绘制，不再通过Lambda捕获characterImage
                    context.DrawImage(characterImage, new Point(x, y), 1F);
                }

                // 绘制字符前面的干扰层
                for (var index = 0; index < 5; index++)
                {
                    var color = lineColors[RandomNumberGenerator.GetInt32(lineColors.Length)];
                    var thickness = RandomNumberGenerator.GetInt32(7, 14) / 10F;
                    context.DrawLine(color, thickness,
                        new PointF(RandomNumberGenerator.GetInt32(width), RandomNumberGenerator.GetInt32(height)),
                        new PointF(RandomNumberGenerator.GetInt32(width), RandomNumberGenerator.GetInt32(height)));
                }

                // 穿过字符的Bezier曲线
                for (var index = 0; index < 2; index++)
                {
                    var color = lineColors[RandomNumberGenerator.GetInt32(lineColors.Length)];
                    context.DrawBeziers(color, RandomNumberGenerator.GetInt32(10, 17) / 10F,
                        new PointF(0, RandomNumberGenerator.GetInt32(8, height - 8)),
                        new PointF(RandomNumberGenerator.GetInt32(width / 5, width / 2), RandomNumberGenerator.GetInt32(height)),
                        new PointF(RandomNumberGenerator.GetInt32(width / 2, width * 4 / 5),
                            RandomNumberGenerator.GetInt32(height)),
                        new PointF(width, RandomNumberGenerator.GetInt32(8, height - 8)));
                }

                // 前景噪点，让部分噪点直接覆盖字符
                for (var index = 0; index < 40; index++)
                {
                    var color = noiseColors[RandomNumberGenerator.GetInt32(noiseColors.Length)];
                    var radius = RandomNumberGenerator.GetInt32(1, 3);
                    context.Fill(color,
                        new EllipsePolygon(RandomNumberGenerator.GetInt32(width), RandomNumberGenerator.GetInt32(height),
                            radius));
                }
            });
        }, 132, 46, 28);

        return (captchaKey, $"data:image/png;base64,{await ImageUtil.ConvertToBase64Image(image)}");
    }

    /// <inheritdoc />
    public async Task VerifyImageCaptcha(string captchaKey, string verificationCode)
    {
        if (!Guid.TryParseExact(captchaKey, "N", out _)
            || string.IsNullOrWhiteSpace(verificationCode)
            || !Regex.IsMatch(verificationCode, RegexConst.ImageCaptchaCode))
        {
            throw new UserFriendlyException("请输入图片验证码！");
        }

        verificationCode = verificationCode.Trim();

        // 获取缓存Key
        var cacheKey = CacheConst.GetCacheKey(CacheConst.ImageCaptcha, captchaKey);
        using var codeLock = _centerCache.Client.TryLock($"{cacheKey}:Lock", 10);
        if (codeLock == null)
        {
            throw new UserFriendlyException("操作过于频繁，请稍后重试！");
        }

        var dto = await _centerCache.GetAsync<VerificationCodeCacheDto>(cacheKey);
        if (dto == null || dto.ClientIdentity != GlobalContext.ClientIdentity)
        {
            throw new UserFriendlyException("验证码无效或已过期！");
        }

        // 图片验证码验证一次后立即失效
        await _centerCache.DelAsync(cacheKey);

        // 忽略大小写
        if (!string.Equals(dto.VerificationCode, verificationCode, StringComparison.OrdinalIgnoreCase))
        {
            throw new UserFriendlyException("验证码无效或已过期！");
        }
    }
}