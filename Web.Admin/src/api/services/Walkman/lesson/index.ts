import { axiosUtil } from "@fast-china/axios";
import type { ElSelectorOutput, PagedResult } from "fast-element-plus";
import type { AddLessonInput } from "./models/AddLessonInput";
import type { EditLessonInput } from "./models/EditLessonInput";
import type { LessonIdInput } from "./models/LessonIdInput";
import type { QueryLessonDetailOutput } from "./models/QueryLessonDetailOutput";
import type { QueryLessonPagedInput } from "./models/QueryLessonPagedInput";
import type { QueryLessonPagedOutput } from "./models/QueryLessonPagedOutput";

/**
 * 课程服务Api
 */
export const lessonApi = {
	/**
	 * 课程分页选择器
	 */
	lessonSelector(data: QueryLessonPagedInput): Promise<PagedResult<ElSelectorOutput<string>>> {
		return axiosUtil.request<PagedResult<ElSelectorOutput<string>>>({
			url: "/lesson/lessonSelector",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取课程分页列表
	 */
	queryLessonPaged(data: QueryLessonPagedInput): Promise<PagedResult<QueryLessonPagedOutput>> {
		return axiosUtil.request<PagedResult<QueryLessonPagedOutput>>({
			url: "/lesson/queryLessonPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取课程详情
	 */
	queryLessonDetail(lessonId: string): Promise<QueryLessonDetailOutput> {
		return axiosUtil.request<QueryLessonDetailOutput>({
			url: "/lesson/queryLessonDetail",
			method: "get",
			params: {
				lessonId,
			},
			requestType: "query",
		});
	},
	/**
	 * 添加课程
	 */
	addLesson(data: AddLessonInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/lesson/addLesson",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑课程
	 */
	editLesson(data: EditLessonInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/lesson/editLesson",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	/**
	 * 删除课程
	 */
	deleteLesson(data: LessonIdInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/lesson/deleteLesson",
			method: "post",
			data,
			requestType: "delete",
		});
	},
};
