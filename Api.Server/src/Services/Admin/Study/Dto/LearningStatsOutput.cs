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

namespace Fast.Admin.Service.Study.Dto;

/// <summary>
/// <see cref="LearningStatsOutput"/> 学习统计输出
/// </summary>
public class LearningStatsOutput
{
    /// <summary>总学习时长（秒）</summary>
    public int TotalDuration { get; set; }

    /// <summary>总课程数</summary>
    public int TotalLessons { get; set; }

    /// <summary>总单词数</summary>
    public int TotalWords { get; set; }

    /// <summary>已掌握单词数</summary>
    public int MasteredWords { get; set; }

    /// <summary>学习中单词数</summary>
    public int LearningWords { get; set; }

    /// <summary>总练习次数</summary>
    public int TotalExercises { get; set; }

    /// <summary>总测试次数</summary>
    public int TotalTests { get; set; }

    /// <summary>平均得分</summary>
    public int AverageScore { get; set; }

    /// <summary>连续学习天数</summary>
    public int ContinuousDays { get; set; }

    /// <summary>每日学习记录</summary>
    public List<DailyLearningOutput> DailyRecords { get; set; }
}

/// <summary>
/// <see cref="DailyLearningOutput"/> 每日学习输出
/// </summary>
public class DailyLearningOutput
{
    /// <summary>日期</summary>
    public DateTime Date { get; set; }

    /// <summary>学习时长（秒）</summary>
    public int Duration { get; set; }

    /// <summary>课程数</summary>
    public int LessonCount { get; set; }

    /// <summary>单词数</summary>
    public int WordCount { get; set; }
}
