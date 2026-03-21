// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称"软件"）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按"原样"提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using Fast.Admin.Entity;
using Fast.Admin.Service.Play.Dto;
using Fast.Center.Entity;
using Microsoft.AspNetCore.Mvc;
using Yitter.IdGenerator;

namespace Fast.Admin.Service.Play;

/// <summary>
/// <see cref="PlayService"/> 播放服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Walkman, Name = "play")]
public class PlayService : IDynamicApplication
{
    private readonly IUser _user;
    private readonly ISqlSugarRepository<TextbookModel> _textbookRepository;
    private readonly ISqlSugarRepository<VolumeModel> _volumeRepository;
    private readonly ISqlSugarRepository<LessonModel> _lessonRepository;
    private readonly ISqlSugarRepository<AudioModel> _audioRepository;
    private readonly ISqlSugarRepository<AudioTypeModel> _audioTypeRepository;
    private readonly ISqlSugarRepository<PronunciationTypeModel> _pronunciationTypeRepository;
    private readonly ISqlSugarRepository<ActivationCodeModel> _activationCodeRepository;
    private readonly ISqlSugarRepository<PlayProgressModel> _playProgressRepository;
    private readonly ISqlSugarRepository<ConfigModel> _configRepository;

    public PlayService(
        IUser user,
        ISqlSugarRepository<TextbookModel> textbookRepository,
        ISqlSugarRepository<VolumeModel> volumeRepository,
        ISqlSugarRepository<LessonModel> lessonRepository,
        ISqlSugarRepository<AudioModel> audioRepository,
        ISqlSugarRepository<AudioTypeModel> audioTypeRepository,
        ISqlSugarRepository<PronunciationTypeModel> pronunciationTypeRepository,
        ISqlSugarRepository<ActivationCodeModel> activationCodeRepository,
        ISqlSugarRepository<PlayProgressModel> playProgressRepository,
        ISqlSugarRepository<ConfigModel> configRepository)
    {
        _user = user;
        _textbookRepository = textbookRepository;
        _volumeRepository = volumeRepository;
        _lessonRepository = lessonRepository;
        _audioRepository = audioRepository;
        _audioTypeRepository = audioTypeRepository;
        _pronunciationTypeRepository = pronunciationTypeRepository;
        _activationCodeRepository = activationCodeRepository;
        _playProgressRepository = playProgressRepository;
        _configRepository = configRepository;
    }

    /// <summary>
    /// 获取教材列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取教材列表", HttpRequestActionEnum.Query)]
    [AllowForbidden]
    public async Task<List<TextbookListOutput>> GetTextbookList()
    {
        return await _textbookRepository.AsQueryable()
            .OrderBy(t => t.Sort)
            .Select(t => new TextbookListOutput
            {
                TextbookId = t.TextbookId,
                TextbookName = t.TextbookName,
                CoverUrl = t.CoverUrl,
                Description = t.Description
            })
            .ToListAsync();
    }

    /// <summary>
    /// 获取册列表
    /// </summary>
    /// <param name="textbookId">教材Id</param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取册列表", HttpRequestActionEnum.Query)]
    [AllowForbidden]
    public async Task<List<VolumeListOutput>> GetVolumeList([Required(ErrorMessage = "教材Id不能为空")] long? textbookId)
    {
        return await _volumeRepository.AsQueryable()
            .Where(v => v.TextbookId == textbookId.Value)
            .OrderBy(v => v.Sort)
            .Select(v => new VolumeListOutput
            {
                VolumeId = v.VolumeId,
                TextbookId = v.TextbookId,
                VolumeName = v.VolumeName,
                CoverUrl = v.CoverUrl
            })
            .ToListAsync();
    }

    /// <summary>
    /// 获取课程列表
    /// </summary>
    /// <param name="volumeId">册Id</param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取课程列表", HttpRequestActionEnum.Query)]
    [AllowForbidden]
    public async Task<List<LessonListOutput>> GetLessonList([Required(ErrorMessage = "册Id不能为空")] long? volumeId)
    {
        // 获取册信息，确定教材Id
        var volume = await _volumeRepository.AsQueryable()
            .FirstAsync(v => v.VolumeId == volumeId.Value);
        if (volume == null)
        {
            throw new UserFriendlyException("册不存在！");
        }

        // 获取免费课程数量
        var freeLessonCount = await GetFreeLessonCount();

        // 检查用户是否已激活该教材
        var isActivated = await IsTextbookActivated(volume.TextbookId);

        // 获取所有课程，按排序
        var lessons = await _lessonRepository.AsQueryable()
            .Where(l => l.VolumeId == volumeId.Value)
            .OrderBy(l => l.Sort)
            .ToListAsync();

        var result = new List<LessonListOutput>();
        for (var i = 0; i < lessons.Count; i++)
        {
            var lesson = lessons[i];
            var isFree = i < freeLessonCount;
            result.Add(new LessonListOutput
            {
                LessonId = lesson.LessonId,
                VolumeId = lesson.VolumeId,
                LessonName = lesson.LessonName,
                LessonNo = lesson.LessonNo,
                IsFree = isFree,
                IsAccessible = isActivated || isFree
            });
        }

        return result;
    }

    /// <summary>
    /// 获取播放列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取播放列表", HttpRequestActionEnum.Query)]
    [AllowForbidden]
    public async Task<List<PlaylistOutput>> GetPlaylist(PlaylistInput input)
    {
        // 获取册信息，确定教材Id
        var volume = await _volumeRepository.AsQueryable()
            .FirstAsync(v => v.VolumeId == input.VolumeId);
        if (volume == null)
        {
            throw new UserFriendlyException("册不存在！");
        }

        // 获取免费课程数量和激活状态
        var freeLessonCount = await GetFreeLessonCount();
        var isActivated = await IsTextbookActivated(volume.TextbookId);

        // 获取所有课程，按排序
        var lessons = await _lessonRepository.AsQueryable()
            .Where(l => l.VolumeId == input.VolumeId)
            .OrderBy(l => l.Sort)
            .ToListAsync();

        // 如果指定了起始课程，从该课程开始
        if (input.StartLessonId.HasValue)
        {
            var startIndex = lessons.FindIndex(l => l.LessonId == input.StartLessonId.Value);
            if (startIndex > 0)
            {
                lessons = lessons.Skip(startIndex).ToList();
            }
        }

        // 获取音频类型和发音类型名称
        var audioType = await _audioTypeRepository.AsQueryable()
            .FirstAsync(a => a.AudioTypeId == input.AudioTypeId);
        var pronunciationType = await _pronunciationTypeRepository.AsQueryable()
            .FirstAsync(p => p.PronunciationTypeId == input.PronunciationTypeId);

        // 获取匹配的音频
        var lessonIds = lessons.Select(l => l.LessonId).ToList();
        var audios = await _audioRepository.AsQueryable()
            .Where(a => lessonIds.Contains(a.LessonId)
                && a.AudioTypeId == input.AudioTypeId
                && a.PronunciationTypeId == input.PronunciationTypeId)
            .ToListAsync();

        var audioDict = audios.ToDictionary(a => a.LessonId, a => a);

        // 获取当前用户的播放进度
        var progressList = await _playProgressRepository.AsQueryable()
            .Where(p => p.AccountId == _user.AccountId
                && lessonIds.Contains(p.LessonId)
                && p.AudioTypeId == input.AudioTypeId
                && p.PronunciationTypeId == input.PronunciationTypeId)
            .ToListAsync();

        var progressDict = progressList.ToDictionary(p => p.LessonId, p => p);

        // 构建完整列表中课程的原始索引用于判断免费
        var allLessons = await _lessonRepository.AsQueryable()
            .Where(l => l.VolumeId == input.VolumeId)
            .OrderBy(l => l.Sort)
            .Select(l => l.LessonId)
            .ToListAsync();

        var result = new List<PlaylistOutput>();
        foreach (var lesson in lessons)
        {
            if (!audioDict.TryGetValue(lesson.LessonId, out var audio))
            {
                continue;
            }

            var originalIndex = allLessons.IndexOf(lesson.LessonId);
            var isFree = originalIndex >= 0 && originalIndex < freeLessonCount;

            progressDict.TryGetValue(lesson.LessonId, out var progress);

            result.Add(new PlaylistOutput
            {
                LessonId = lesson.LessonId,
                LessonName = lesson.LessonName,
                LessonNo = lesson.LessonNo,
                AudioId = audio.AudioId,
                AudioUrl = audio.AudioUrl,
                Duration = audio.Duration,
                AudioTypeId = input.AudioTypeId,
                AudioTypeName = audioType?.AudioTypeName,
                PronunciationTypeId = input.PronunciationTypeId,
                PronunciationTypeName = pronunciationType?.PronunciationTypeName,
                IsFree = isFree,
                IsAccessible = isActivated || isFree,
                Progress = progress?.Progress ?? 0
            });
        }

        return result;
    }

    /// <summary>
    /// 保存播放进度
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("保存播放进度", HttpRequestActionEnum.Edit)]
    [AllowForbidden]
    public async Task SaveProgress(SaveProgressInput input)
    {
        var existing = await _playProgressRepository.AsQueryable()
            .FirstAsync(p => p.AccountId == _user.AccountId
                && p.LessonId == input.LessonId
                && p.AudioTypeId == input.AudioTypeId
                && p.PronunciationTypeId == input.PronunciationTypeId);

        if (existing != null)
        {
            existing.Progress = input.Progress;
            existing.Duration = input.Duration;
            await _playProgressRepository.AsUpdateable(existing)
                .UpdateColumns(p => new { p.Progress, p.Duration })
                .ExecuteCommandAsync();
        }
        else
        {
            var entity = new PlayProgressModel
            {
                PlayProgressId = YitIdHelper.NextId(),
                AccountId = _user.AccountId,
                LessonId = input.LessonId,
                AudioTypeId = input.AudioTypeId,
                PronunciationTypeId = input.PronunciationTypeId,
                Progress = input.Progress,
                Duration = input.Duration
            };
            await _playProgressRepository.AsInsertable(entity).ExecuteCommandAsync();
        }
    }

    /// <summary>
    /// 获取播放进度
    /// </summary>
    /// <param name="lessonId">课程Id</param>
    /// <param name="audioTypeId">音频类型Id</param>
    /// <param name="pronunciationTypeId">发音类型Id</param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取播放进度", HttpRequestActionEnum.Query)]
    [AllowForbidden]
    public async Task<PlayProgressOutput> GetPlayProgress(
        [Required(ErrorMessage = "课程Id不能为空")] long? lessonId,
        [Required(ErrorMessage = "音频类型Id不能为空")] long? audioTypeId,
        [Required(ErrorMessage = "发音类型Id不能为空")] long? pronunciationTypeId)
    {
        var progress = await _playProgressRepository.AsQueryable()
            .FirstAsync(p => p.AccountId == _user.AccountId
                && p.LessonId == lessonId.Value
                && p.AudioTypeId == audioTypeId.Value
                && p.PronunciationTypeId == pronunciationTypeId.Value);

        if (progress == null)
        {
            return null;
        }

        return new PlayProgressOutput
        {
            LessonId = progress.LessonId,
            AudioTypeId = progress.AudioTypeId,
            PronunciationTypeId = progress.PronunciationTypeId,
            Progress = progress.Progress,
            Duration = progress.Duration
        };
    }

    /// <summary>
    /// 获取激活状态
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取激活状态", HttpRequestActionEnum.Query)]
    [AllowForbidden]
    public async Task<ActivationStatusOutput> GetActivationStatus()
    {
        var activatedTextbookIds = await _activationCodeRepository.AsQueryable()
            .Where(a => a.AccountId == _user.AccountId && a.Status == ActivationCodeStatusEnum.Used)
            .Select(a => a.TextbookId)
            .Distinct()
            .ToListAsync();

        return new ActivationStatusOutput
        {
            IsActivated = activatedTextbookIds.Any(),
            ActivatedTextbookIds = activatedTextbookIds
        };
    }

    /// <summary>
    /// 使用激活码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("使用激活码", HttpRequestActionEnum.Edit)]
    [AllowForbidden]
    public async Task UseActivationCode(UseActivationCodeInput input)
    {
        var activationCode = await _activationCodeRepository.AsQueryable()
            .FirstAsync(a => a.Code == input.Code);

        if (activationCode == null)
        {
            throw new UserFriendlyException("激活码不存在！");
        }

        if (activationCode.Status == ActivationCodeStatusEnum.Used)
        {
            throw new UserFriendlyException("激活码已被使用！");
        }

        activationCode.Status = ActivationCodeStatusEnum.Used;
        activationCode.AccountId = _user.AccountId;
        activationCode.UsedTime = DateTime.Now;

        await _activationCodeRepository.AsUpdateable(activationCode)
            .UpdateColumns(a => new { a.Status, a.AccountId, a.UsedTime })
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取免费课程数量
    /// </summary>
    /// <returns></returns>
    private async Task<int> GetFreeLessonCount()
    {
        var config = await _configRepository.AsQueryable()
            .FirstAsync(c => c.ConfigCode == ConfigConst.FreeLessonCount);
        if (config != null && int.TryParse(config.ConfigValue, out var count))
            return count;
        return 3; // 默认免费3课
    }

    /// <summary>
    /// 检查用户是否已激活教材
    /// </summary>
    /// <param name="textbookId">教材Id</param>
    /// <returns></returns>
    private async Task<bool> IsTextbookActivated(long textbookId)
    {
        return await _activationCodeRepository.AsQueryable()
            .AnyAsync(a => a.AccountId == _user.AccountId
                && a.TextbookId == textbookId
                && a.Status == ActivationCodeStatusEnum.Used);
    }
}
