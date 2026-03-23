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

namespace Fast.Admin.Entity;

/// <summary>
/// <see cref="ApiInfoModel"/> 接口信息表Model类
/// </summary>
[SugarTable("ApiInfo", "接口信息表")]
[SugarDbType(DatabaseTypeEnum.Admin)]
public class ApiInfoModel : IDatabaseEntity
{
    /// <summary>
    /// 接口Id
    /// </summary>
    [SugarColumn(ColumnDescription = "接口Id", IsPrimaryKey = true)]
    public long ApiId { get; set; }

    /// <summary>
    /// 服务名称
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "服务名称", Length = 50)]
    public string ServiceName { get; set; }

    /// <summary>
    /// 分组名称
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "分组名称", Length = 50)]
    public string GroupName { get; set; }

    /// <summary>
    /// 分组标题
    /// </summary>
    [SugarColumn(ColumnDescription = "分组标题", Length = 20)]
    public string GroupTitle { get; set; }

    /// <summary>
    /// 版本
    /// </summary>
    [SugarColumn(ColumnDescription = "版本", Length = 20)]
    public string Version { get; set; }

    /// <summary>
    /// 分组描述
    /// </summary>
    [SugarColumn(ColumnDescription = "分组描述", Length = 200)]
    public string Description { get; set; }

    /// <summary>
    /// 模块名称
    /// </summary>
    [Required]
    [SugarSearchValue]
    [SugarColumn(ColumnDescription = "模块名称", Length = 50)]
    public string ModuleName { get; set; }

    /// <summary>
    /// 接口地址
    /// </summary>
    [Required]
    [SugarSearchValue]
    [SugarColumn(ColumnDescription = "接口地址", Length = 200)]
    public string ApiUrl { get; set; }

    /// <summary>
    /// 接口名称
    /// </summary>
    [SugarSearchValue]
    [SugarColumn(ColumnDescription = "接口名称", Length = 50)]
    public string ApiName { get; set; }

    /// <summary>
    /// 请求方式
    /// </summary>
    [SugarColumn(ColumnDescription = "请求方式")]
    public HttpRequestMethodEnum Method { get; set; }

    /// <summary>
    /// 操作方式
    /// </summary>
    [SugarColumn(ColumnDescription = "操作方式")]
    public HttpRequestActionEnum Action { get; set; }

    /// <summary>
    /// 是否检测授权
    /// </summary>
    [SugarColumn(ColumnDescription = "是否检测授权")]
    public bool HasAuth { get; set; }

    /// <summary>
    /// 是否检测权限
    /// </summary>
    [SugarColumn(ColumnDescription = "是否检测权限")]
    public bool HasPermission { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    [SugarColumn(ColumnDescription = "标签", Length = 200, IsJson = true)]
    public List<string> Tags { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Required, SugarColumn(ColumnDescription = "创建时间", CreateTableFieldSort = 993)]
    public DateTime? CreatedTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间", CreateTableFieldSort = 996)]
    public DateTime? UpdatedTime { get; set; }

    /// <summary>Serves as the default hash function.</summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return ApiId.GetHashCode();
    }

    /// <summary>Determines whether the specified object is equal to the current object.</summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
    public override bool Equals(object obj)
    {
        if (obj is not ApiInfoModel oldApiModel)
            return false;

        return ApiId == oldApiModel.ApiId
               && ServiceName == oldApiModel.ServiceName
               && GroupName == oldApiModel.GroupName
               && GroupTitle == oldApiModel.GroupTitle
               && Version == oldApiModel.Version
               && Description == oldApiModel.Description
               && ModuleName == oldApiModel.ModuleName
               && ApiUrl == oldApiModel.ApiUrl
               && ApiName == oldApiModel.ApiName
               && Method == oldApiModel.Method
               && Action == oldApiModel.Action
               && HasAuth == oldApiModel.HasAuth
               && HasPermission == oldApiModel.HasPermission
               && Tags.SequenceEqual(oldApiModel.Tags)
               && Sort == oldApiModel.Sort;
    }
}