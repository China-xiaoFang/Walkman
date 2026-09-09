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

using Fast.Admin.Service;
using Fast.AdminLog.Domain;
using Fast.Walkman.Domain;
using Fast.Walkman.Service.AudioAsset.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yitter.IdGenerator;

namespace Fast.Walkman.Service.AudioAsset;

/// <summary>
/// 音频资源服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Walkman, Name = "audioAsset")]
public class AudioAssetService : IDynamicApplication
{
    private readonly ISqlSugarRepository<AudioAssetModel> _repository;
    private readonly ISqlSugarRepository<LyricDocumentModel> _lyricDocumentRepository;

    public AudioAssetService(ISqlSugarRepository<AudioAssetModel> repository,
        ISqlSugarRepository<LyricDocumentModel> lyricDocumentRepository)
    {
        _repository = repository;
        _lyricDocumentRepository = lyricDocumentRepository;
    }

    /// <summary>
    /// 获取音频资源分页列表
    /// </summary>
    [HttpPost]
    [ApiInfo("获取音频资源分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.AudioAsset.Paged)]
    public async Task<PagedResult<QueryAudioAssetPagedOutput>> QueryAudioAssetPaged(QueryAudioAssetPagedInput input)
    {
        return await _repository.Entities.LeftJoin<BookModel>((t1, t2) => t1.BookId == t2.BookId)
            .LeftJoin<LessonModel>((t1, t2, t3) => t1.LessonId == t3.LessonId)
            .WhereIF(input.BookId != null, t1 => t1.BookId == input.BookId)
            .WhereIF(input.LessonId != null, t1 => t1.LessonId == input.LessonId)
            .WhereIF(input.AudioType != null, t1 => t1.AudioType == input.AudioType)
            .OrderBy(t1 => t1.BookId)
            .OrderBy(t1 => t1.LessonId)
            .OrderBy(t1 => t1.AudioType)
            .Select((t1, t2, t3) => new QueryAudioAssetPagedOutput
            {
                AudioAssetId = t1.AudioAssetId,
                BookId = t1.BookId,
                BookName = t2.BookName,
                LessonId = t1.LessonId,
                LessonTitle = t3.LessonTitle,
                LessonNumber = t3.LessonNumber,
                AudioType = t1.AudioType,
                AudioUrl = FileContext.CreateMediaAssetTicket(t1.AudioUrl, Math.Ceiling(t1.AudioDuration.TotalMinutes) + 10),
                AudioDuration = t1.AudioDuration,
                CreatedUserName = t1.CreatedUserName,
                CreatedTime = t1.CreatedTime,
                UpdatedUserName = t1.UpdatedUserName,
                UpdatedTime = t1.UpdatedTime,
                RowVersion = t1.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取音频资源详情
    /// </summary>
    [HttpGet]
    [ApiInfo("获取音频资源详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.AudioAsset.Detail)]
    public async Task<QueryAudioAssetDetailOutput> QueryAudioAssetDetail(
        [Required(ErrorMessage = "音频资源Id不能为空")] long? audioAssetId)
    {
        var result = await _repository.Entities.LeftJoin<BookModel>((t1, t2) => t1.BookId == t2.BookId)
            .LeftJoin<LessonModel>((t1, t2, t3) => t1.LessonId == t3.LessonId)
            .Where(t1 => t1.AudioAssetId == audioAssetId)
            .Select((t1, t2, t3) => new QueryAudioAssetDetailOutput
            {
                AudioAssetId = t1.AudioAssetId,
                BookId = t1.BookId,
                BookName = t2.BookName,
                LessonId = t1.LessonId,
                LessonTitle = t3.LessonTitle,
                LessonNumber = t3.LessonNumber,
                AudioType = t1.AudioType,
                AudioUrl = t1.AudioUrl,
                AudioDuration = t1.AudioDuration,
                CreatedUserName = t1.CreatedUserName,
                CreatedTime = t1.CreatedTime,
                UpdatedUserName = t1.UpdatedUserName,
                UpdatedTime = t1.UpdatedTime,
                RowVersion = t1.RowVersion
            })
            .SingleAsync();

        if (result == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        result.LyricDocumentList = await _lyricDocumentRepository.Entities.Where(wh => wh.AudioAssetId == result.AudioAssetId)
            .OrderBy(ob => ob.StartTime)
            .OrderBy(ob => ob.EndTime)
            .OrderBy(ob => ob.RecordId)
            .Select(sl => new QueryLyricDocumentOutput
            {
                RecordId = sl.RecordId,
                English = sl.English,
                Chinese = sl.Chinese,
                StartTime = sl.StartTime,
                EndTime = sl.EndTime,
                RowVersion = sl.RowVersion
            })
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// 添加音频资源
    /// </summary>
    [HttpPost]
    [ApiInfo("添加音频资源", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.AudioAsset.Add)]
    public async Task AddAudioAsset(AddAudioAssetInput input)
    {
        if (input.AudioDuration <= TimeSpan.Zero)
        {
            throw new UserFriendlyException("音频时长必须大于0！");
        }

        if (input.LyricDocumentList == null || !input.LyricDocumentList.Any())
        {
            throw new UserFriendlyException("歌词文档集合不能为空！");
        }

        if (input.LyricDocumentList.Any(a => a.StartTime < TimeSpan.Zero || a.EndTime <= a.StartTime))
        {
            throw new UserFriendlyException("歌词文档时间范围无效！");
        }

        if (input.LyricDocumentList.Any(a => a.EndTime > input.AudioDuration))
        {
            throw new UserFriendlyException("歌词文档时间不能超过音频时长！");
        }

        var orderedLyricDocumentList = input.LyricDocumentList.OrderBy(ob => ob.StartTime)
            .ToList();
        if (orderedLyricDocumentList.Skip(1)
            .Where((item, index) => orderedLyricDocumentList[index].EndTime > item.StartTime)
            .Any())
        {
            throw new UserFriendlyException("歌词文档时间不能重叠！");
        }

        if (!await _repository.Queryable<LessonModel>()
                .AnyAsync(a => a.LessonId == input.LessonId && a.BookId == input.BookId))
        {
            throw new UserFriendlyException("课程不存在或不属于当前教材！");
        }

        if (await _repository.AnyAsync(a => a.LessonId == input.LessonId && a.AudioType == input.AudioType))
        {
            throw new UserFriendlyException("同一课程下的音频类型不能重复！");
        }

        var audioAssetModel = new AudioAssetModel
        {
            AudioAssetId = YitIdHelper.NextId(),
            BookId = input.BookId,
            LessonId = input.LessonId,
            AudioType = input.AudioType,
            AudioUrl = input.AudioUrl,
            AudioDuration = input.AudioDuration
        };
        var lyricDocumentList = input.LyricDocumentList.Select(sl => new LyricDocumentModel
            {
                RecordId = YitIdHelper.NextId(),
                AudioAssetId = audioAssetModel.AudioAssetId,
                English = sl.English,
                Chinese = sl.Chinese,
                StartTime = sl.StartTime,
                EndTime = sl.EndTime
            })
            .ToList();

        await _repository.Ado.UseTranAsync(async () =>
        {
            await _repository.InsertAsync(audioAssetModel);
            await _lyricDocumentRepository.InsertAsync(lyricDocumentList);
        }, ex => throw ex);

        await LogContext.OperateLog(new OperateLogDto
        {
            Title = "添加音频资源",
            OperateType = OperateLogTypeEnum.Content,
            BizId = audioAssetModel.AudioAssetId,
            BizNo = null,
            Description = $"添加音频资源：{audioAssetModel.AudioUrl}"
        });
    }

    /// <summary>
    /// 编辑音频资源
    /// </summary>
    [HttpPost]
    [ApiInfo("编辑音频资源", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.AudioAsset.Edit)]
    public async Task EditAudioAsset(EditAudioAssetInput input)
    {
        if (input.AudioDuration <= TimeSpan.Zero)
        {
            throw new UserFriendlyException("音频时长必须大于0！");
        }

        if (input.LyricDocumentList == null || !input.LyricDocumentList.Any())
        {
            throw new UserFriendlyException("歌词文档集合不能为空！");
        }

        if (input.LyricDocumentList.Any(a => a.StartTime < TimeSpan.Zero || a.EndTime <= a.StartTime))
        {
            throw new UserFriendlyException("歌词文档时间范围无效！");
        }

        if (input.LyricDocumentList.Any(a => a.EndTime > input.AudioDuration))
        {
            throw new UserFriendlyException("歌词文档时间不能超过音频时长！");
        }

        var orderedLyricDocumentList = input.LyricDocumentList.OrderBy(ob => ob.StartTime)
            .ToList();
        if (orderedLyricDocumentList.Skip(1)
            .Where((item, index) => orderedLyricDocumentList[index].EndTime > item.StartTime)
            .Any())
        {
            throw new UserFriendlyException("歌词文档时间不能重叠！");
        }

        if (!await _repository.Queryable<LessonModel>()
                .AnyAsync(a => a.LessonId == input.LessonId && a.BookId == input.BookId))
        {
            throw new UserFriendlyException("课程不存在或不属于当前教材！");
        }

        if (await _repository.AnyAsync(a =>
                a.LessonId == input.LessonId && a.AudioType == input.AudioType && a.AudioAssetId != input.AudioAssetId))
        {
            throw new UserFriendlyException("同一课程下的音频类型不能重复！");
        }

        var audioAssetModel = await _repository.SingleOrDefaultAsync(input.AudioAssetId);
        if (audioAssetModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        var lyricDocumentList = await _lyricDocumentRepository.Entities
            .Where(wh => wh.AudioAssetId == audioAssetModel.AudioAssetId)
            .ToListAsync();
        var inputRecordIds = input.LyricDocumentList.Where(wh => wh.RecordId != null)
            .Select(sl => sl.RecordId!.Value)
            .ToList();
        if (inputRecordIds.Count
            != inputRecordIds.Distinct()
                .Count()
            || inputRecordIds.Any(recordId => lyricDocumentList.All(a => a.RecordId != recordId)))
        {
            throw new UserFriendlyException("歌词文档数据无效！");
        }

        audioAssetModel.BookId = input.BookId;
        audioAssetModel.LessonId = input.LessonId;
        audioAssetModel.AudioType = input.AudioType;
        audioAssetModel.AudioUrl = input.AudioUrl;
        audioAssetModel.AudioDuration = input.AudioDuration;
        audioAssetModel.RowVersion = input.RowVersion;

        var addLyricDocumentList = input.LyricDocumentList.Where(wh => wh.RecordId == null)
            .Select(sl => new LyricDocumentModel
            {
                RecordId = YitIdHelper.NextId(),
                AudioAssetId = audioAssetModel.AudioAssetId,
                English = sl.English,
                Chinese = sl.Chinese,
                StartTime = sl.StartTime,
                EndTime = sl.EndTime
            })
            .ToList();
        var updateLyricDocumentList = input.LyricDocumentList.Where(wh => wh.RecordId != null)
            .Select(sl =>
            {
                var lyricDocumentModel = lyricDocumentList.First(f => f.RecordId == sl.RecordId);
                lyricDocumentModel.English = sl.English;
                lyricDocumentModel.Chinese = sl.Chinese;
                lyricDocumentModel.StartTime = sl.StartTime;
                lyricDocumentModel.EndTime = sl.EndTime;
                lyricDocumentModel.RowVersion = sl.RowVersion;
                return lyricDocumentModel;
            })
            .ToList();
        var deleteLyricDocumentList = lyricDocumentList.Where(wh => inputRecordIds.All(a => a != wh.RecordId))
            .ToList();

        await _repository.Ado.UseTranAsync(async () =>
        {
            await _repository.UpdateAsync(audioAssetModel);
            await _lyricDocumentRepository.DeleteAsync(deleteLyricDocumentList);
            await _lyricDocumentRepository.UpdateAsync(updateLyricDocumentList);
            await _lyricDocumentRepository.InsertAsync(addLyricDocumentList);
        }, ex => throw ex);

        await LogContext.OperateLog(new OperateLogDto
        {
            Title = "编辑音频资源",
            OperateType = OperateLogTypeEnum.Content,
            BizId = audioAssetModel.AudioAssetId,
            BizNo = null,
            Description = $"编辑音频资源：{audioAssetModel.AudioUrl}"
        });
    }

    /// <summary>
    /// 删除音频资源
    /// </summary>
    [HttpPost]
    [ApiInfo("删除音频资源", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.AudioAsset.Delete)]
    public async Task DeleteAudioAsset(AudioAssetIdInput input)
    {
        var audioAssetModel = await _repository.SingleOrDefaultAsync(input.AudioAssetId);
        if (audioAssetModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        var lyricDocumentList = await _lyricDocumentRepository.Entities
            .Where(wh => wh.AudioAssetId == audioAssetModel.AudioAssetId)
            .ToListAsync();
        await _repository.Ado.UseTranAsync(async () =>
        {
            await _lyricDocumentRepository.DeleteAsync(lyricDocumentList);
            await _repository.DeleteAsync(audioAssetModel);
        }, ex => throw ex);

        await LogContext.OperateLog(new OperateLogDto
        {
            Title = "删除音频资源",
            OperateType = OperateLogTypeEnum.Content,
            BizId = audioAssetModel.AudioAssetId,
            BizNo = null,
            Description = $"删除音频资源：{audioAssetModel.AudioUrl}"
        });
    }
}