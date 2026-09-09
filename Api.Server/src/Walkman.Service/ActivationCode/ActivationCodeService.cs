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

using System.Security.Cryptography;
using Fast.Admin.Service;
using Fast.AdminLog.Domain;
using Fast.Center.Domain;
using Fast.Walkman.Domain;
using Fast.Walkman.Service.ActivationCode.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Walkman.Service.ActivationCode;

/// <summary>
/// 激活码服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Walkman, Name = "activationCode")]
public partial class ActivationCodeService : IDynamicApplication
{
    private readonly IUser _user;
    private readonly ISqlSugarRepository<ActivationCodeModel> _repository;

    public ActivationCodeService(IUser user, ISqlSugarRepository<ActivationCodeModel> repository)
    {
        _user = user;
        _repository = repository;
    }

    /// <summary>
    /// 获取激活码分页列表
    /// </summary>
    [HttpPost]
    [ApiInfo("获取激活码分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.ActivationCode.Paged)]
    public async Task<PagedResult<QueryActivationCodePagedOutput>> QueryActivationCodePaged(QueryActivationCodePagedInput input)
    {
        return await _repository.Entities.WhereIF(input.IsUsed == true, wh => wh.UserId != null)
            .WhereIF(input.IsUsed == false, wh => wh.UserId == null)
            .OrderByDescending(ob => ob.CreatedTime)
            .OrderByDescending(ob => ob.ActivationCodeId)
            .Select(sl => new QueryActivationCodePagedOutput
            {
                ActivationCodeId = sl.ActivationCodeId,
                Code = sl.Code,
                ExpireTime = sl.ExpireTime,
                UserId = sl.UserId,
                ActivationTime = sl.ActivationTime,
                CreatedUserName = sl.CreatedUserName,
                CreatedTime = sl.CreatedTime,
                RowVersion = sl.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取激活码详情
    /// </summary>
    [HttpGet]
    [ApiInfo("获取激活码详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.ActivationCode.Detail)]
    public async Task<QueryActivationCodeDetailOutput> QueryActivationCodeDetail(
        [Required(ErrorMessage = "激活码Id不能为空")] long? activationCodeId)
    {
        var result = await _repository.Entities.LeftJoin<ClientUserModel>((t1, t2) => t1.UserId == t2.UserId)
            .Where(t1 => t1.ActivationCodeId == activationCodeId)
            .Select((t1, t2) => new QueryActivationCodeDetailOutput
            {
                ActivationCodeId = t1.ActivationCodeId,
                Code = t1.Code,
                ExpireTime = t1.ExpireTime,
                UserId = t1.UserId,
                Mobile = t2.Mobile,
                OpenId = t2.OpenId,
                NickName = t2.NickName,
                Avatar = t2.Avatar,
                ActivationTime = t1.ActivationTime,
                CreatedUserName = t1.CreatedUserName,
                CreatedTime = t1.CreatedTime,
                RowVersion = t1.RowVersion
            })
            .SingleAsync();

        if (result == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        return result;
    }

    /// <summary>
    /// 创建高强度随机激活码
    /// </summary>
    private string CreateActivationCode()
    {
        var value = Convert.ToHexString(RandomNumberGenerator.GetBytes(10));
        return string.Join('-', Enumerable.Range(0, 5)
            .Select(index => value.Substring(index * 4, 4)));
    }

    /// <summary>
    /// 批量生成激活码
    /// </summary>
    [HttpPost]
    [ApiInfo("批量生成激活码", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.ActivationCode.Generate)]
    public async Task GenerateActivationCode(GenerateActivationCodeInput input)
    {
        var codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        while (codes.Count < input.Count)
        {
            codes.Add(CreateActivationCode());
        }

        // 去重
        var existingCodes = await _repository.Entities.Where(wh => codes.Contains(wh.Code))
            .Select(sl => sl.Code)
            .ToListAsync();
        while (existingCodes.Count > 0)
        {
            codes.ExceptWith(existingCodes);
            while (codes.Count < input.Count)
            {
                codes.Add(CreateActivationCode());
            }

            existingCodes = await _repository.Entities.Where(wh => codes.Contains(wh.Code))
                .Select(sl => sl.Code)
                .ToListAsync();
        }

        var activationCodeList = codes.Select(code => new ActivationCodeModel {Code = code, ExpireTime = null})
            .ToList();
        await _repository.Insertable(activationCodeList)
            .ExecuteCommandAsync();

        // 操作日志
        await LogContext.OperateLog(new OperateLogDto
        {
            Title = "生成激活码",
            OperateType = OperateLogTypeEnum.Content,
            BizId = null,
            BizNo = null,
            Description = $"数量：{activationCodeList.Count}"
        });
    }
}