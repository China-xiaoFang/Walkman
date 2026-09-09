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
using Fast.Walkman.Service.Lesson.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Walkman.Service.Lesson;

/// <summary>
/// 课程服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Walkman, Name = "lesson")]
public class LessonService : IDynamicApplication
{
    private readonly ISqlSugarRepository<LessonModel> _repository;

    public LessonService(ISqlSugarRepository<LessonModel> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 课程分页选择器
    /// </summary>
    [HttpPost]
    [ApiInfo("课程分页选择器", HttpRequestActionEnum.Paged)]
    public async Task<PagedResult<ElSelectorOutput<long>>> LessonSelector(QueryLessonPagedInput input)
    {
        var pagedData = await _repository.Entities.LeftJoin<BookModel>((t1, t2) => t1.BookId == t2.BookId)
            .WhereIF(input.BookId != null, t1 => t1.BookId == input.BookId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.SearchValue),
                t1 => t1.LessonTitle.Contains(input.SearchValue)
                      || t1.English.Contains(input.SearchValue)
                      || t1.Chinese.Contains(input.SearchValue))
            .OrderBy(t1 => t1.BookId)
            .OrderBy(t1 => t1.LessonNumber)
            .Select((t1, t2) => new
            {
                t1.LessonId,
                t1.BookId,
                t2.BookName,
                t2.Status,
                t1.LessonTitle,
                t1.LessonNumber
            })
            .ToPagedListAsync(input);

        return pagedData.ToPagedData(sl => new ElSelectorOutput<long>
        {
            Value = sl.LessonId,
            Label = sl.LessonTitle,
            Disabled = sl.Status == CommonStatusEnum.Disable,
            Data = new {sl.BookId, sl.BookName, sl.LessonNumber}
        });
    }

    /// <summary>
    /// 获取课程分页列表
    /// </summary>
    [HttpPost]
    [ApiInfo("获取课程分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.Lesson.Paged)]
    public async Task<PagedResult<QueryLessonPagedOutput>> QueryLessonPaged(QueryLessonPagedInput input)
    {
        return await _repository.Entities.LeftJoin<BookModel>((t1, t2) => t1.BookId == t2.BookId)
            .WhereIF(input.BookId != null, t1 => t1.BookId == input.BookId)
            .OrderBy(t1 => t1.BookId)
            .OrderBy(t1 => t1.LessonNumber)
            .Select((t1, t2) => new QueryLessonPagedOutput
            {
                LessonId = t1.LessonId,
                BookId = t1.BookId,
                BookName = t2.BookName,
                LessonTitle = t1.LessonTitle,
                LessonNumber = t1.LessonNumber,
                English = t1.English,
                Chinese = t1.Chinese,
                Remark = t1.Remark,
                CreatedUserName = t1.CreatedUserName,
                CreatedTime = t1.CreatedTime,
                UpdatedUserName = t1.UpdatedUserName,
                UpdatedTime = t1.UpdatedTime,
                RowVersion = t1.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取课程详情
    /// </summary>
    [HttpGet]
    [ApiInfo("获取课程详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Lesson.Detail)]
    public async Task<QueryLessonDetailOutput> QueryLessonDetail([Required(ErrorMessage = "课程Id不能为空")] long? lessonId)
    {
        var result = await _repository.Entities.LeftJoin<BookModel>((t1, t2) => t1.BookId == t2.BookId)
            .Where(t1 => t1.LessonId == lessonId)
            .Select((t1, t2) => new QueryLessonDetailOutput
            {
                LessonId = t1.LessonId,
                BookId = t1.BookId,
                BookName = t2.BookName,
                LessonTitle = t1.LessonTitle,
                LessonNumber = t1.LessonNumber,
                English = t1.English,
                Chinese = t1.Chinese,
                Remark = t1.Remark,
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

        return result;
    }

    /// <summary>
    /// 添加课程
    /// </summary>
    [HttpPost]
    [ApiInfo("添加课程", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.Lesson.Add)]
    public async Task AddLesson(AddLessonInput input)
    {
        if (!await _repository.Queryable<BookModel>()
                .AnyAsync(a => a.BookId == input.BookId))
        {
            throw new UserFriendlyException("教材不存在！");
        }

        if (await _repository.AnyAsync(a => a.BookId == input.BookId && a.LessonNumber == input.LessonNumber))
        {
            throw new UserFriendlyException("同一教材下的课程编号不能重复！");
        }

        var lessonModel = new LessonModel
        {
            BookId = input.BookId,
            LessonTitle = input.LessonTitle,
            LessonNumber = input.LessonNumber,
            English = input.English,
            Chinese = input.Chinese,
            Remark = input.Remark
        };

        await _repository.InsertAsync(lessonModel);

        await LogContext.OperateLog(new OperateLogDto
        {
            Title = "添加课程",
            OperateType = OperateLogTypeEnum.Content,
            BizId = lessonModel.LessonId,
            BizNo = null,
            Description = $"添加课程：{lessonModel.LessonTitle}"
        });
    }

    /// <summary>
    /// 编辑课程
    /// </summary>
    [HttpPost]
    [ApiInfo("编辑课程", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Lesson.Edit)]
    public async Task EditLesson(EditLessonInput input)
    {
        if (!await _repository.Queryable<BookModel>()
                .AnyAsync(a => a.BookId == input.BookId))
        {
            throw new UserFriendlyException("教材不存在！");
        }

        if (await _repository.AnyAsync(a =>
                a.BookId == input.BookId && a.LessonNumber == input.LessonNumber && a.LessonId != input.LessonId))
        {
            throw new UserFriendlyException("同一教材下的课程编号不能重复！");
        }

        var lessonModel = await _repository.SingleOrDefaultAsync(input.LessonId);
        if (lessonModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        lessonModel.BookId = input.BookId;
        lessonModel.LessonTitle = input.LessonTitle;
        lessonModel.LessonNumber = input.LessonNumber;
        lessonModel.English = input.English;
        lessonModel.Chinese = input.Chinese;
        lessonModel.Remark = input.Remark;
        lessonModel.RowVersion = input.RowVersion;

        await _repository.Ado.UseTranAsync(async () =>
        {
            await _repository.UpdateAsync(lessonModel);
            await _repository.Updateable<AudioAssetModel>()
                .SetColumns(_ => new AudioAssetModel {BookId = lessonModel.BookId})
                .Where(wh => wh.LessonId == lessonModel.LessonId)
                .ExecuteCommandAsync();
        }, ex => throw ex);

        await LogContext.OperateLog(new OperateLogDto
        {
            Title = "编辑课程",
            OperateType = OperateLogTypeEnum.Content,
            BizId = lessonModel.LessonId,
            BizNo = null,
            Description = $"编辑课程：{lessonModel.LessonTitle}"
        });
    }

    /// <summary>
    /// 删除课程
    /// </summary>
    [HttpPost]
    [ApiInfo("删除课程", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.Lesson.Delete)]
    public async Task DeleteLesson(LessonIdInput input)
    {
        if (await _repository.Queryable<AudioAssetModel>()
                .AnyAsync(a => a.LessonId == input.LessonId))
        {
            throw new UserFriendlyException("课程下存在音频资源，无法删除！");
        }

        var lessonModel = await _repository.SingleOrDefaultAsync(input.LessonId);
        if (lessonModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        await _repository.DeleteAsync(lessonModel);

        await LogContext.OperateLog(new OperateLogDto
        {
            Title = "删除课程",
            OperateType = OperateLogTypeEnum.Content,
            BizId = lessonModel.LessonId,
            BizNo = null,
            Description = $"删除课程：{lessonModel.LessonTitle}"
        });
    }
}