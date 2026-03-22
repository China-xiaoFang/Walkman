import { axiosUtil } from "@fast-china/axios";
import { PagedResult } from "fast-element-plus";
import { QueryLessonContentPagedOutput } from "./models/QueryLessonContentPagedOutput";
import { QueryLessonContentPagedInput } from "./models/QueryLessonContentPagedInput";
import { AddLessonContentInput } from "./models/AddLessonContentInput";
import { EditLessonContentInput } from "./models/EditLessonContentInput";
import { LessonContentIdInput } from "./models/LessonContentIdInput";

/**
 * Fast.Admin.Service.LessonContent.LessonContentService 课程内容服务Api
 */
export const lessonContentApi = {
	queryLessonContentPaged(data: QueryLessonContentPagedInput) {
		return axiosUtil.request<PagedResult<QueryLessonContentPagedOutput>>({
			url: "/lessonContent/queryLessonContentPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	queryLessonContentDetail(lessonContentId: number) {
		return axiosUtil.request<QueryLessonContentPagedOutput>({
			url: "/lessonContent/queryLessonContentDetail",
			method: "get",
			params: { lessonContentId },
			requestType: "query",
		});
	},
	addLessonContent(data: AddLessonContentInput) {
		return axiosUtil.request({
			url: "/lessonContent/addLessonContent",
			method: "post",
			data,
			requestType: "add",
		});
	},
	editLessonContent(data: EditLessonContentInput) {
		return axiosUtil.request({
			url: "/lessonContent/editLessonContent",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	deleteLessonContent(data: LessonContentIdInput) {
		return axiosUtil.request({
			url: "/lessonContent/deleteLessonContent",
			method: "post",
			data,
			requestType: "delete",
		});
	},
};
