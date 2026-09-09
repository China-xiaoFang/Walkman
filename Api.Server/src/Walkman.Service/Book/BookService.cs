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

using Fast.Admin.Service;
using Fast.AdminLog.Domain;
using Fast.Walkman.Domain;
using Fast.Walkman.Service.Book.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Walkman.Service.Book;

/// <summary>
/// 教材服务
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Walkman, Name = "book")]
public class BookService : IDynamicApplication
{
    private readonly ISqlSugarRepository<BookModel> _repository;

    public BookService(ISqlSugarRepository<BookModel> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 教材分页选择器
    /// </summary>
    [HttpPost]
    [ApiInfo("教材分页选择器", HttpRequestActionEnum.Paged)]
    public async Task<PagedResult<ElSelectorOutput<long>>> BookSelector(PagedInput input)
    {
        var pagedData = await _repository.Entities.WhereIF(!string.IsNullOrWhiteSpace(input.SearchValue),
                wh => wh.BookName.Contains(input.SearchValue))
            .OrderBy(ob => ob.Sort)
            .OrderBy(ob => ob.BookName)
            .Select(sl => new {sl.BookId, sl.BookName, sl.Status})
            .ToPagedListAsync(input);

        return pagedData.ToPagedData(sl => new ElSelectorOutput<long>
        {
            Value = sl.BookId, Label = sl.BookName, Disabled = sl.Status == CommonStatusEnum.Disable
        });
    }

    /// <summary>
    /// 获取教材分页列表
    /// </summary>
    [HttpPost]
    [ApiInfo("获取教材分页列表", HttpRequestActionEnum.Paged)]
    [Permission(PermissionConst.Book.Paged)]
    public async Task<PagedResult<QueryBookPagedOutput>> QueryBookPaged(QueryBookPagedInput input)
    {
        return await _repository.Entities.WhereIF(input.Status != null, wh => wh.Status == input.Status)
            .OrderByIF(input.IsOrderBy, ob => ob.Sort)
            .Select(sl => new QueryBookPagedOutput
            {
                BookId = sl.BookId,
                BookName = sl.BookName,
                Description = sl.Description,
                CoverUrl = sl.CoverUrl,
                Status = sl.Status,
                Sort = sl.Sort,
                Remark = sl.Remark,
                CreatedUserName = sl.CreatedUserName,
                CreatedTime = sl.CreatedTime,
                UpdatedUserName = sl.UpdatedUserName,
                UpdatedTime = sl.UpdatedTime,
                RowVersion = sl.RowVersion
            })
            .ToPagedListAsync(input);
    }

    /// <summary>
    /// 获取教材详情
    /// </summary>
    [HttpGet]
    [ApiInfo("获取教材详情", HttpRequestActionEnum.Query)]
    [Permission(PermissionConst.Book.Detail)]
    public async Task<QueryBookDetailOutput> QueryBookDetail([Required(ErrorMessage = "教材Id不能为空")] long? bookId)
    {
        var result = await _repository.Entities.Where(wh => wh.BookId == bookId)
            .Select(sl => new QueryBookDetailOutput
            {
                BookId = sl.BookId,
                BookName = sl.BookName,
                Description = sl.Description,
                CoverUrl = sl.CoverUrl,
                Status = sl.Status,
                Sort = sl.Sort,
                Remark = sl.Remark,
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
    /// 添加教材
    /// </summary>
    [HttpPost]
    [ApiInfo("添加教材", HttpRequestActionEnum.Add)]
    [Permission(PermissionConst.Book.Add)]
    public async Task AddBook(AddBookInput input)
    {
        if (await _repository.AnyAsync(a => a.BookName == input.BookName))
        {
            throw new UserFriendlyException("教材名称重复！");
        }

        var bookModel = new BookModel
        {
            BookName = input.BookName,
            Description = input.Description,
            CoverUrl = input.CoverUrl,
            Status = CommonStatusEnum.Enable,
            Sort = input.Sort,
            Remark = input.Remark
        };

        await _repository.InsertAsync(bookModel);

        await LogContext.OperateLog(new OperateLogDto
        {
            Title = "添加教材",
            OperateType = OperateLogTypeEnum.Content,
            BizId = bookModel.BookId,
            BizNo = null,
            Description = $"添加教材：{bookModel.BookName}"
        });
    }

    /// <summary>
    /// 编辑教材
    /// </summary>
    [HttpPost]
    [ApiInfo("编辑教材", HttpRequestActionEnum.Edit)]
    [Permission(PermissionConst.Book.Edit)]
    public async Task EditBook(EditBookInput input)
    {
        if (await _repository.AnyAsync(a => a.BookName == input.BookName && a.BookId != input.BookId))
        {
            throw new UserFriendlyException("教材名称重复！");
        }

        var bookModel = await _repository.SingleOrDefaultAsync(input.BookId);
        if (bookModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        bookModel.BookName = input.BookName;
        bookModel.Description = input.Description;
        bookModel.CoverUrl = input.CoverUrl;
        bookModel.Status = input.Status;
        bookModel.Sort = input.Sort;
        bookModel.Remark = input.Remark;
        bookModel.RowVersion = input.RowVersion;

        await _repository.UpdateAsync(bookModel);

        await LogContext.OperateLog(new OperateLogDto
        {
            Title = "编辑教材",
            OperateType = OperateLogTypeEnum.Content,
            BizId = bookModel.BookId,
            BizNo = null,
            Description = $"编辑教材：{bookModel.BookName}"
        });
    }

    /// <summary>
    /// 删除教材
    /// </summary>
    [HttpPost]
    [ApiInfo("删除教材", HttpRequestActionEnum.Delete)]
    [Permission(PermissionConst.Book.Delete)]
    public async Task DeleteBook(BookIdInput input)
    {
        if (await _repository.Queryable<LessonModel>()
                .AnyAsync(a => a.BookId == input.BookId))
        {
            throw new UserFriendlyException("教材下存在课程，无法删除！");
        }

        var bookModel = await _repository.SingleOrDefaultAsync(input.BookId);
        if (bookModel == null)
        {
            throw new UserFriendlyException("数据不存在！");
        }

        await _repository.DeleteAsync(bookModel);

        await LogContext.OperateLog(new OperateLogDto
        {
            Title = "删除教材",
            OperateType = OperateLogTypeEnum.Content,
            BizId = bookModel.BookId,
            BizNo = null,
            Description = $"删除教材：{bookModel.BookName}"
        });
    }
}