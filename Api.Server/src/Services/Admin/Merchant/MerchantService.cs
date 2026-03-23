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
using Fast.Admin.Service.Merchant.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Service.Merchant;

/// <summary>
/// <see cref="MerchantService"/> 商户号服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Admin, Name = "merchant")]
public class MerchantService : IDynamicApplication
{
    private readonly ISqlSugarRepository<MerchantModel> _repository;

    public MerchantService(ISqlSugarRepository<MerchantModel> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 商户号选择器
    /// </summary>
    /// <param name="merchantType"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("商户号选择器", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Merchant.Paged)]
    public async Task<List<ElSelectorOutput<long>>> MerchantSelector(PaymentChannelEnum? merchantType)
    {
        var data = await _repository.Entities.WhereIF(merchantType != null, wh => wh.MerchantType == merchantType)
            .OrderBy(ob => ob.MerchantNo)
            .Select(sl => new {sl.MerchantId, sl.MerchantName, sl.MerchantNo, sl.MerchantType})
            .ToListAsync();

        return data.Select(sl => new ElSelectorOutput<long>
            {
                Value = sl.MerchantId, Label = sl.MerchantNo, Data = new {sl.MerchantName, sl.MerchantType}
            })
            .ToList();
    }

    /// <summary>
    /// 获取商户号分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取商户号分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.Merchant.Paged)]
    public async Task<PagedResult<QueryMerchantPagedOutput>> QueryMerchantPaged(QueryMerchantPagedInput input)
    {
        return await _repository.Entities.WhereIF(input.MerchantType != null, wh => wh.MerchantType == input.MerchantType)
            .OrderByIF(input.IsOrderBy, ob => ob.CreatedTime, OrderByType.Desc)
            .Select(sl => new QueryMerchantPagedOutput
            {
                MerchantId = sl.MerchantId,
                MerchantType = sl.MerchantType,
                MerchantName = sl.MerchantName,
                MerchantNo = sl.MerchantNo,
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
    /// 获取商户号详情
    /// </summary>
    /// <param name="merchantId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiInfo("获取商户号详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Merchant.Detail)]
    public async Task<QueryMerchantDetailOutput> QueryMerchantDetail([Required(ErrorMessage = "商户号Id不能为空")] long? merchantId)
    {
        var result = await _repository.Entities.Where(wh => wh.MerchantId == merchantId)
            .Select(sl => new QueryMerchantDetailOutput
            {
                MerchantId = sl.MerchantId,
                MerchantType = sl.MerchantType,
                MerchantName = sl.MerchantName,
                MerchantNo = sl.MerchantNo,
                MerchantSecret = sl.MerchantSecret,
                PublicSerialNum = sl.PublicSerialNum,
                PublicKey = sl.PublicKey,
                CertSerialNum = sl.CertSerialNum,
                Cert = sl.Cert,
                CertPrivateKey = sl.CertPrivateKey,
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
    /// 添加商户号
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("添加商户号", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.Merchant.Add)]
    public async Task AddMerchant(AddMerchantInput input)
    {
        if (await _repository.AnyAsync(a => a.MerchantNo == input.MerchantNo))
        {
            throw new UserFriendlyException("商户号重复！");
        }

        var merchantModel = new MerchantModel
        {
            MerchantType = input.MerchantType,
            MerchantName = input.MerchantName,
            MerchantNo = input.MerchantNo,
            MerchantSecret = input.MerchantSecret,
            PublicSerialNum = input.PublicSerialNum,
            PublicKey = input.PublicKey,
            CertSerialNum = input.CertSerialNum,
            Cert = input.Cert,
            CertPrivateKey = input.CertPrivateKey,
            Remark = input.Remark
        };

        await _repository.InsertAsync(merchantModel);
    }

    /// <summary>
    /// 编辑商户号
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("编辑商户号", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Merchant.Edit)]
    public async Task EditMerchant(EditMerchantInput input)
    {
        if (await _repository.AnyAsync(a => a.MerchantNo == input.MerchantNo && a.MerchantId != input.MerchantId))
        {
            throw new UserFriendlyException("商户号重复！");
        }

        var merchantModel = await _repository.SingleOrDefaultAsync(s => s.MerchantId == input.MerchantId);
        if (merchantModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        merchantModel.MerchantType = input.MerchantType;
        merchantModel.MerchantName = input.MerchantName;
        merchantModel.MerchantNo = input.MerchantNo;
        merchantModel.MerchantSecret = input.MerchantSecret;
        merchantModel.PublicSerialNum = input.PublicSerialNum;
        merchantModel.PublicKey = input.PublicKey;
        merchantModel.CertSerialNum = input.CertSerialNum;
        merchantModel.Cert = input.Cert;
        merchantModel.CertPrivateKey = input.CertPrivateKey;
        merchantModel.Remark = input.Remark;
        merchantModel.RowVersion = input.RowVersion;

        await _repository.Ado.UseTranAsync(async () =>
        {
            await _repository.UpdateAsync(merchantModel);
            await _repository.Updateable<ApplicationOpenIdModel>()
                .SetColumns(_ => new ApplicationOpenIdModel {WeChatMerchantNo = merchantModel.MerchantNo})
                .Where(wh => wh.WeChatMerchantId == merchantModel.MerchantId)
                .ExecuteCommandAsync();
            await _repository.Updateable<ApplicationOpenIdModel>()
                .SetColumns(_ => new ApplicationOpenIdModel {AlipayMerchantNo = merchantModel.MerchantNo})
                .Where(wh => wh.AlipayMerchantId == merchantModel.MerchantId)
                .ExecuteCommandAsync();
        }, ex => throw ex);

        // 删除缓存
        await MerchantContext.DeleteMerchant(merchantModel.MerchantNo);
    }

    /// <summary>
    /// 删除商户号
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("删除商户号", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.Merchant.Delete)]
    public async Task DeleteMerchant(MerchantIdInput input)
    {
        if (await _repository.Queryable<ApplicationOpenIdModel>()
                .AnyAsync(a => a.WeChatMerchantId == input.MerchantId || a.AlipayMerchantId == input.MerchantId))
        {
            throw new UserFriendlyException("商户号存在绑定应用，无法删除！");
        }

        var merchantModel = await _repository.SingleOrDefaultAsync(s => s.MerchantId == input.MerchantId);
        if (merchantModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        await _repository.DeleteAsync(merchantModel);
        // 删除缓存
        await MerchantContext.DeleteMerchant(merchantModel.MerchantNo);
    }
}