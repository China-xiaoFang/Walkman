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
using Fast.Admin.Service.LessonContent.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Service.LessonContent;

/// <summary>
/// <see cref="LessonContentService"/> 课程内容服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Walkman, Name = "lessonContent")]
public class LessonContentService : IDynamicApplication
{
    private readonly ISqlSugarRepository<LessonContentModel> _lessonContentRepository;
    private readonly ISqlSugarRepository<LessonModel> _lessonRepository;

    public LessonContentService(ISqlSugarRepository<LessonContentModel> lessonContentRepository,
        ISqlSugarRepository<LessonModel> lessonRepository)
    {
        _lessonContentRepository = lessonContentRepository;
        _lessonRepository = lessonRepository;
    }

    /// <summary>
    /// 获取课程内容分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取课程内容分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.LessonContentManage.Paged)]
    public async Task<PagedResult<QueryLessonContentPagedOutput>> QueryLessonContentPaged(
        QueryLessonContentPagedInput input)
    {
        return await _lessonContentRepository.Entities
            .LeftJoin<LessonModel>((lc, l) => lc.LessonId == l.LessonId)
            .WhereIF(input.LessonId != null, (lc, l) => lc.LessonId == input.LessonId)
            .Select((lc, l) => new QueryLessonContentPagedOutput
            {
                LessonContentId = lc.LessonContentId,
                LessonId = lc.LessonId,
                LessonName = l.LessonName,
                EnglishText = lc.EnglishText,
                ChineseText = lc.ChineseText,
                GrammarPoints = lc.GrammarPoints,
                KnowledgePoints = lc.KnowledgePoints,
                DepartmentName = lc.DepartmentName,
                CreatedUserName = lc.CreatedUserName,
                CreatedTime = lc.CreatedTime,
                UpdatedUserName = lc.UpdatedUserName,
                UpdatedTime = lc.UpdatedTime,
                RowVersion = lc.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取课程内容详情
    /// </summary>
    /// <param name="lessonContentId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取课程内容详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.LessonContentManage.Detail)]
    public async Task<QueryLessonContentPagedOutput> QueryLessonContentDetail(
        [Required(ErrorMessage = "课程内容Id不能为空")] long? lessonContentId)
    {
        var result = await _lessonContentRepository.Entities
            .LeftJoin<LessonModel>((lc, l) => lc.LessonId == l.LessonId)
            .Where((lc, l) => lc.LessonContentId == lessonContentId)
            .Select((lc, l) => new QueryLessonContentPagedOutput
            {
                LessonContentId = lc.LessonContentId,
                LessonId = lc.LessonId,
                LessonName = l.LessonName,
                EnglishText = lc.EnglishText,
                ChineseText = lc.ChineseText,
                GrammarPoints = lc.GrammarPoints,
                KnowledgePoints = lc.KnowledgePoints,
                DepartmentName = lc.DepartmentName,
                CreatedUserName = lc.CreatedUserName,
                CreatedTime = lc.CreatedTime,
                UpdatedUserName = lc.UpdatedUserName,
                UpdatedTime = lc.UpdatedTime,
                RowVersion = lc.RowVersion
            })
            .SingleAsync();

        if (result == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        return result;
    }

    /// <summary>
    /// 添加课程内容
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加课程内容", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.LessonContentManage.Add)]
    public async Task AddLessonContent(AddLessonContentInput input)
    {
        if (!await _lessonRepository.AnyAsync(a => a.LessonId == input.LessonId))
        {
            throw new UserFriendlyException("课程不存在！");
        }

        var lessonContentModel = new LessonContentModel
        {
            LessonId = input.LessonId,
            EnglishText = input.EnglishText,
            ChineseText = input.ChineseText,
            GrammarPoints = input.GrammarPoints,
            KnowledgePoints = input.KnowledgePoints
        };

        await _lessonContentRepository.InsertAsync(lessonContentModel);
    }

    /// <summary>
    /// 编辑课程内容
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑课程内容", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.LessonContentManage.Edit)]
    public async Task EditLessonContent(EditLessonContentInput input)
    {
        if (!await _lessonRepository.AnyAsync(a => a.LessonId == input.LessonId))
        {
            throw new UserFriendlyException("课程不存在！");
        }

        var lessonContentModel = await _lessonContentRepository.SingleOrDefaultAsync(input.LessonContentId);
        if (lessonContentModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        lessonContentModel.LessonId = input.LessonId;
        lessonContentModel.EnglishText = input.EnglishText;
        lessonContentModel.ChineseText = input.ChineseText;
        lessonContentModel.GrammarPoints = input.GrammarPoints;
        lessonContentModel.KnowledgePoints = input.KnowledgePoints;
        lessonContentModel.RowVersion = input.RowVersion;

        await _lessonContentRepository.UpdateAsync(lessonContentModel);
    }

    /// <summary>
    /// 删除课程内容
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除课程内容", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.LessonContentManage.Delete)]
    public async Task DeleteLessonContent(LessonContentIdInput input)
    {
        var lessonContentModel = await _lessonContentRepository.SingleOrDefaultAsync(input.LessonContentId);
        if (lessonContentModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        await _lessonContentRepository.DeleteAsync(lessonContentModel);
    }
}
