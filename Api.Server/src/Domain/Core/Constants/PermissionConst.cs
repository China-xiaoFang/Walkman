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
/// <see cref="PermissionConst"/> 权限常量
/// </summary>
/// <remarks>所有权限都在这里</remarks>
[SuppressSniffer]
public partial class PermissionConst
{
    /// <summary>客户端服务</summary>
    public const string ClientService = "ClientService";

    /// <summary>接口列表</summary>
    public const string ApiPaged = "Api:Paged";

    /// <summary>Swagger</summary>
    public const string ApiSwagger = "Api:Swagger";

    /// <summary>Knife4j</summary>
    public const string ApiKnife4j = "Api:Knife4j";

    /// <summary><see cref="Config"/> 配置</summary>
    public class Config
    {
        /// <summary>配置列表</summary>
        public const string Paged = "Config:Paged";

        /// <summary>配置详情</summary>
        public const string Detail = "Config:Detail";

        /// <summary>配置编辑</summary>
        public const string Edit = "Config:Edit";
    }

    /// <summary><see cref="Menu"/> 菜单</summary>
    public class Menu
    {
        /// <summary>菜单列表</summary>
        public const string Paged = "Menu:Paged";

        /// <summary>菜单详情</summary>
        public const string Detail = "Menu:Detail";

        /// <summary>菜单新增</summary>
        public const string Add = "Menu:Add";

        /// <summary>菜单编辑</summary>
        public const string Edit = "Menu:Edit";

        /// <summary>菜单删除</summary>
        public const string Delete = "Menu:Delete";

        /// <summary>菜单状态更改</summary>
        public const string Status = "Menu:Status";
    }

    /// <summary><see cref="SysSerial"/> 系统序号</summary>
    public class SysSerial
    {
        /// <summary>系统序号列表</summary>
        public const string Paged = "SysSerial:Paged";

        /// <summary>系统序号详情</summary>
        public const string Detail = "SysSerial:Detail";

        /// <summary>系统序号新增</summary>
        public const string Add = "SysSerial:Add";

        /// <summary>系统序号编辑</summary>
        public const string Edit = "SysSerial:Edit";
    }

    /// <summary><see cref="Dictionary"/> 字典</summary>
    public class Dictionary
    {
        /// <summary>字典列表</summary>
        public const string Paged = "Dictionary:Paged";

        /// <summary>字典详情</summary>
        public const string Detail = "Dictionary:Detail";

        /// <summary>字典新增</summary>
        public const string Add = "Dictionary:Add";

        /// <summary>字典编辑</summary>
        public const string Edit = "Dictionary:Edit";

        /// <summary>字典删除</summary>
        public const string Delete = "Dictionary:Delete";
    }

    /// <summary><see cref="Table"/> 表格</summary>
    public class Table
    {
        /// <summary>表格列表</summary>
        public const string Paged = "Table:Paged";

        /// <summary>表格详情</summary>
        public const string Detail = "Table:Detail";

        /// <summary>表格新增</summary>
        public const string Add = "Table:Add";

        /// <summary>表格编辑</summary>
        public const string Edit = "Table:Edit";

        /// <summary>表格删除</summary>
        public const string Delete = "Table:Delete";
    }

    /// <summary><see cref="Scheduler"/> 调度程序</summary>
    public class Scheduler
    {
        /// <summary>调度程序列表</summary>
        public const string Paged = "Scheduler:Paged";

        /// <summary>调度程序详情</summary>
        public const string Detail = "Scheduler:Detail";

        /// <summary>调度程序新增</summary>
        public const string Add = "Scheduler:Add";

        /// <summary>调度程序编辑</summary>
        public const string Edit = "Scheduler:Edit";

        /// <summary>调度程序删除</summary>
        public const string Delete = "Scheduler:Delete";

        /// <summary>调度程序启动</summary>
        public const string Start = "Scheduler:Start";

        /// <summary>调度程序暂停</summary>
        public const string Stop = "Scheduler:Stop";

        /// <summary>调度程序执行作业</summary>
        public const string Trigger = "Scheduler:Trigger";

        /// <summary>调度程序恢复作业</summary>
        public const string ResumeJob = "Scheduler:ResumeJob";

        /// <summary>调度程序暂停作业</summary>
        public const string StopJob = "Scheduler:StopJob";
    }

    /// <summary>密码记录列表</summary>
    public const string PasswordRecordPaged = "PasswordRecord:Paged";

    /// <summary>异常日志列表</summary>
    public const string ExceptionLogPaged = "ExceptionLog:Paged";

    /// <summary>Sql异常日志列表</summary>
    public const string SqlExceptionLogPaged = "SqlExceptionLog:Paged";

    /// <summary>Sql超时日志列表</summary>
    public const string SqlTimeoutLogPaged = "SqlTimeoutLog:Paged";

    /// <summary>Sql执行日志列表</summary>
    public const string SqlExecutionLogPaged = "SqlExecutionLog:Paged";

    /// <summary>Sql差异日志列表</summary>
    public const string SqlDiffLogPaged = "SqlDiffLog:Paged";


    /// <summary>系统监控</summary>
    public const string SystemMonitor = "System:Monitor";

    /// <summary>文件列表</summary>
    public const string FilePaged = "File:Paged";

    /// <summary><see cref="Tenant"/> 租户</summary>
    public class Tenant
    {
        /// <summary>租户列表</summary>
        public const string Paged = "Tenant:Paged";

        /// <summary>租户详情</summary>
        public const string Detail = "Tenant:Detail";

        /// <summary>租户新增</summary>
        public const string Add = "Tenant:Add";

        /// <summary>租户编辑</summary>
        public const string Edit = "Tenant:Edit";

        /// <summary>租户状态更改</summary>
        public const string Status = "Tenant:Status";
    }

    /// <summary><see cref="Database"/> 数据库</summary>
    public class Database
    {
        /// <summary>数据库列表</summary>
        public const string Paged = "Database:Paged";

        /// <summary>数据库详情</summary>
        public const string Detail = "Database:Detail";

        /// <summary>数据库新增</summary>
        public const string Add = "Database:Add";

        /// <summary>数据库编辑</summary>
        public const string Edit = "Database:Edit";

        /// <summary>数据库删除</summary>
        public const string Delete = "Database:Delete";
    }

    /// <summary><see cref="App"/> 应用</summary>
    public class App
    {
        /// <summary>应用列表</summary>
        public const string Paged = "App:Paged";

        /// <summary>应用详情</summary>
        public const string Detail = "App:Detail";

        /// <summary>应用新增</summary>
        public const string Add = "App:Add";

        /// <summary>应用编辑</summary>
        public const string Edit = "App:Edit";

        /// <summary>应用删除</summary>
        public const string Delete = "App:Delete";
    }

    /// <summary><see cref="AppOpenId"/> 应用标识</summary>
    public class AppOpenId
    {
        /// <summary>应用标识列表</summary>
        public const string Paged = "AppOpenId:Paged";

        /// <summary>应用标识详情</summary>
        public const string Detail = "AppOpenId:Detail";

        /// <summary>应用标识新增</summary>
        public const string Add = "AppOpenId:Add";

        /// <summary>应用标识编辑</summary>
        public const string Edit = "AppOpenId:Edit";

        /// <summary>应用标识删除</summary>
        public const string Delete = "AppOpenId:Delete";
    }

    /// <summary><see cref="Account"/> 账号</summary>
    public class Account
    {
        /// <summary>账号列表</summary>
        public const string Paged = "Account:Paged";

        /// <summary>账号详情</summary>
        public const string Detail = "Account:Detail";

        /// <summary>账号解除锁定</summary>
        public const string Unlock = "Account:Unlock";

        /// <summary>账号重置密码</summary>
        public const string ResetPassword = "Account:ResetPassword";

        /// <summary>账号状态更改</summary>
        public const string Status = "Account:Status";
    }

    /// <summary><see cref="Serial"/> 序号</summary>
    public class Serial
    {
        /// <summary>序号列表</summary>
        public const string Paged = "Serial:Paged";

        /// <summary>序号详情</summary>
        public const string Detail = "Serial:Detail";

        /// <summary>序号新增</summary>
        public const string Add = "Serial:Add";

        /// <summary>序号编辑</summary>
        public const string Edit = "Serial:Edit";
    }

    /// <summary><see cref="Position"/> 职位</summary>
    public class Position
    {
        /// <summary>职位列表</summary>
        public const string Paged = "Position:Paged";

        /// <summary>职位详情</summary>
        public const string Detail = "Position:Detail";

        /// <summary>职位新增</summary>
        public const string Add = "Position:Add";

        /// <summary>职位编辑</summary>
        public const string Edit = "Position:Edit";

        /// <summary>职位删除</summary>
        public const string Delete = "Position:Delete";
    }

    /// <summary><see cref="JobLevel"/> 职级</summary>
    public class JobLevel
    {
        /// <summary>职级列表</summary>
        public const string Paged = "JobLevel:Paged";

        /// <summary>职级详情</summary>
        public const string Detail = "JobLevel:Detail";

        /// <summary>职级新增</summary>
        public const string Add = "JobLevel:Add";

        /// <summary>职级编辑</summary>
        public const string Edit = "JobLevel:Edit";

        /// <summary>职级删除</summary>
        public const string Delete = "JobLevel:Delete";
    }

    /// <summary><see cref="Role"/> 角色</summary>
    public class Role
    {
        /// <summary>角色列表</summary>
        public const string Paged = "Role:Paged";

        /// <summary>角色详情</summary>
        public const string Detail = "Role:Detail";

        /// <summary>角色新增</summary>
        public const string Add = "Role:Add";

        /// <summary>角色编辑</summary>
        public const string Edit = "Role:Edit";

        /// <summary>角色删除</summary>
        public const string Delete = "Role:Delete";
    }

    /// <summary><see cref="Department"/> 部门</summary>
    public class Department
    {
        /// <summary>部门列表</summary>
        public const string Paged = "Department:Paged";

        /// <summary>部门详情</summary>
        public const string Detail = "Department:Detail";

        /// <summary>部门新增</summary>
        public const string Add = "Department:Add";

        /// <summary>部门编辑</summary>
        public const string Edit = "Department:Edit";

        /// <summary>部门删除</summary>
        public const string Delete = "Department:Delete";
    }

    /// <summary><see cref="Employee"/> 职员</summary>
    public class Employee
    {
        /// <summary>职员列表</summary>
        public const string Paged = "Employee:Paged";

        /// <summary>职员详情</summary>
        public const string Detail = "Employee:Detail";

        /// <summary>职员新增</summary>
        public const string Add = "Employee:Add";

        /// <summary>职员编辑</summary>
        public const string Edit = "Employee:Edit";

        /// <summary>职员状态更改</summary>
        public const string Status = "Employee:Status";
    }

    /// <summary><see cref="TenantOnlineUser"/> 在线用户</summary>
    public class TenantOnlineUser
    {
        /// <summary>在线用户列表</summary>
        public const string Paged = "TenantOnlineUser:Paged";

        /// <summary>强制下线</summary>
        public const string ForceOffline = "TenantOnlineUser:ForceOffline";
    }

    /// <summary><see cref="Merchant"/> 商户号</summary>
    public class Merchant
    {
        /// <summary>商户号列表</summary>
        public const string Paged = "Merchant:Paged";

        /// <summary>商户号详情</summary>
        public const string Detail = "Merchant:Detail";

        /// <summary>商户号新增</summary>
        public const string Add = "Merchant:Add";

        /// <summary>商户号编辑</summary>
        public const string Edit = "Merchant:Edit";

        /// <summary>商户号删除</summary>
        public const string Delete = "Merchant:Delete";
    }

    /// <summary>支付记录列表</summary>
    public const string PayRecordPaged = "PayRecord:Paged";

    /// <summary>退款记录列表</summary>
    public const string RefundRecordPaged = "RefundRecord:Paged";

    /// <summary><see cref="WeChat"/> 微信用户</summary>
    public class WeChat
    {
        /// <summary>微信用户列表</summary>
        public const string Paged = "WeChat:Paged";
    }

    /// <summary><see cref="Complaint"/> 投诉</summary>
    public class Complaint
    {
        /// <summary>投诉工单列表</summary>
        public const string Paged = "Complaint:Paged";

        /// <summary>投诉工单详情</summary>
        public const string Detail = "Complaint:Detail";

        /// <summary>处理投诉工单</summary>
        public const string Handle = "Complaint:Handle";

        /// <summary>用户投诉列表</summary>
        public const string TenantPaged = "Complaint:TenantPaged";

        /// <summary>用户投诉详情</summary>
        public const string TenantDetail = "Complaint:TenantDetail";

        /// <summary>处理用户投诉</summary>
        public const string TenantHandle = "Complaint:TenantHandle";
    }

    /// <summary>访问日志列表</summary>
    public const string VisitLogPaged = "VisitLog:Paged";

    /// <summary>操作日志列表</summary>
    public const string OperateLogPaged = "OperateLog:Paged";

    /// <summary>请求日志列表</summary>
    public const string RequestLogPaged = "RequestLog:Paged";

    /// <summary><see cref="Textbook"/> 教材</summary>
    public class Textbook
    {
        /// <summary>教材列表</summary>
        public const string Paged = "Textbook:Paged";

        /// <summary>教材详情</summary>
        public const string Detail = "Textbook:Detail";

        /// <summary>教材新增</summary>
        public const string Add = "Textbook:Add";

        /// <summary>教材编辑</summary>
        public const string Edit = "Textbook:Edit";

        /// <summary>教材删除</summary>
        public const string Delete = "Textbook:Delete";
    }

    /// <summary><see cref="Volume"/> 册</summary>
    public class Volume
    {
        /// <summary>册列表</summary>
        public const string Paged = "Volume:Paged";

        /// <summary>册详情</summary>
        public const string Detail = "Volume:Detail";

        /// <summary>册新增</summary>
        public const string Add = "Volume:Add";

        /// <summary>册编辑</summary>
        public const string Edit = "Volume:Edit";

        /// <summary>册删除</summary>
        public const string Delete = "Volume:Delete";
    }

    /// <summary><see cref="Lesson"/> 课程</summary>
    public class Lesson
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

    /// <summary><see cref="AudioManage"/> 音频管理</summary>
    public class AudioManage
    {
        /// <summary>音频列表</summary>
        public const string Paged = "Audio:Paged";

        /// <summary>音频详情</summary>
        public const string Detail = "Audio:Detail";

        /// <summary>音频新增</summary>
        public const string Add = "Audio:Add";

        /// <summary>音频编辑</summary>
        public const string Edit = "Audio:Edit";

        /// <summary>音频删除</summary>
        public const string Delete = "Audio:Delete";
    }

    /// <summary><see cref="AudioTypeManage"/> 音频类型</summary>
    public class AudioTypeManage
    {
        /// <summary>音频类型列表</summary>
        public const string Paged = "AudioType:Paged";

        /// <summary>音频类型详情</summary>
        public const string Detail = "AudioType:Detail";

        /// <summary>音频类型新增</summary>
        public const string Add = "AudioType:Add";

        /// <summary>音频类型编辑</summary>
        public const string Edit = "AudioType:Edit";

        /// <summary>音频类型删除</summary>
        public const string Delete = "AudioType:Delete";
    }

    /// <summary><see cref="PronunciationTypeManage"/> 发音类型</summary>
    public class PronunciationTypeManage
    {
        /// <summary>发音类型列表</summary>
        public const string Paged = "PronunciationType:Paged";

        /// <summary>发音类型详情</summary>
        public const string Detail = "PronunciationType:Detail";

        /// <summary>发音类型新增</summary>
        public const string Add = "PronunciationType:Add";

        /// <summary>发音类型编辑</summary>
        public const string Edit = "PronunciationType:Edit";

        /// <summary>发音类型删除</summary>
        public const string Delete = "PronunciationType:Delete";
    }

    /// <summary><see cref="WordManage"/> 单词</summary>
    public class WordManage
    {
        /// <summary>单词列表</summary>
        public const string Paged = "Word:Paged";

        /// <summary>单词详情</summary>
        public const string Detail = "Word:Detail";

        /// <summary>单词新增</summary>
        public const string Add = "Word:Add";

        /// <summary>单词编辑</summary>
        public const string Edit = "Word:Edit";

        /// <summary>单词删除</summary>
        public const string Delete = "Word:Delete";
    }

    /// <summary><see cref="ActivationCode"/> 激活码</summary>
    public class ActivationCode
    {
        /// <summary>激活码列表</summary>
        public const string Paged = "ActivationCode:Paged";

        /// <summary>激活码详情</summary>
        public const string Detail = "ActivationCode:Detail";

        /// <summary>激活码批量生成</summary>
        public const string Generate = "ActivationCode:Generate";
    }
}