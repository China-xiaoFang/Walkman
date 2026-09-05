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

using Fast.Cache;
using Fast.Core;
using Fast.DependencyInjection;
using Fast.Logging;
using Fast.NET.Core;
using Fast.Scheduler;
using Fast.Scheduler.BackgroundServices;
using Fast.Serialization;
using Fast.SqlSugar;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// 初始化框架
builder.Initialize();

// 添加序列化服务
builder.Services.AddSerialization();

// 添加日志服务
builder.Services.AddLoggingService(builder.Configuration);

// 添加依赖注入服务
builder.Services.AddDependencyInjection();

// 添加缓存服务
builder.Services.AddCache();

var redisOptions = builder.Configuration.GetSection("RedisSettings")
    .Get<RedisSettingsOptions>();
if (redisOptions != null)
{
    // 添加分布式缓存
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.ConfigurationOptions = new ConfigurationOptions
        {
            Password = redisOptions.DbPwd,
            DefaultDatabase = redisOptions.DbName ?? 2,
            AbortOnConnectFail = false,
            EndPoints = {{redisOptions.ServiceIp, redisOptions.Port ?? 6379}}
        };
        options.InstanceName = $"{nameof(Fast)}:";
    });
}

// 添加雪花Id
builder.Services.AddSnowflake(builder.Configuration);

// 添加 SqlSugar
builder.Services.AddSqlSugar(builder.Configuration, builder.Environment);

builder.Services.AddHttpClient();

// 添加 Quartz 服务
builder.Services.AddQuartzService(builder.Configuration);

// 添加删除日志托管服务
builder.Services.AddHostedService<DeleteLogBackgroundService>();

// 添加 SqlSugar 日志后台服务
builder.Services.AddHostedService<SqlSugarLogBackgroundService>();

// 添加应用程序生命周期托管服务
builder.Services.AddHostedService<ApplicationLifecycleHostedService>();

// 添加调度后台托管服务
builder.Services.AddHostedService<SchedulerHostedService>();

var app = builder.Build();

app.Run();