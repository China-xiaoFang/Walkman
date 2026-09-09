import { axiosUtil } from "@fast-china/axios";
import type { ElSelectorOutput, PagedInput, PagedResult } from "fast-element-plus";
import type { AddBookInput } from "./models/AddBookInput";
import type { BookIdInput } from "./models/BookIdInput";
import type { EditBookInput } from "./models/EditBookInput";
import type { QueryBookDetailOutput } from "./models/QueryBookDetailOutput";
import type { QueryBookPagedInput } from "./models/QueryBookPagedInput";
import type { QueryBookPagedOutput } from "./models/QueryBookPagedOutput";

/**
 * 教材服务Api
 */
export const bookApi = {
	/**
	 * 教材分页选择器
	 */
	bookSelector(data: PagedInput): Promise<PagedResult<ElSelectorOutput<string>>> {
		return axiosUtil.request<PagedResult<ElSelectorOutput<string>>>({
			url: "/book/bookSelector",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取教材分页列表
	 */
	queryBookPaged(data: QueryBookPagedInput): Promise<PagedResult<QueryBookPagedOutput>> {
		return axiosUtil.request<PagedResult<QueryBookPagedOutput>>({
			url: "/book/queryBookPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取教材详情
	 */
	queryBookDetail(bookId: string): Promise<QueryBookDetailOutput> {
		return axiosUtil.request<QueryBookDetailOutput>({
			url: "/book/queryBookDetail",
			method: "get",
			params: {
				bookId,
			},
			requestType: "query",
		});
	},
	/**
	 * 添加教材
	 */
	addBook(data: AddBookInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/book/addBook",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑教材
	 */
	editBook(data: EditBookInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/book/editBook",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	/**
	 * 删除教材
	 */
	deleteBook(data: BookIdInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/book/deleteBook",
			method: "post",
			data,
			requestType: "delete",
		});
	},
};
