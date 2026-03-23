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

using Fast.Runtime;
using Microsoft.AspNetCore.Http;

namespace Fast.Admin.Entity;

/// <summary>
/// <see cref="PayRecordModel"/> 支付记录表Model类
/// </summary>
[SugarTable("PayRecord", "支付记录表")]
[SugarDbType(DatabaseTypeEnum.Admin)]
[SugarIndex($"IX_{{table}}_{nameof(BizOrderNo)}", nameof(BizOrderId), OrderByType.Desc, nameof(BizOrderNo), OrderByType.Desc,
    true)]
[SugarIndex($"IX_{{table}}_{nameof(TransactionId)}", nameof(TransactionId), OrderByType.Desc)]
public class PayRecordModel : IUpdateVersion
{
    /// <summary>
    /// 记录Id
    /// </summary>
    [SugarColumn(ColumnDescription = "记录Id", IsPrimaryKey = true)]
    public long RecordId { get; set; }

    /// <summary>
    /// 应用标识
    /// </summary>
    [SugarColumn(ColumnDescription = "应用标识", Length = 50)]
    public string AppOpenId { get; set; }

    /// <summary>
    /// 收款商户号
    /// </summary>
    [SugarColumn(ColumnDescription = "收款商户号", Length = 32)]
    public string MerchantNo { get; set; }

    /// <summary>
    /// 业务订单Id
    /// </summary>
    [SugarColumn(ColumnDescription = "业务订单Id")]
    public long BizOrderId { get; set; }

    /// <summary>
    /// 业务订单号
    /// </summary>
    [SugarSearchValue]
    [SugarColumn(ColumnDescription = "业务订单号", Length = 30)]
    public string BizOrderNo { get; set; }

    /// <summary>
    /// 支付渠道
    /// </summary>
    [SugarColumn(ColumnDescription = "支付渠道")]
    public PaymentChannelEnum PaymentChannel { get; set; }

    /// <summary>
    /// 唯一用户标识
    /// </summary>
    [SugarSearchValue]
    [SugarColumn(ColumnDescription = "唯一用户标识", Length = 28)]
    public string OpenId { get; set; }

    /// <summary>
    /// 统一用户标识
    /// </summary>
    [SugarSearchValue]
    [SugarColumn(ColumnDescription = "统一用户标识", Length = 28)]
    public string UnionId { get; set; }

    /// <summary>
    /// 手机
    /// </summary>
    [SugarSearchValue]
    [SugarColumn(ColumnDescription = "手机", ColumnDataType = "varchar(11)")]
    public string Mobile { get; set; }

    /// <summary>
    /// 订单金额
    /// </summary>
    [SugarColumn(ColumnDescription = "订单金额", Length = 18, DecimalDigits = 2)]
    public decimal OrderAmount { get; set; }

    /// <summary>
    /// 回调地址
    /// </summary>
    [SugarColumn(ColumnDescription = "回调地址", Length = 200)]
    public string NotifyUrl { get; set; }

    /// <summary>
    /// 是否已支付
    /// </summary>
    [SugarColumn(ColumnDescription = "是否已支付")]
    public bool IsPaid { get; set; }

    /// <summary>
    /// 支付过期时间
    /// </summary>
    [SugarColumn(ColumnDescription = "支付订单过期时间")]
    public DateTime PayExpireTime { get; set; }

    /// <summary>
    /// 是否已关闭
    /// </summary>
    [SugarColumn(ColumnDescription = "是否已关闭")]
    public bool IsClosed { get; set; }

    /// <summary>
    /// 关闭时间
    /// </summary>
    [SugarColumn(ColumnDescription = "关闭时间")]
    public DateTime? CloseTime { get; set; }

    /// <summary>
    /// 支付金额
    /// </summary>
    [SugarColumn(ColumnDescription = "支付金额", Length = 18, DecimalDigits = 2)]
    public decimal? PaymentAmount { get; set; }

    /// <summary>
    /// 交易流水号
    /// </summary>
    [SugarColumn(ColumnDescription = "交易流水号", Length = 128)]
    public string TransactionId { get; set; }

    /// <summary>
    /// 支付时间
    /// </summary>
    [SugarColumn(ColumnDescription = "支付时间")]
    public DateTime? PaymentTime { get; set; }

    /// <summary>
    /// 设备
    /// </summary>
    [SugarColumn(ColumnDescription = "设备", Length = 50, CreateTableFieldSort = 983)]
    public string Device { get; set; }

    /// <summary>
    /// 操作系统（版本）
    /// </summary>
    [SugarColumn(ColumnDescription = "操作系统（版本）", Length = 50, CreateTableFieldSort = 984)]
    public string OS { get; set; }

    /// <summary>
    /// 浏览器（版本）
    /// </summary>
    [SugarColumn(ColumnDescription = "浏览器（版本）", Length = 50, CreateTableFieldSort = 985)]
    public string Browser { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    [SugarColumn(ColumnDescription = "省份", Length = 20, CreateTableFieldSort = 986)]
    public string Province { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    [SugarColumn(ColumnDescription = "城市", Length = 20, CreateTableFieldSort = 987)]
    public string City { get; set; }

    /// <summary>
    /// Ip
    /// </summary>
    [SugarColumn(ColumnDescription = "Ip", Length = 15, CreateTableFieldSort = 988)]
    public string Ip { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarSearchTime]
    [Required, SugarColumn(ColumnDescription = "创建时间", CreateTableFieldSort = 993)]
    public DateTime? CreatedTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间", CreateTableFieldSort = 996)]
    public DateTime? UpdatedTime { get; set; }

    /// <summary>
    /// 更新版本控制字段
    /// </summary>
    [SugarColumn(ColumnDescription = "更新版本控制字段", IsEnableUpdateVersionValidation = true, CreateTableFieldSort = 998)]
    public long RowVersion { get; set; }

    /// <summary>
    /// 记录表创建
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/> 请求上下文</param>
    public void RecordCreate(HttpContext httpContext)
    {
        var userAgentInfo = httpContext.RequestUserAgentInfo();
        var wanInfo = httpContext.RemoteIpv4Info();

        Device = userAgentInfo.Device;
        OS = userAgentInfo.OS;
        Browser = userAgentInfo.Browser;
        Province = wanInfo.Province;
        City = wanInfo.City;
        Ip = wanInfo.Ip;
    }
}