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

using Fast.Admin.Enum;

namespace Fast.Scheduler;

/// <summary>
/// <see cref="ISchedulerCenter"/> 调度中心
/// </summary>
public interface ISchedulerCenter
{
    /// <summary>
    /// 初始化调度程序
    /// </summary>
    /// <returns></returns>
    Task InitializeScheduler();

    /// <summary>
    /// 获取调度器详情
    /// </summary>
    /// <returns></returns>
    Task<QuerySchedulerDetailOutput> QuerySchedulerDetail();

    /// <summary>
    /// 启动调度器
    /// </summary>
    /// <returns></returns>
    Task<bool> StartScheduler();

    /// <summary>
    /// 停止调度器
    /// </summary>
    /// <returns></returns>
    Task<bool> StopScheduler();

    /// <summary>
    /// 暂停调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task StopSchedulerJob(SchedulerJobKeyInput input);

    /// <summary>
    /// 恢复调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task ResumeSchedulerJob(SchedulerJobKeyInput input);

    /// <summary>
    /// 立即执行调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task TriggerSchedulerJob(SchedulerJobKeyInput input);

    /// <summary>
    /// 是否存在调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<bool> ExistsSchedulerJob(SchedulerJobKeyInput input);

    /// <summary>
    /// 获取调度作业日志
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<List<string>> QuerySchedulerJobLogs(SchedulerJobKeyInput input);

    /// <summary>
    /// 获取调度作业运行次数
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<long> QuerySchedulerJobRunNumber(SchedulerJobKeyInput input);

    /// <summary>
    /// 获取全部调度作业
    /// </summary>
    /// <param name="jobGroup"></param>
    /// <returns></returns>
    Task<List<QueryAllSchedulerJobOutput>> QueryAllSchedulerJob(SchedulerJobGroupEnum? jobGroup = null);

    /// <summary>
    /// 获取调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<SchedulerJobInfo> QuerySchedulerJob(SchedulerJobKeyInput input);

    /// <summary>
    /// 添加调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task AddSchedulerJob(AddSchedulerJobInput input);

    /// <summary>
    /// 编辑调度作业
    /// </summary>
    /// <remarks>注：这里更新作业会导致触发器的执行记录被清空。所以会导致更新后可能会立即执行一次。</remarks>
    /// <param name="input"></param>
    /// <returns></returns>
    Task EditSchedulerJob(EditSchedulerJobInput input);

    /// <summary>
    /// 删除调度作业
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task DeleteSchedulerJob(SchedulerJobKeyInput input);

    /// <summary>
    /// 移除调度作业异常信息
    /// </summary>
    /// <remarks>因为只能在 IJob 持久化操作 JobDataMap，所以这里直接暴力操作数据库</remarks>
    /// <param name="input"></param>
    /// <returns></returns>
    Task DeleteSchedulerJobException(SchedulerJobKeyInput input);
}