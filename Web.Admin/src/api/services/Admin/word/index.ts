import { axiosUtil } from "@fast-china/axios";
import { PagedResult } from "fast-element-plus";
import { QueryWordPagedOutput } from "./models/QueryWordPagedOutput";
import { QueryWordPagedInput } from "./models/QueryWordPagedInput";
import { AddWordInput } from "./models/AddWordInput";
import { EditWordInput } from "./models/EditWordInput";
import { WordIdInput } from "./models/WordIdInput";

/**
 * Fast.Admin.Service.Word.WordService 单词服务Api
 */
export const wordApi = {
	queryWordPaged(data: QueryWordPagedInput) {
		return axiosUtil.request<PagedResult<QueryWordPagedOutput>>({
			url: "/word/queryWordPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	queryWordDetail(wordId: number) {
		return axiosUtil.request<QueryWordPagedOutput>({
			url: "/word/queryWordDetail",
			method: "get",
			params: { wordId },
			requestType: "query",
		});
	},
	addWord(data: AddWordInput) {
		return axiosUtil.request({
			url: "/word/addWord",
			method: "post",
			data,
			requestType: "add",
		});
	},
	editWord(data: EditWordInput) {
		return axiosUtil.request({
			url: "/word/editWord",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	deleteWord(data: WordIdInput) {
		return axiosUtil.request({
			url: "/word/deleteWord",
			method: "post",
			data,
			requestType: "delete",
		});
	},
};
