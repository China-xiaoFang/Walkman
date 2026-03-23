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

namespace Fast.Admin.Enum;

/// <summary>
/// <see cref="SchedulerJobGroupEnum"/> 调度作业分组枚举
/// </summary>
[Flags]
[FastEnum("调度作业分组枚举")]
public enum SchedulerJobGroupEnum : byte
{
    /// <summary>
    /// 系统管理
    /// </summary>
    [TagType(TagTypeEnum.Primary)]
    [Description("系统管理")]
    System = 1,

    /// <summary>
    /// 业务处理
    /// </summary>
    [TagType(TagTypeEnum.Warning)]
    [Description("业务处理")]
    Business = 2,

    /// <summary>
    /// 第三方集成
    /// </summary>
    [TagType(TagTypeEnum.Danger)]
    [Description("第三方集成")]
    ThirdParty = 4,

    /// <summary>
    /// 自定义
    /// </summary>
    [TagType(TagTypeEnum.Info)]
    [Description("自定义")]
    Custom = 8
}