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
using Fast.Admin.Service.Question.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Service.Question;

/// <summary>
/// <see cref="QuestionService"/> 题目服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Walkman, Name = "question")]
public class QuestionService : IDynamicApplication
{
    private readonly ISqlSugarRepository<QuestionModel> _questionRepository;
    private readonly ISqlSugarRepository<QuestionOptionModel> _questionOptionRepository;
    private readonly ISqlSugarRepository<LessonModel> _lessonRepository;

    public QuestionService(ISqlSugarRepository<QuestionModel> questionRepository,
        ISqlSugarRepository<QuestionOptionModel> questionOptionRepository,
        ISqlSugarRepository<LessonModel> lessonRepository)
    {
        _questionRepository = questionRepository;
        _questionOptionRepository = questionOptionRepository;
        _lessonRepository = lessonRepository;
    }

    /// <summary>
    /// 获取题目分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取题目分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.QuestionManage.Paged)]
    public async Task<PagedResult<QueryQuestionPagedOutput>> QueryQuestionPaged(QueryQuestionPagedInput input)
    {
        return await _questionRepository.Entities
            .LeftJoin<LessonModel>((q, l) => q.LessonId == l.LessonId)
            .WhereIF(input.LessonId != null, (q, l) => q.LessonId == input.LessonId)
            .WhereIF(input.QuestionType != null, (q, l) => q.QuestionType == input.QuestionType)
            .OrderByIF(input.IsOrderBy, (q, l) => q.Sort)
            .Select((q, l) => new QueryQuestionPagedOutput
            {
                QuestionId = q.QuestionId,
                LessonId = q.LessonId,
                LessonName = l.LessonName,
                QuestionType = q.QuestionType,
                Content = q.Content,
                Answer = q.Answer,
                Explanation = q.Explanation,
                Score = q.Score,
                Sort = q.Sort,
                DepartmentName = q.DepartmentName,
                CreatedUserName = q.CreatedUserName,
                CreatedTime = q.CreatedTime,
                UpdatedUserName = q.UpdatedUserName,
                UpdatedTime = q.UpdatedTime,
                RowVersion = q.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取题目详情
    /// </summary>
    /// <param name="questionId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取题目详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.QuestionManage.Detail)]
    public async Task<QueryQuestionDetailOutput> QueryQuestionDetail(
        [Required(ErrorMessage = "题目Id不能为空")] long? questionId)
    {
        var result = await _questionRepository.Entities
            .LeftJoin<LessonModel>((q, l) => q.LessonId == l.LessonId)
            .Where((q, l) => q.QuestionId == questionId)
            .Select((q, l) => new QueryQuestionDetailOutput
            {
                QuestionId = q.QuestionId,
                LessonId = q.LessonId,
                LessonName = l.LessonName,
                QuestionType = q.QuestionType,
                Content = q.Content,
                Answer = q.Answer,
                Explanation = q.Explanation,
                Score = q.Score,
                Sort = q.Sort,
                DepartmentName = q.DepartmentName,
                CreatedUserName = q.CreatedUserName,
                CreatedTime = q.CreatedTime,
                UpdatedUserName = q.UpdatedUserName,
                UpdatedTime = q.UpdatedTime,
                RowVersion = q.RowVersion
            })
            .SingleAsync();

        if (result == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        result.Options = await _questionOptionRepository.Entities
            .Where(o => o.QuestionId == questionId)
            .OrderBy(o => o.Sort)
            .Select(o => new QueryQuestionOptionOutput
            {
                QuestionOptionId = o.QuestionOptionId,
                Label = o.Label,
                Content = o.Content,
                IsCorrect = o.IsCorrect,
                Sort = o.Sort
            })
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// 添加题目
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加题目", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.QuestionManage.Add)]
    public async Task AddQuestion(AddQuestionInput input)
    {
        if (!await _lessonRepository.AnyAsync(a => a.LessonId == input.LessonId))
        {
            throw new UserFriendlyException("课程不存在！");
        }

        var questionModel = new QuestionModel
        {
            LessonId = input.LessonId,
            QuestionType = input.QuestionType,
            Content = input.Content,
            Answer = input.Answer,
            Explanation = input.Explanation,
            Score = input.Score,
            Sort = input.Sort
        };

        await _questionRepository.InsertAsync(questionModel);

        if (input.Options is { Count: > 0 })
        {
            var optionModels = input.Options.Select(o => new QuestionOptionModel
            {
                QuestionId = questionModel.QuestionId,
                Label = o.Label,
                Content = o.Content,
                IsCorrect = o.IsCorrect,
                Sort = o.Sort
            }).ToList();

            await _questionOptionRepository.InsertRangeAsync(optionModels);
        }
    }

    /// <summary>
    /// 编辑题目
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑题目", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.QuestionManage.Edit)]
    public async Task EditQuestion(EditQuestionInput input)
    {
        if (!await _lessonRepository.AnyAsync(a => a.LessonId == input.LessonId))
        {
            throw new UserFriendlyException("课程不存在！");
        }

        var questionModel = await _questionRepository.SingleOrDefaultAsync(input.QuestionId);
        if (questionModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        questionModel.LessonId = input.LessonId;
        questionModel.QuestionType = input.QuestionType;
        questionModel.Content = input.Content;
        questionModel.Answer = input.Answer;
        questionModel.Explanation = input.Explanation;
        questionModel.Score = input.Score;
        questionModel.Sort = input.Sort;
        questionModel.RowVersion = input.RowVersion;

        await _questionRepository.UpdateAsync(questionModel);

        // 删除旧选项
        await _questionOptionRepository.DeleteAsync(o => o.QuestionId == input.QuestionId);

        // 插入新选项
        if (input.Options is { Count: > 0 })
        {
            var optionModels = input.Options.Select(o => new QuestionOptionModel
            {
                QuestionId = input.QuestionId,
                Label = o.Label,
                Content = o.Content,
                IsCorrect = o.IsCorrect,
                Sort = o.Sort
            }).ToList();

            await _questionOptionRepository.InsertRangeAsync(optionModels);
        }
    }

    /// <summary>
    /// 删除题目
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除题目", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.QuestionManage.Delete)]
    public async Task DeleteQuestion(QuestionIdInput input)
    {
        var questionModel = await _questionRepository.SingleOrDefaultAsync(input.QuestionId);
        if (questionModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        // 删除关联选项
        await _questionOptionRepository.DeleteAsync(o => o.QuestionId == input.QuestionId);

        await _questionRepository.DeleteAsync(questionModel);
    }
}
