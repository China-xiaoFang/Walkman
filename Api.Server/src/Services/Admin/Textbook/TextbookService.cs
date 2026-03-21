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
using Fast.Admin.Service.Textbook.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Service.Textbook;

/// <summary>
/// <see cref="TextbookService"/> 教材服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Walkman, Name = "textbook")]
public class TextbookService : IDynamicApplication
{
    private readonly ISqlSugarRepository<TextbookModel> _textbookRepository;
    private readonly ISqlSugarRepository<VolumeModel> _volumeRepository;
    private readonly ISqlSugarRepository<LessonModel> _lessonRepository;
    private readonly ISqlSugarRepository<AudioModel> _audioRepository;

    public TextbookService(ISqlSugarRepository<TextbookModel> textbookRepository,
        ISqlSugarRepository<VolumeModel> volumeRepository, ISqlSugarRepository<LessonModel> lessonRepository,
        ISqlSugarRepository<AudioModel> audioRepository)
    {
        _textbookRepository = textbookRepository;
        _volumeRepository = volumeRepository;
        _lessonRepository = lessonRepository;
        _audioRepository = audioRepository;
    }

    #region 教材管理

    /// <summary>
    /// 获取教材分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取教材分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.Textbook.Paged)]
    public async Task<PagedResult<QueryTextbookPagedOutput>> QueryTextbookPaged(QueryTextbookPagedInput input)
    {
        return await _textbookRepository.Entities
            .OrderByIF(input.IsOrderBy, ob => ob.Sort)
            .Select(sl => new QueryTextbookPagedOutput
            {
                TextbookId = sl.TextbookId,
                TextbookName = sl.TextbookName,
                CoverUrl = sl.CoverUrl,
                Description = sl.Description,
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
    /// 获取教材详情
    /// </summary>
    /// <param name="textbookId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取教材详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Textbook.Detail)]
    public async Task<QueryTextbookDetailOutput> QueryTextbookDetail(
        [Required(ErrorMessage = "教材Id不能为空")] long? textbookId)
    {
        var result = await _textbookRepository.Entities.Where(wh => wh.TextbookId == textbookId)
            .Select(sl => new QueryTextbookDetailOutput
            {
                TextbookId = sl.TextbookId,
                TextbookName = sl.TextbookName,
                CoverUrl = sl.CoverUrl,
                Description = sl.Description,
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
    /// 添加教材
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加教材", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.Textbook.Add)]
    public async Task AddTextbook(AddTextbookInput input)
    {
        if (await _textbookRepository.AnyAsync(a => a.TextbookName == input.TextbookName))
        {
            throw new UserFriendlyException("教材名称重复！");
        }

        var textbookModel = new TextbookModel
        {
            TextbookName = input.TextbookName,
            CoverUrl = input.CoverUrl,
            Description = input.Description,
            Sort = input.Sort,
            Remark = input.Remark
        };

        await _textbookRepository.InsertAsync(textbookModel);
    }

    /// <summary>
    /// 编辑教材
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑教材", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Textbook.Edit)]
    public async Task EditTextbook(EditTextbookInput input)
    {
        if (await _textbookRepository.AnyAsync(a => a.TextbookName == input.TextbookName && a.TextbookId != input.TextbookId))
        {
            throw new UserFriendlyException("教材名称重复！");
        }

        var textbookModel = await _textbookRepository.SingleOrDefaultAsync(input.TextbookId);
        if (textbookModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        textbookModel.TextbookName = input.TextbookName;
        textbookModel.CoverUrl = input.CoverUrl;
        textbookModel.Description = input.Description;
        textbookModel.Sort = input.Sort;
        textbookModel.Remark = input.Remark;
        textbookModel.RowVersion = input.RowVersion;

        await _textbookRepository.UpdateAsync(textbookModel);
    }

    /// <summary>
    /// 删除教材
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除教材", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.Textbook.Delete)]
    public async Task DeleteTextbook(TextbookIdInput input)
    {
        if (await _volumeRepository.AnyAsync(a => a.TextbookId == input.TextbookId))
        {
            throw new UserFriendlyException("教材存在册关联，无法删除！");
        }

        var textbookModel = await _textbookRepository.SingleOrDefaultAsync(input.TextbookId);
        if (textbookModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        await _textbookRepository.DeleteAsync(textbookModel);
    }

    #endregion

    #region 册管理

    /// <summary>
    /// 获取册分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取册分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.Volume.Paged)]
    public async Task<PagedResult<QueryVolumePagedOutput>> QueryVolumePaged(QueryVolumePagedInput input)
    {
        return await _volumeRepository.Entities
            .LeftJoin<TextbookModel>((v, t) => v.TextbookId == t.TextbookId)
            .WhereIF(input.TextbookId != null, (v, t) => v.TextbookId == input.TextbookId)
            .OrderByIF(input.IsOrderBy, (v, t) => v.Sort)
            .Select((v, t) => new QueryVolumePagedOutput
            {
                VolumeId = v.VolumeId,
                TextbookId = v.TextbookId,
                TextbookName = t.TextbookName,
                VolumeName = v.VolumeName,
                CoverUrl = v.CoverUrl,
                Sort = v.Sort,
                Remark = v.Remark,
                DepartmentName = v.DepartmentName,
                CreatedUserName = v.CreatedUserName,
                CreatedTime = v.CreatedTime,
                UpdatedUserName = v.UpdatedUserName,
                UpdatedTime = v.UpdatedTime,
                RowVersion = v.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取册详情
    /// </summary>
    /// <param name="volumeId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取册详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Volume.Detail)]
    public async Task<QueryVolumePagedOutput> QueryVolumeDetail([Required(ErrorMessage = "册Id不能为空")] long? volumeId)
    {
        var result = await _volumeRepository.Entities
            .LeftJoin<TextbookModel>((v, t) => v.TextbookId == t.TextbookId)
            .Where((v, t) => v.VolumeId == volumeId)
            .Select((v, t) => new QueryVolumePagedOutput
            {
                VolumeId = v.VolumeId,
                TextbookId = v.TextbookId,
                TextbookName = t.TextbookName,
                VolumeName = v.VolumeName,
                CoverUrl = v.CoverUrl,
                Sort = v.Sort,
                Remark = v.Remark,
                DepartmentName = v.DepartmentName,
                CreatedUserName = v.CreatedUserName,
                CreatedTime = v.CreatedTime,
                UpdatedUserName = v.UpdatedUserName,
                UpdatedTime = v.UpdatedTime,
                RowVersion = v.RowVersion
            })
            .SingleAsync();

        if (result == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        return result;
    }

    /// <summary>
    /// 添加册
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加册", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.Volume.Add)]
    public async Task AddVolume(AddVolumeInput input)
    {
        if (!await _textbookRepository.AnyAsync(a => a.TextbookId == input.TextbookId))
        {
            throw new UserFriendlyException("教材不存在！");
        }

        var volumeModel = new VolumeModel
        {
            TextbookId = input.TextbookId,
            VolumeName = input.VolumeName,
            CoverUrl = input.CoverUrl,
            Sort = input.Sort,
            Remark = input.Remark
        };

        await _volumeRepository.InsertAsync(volumeModel);
    }

    /// <summary>
    /// 编辑册
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑册", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Volume.Edit)]
    public async Task EditVolume(EditVolumeInput input)
    {
        if (!await _textbookRepository.AnyAsync(a => a.TextbookId == input.TextbookId))
        {
            throw new UserFriendlyException("教材不存在！");
        }

        var volumeModel = await _volumeRepository.SingleOrDefaultAsync(input.VolumeId);
        if (volumeModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        volumeModel.TextbookId = input.TextbookId;
        volumeModel.VolumeName = input.VolumeName;
        volumeModel.CoverUrl = input.CoverUrl;
        volumeModel.Sort = input.Sort;
        volumeModel.Remark = input.Remark;
        volumeModel.RowVersion = input.RowVersion;

        await _volumeRepository.UpdateAsync(volumeModel);
    }

    /// <summary>
    /// 删除册
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除册", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.Volume.Delete)]
    public async Task DeleteVolume(VolumeIdInput input)
    {
        if (await _lessonRepository.AnyAsync(a => a.VolumeId == input.VolumeId))
        {
            throw new UserFriendlyException("册存在课程关联，无法删除！");
        }

        var volumeModel = await _volumeRepository.SingleOrDefaultAsync(input.VolumeId);
        if (volumeModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        await _volumeRepository.DeleteAsync(volumeModel);
    }

    #endregion

    #region 课程管理

    /// <summary>
    /// 获取课程分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取课程分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.Lesson.Paged)]
    public async Task<PagedResult<QueryLessonPagedOutput>> QueryLessonPaged(QueryLessonPagedInput input)
    {
        return await _lessonRepository.Entities
            .LeftJoin<VolumeModel>((l, v) => l.VolumeId == v.VolumeId)
            .WhereIF(input.VolumeId != null, (l, v) => l.VolumeId == input.VolumeId)
            .OrderByIF(input.IsOrderBy, (l, v) => l.Sort)
            .Select((l, v) => new QueryLessonPagedOutput
            {
                LessonId = l.LessonId,
                VolumeId = l.VolumeId,
                VolumeName = v.VolumeName,
                LessonName = l.LessonName,
                LessonNo = l.LessonNo,
                Sort = l.Sort,
                Remark = l.Remark,
                DepartmentName = l.DepartmentName,
                CreatedUserName = l.CreatedUserName,
                CreatedTime = l.CreatedTime,
                UpdatedUserName = l.UpdatedUserName,
                UpdatedTime = l.UpdatedTime,
                RowVersion = l.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取课程详情
    /// </summary>
    /// <param name="lessonId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取课程详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Lesson.Detail)]
    public async Task<QueryLessonPagedOutput> QueryLessonDetail([Required(ErrorMessage = "课程Id不能为空")] long? lessonId)
    {
        var result = await _lessonRepository.Entities
            .LeftJoin<VolumeModel>((l, v) => l.VolumeId == v.VolumeId)
            .Where((l, v) => l.LessonId == lessonId)
            .Select((l, v) => new QueryLessonPagedOutput
            {
                LessonId = l.LessonId,
                VolumeId = l.VolumeId,
                VolumeName = v.VolumeName,
                LessonName = l.LessonName,
                LessonNo = l.LessonNo,
                Sort = l.Sort,
                Remark = l.Remark,
                DepartmentName = l.DepartmentName,
                CreatedUserName = l.CreatedUserName,
                CreatedTime = l.CreatedTime,
                UpdatedUserName = l.UpdatedUserName,
                UpdatedTime = l.UpdatedTime,
                RowVersion = l.RowVersion
            })
            .SingleAsync();

        if (result == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        return result;
    }

    /// <summary>
    /// 添加课程
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加课程", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.Lesson.Add)]
    public async Task AddLesson(AddLessonInput input)
    {
        if (!await _volumeRepository.AnyAsync(a => a.VolumeId == input.VolumeId))
        {
            throw new UserFriendlyException("册不存在！");
        }

        var lessonModel = new LessonModel
        {
            VolumeId = input.VolumeId,
            LessonName = input.LessonName,
            LessonNo = input.LessonNo,
            Sort = input.Sort,
            Remark = input.Remark
        };

        await _lessonRepository.InsertAsync(lessonModel);
    }

    /// <summary>
    /// 编辑课程
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑课程", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Lesson.Edit)]
    public async Task EditLesson(EditLessonInput input)
    {
        if (!await _volumeRepository.AnyAsync(a => a.VolumeId == input.VolumeId))
        {
            throw new UserFriendlyException("册不存在！");
        }

        var lessonModel = await _lessonRepository.SingleOrDefaultAsync(input.LessonId);
        if (lessonModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        lessonModel.VolumeId = input.VolumeId;
        lessonModel.LessonName = input.LessonName;
        lessonModel.LessonNo = input.LessonNo;
        lessonModel.Sort = input.Sort;
        lessonModel.Remark = input.Remark;
        lessonModel.RowVersion = input.RowVersion;

        await _lessonRepository.UpdateAsync(lessonModel);
    }

    /// <summary>
    /// 删除课程
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除课程", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.Lesson.Delete)]
    public async Task DeleteLesson(LessonIdInput input)
    {
        if (await _audioRepository.AnyAsync(a => a.LessonId == input.LessonId))
        {
            throw new UserFriendlyException("课程存在音频关联，无法删除！");
        }

        var lessonModel = await _lessonRepository.SingleOrDefaultAsync(input.LessonId);
        if (lessonModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        await _lessonRepository.DeleteAsync(lessonModel);
    }

    #endregion
}
