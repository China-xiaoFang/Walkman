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
using Fast.Admin.Service.ActivationCode.Dto;
using Microsoft.AspNetCore.Mvc;
using Yitter.IdGenerator;

namespace Fast.Admin.Service.ActivationCode;

/// <summary>
/// <see cref="ActivationCodeService"/> 激活码服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Walkman, Name = "activationCode")]
public class ActivationCodeService : IDynamicApplication
{
    private readonly ISqlSugarRepository<ActivationCodeModel> _repository;
    private readonly ISqlSugarRepository<TextbookModel> _textbookRepository;

    public ActivationCodeService(ISqlSugarRepository<ActivationCodeModel> repository,
        ISqlSugarRepository<TextbookModel> textbookRepository)
    {
        _repository = repository;
        _textbookRepository = textbookRepository;
    }

    /// <summary>
    /// 获取激活码分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取激活码分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.ActivationCode.Paged)]
    public async Task<PagedResult<QueryActivationCodePagedOutput>> QueryActivationCodePaged(
        QueryActivationCodePagedInput input)
    {
        return await _repository.Entities
            .LeftJoin<TextbookModel>((a, t) => a.TextbookId == t.TextbookId)
            .WhereIF(input.TextbookId.HasValue, (a, t) => a.TextbookId == input.TextbookId.Value)
            .WhereIF(input.Status.HasValue, (a, t) => a.Status == input.Status.Value)
            .WhereIF(input.AccountId.HasValue, (a, t) => a.AccountId == input.AccountId.Value)
            .OrderByDescending((a, t) => a.CreatedTime)
            .Select((a, t) => new QueryActivationCodePagedOutput
            {
                ActivationCodeId = a.ActivationCodeId,
                Code = a.Code,
                TextbookId = a.TextbookId,
                TextbookName = t.TextbookName,
                AccountId = a.AccountId,
                Status = a.Status,
                UsedTime = a.UsedTime,
                Remark = a.Remark,
                CreatedUserName = a.CreatedUserName,
                CreatedTime = a.CreatedTime,
                UpdatedUserName = a.UpdatedUserName,
                UpdatedTime = a.UpdatedTime,
                RowVersion = a.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取激活码详情
    /// </summary>
    /// <param name="activationCodeId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取激活码详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.ActivationCode.Detail)]
    public async Task<QueryActivationCodeDetailOutput> QueryActivationCodeDetail(
        [Required(ErrorMessage = "激活码Id不能为空")] long? activationCodeId)
    {
        var result = await _repository.Entities
            .LeftJoin<TextbookModel>((a, t) => a.TextbookId == t.TextbookId)
            .Where((a, t) => a.ActivationCodeId == activationCodeId)
            .Select((a, t) => new QueryActivationCodeDetailOutput
            {
                ActivationCodeId = a.ActivationCodeId,
                Code = a.Code,
                TextbookId = a.TextbookId,
                TextbookName = t.TextbookName,
                AccountId = a.AccountId,
                Status = a.Status,
                UsedTime = a.UsedTime,
                Remark = a.Remark,
                CreatedUserName = a.CreatedUserName,
                CreatedTime = a.CreatedTime,
                UpdatedUserName = a.UpdatedUserName,
                UpdatedTime = a.UpdatedTime,
                RowVersion = a.RowVersion
            })
            .SingleAsync();

        if (result == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        return result;
    }

    /// <summary>
    /// 批量生成激活码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("批量生成激活码", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.ActivationCode.Generate)]
    public async Task GenerateActivationCode(GenerateActivationCodeInput input)
    {
        if (!await _textbookRepository.AnyAsync(a => a.TextbookId == input.TextbookId))
        {
            throw new UserFriendlyException("教材不存在！");
        }

        var codes = new List<ActivationCodeModel>();
        for (var i = 0; i < input.Count; i++)
        {
            codes.Add(new ActivationCodeModel
            {
                ActivationCodeId = YitIdHelper.NextId(),
                Code = Guid.NewGuid().ToString("N")[..16].ToUpper(),
                TextbookId = input.TextbookId,
                Status = ActivationCodeStatusEnum.Unused,
                Remark = input.Remark
            });
        }

        await _repository.AsInsertable(codes).ExecuteCommandAsync();
    }
}
