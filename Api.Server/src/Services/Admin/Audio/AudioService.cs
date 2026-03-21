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
using Fast.Admin.Service.Audio.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Service.Audio;

/// <summary>
/// <see cref="AudioService"/> 音频服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Walkman, Name = "audio")]
public class AudioService : IDynamicApplication
{
    private readonly ISqlSugarRepository<AudioModel> _audioRepository;
    private readonly ISqlSugarRepository<AudioTypeModel> _audioTypeRepository;
    private readonly ISqlSugarRepository<PronunciationTypeModel> _pronunciationTypeRepository;
    private readonly ISqlSugarRepository<LessonModel> _lessonRepository;

    public AudioService(ISqlSugarRepository<AudioModel> audioRepository,
        ISqlSugarRepository<AudioTypeModel> audioTypeRepository,
        ISqlSugarRepository<PronunciationTypeModel> pronunciationTypeRepository,
        ISqlSugarRepository<LessonModel> lessonRepository)
    {
        _audioRepository = audioRepository;
        _audioTypeRepository = audioTypeRepository;
        _pronunciationTypeRepository = pronunciationTypeRepository;
        _lessonRepository = lessonRepository;
    }

    #region 音频类型管理

    /// <summary>
    /// 音频类型选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("音频类型选择器", HttpRequestActionEnum.Query)]
    public async Task<List<ElSelectorOutput<long>>> AudioTypeSelector()
    {
        return await _audioTypeRepository.AsQueryable()
            .OrderBy(r => r.Sort)
            .Select(r => new ElSelectorOutput<long>
            {
                Label = r.AudioTypeName,
                Value = r.AudioTypeId
            }).ToListAsync();
    }

    /// <summary>
    /// 获取音频类型分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取音频类型分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.AudioTypeManage.Paged)]
    public async Task<PagedResult<QueryAudioTypePagedOutput>> QueryAudioTypePaged(QueryAudioTypePagedInput input)
    {
        return await _audioTypeRepository.Entities
            .OrderByIF(input.IsOrderBy, ob => ob.Sort)
            .Select(sl => new QueryAudioTypePagedOutput
            {
                AudioTypeId = sl.AudioTypeId,
                AudioTypeName = sl.AudioTypeName,
                Sort = sl.Sort,
                Remark = sl.Remark,
                DepartmentName = sl.DepartmentName,
                CreatedUserName = sl.CreatedUserName,
                CreatedTime = sl.CreatedTime,
                UpdatedUserName = sl.UpdatedUserName,
                UpdatedTime = sl.UpdatedTime,
                RowVersion = sl.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取音频类型详情
    /// </summary>
    /// <param name="audioTypeId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取音频类型详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.AudioTypeManage.Detail)]
    public async Task<QueryAudioTypePagedOutput> QueryAudioTypeDetail(
        [Required(ErrorMessage = "音频类型Id不能为空")] long? audioTypeId)
    {
        var result = await _audioTypeRepository.Entities.Where(wh => wh.AudioTypeId == audioTypeId)
            .Select(sl => new QueryAudioTypePagedOutput
            {
                AudioTypeId = sl.AudioTypeId,
                AudioTypeName = sl.AudioTypeName,
                Sort = sl.Sort,
                Remark = sl.Remark,
                DepartmentName = sl.DepartmentName,
                CreatedUserName = sl.CreatedUserName,
                CreatedTime = sl.CreatedTime,
                UpdatedUserName = sl.UpdatedUserName,
                UpdatedTime = sl.UpdatedTime,
                RowVersion = sl.RowVersion
            })
            .SingleAsync();

        if (result == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        return result;
    }

    /// <summary>
    /// 添加音频类型
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加音频类型", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.AudioTypeManage.Add)]
    public async Task AddAudioType(AddAudioTypeInput input)
    {
        if (await _audioTypeRepository.AnyAsync(a => a.AudioTypeName == input.AudioTypeName))
        {
            throw new UserFriendlyException("音频类型名称重复！");
        }

        var audioTypeModel = new AudioTypeModel
        {
            AudioTypeName = input.AudioTypeName,
            Sort = input.Sort,
            Remark = input.Remark
        };

        await _audioTypeRepository.InsertAsync(audioTypeModel);
    }

    /// <summary>
    /// 编辑音频类型
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑音频类型", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.AudioTypeManage.Edit)]
    public async Task EditAudioType(EditAudioTypeInput input)
    {
        if (await _audioTypeRepository.AnyAsync(a =>
                a.AudioTypeName == input.AudioTypeName && a.AudioTypeId != input.AudioTypeId))
        {
            throw new UserFriendlyException("音频类型名称重复！");
        }

        var audioTypeModel = await _audioTypeRepository.SingleOrDefaultAsync(input.AudioTypeId);
        if (audioTypeModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        audioTypeModel.AudioTypeName = input.AudioTypeName;
        audioTypeModel.Sort = input.Sort;
        audioTypeModel.Remark = input.Remark;
        audioTypeModel.RowVersion = input.RowVersion;

        await _audioTypeRepository.UpdateAsync(audioTypeModel);
    }

    /// <summary>
    /// 删除音频类型
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除音频类型", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.AudioTypeManage.Delete)]
    public async Task DeleteAudioType(AudioTypeIdInput input)
    {
        if (await _audioRepository.AnyAsync(a => a.AudioTypeId == input.AudioTypeId))
        {
            throw new UserFriendlyException("音频类型存在音频关联，无法删除！");
        }

        var audioTypeModel = await _audioTypeRepository.SingleOrDefaultAsync(input.AudioTypeId);
        if (audioTypeModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        await _audioTypeRepository.DeleteAsync(audioTypeModel);
    }

    #endregion

    #region 发音类型管理

    /// <summary>
    /// 发音类型选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("发音类型选择器", HttpRequestActionEnum.Query)]
    public async Task<List<ElSelectorOutput<long>>> PronunciationTypeSelector()
    {
        return await _pronunciationTypeRepository.AsQueryable()
            .OrderBy(r => r.Sort)
            .Select(r => new ElSelectorOutput<long>
            {
                Label = r.PronunciationTypeName,
                Value = r.PronunciationTypeId
            }).ToListAsync();
    }

    /// <summary>
    /// 获取发音类型分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取发音类型分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.PronunciationTypeManage.Paged)]
    public async Task<PagedResult<QueryPronunciationTypePagedOutput>> QueryPronunciationTypePaged(
        QueryPronunciationTypePagedInput input)
    {
        return await _pronunciationTypeRepository.Entities
            .OrderByIF(input.IsOrderBy, ob => ob.Sort)
            .Select(sl => new QueryPronunciationTypePagedOutput
            {
                PronunciationTypeId = sl.PronunciationTypeId,
                PronunciationTypeName = sl.PronunciationTypeName,
                Sort = sl.Sort,
                Remark = sl.Remark,
                DepartmentName = sl.DepartmentName,
                CreatedUserName = sl.CreatedUserName,
                CreatedTime = sl.CreatedTime,
                UpdatedUserName = sl.UpdatedUserName,
                UpdatedTime = sl.UpdatedTime,
                RowVersion = sl.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取发音类型详情
    /// </summary>
    /// <param name="pronunciationTypeId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取发音类型详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.PronunciationTypeManage.Detail)]
    public async Task<QueryPronunciationTypePagedOutput> QueryPronunciationTypeDetail(
        [Required(ErrorMessage = "发音类型Id不能为空")] long? pronunciationTypeId)
    {
        var result = await _pronunciationTypeRepository.Entities
            .Where(wh => wh.PronunciationTypeId == pronunciationTypeId)
            .Select(sl => new QueryPronunciationTypePagedOutput
            {
                PronunciationTypeId = sl.PronunciationTypeId,
                PronunciationTypeName = sl.PronunciationTypeName,
                Sort = sl.Sort,
                Remark = sl.Remark,
                DepartmentName = sl.DepartmentName,
                CreatedUserName = sl.CreatedUserName,
                CreatedTime = sl.CreatedTime,
                UpdatedUserName = sl.UpdatedUserName,
                UpdatedTime = sl.UpdatedTime,
                RowVersion = sl.RowVersion
            })
            .SingleAsync();

        if (result == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        return result;
    }

    /// <summary>
    /// 添加发音类型
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加发音类型", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.PronunciationTypeManage.Add)]
    public async Task AddPronunciationType(AddPronunciationTypeInput input)
    {
        if (await _pronunciationTypeRepository.AnyAsync(a => a.PronunciationTypeName == input.PronunciationTypeName))
        {
            throw new UserFriendlyException("发音类型名称重复！");
        }

        var pronunciationTypeModel = new PronunciationTypeModel
        {
            PronunciationTypeName = input.PronunciationTypeName,
            Sort = input.Sort,
            Remark = input.Remark
        };

        await _pronunciationTypeRepository.InsertAsync(pronunciationTypeModel);
    }

    /// <summary>
    /// 编辑发音类型
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑发音类型", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.PronunciationTypeManage.Edit)]
    public async Task EditPronunciationType(EditPronunciationTypeInput input)
    {
        if (await _pronunciationTypeRepository.AnyAsync(a =>
                a.PronunciationTypeName == input.PronunciationTypeName &&
                a.PronunciationTypeId != input.PronunciationTypeId))
        {
            throw new UserFriendlyException("发音类型名称重复！");
        }

        var pronunciationTypeModel =
            await _pronunciationTypeRepository.SingleOrDefaultAsync(input.PronunciationTypeId);
        if (pronunciationTypeModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        pronunciationTypeModel.PronunciationTypeName = input.PronunciationTypeName;
        pronunciationTypeModel.Sort = input.Sort;
        pronunciationTypeModel.Remark = input.Remark;
        pronunciationTypeModel.RowVersion = input.RowVersion;

        await _pronunciationTypeRepository.UpdateAsync(pronunciationTypeModel);
    }

    /// <summary>
    /// 删除发音类型
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除发音类型", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.PronunciationTypeManage.Delete)]
    public async Task DeletePronunciationType(PronunciationTypeIdInput input)
    {
        if (await _audioRepository.AnyAsync(a => a.PronunciationTypeId == input.PronunciationTypeId))
        {
            throw new UserFriendlyException("发音类型存在音频关联，无法删除！");
        }

        var pronunciationTypeModel =
            await _pronunciationTypeRepository.SingleOrDefaultAsync(input.PronunciationTypeId);
        if (pronunciationTypeModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        await _pronunciationTypeRepository.DeleteAsync(pronunciationTypeModel);
    }

    #endregion

    #region 音频管理

    /// <summary>
    /// 获取音频分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取音频分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.AudioManage.Paged)]
    public async Task<PagedResult<QueryAudioPagedOutput>> QueryAudioPaged(QueryAudioPagedInput input)
    {
        return await _audioRepository.Entities
            .LeftJoin<LessonModel>((a, l) => a.LessonId == l.LessonId)
            .LeftJoin<AudioTypeModel>((a, l, at) => a.AudioTypeId == at.AudioTypeId)
            .LeftJoin<PronunciationTypeModel>((a, l, at, pt) => a.PronunciationTypeId == pt.PronunciationTypeId)
            .WhereIF(input.LessonId != null, (a, l, at, pt) => a.LessonId == input.LessonId)
            .WhereIF(input.AudioTypeId != null, (a, l, at, pt) => a.AudioTypeId == input.AudioTypeId)
            .WhereIF(input.PronunciationTypeId != null,
                (a, l, at, pt) => a.PronunciationTypeId == input.PronunciationTypeId)
            .OrderByIF(input.IsOrderBy, (a, l, at, pt) => a.Sort)
            .Select((a, l, at, pt) => new QueryAudioPagedOutput
            {
                AudioId = a.AudioId,
                LessonId = a.LessonId,
                LessonName = l.LessonName,
                AudioTypeId = a.AudioTypeId,
                AudioTypeName = at.AudioTypeName,
                PronunciationTypeId = a.PronunciationTypeId,
                PronunciationTypeName = pt.PronunciationTypeName,
                AudioUrl = a.AudioUrl,
                Duration = a.Duration,
                Sort = a.Sort,
                Remark = a.Remark,
                DepartmentName = a.DepartmentName,
                CreatedUserName = a.CreatedUserName,
                CreatedTime = a.CreatedTime,
                UpdatedUserName = a.UpdatedUserName,
                UpdatedTime = a.UpdatedTime,
                RowVersion = a.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取音频详情
    /// </summary>
    /// <param name="audioId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取音频详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.AudioManage.Detail)]
    public async Task<QueryAudioPagedOutput> QueryAudioDetail(
        [Required(ErrorMessage = "音频Id不能为空")] long? audioId)
    {
        var result = await _audioRepository.Entities
            .LeftJoin<LessonModel>((a, l) => a.LessonId == l.LessonId)
            .LeftJoin<AudioTypeModel>((a, l, at) => a.AudioTypeId == at.AudioTypeId)
            .LeftJoin<PronunciationTypeModel>((a, l, at, pt) => a.PronunciationTypeId == pt.PronunciationTypeId)
            .Where((a, l, at, pt) => a.AudioId == audioId)
            .Select((a, l, at, pt) => new QueryAudioPagedOutput
            {
                AudioId = a.AudioId,
                LessonId = a.LessonId,
                LessonName = l.LessonName,
                AudioTypeId = a.AudioTypeId,
                AudioTypeName = at.AudioTypeName,
                PronunciationTypeId = a.PronunciationTypeId,
                PronunciationTypeName = pt.PronunciationTypeName,
                AudioUrl = a.AudioUrl,
                Duration = a.Duration,
                Sort = a.Sort,
                Remark = a.Remark,
                DepartmentName = a.DepartmentName,
                CreatedUserName = a.CreatedUserName,
                CreatedTime = a.CreatedTime,
                UpdatedUserName = a.UpdatedUserName,
                UpdatedTime = a.UpdatedTime,
                RowVersion = a.RowVersion
            })
            .SingleAsync();

        if (result == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        return result;
    }

    /// <summary>
    /// 添加音频
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加音频", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.AudioManage.Add)]
    public async Task AddAudio(AddAudioInput input)
    {
        if (!await _lessonRepository.AnyAsync(a => a.LessonId == input.LessonId))
        {
            throw new UserFriendlyException("课程不存在！");
        }

        if (!await _audioTypeRepository.AnyAsync(a => a.AudioTypeId == input.AudioTypeId))
        {
            throw new UserFriendlyException("音频类型不存在！");
        }

        if (!await _pronunciationTypeRepository.AnyAsync(a => a.PronunciationTypeId == input.PronunciationTypeId))
        {
            throw new UserFriendlyException("发音类型不存在！");
        }

        if (await _audioRepository.AnyAsync(a =>
                a.LessonId == input.LessonId && a.AudioTypeId == input.AudioTypeId &&
                a.PronunciationTypeId == input.PronunciationTypeId))
        {
            throw new UserFriendlyException("相同课程、音频类型、发音类型的音频已存在！");
        }

        var audioModel = new AudioModel
        {
            LessonId = input.LessonId,
            AudioTypeId = input.AudioTypeId,
            PronunciationTypeId = input.PronunciationTypeId,
            AudioUrl = input.AudioUrl,
            Duration = input.Duration,
            Sort = input.Sort,
            Remark = input.Remark
        };

        await _audioRepository.InsertAsync(audioModel);
    }

    /// <summary>
    /// 编辑音频
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑音频", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.AudioManage.Edit)]
    public async Task EditAudio(EditAudioInput input)
    {
        if (!await _lessonRepository.AnyAsync(a => a.LessonId == input.LessonId))
        {
            throw new UserFriendlyException("课程不存在！");
        }

        if (!await _audioTypeRepository.AnyAsync(a => a.AudioTypeId == input.AudioTypeId))
        {
            throw new UserFriendlyException("音频类型不存在！");
        }

        if (!await _pronunciationTypeRepository.AnyAsync(a => a.PronunciationTypeId == input.PronunciationTypeId))
        {
            throw new UserFriendlyException("发音类型不存在！");
        }

        if (await _audioRepository.AnyAsync(a =>
                a.LessonId == input.LessonId && a.AudioTypeId == input.AudioTypeId &&
                a.PronunciationTypeId == input.PronunciationTypeId && a.AudioId != input.AudioId))
        {
            throw new UserFriendlyException("相同课程、音频类型、发音类型的音频已存在！");
        }

        var audioModel = await _audioRepository.SingleOrDefaultAsync(input.AudioId);
        if (audioModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        audioModel.LessonId = input.LessonId;
        audioModel.AudioTypeId = input.AudioTypeId;
        audioModel.PronunciationTypeId = input.PronunciationTypeId;
        audioModel.AudioUrl = input.AudioUrl;
        audioModel.Duration = input.Duration;
        audioModel.Sort = input.Sort;
        audioModel.Remark = input.Remark;
        audioModel.RowVersion = input.RowVersion;

        await _audioRepository.UpdateAsync(audioModel);
    }

    /// <summary>
    /// 删除音频
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除音频", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.AudioManage.Delete)]
    public async Task DeleteAudio(AudioIdInput input)
    {
        var audioModel = await _audioRepository.SingleOrDefaultAsync(input.AudioId);
        if (audioModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        await _audioRepository.DeleteAsync(audioModel);
    }

    #endregion
}
