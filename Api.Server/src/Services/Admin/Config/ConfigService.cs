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

using Fast.Admin.Entity;
using Fast.Admin.Service.Config.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Service.Config;

/// <summary>
/// <see cref="ConfigService"/> 配置服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Center, Name = "config")]
public class ConfigService : IDynamicApplication
{
    private readonly IUser _user;
    private readonly ISqlSugarRepository<ConfigModel> _repository;

    public ConfigService(IUser user, ISqlSugarRepository<ConfigModel> repository)
    {
        _user = user;
        _repository = repository;
    }

    /// <summary>
    /// 获取配置分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取配置分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.Config.Paged)]
    public async Task<PagedResult<QueryConfigPagedOutput>> QueryConfigPaged(PagedInput input)
    {
        return await _repository.Entities.Select(sl => new QueryConfigPagedOutput
            {
                ConfigId = sl.ConfigId,
                ConfigCode = sl.ConfigCode,
                ConfigName = sl.ConfigName,
                ConfigValue = sl.ConfigValue,
                Remark = sl.Remark,
                DepartmentName = sl.DepartmentName,
                CreatedUserName = sl.CreatedUserName,
                CreatedTime = sl.CreatedTime,
                UpdatedUserName = sl.UpdatedUserName,
                UpdatedTime = sl.UpdatedTime,
                RowVersion = sl.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取配置详情
    /// </summary>
    /// <param name="configId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取配置详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Config.Detail)]
    public async Task<QueryConfigDetailOutput> QueryConfigDetail([Required(ErrorMessage = "配置Id不能为空")] long? configId)
    {
        var result = await _repository.Entities.Where(wh => wh.ConfigId == configId)
            .Select(sl => new QueryConfigDetailOutput
            {
                ConfigId = sl.ConfigId,
                ConfigCode = sl.ConfigCode,
                ConfigName = sl.ConfigName,
                ConfigValue = sl.ConfigValue,
                Remark = sl.Remark,
                DepartmentName = sl.DepartmentName,
                CreatedUserName = sl.CreatedUserName,
                CreatedTime = sl.CreatedTime,
                UpdatedUserName = sl.UpdatedUserName,
                UpdatedTime = sl.UpdatedTime,
                RowVersion = sl.RowVersion
            })
            .SingleAsync();

        if (result == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        return result;
    }

    /// <summary>
    /// 添加配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加配置", HttpRequestActionEnum.Add)]
    public async Task AddConfig(AddConfigInput input)
    {
        if (_user?.IsSuperAdmin == false)
            throw new UserFriendlyException("非超级管理员禁止操作！");

        if (await _repository.AnyAsync(a => a.ConfigCode == input.ConfigCode))
        {
            throw new UserFriendlyException("配置编码重复！");
        }

        var configModel = new ConfigModel
        {
            ConfigCode = input.ConfigCode,
            ConfigName = input.ConfigName,
            ConfigValue = input.ConfigValue,
            Remark = input.Remark
        };

        await _repository.InsertAsync(configModel);
    }

    /// <summary>
    /// 编辑配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑配置", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Config.Edit)]
    public async Task EditConfig(EditConfigInput input)
    {
        var configModel = await _repository.SingleOrDefaultAsync(input.ConfigId);
        if (configModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        configModel.ConfigName = input.ConfigName;
        configModel.ConfigValue = input.ConfigValue;
        configModel.Remark = input.Remark;
        configModel.RowVersion = input.RowVersion;

        await _repository.UpdateAsync(configModel);
        // 删除缓存
        await ConfigContext.DeleteConfig(configModel.ConfigCode);
    }

    /// <summary>
    /// 删除配置缓存
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除配置缓存", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.Config.Edit)]
    public async Task DeleteConfigCache(DeleteConfigCacheInput input)
    {
        // 删除缓存
        await ConfigContext.DeleteConfig(input.ConfigCode);
    }

    /// <summary>
    /// 删除所有配置缓存
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除所有配置缓存", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.Config.Edit)]
    public async Task DeleteAllConfigCache()
    {
        // 删除缓存
        await ConfigContext.DeleteAllConfig();
    }
}