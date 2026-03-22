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
using Fast.Admin.Service.Study.Dto;
using Fast.Center.Entity;
using Microsoft.AspNetCore.Mvc;
using Yitter.IdGenerator;

namespace Fast.Admin.Service.Study;

/// <summary>
/// <see cref="StudyService"/> 学习服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Walkman, Name = "study")]
public class StudyService : IDynamicApplication
{
    private readonly IUser _user;
    private readonly ISqlSugarRepository<WordModel> _wordRepository;
    private readonly ISqlSugarRepository<UserWordRecordModel> _userWordRecordRepository;
    private readonly ISqlSugarRepository<LessonModel> _lessonRepository;
    private readonly ISqlSugarRepository<LessonContentModel> _lessonContentRepository;
    private readonly ISqlSugarRepository<QuestionModel> _questionRepository;
    private readonly ISqlSugarRepository<QuestionOptionModel> _questionOptionRepository;
    private readonly ISqlSugarRepository<UserLearningRecordModel> _learningRecordRepository;
    private readonly ISqlSugarClient _centerRepository;

    public StudyService(
        IUser user,
        ISqlSugarRepository<WordModel> wordRepository,
        ISqlSugarRepository<UserWordRecordModel> userWordRecordRepository,
        ISqlSugarRepository<LessonModel> lessonRepository,
        ISqlSugarRepository<LessonContentModel> lessonContentRepository,
        ISqlSugarRepository<QuestionModel> questionRepository,
        ISqlSugarRepository<QuestionOptionModel> questionOptionRepository,
        ISqlSugarRepository<UserLearningRecordModel> learningRecordRepository,
        ISqlSugarClient centerRepository)
    {
        _user = user;
        _wordRepository = wordRepository;
        _userWordRecordRepository = userWordRecordRepository;
        _lessonRepository = lessonRepository;
        _lessonContentRepository = lessonContentRepository;
        _questionRepository = questionRepository;
        _questionOptionRepository = questionOptionRepository;
        _learningRecordRepository = learningRecordRepository;
        _centerRepository = centerRepository;
    }

    /// <summary>
    /// 获取单词列表
    /// </summary>
    /// <param name="lessonId">课程Id</param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取单词列表", HttpRequestActionEnum.Query)]
    [AllowForbidden]
    public async Task<List<WordListOutput>> GetWordList([Required(ErrorMessage = "课程Id不能为空")] long? lessonId)
    {
        var accountId = _user.AccountId;

        var words = await _wordRepository.AsQueryable()
            .LeftJoin<UserWordRecordModel>((w, r) => w.WordId == r.WordId && r.AccountId == accountId)
            .Where((w, r) => w.LessonId == lessonId.Value)
            .OrderBy((w, r) => w.Sort)
            .Select((w, r) => new WordListOutput
            {
                WordId = w.WordId,
                LessonId = w.LessonId,
                English = w.English,
                Chinese = w.Chinese,
                Phonetic = w.Phonetic,
                AudioUrl = w.AudioUrl,
                ExampleSentence = w.ExampleSentence,
                ExampleSentenceCn = w.ExampleSentenceCn,
                Status = r.RecordId != 0 ? r.Status : WordStatusEnum.New
            })
            .ToListAsync();

        return words;
    }

    /// <summary>
    /// 获取课程内容
    /// </summary>
    /// <param name="lessonId">课程Id</param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取课程内容", HttpRequestActionEnum.Query)]
    [AllowForbidden]
    public async Task<LessonContentOutput> GetLessonContent([Required(ErrorMessage = "课程Id不能为空")] long? lessonId)
    {
        var lesson = await _lessonRepository.AsQueryable()
            .FirstAsync(l => l.LessonId == lessonId.Value);
        if (lesson == null)
        {
            throw new UserFriendlyException("课程不存在！");
        }

        var content = await _lessonContentRepository.AsQueryable()
            .FirstAsync(c => c.LessonId == lessonId.Value);
        if (content == null)
        {
            throw new UserFriendlyException("课程内容不存在！");
        }

        return new LessonContentOutput
        {
            LessonContentId = content.LessonContentId,
            LessonId = content.LessonId,
            LessonName = lesson.LessonName,
            EnglishText = content.EnglishText,
            ChineseText = content.ChineseText,
            GrammarPoints = content.GrammarPoints,
            KnowledgePoints = content.KnowledgePoints
        };
    }

    /// <summary>
    /// 获取练习题目
    /// </summary>
    /// <param name="lessonId">课程Id</param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取练习题目", HttpRequestActionEnum.Query)]
    [AllowForbidden]
    public async Task<List<ExerciseOutput>> GetExercises([Required(ErrorMessage = "课程Id不能为空")] long? lessonId)
    {
        var questions = await _questionRepository.AsQueryable()
            .Where(q => q.LessonId == lessonId.Value)
            .OrderBy(q => q.Sort)
            .ToListAsync();

        var questionIds = questions.Select(q => q.QuestionId).ToList();

        var options = await _questionOptionRepository.AsQueryable()
            .Where(o => questionIds.Contains(o.QuestionId))
            .OrderBy(o => o.Sort)
            .ToListAsync();

        var optionDict = options.GroupBy(o => o.QuestionId)
            .ToDictionary(g => g.Key, g => g.ToList());

        return questions.Select(q => new ExerciseOutput
        {
            QuestionId = q.QuestionId,
            QuestionType = q.QuestionType,
            Content = q.Content,
            Score = q.Score,
            Options = optionDict.TryGetValue(q.QuestionId, out var opts)
                ? opts.Select(o => new ExerciseOptionOutput
                {
                    QuestionOptionId = o.QuestionOptionId,
                    Label = o.Label,
                    Content = o.Content
                }).ToList()
                : new List<ExerciseOptionOutput>()
        }).ToList();
    }

    /// <summary>
    /// 提交练习
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("提交练习", HttpRequestActionEnum.Edit)]
    [AllowForbidden]
    public async Task<ExerciseResultOutput> SubmitExercise(SubmitExerciseInput input)
    {
        var questions = await _questionRepository.AsQueryable()
            .Where(q => q.LessonId == input.LessonId)
            .ToListAsync();

        var questionDict = questions.ToDictionary(q => q.QuestionId, q => q);

        var totalScore = 0;
        var score = 0;
        var correctCount = 0;
        var results = new List<AnswerResultItem>();

        foreach (var answer in input.Answers)
        {
            if (!questionDict.TryGetValue(answer.QuestionId, out var question))
            {
                continue;
            }

            totalScore += question.Score;
            var isCorrect = string.Equals(answer.Answer?.Trim(), question.Answer?.Trim(),
                StringComparison.OrdinalIgnoreCase);

            if (isCorrect)
            {
                score += question.Score;
                correctCount++;
            }

            results.Add(new AnswerResultItem
            {
                QuestionId = question.QuestionId,
                IsCorrect = isCorrect,
                CorrectAnswer = question.Answer,
                UserAnswer = answer.Answer,
                Explanation = question.Explanation
            });
        }

        // 保存学习记录
        var record = new UserLearningRecordModel
        {
            RecordId = YitIdHelper.NextId(),
            AccountId = _user.AccountId,
            LessonId = input.LessonId,
            LearningType = input.LearningType,
            Duration = input.Duration,
            Score = score,
            TotalScore = totalScore,
            LearningDate = DateTime.Now.Date
        };
        await _learningRecordRepository.AsInsertable(record).ExecuteCommandAsync();

        return new ExerciseResultOutput
        {
            TotalScore = totalScore,
            Score = score,
            CorrectCount = correctCount,
            TotalCount = input.Answers.Count,
            Results = results
        };
    }

    /// <summary>
    /// 更新单词状态
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("更新单词状态", HttpRequestActionEnum.Edit)]
    [AllowForbidden]
    public async Task UpdateWordStatus(UpdateWordStatusInput input)
    {
        var existing = await _userWordRecordRepository.AsQueryable()
            .FirstAsync(r => r.AccountId == _user.AccountId && r.WordId == input.WordId);

        if (existing != null)
        {
            existing.Status = input.Status;
            existing.ReviewCount = existing.ReviewCount + 1;
            await _userWordRecordRepository.AsUpdateable(existing)
                .UpdateColumns(r => new { r.Status, r.ReviewCount })
                .ExecuteCommandAsync();
        }
        else
        {
            var entity = new UserWordRecordModel
            {
                RecordId = YitIdHelper.NextId(),
                AccountId = _user.AccountId,
                WordId = input.WordId,
                Status = input.Status,
                ReviewCount = 1
            };
            await _userWordRecordRepository.AsInsertable(entity).ExecuteCommandAsync();
        }
    }

    /// <summary>
    /// 记录学习活动
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("记录学习活动", HttpRequestActionEnum.Edit)]
    [AllowForbidden]
    public async Task RecordLearning(RecordLearningInput input)
    {
        var record = new UserLearningRecordModel
        {
            RecordId = YitIdHelper.NextId(),
            AccountId = _user.AccountId,
            LessonId = input.LessonId,
            LearningType = input.LearningType,
            Duration = input.Duration,
            LearningDate = DateTime.Now.Date
        };
        await _learningRecordRepository.AsInsertable(record).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取排行榜
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取排行榜", HttpRequestActionEnum.Query)]
    [AllowForbidden]
    public async Task<List<RankingOutput>> GetRanking()
    {
        // 按用户汇总学习时长
        var rankingData = await _learningRecordRepository.AsQueryable()
            .GroupBy(r => r.AccountId)
            .Select(r => new
            {
                AccountId = r.AccountId,
                TotalDuration = SqlFunc.AggregateSum(r.Duration),
                TotalLessons = SqlFunc.AggregateDistinctCount(r.LessonId),
                TotalScore = SqlFunc.AggregateSum(r.Score ?? 0)
            })
            .OrderByDescending(r => r.TotalDuration)
            .Take(50)
            .ToListAsync();

        if (!rankingData.Any())
        {
            return new List<RankingOutput>();
        }

        // 获取用户信息
        var accountIds = rankingData.Select(r => r.AccountId).ToList();
        var accounts = await _centerRepository.Queryable<AccountModel>()
            .Where(a => accountIds.Contains(a.AccountId))
            .ToListAsync();
        var accountDict = accounts.ToDictionary(a => a.AccountId, a => a);

        // 获取用户掌握单词数
        var wordCounts = await _userWordRecordRepository.AsQueryable()
            .Where(r => accountIds.Contains(r.AccountId))
            .GroupBy(r => r.AccountId)
            .Select(r => new
            {
                AccountId = r.AccountId,
                TotalWords = SqlFunc.AggregateCount(r.WordId)
            })
            .ToListAsync();
        var wordCountDict = wordCounts.ToDictionary(w => w.AccountId, w => w.TotalWords);

        var result = new List<RankingOutput>();
        for (var i = 0; i < rankingData.Count; i++)
        {
            var data = rankingData[i];
            accountDict.TryGetValue(data.AccountId, out var account);
            wordCountDict.TryGetValue(data.AccountId, out var totalWords);

            result.Add(new RankingOutput
            {
                Rank = i + 1,
                AccountId = data.AccountId,
                NickName = account?.NickName,
                Avatar = account?.Avatar,
                TotalDuration = data.TotalDuration,
                TotalLessons = data.TotalLessons,
                TotalWords = totalWords,
                TotalScore = data.TotalScore
            });
        }

        return result;
    }

    /// <summary>
    /// 获取学习统计
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取学习统计", HttpRequestActionEnum.Query)]
    [AllowForbidden]
    public async Task<LearningStatsOutput> GetLearningStats()
    {
        var accountId = _user.AccountId;

        // 学习记录汇总
        var records = await _learningRecordRepository.AsQueryable()
            .Where(r => r.AccountId == accountId)
            .ToListAsync();

        var totalDuration = records.Sum(r => r.Duration);
        var totalLessons = records.Select(r => r.LessonId).Distinct().Count();
        var totalExercises = records.Count(r => r.LearningType == LearningTypeEnum.Exercise);
        var totalTests = records.Count(r => r.LearningType == LearningTypeEnum.Test);

        var scoredRecords = records.Where(r => r.Score.HasValue && r.TotalScore.HasValue && r.TotalScore.Value > 0).ToList();
        var averageScore = scoredRecords.Any()
            ? (int)Math.Round(scoredRecords.Average(r => (double)r.Score.Value / r.TotalScore.Value * 100))
            : 0;

        // 单词统计
        var wordRecords = await _userWordRecordRepository.AsQueryable()
            .Where(r => r.AccountId == accountId)
            .ToListAsync();

        var totalWords = wordRecords.Count;
        var masteredWords = wordRecords.Count(r => r.Status == WordStatusEnum.Mastered);
        var learningWords = wordRecords.Count(r => r.Status == WordStatusEnum.Learning);

        // 连续学习天数
        var learningDates = records
            .Select(r => r.LearningDate.Date)
            .Distinct()
            .OrderByDescending(d => d)
            .ToList();

        var continuousDays = 0;
        var checkDate = DateTime.Now.Date;
        foreach (var date in learningDates)
        {
            if (date == checkDate)
            {
                continuousDays++;
                checkDate = checkDate.AddDays(-1);
            }
            else
            {
                break;
            }
        }

        // 最近30天每日学习记录
        var thirtyDaysAgo = DateTime.Now.Date.AddDays(-29);
        var dailyRecords = records
            .Where(r => r.LearningDate.Date >= thirtyDaysAgo)
            .GroupBy(r => r.LearningDate.Date)
            .Select(g => new DailyLearningOutput
            {
                Date = g.Key,
                Duration = g.Sum(r => r.Duration),
                LessonCount = g.Select(r => r.LessonId).Distinct().Count(),
                WordCount = 0
            })
            .OrderBy(d => d.Date)
            .ToList();

        // 补充每日单词学习数量
        var dailyWordCounts = await _userWordRecordRepository.AsQueryable()
            .Where(r => r.AccountId == accountId && r.CreatedTime >= thirtyDaysAgo)
            .GroupBy(r => r.CreatedTime.Value.Date)
            .Select(r => new
            {
                Date = r.CreatedTime.Value.Date,
                WordCount = SqlFunc.AggregateCount(r.WordId)
            })
            .ToListAsync();

        var wordCountByDate = dailyWordCounts.ToDictionary(w => w.Date, w => w.WordCount);
        foreach (var daily in dailyRecords)
        {
            if (wordCountByDate.TryGetValue(daily.Date, out var wordCount))
            {
                daily.WordCount = wordCount;
            }
        }

        return new LearningStatsOutput
        {
            TotalDuration = totalDuration,
            TotalLessons = totalLessons,
            TotalWords = totalWords,
            MasteredWords = masteredWords,
            LearningWords = learningWords,
            TotalExercises = totalExercises,
            TotalTests = totalTests,
            AverageScore = averageScore,
            ContinuousDays = continuousDays,
            DailyRecords = dailyRecords
        };
    }

    /// <summary>
    /// 获取今日签到状态
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取今日签到状态", HttpRequestActionEnum.Query)]
    [AllowForbidden]
    public async Task<bool> GetDailyCheckIn()
    {
        var today = DateTime.Now.Date;
        return await _learningRecordRepository.AsQueryable()
            .AnyAsync(r => r.AccountId == _user.AccountId && r.LearningDate == today);
    }
}
