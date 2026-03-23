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
using Fast.Admin.Service.File.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Service.File;

/// <summary>
/// <see cref="FileService"/> 文件服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.File, Name = "file", Order = 997)]
public class FileService : IDynamicApplication
{
    private readonly IUser _user;
    private readonly ISqlSugarRepository<FileModel> _repository;

    public FileService(IUser user, ISqlSugarRepository<FileModel> repository)
    {
        _user = user;
        _repository = repository;
    }

    /// <summary>
    /// 获取文件分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiInfo("获取文件分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.FilePaged)]
    public async Task<PagedResult<QueryFilePagedOutput>> QueryFilePaged(PagedInput input)
    {
        var queryable = _repository.Entities;

        if (!_user.IsSuperAdmin && !_user.IsAdmin)
        {
            queryable = queryable.Where(t1 => t1.CreatedUserId == _user.EmployeeId);
        }

        return await queryable.SelectMergeTable(sl => new QueryFilePagedOutput
            {
                FileId = sl.FileId,
                FileObjectName = sl.FileObjectName,
                FileOriginName = sl.FileOriginName,
                FileSuffix = sl.FileSuffix,
                FileMimeType = sl.FileMimeType,
                FileSizeKb = sl.FileSizeKb,
                FilePath = sl.FilePath,
                FileLocation = sl.FileLocation,
                FileHash = sl.FileHash,
                UploadDevice = sl.UploadDevice,
                UploadOS = sl.UploadOS,
                UploadBrowser = sl.UploadBrowser,
                UploadProvince = sl.UploadProvince,
                UploadCity = sl.UploadCity,
                UploadIp = sl.UploadIp,
                CreatedUserName = sl.CreatedUserName,
                CreatedTime = sl.CreatedTime
            })
            .OrderByIF(input.IsOrderBy, ob => ob.CreatedTime, OrderByType.Desc)
            .ToPagedListAsync(input);
    }
}