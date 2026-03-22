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
using Fast.Admin.Service.Word.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Service.Word;

/// <summary>
/// <see cref="WordService"/> 单词服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Walkman, Name = "word")]
public class WordService : IDynamicApplication
{
    private readonly ISqlSugarRepository<WordModel> _wordRepository;
    private readonly ISqlSugarRepository<LessonModel> _lessonRepository;

    public WordService(ISqlSugarRepository<WordModel> wordRepository,
        ISqlSugarRepository<LessonModel> lessonRepository)
    {
        _wordRepository = wordRepository;
        _lessonRepository = lessonRepository;
    }

    /// <summary>
    /// 获取单词分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取单词分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.WordManage.Paged)]
    public async Task<PagedResult<QueryWordPagedOutput>> QueryWordPaged(QueryWordPagedInput input)
    {
        return await _wordRepository.Entities
            .LeftJoin<LessonModel>((w, l) => w.LessonId == l.LessonId)
            .WhereIF(input.LessonId != null, (w, l) => w.LessonId == input.LessonId)
            .OrderByIF(input.IsOrderBy, (w, l) => w.Sort)
            .Select((w, l) => new QueryWordPagedOutput
            {
                WordId = w.WordId,
                LessonId = w.LessonId,
                LessonName = l.LessonName,
                English = w.English,
                Chinese = w.Chinese,
                Phonetic = w.Phonetic,
                AudioUrl = w.AudioUrl,
                ExampleSentence = w.ExampleSentence,
                ExampleSentenceCn = w.ExampleSentenceCn,
                Sort = w.Sort,
                DepartmentName = w.DepartmentName,
                CreatedUserName = w.CreatedUserName,
                CreatedTime = w.CreatedTime,
                UpdatedUserName = w.UpdatedUserName,
                UpdatedTime = w.UpdatedTime,
                RowVersion = w.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取单词详情
    /// </summary>
    /// <param name="wordId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取单词详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.WordManage.Detail)]
    public async Task<QueryWordPagedOutput> QueryWordDetail([Required(ErrorMessage = "单词Id不能为空")] long? wordId)
    {
        var result = await _wordRepository.Entities
            .LeftJoin<LessonModel>((w, l) => w.LessonId == l.LessonId)
            .Where((w, l) => w.WordId == wordId)
            .Select((w, l) => new QueryWordPagedOutput
            {
                WordId = w.WordId,
                LessonId = w.LessonId,
                LessonName = l.LessonName,
                English = w.English,
                Chinese = w.Chinese,
                Phonetic = w.Phonetic,
                AudioUrl = w.AudioUrl,
                ExampleSentence = w.ExampleSentence,
                ExampleSentenceCn = w.ExampleSentenceCn,
                Sort = w.Sort,
                DepartmentName = w.DepartmentName,
                CreatedUserName = w.CreatedUserName,
                CreatedTime = w.CreatedTime,
                UpdatedUserName = w.UpdatedUserName,
                UpdatedTime = w.UpdatedTime,
                RowVersion = w.RowVersion
            })
            .SingleAsync();

        if (result == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        return result;
    }

    /// <summary>
    /// 添加单词
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加单词", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.WordManage.Add)]
    public async Task AddWord(AddWordInput input)
    {
        if (!await _lessonRepository.AnyAsync(a => a.LessonId == input.LessonId))
        {
            throw new UserFriendlyException("课程不存在！");
        }

        var wordModel = new WordModel
        {
            LessonId = input.LessonId,
            English = input.English,
            Chinese = input.Chinese,
            Phonetic = input.Phonetic,
            AudioUrl = input.AudioUrl,
            ExampleSentence = input.ExampleSentence,
            ExampleSentenceCn = input.ExampleSentenceCn,
            Sort = input.Sort
        };

        await _wordRepository.InsertAsync(wordModel);
    }

    /// <summary>
    /// 编辑单词
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑单词", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.WordManage.Edit)]
    public async Task EditWord(EditWordInput input)
    {
        if (!await _lessonRepository.AnyAsync(a => a.LessonId == input.LessonId))
        {
            throw new UserFriendlyException("课程不存在！");
        }

        var wordModel = await _wordRepository.SingleOrDefaultAsync(input.WordId);
        if (wordModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        wordModel.LessonId = input.LessonId;
        wordModel.English = input.English;
        wordModel.Chinese = input.Chinese;
        wordModel.Phonetic = input.Phonetic;
        wordModel.AudioUrl = input.AudioUrl;
        wordModel.ExampleSentence = input.ExampleSentence;
        wordModel.ExampleSentenceCn = input.ExampleSentenceCn;
        wordModel.Sort = input.Sort;
        wordModel.RowVersion = input.RowVersion;

        await _wordRepository.UpdateAsync(wordModel);
    }

    /// <summary>
    /// 删除单词
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除单词", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.WordManage.Delete)]
    public async Task DeleteWord(WordIdInput input)
    {
        var wordModel = await _wordRepository.SingleOrDefaultAsync(input.WordId);
        if (wordModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        await _wordRepository.DeleteAsync(wordModel);
    }
}
