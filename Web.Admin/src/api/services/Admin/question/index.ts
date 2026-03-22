import { axiosUtil } from "@fast-china/axios";
import { PagedResult } from "fast-element-plus";
import { QueryQuestionPagedOutput } from "./models/QueryQuestionPagedOutput";
import { QueryQuestionPagedInput } from "./models/QueryQuestionPagedInput";
import { QueryQuestionDetailOutput } from "./models/QueryQuestionDetailOutput";
import { AddQuestionInput } from "./models/AddQuestionInput";
import { EditQuestionInput } from "./models/EditQuestionInput";
import { QuestionIdInput } from "./models/QuestionIdInput";

/**
 * Fast.Admin.Service.Question.QuestionService 题目服务Api
 */
export const questionApi = {
	queryQuestionPaged(data: QueryQuestionPagedInput) {
		return axiosUtil.request<PagedResult<QueryQuestionPagedOutput>>({
			url: "/question/queryQuestionPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	queryQuestionDetail(questionId: number) {
		return axiosUtil.request<QueryQuestionDetailOutput>({
			url: "/question/queryQuestionDetail",
			method: "get",
			params: { questionId },
			requestType: "query",
		});
	},
	addQuestion(data: AddQuestionInput) {
		return axiosUtil.request({
			url: "/question/addQuestion",
			method: "post",
			data,
			requestType: "add",
		});
	},
	editQuestion(data: EditQuestionInput) {
		return axiosUtil.request({
			url: "/question/editQuestion",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	deleteQuestion(data: QuestionIdInput) {
		return axiosUtil.request({
			url: "/question/deleteQuestion",
			method: "post",
			data,
			requestType: "delete",
		});
	},
};
