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

namespace Fast.Core;

/// <summary>
/// 权限常量
/// </summary>
/// <remarks>自己的业务权限放这里</remarks>
public static partial class PermissionConst
{
    /// <summary>激活码</summary>
    public static class ActivationCode
    {
        /// <summary>激活码列表</summary>
        public const string Paged = "ActivationCode:Paged";

        /// <summary>激活码详情</summary>
        public const string Detail = "ActivationCode:Detail";

        /// <summary>激活码生成</summary>
        public const string Generate = "ActivationCode:Generate";
    }

    /// <summary>教材</summary>
    public static class Book
    {
        /// <summary>教材列表</summary>
        public const string Paged = "Book:Paged";

        /// <summary>教材详情</summary>
        public const string Detail = "Book:Detail";

        /// <summary>教材新增</summary>
        public const string Add = "Book:Add";

        /// <summary>教材编辑</summary>
        public const string Edit = "Book:Edit";

        /// <summary>教材删除</summary>
        public const string Delete = "Book:Delete";
    }

    /// <summary>课程</summary>
    public static class Lesson
    {
        /// <summary>课程列表</summary>
        public const string Paged = "Lesson:Paged";

        /// <summary>课程详情</summary>
        public const string Detail = "Lesson:Detail";

        /// <summary>课程新增</summary>
        public const string Add = "Lesson:Add";

        /// <summary>课程编辑</summary>
        public const string Edit = "Lesson:Edit";

        /// <summary>课程删除</summary>
        public const string Delete = "Lesson:Delete";
    }

    /// <summary>音频资源</summary>
    public static class AudioAsset
    {
        /// <summary>音频资源列表</summary>
        public const string Paged = "AudioAsset:Paged";

        /// <summary>音频资源详情</summary>
        public const string Detail = "AudioAsset:Detail";

        /// <summary>音频资源新增</summary>
        public const string Add = "AudioAsset:Add";

        /// <summary>音频资源编辑</summary>
        public const string Edit = "AudioAsset:Edit";

        /// <summary>音频资源删除</summary>
        public const string Delete = "AudioAsset:Delete";
    }
}