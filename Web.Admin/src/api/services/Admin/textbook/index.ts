import { axiosUtil } from "@fast-china/axios";
import { PagedResult } from "fast-element-plus";
import { QueryTextbookPagedOutput } from "./models/QueryTextbookPagedOutput";
import { QueryTextbookPagedInput } from "./models/QueryTextbookPagedInput";
import { QueryTextbookDetailOutput } from "./models/QueryTextbookDetailOutput";
import { AddTextbookInput } from "./models/AddTextbookInput";
import { EditTextbookInput } from "./models/EditTextbookInput";
import { TextbookIdInput } from "./models/TextbookIdInput";
import { QueryVolumePagedOutput } from "./models/QueryVolumePagedOutput";
import { QueryVolumePagedInput } from "./models/QueryVolumePagedInput";
import { AddVolumeInput } from "./models/AddVolumeInput";
import { EditVolumeInput } from "./models/EditVolumeInput";
import { VolumeIdInput } from "./models/VolumeIdInput";
import { QueryLessonPagedOutput } from "./models/QueryLessonPagedOutput";
import { QueryLessonPagedInput } from "./models/QueryLessonPagedInput";
import { AddLessonInput } from "./models/AddLessonInput";
import { EditLessonInput } from "./models/EditLessonInput";
import { LessonIdInput } from "./models/LessonIdInput";

/**
 * Fast.Admin.Service.Textbook.TextbookService 教材服务Api
 */
export const textbookApi = {
	/**
	 * 获取教材分页列表
	 */
	queryTextbookPaged(data: QueryTextbookPagedInput) {
		return axiosUtil.request<PagedResult<QueryTextbookPagedOutput>>({
			url: "/textbook/queryTextbookPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取教材详情
	 */
	queryTextbookDetail(textbookId: number) {
		return axiosUtil.request<QueryTextbookDetailOutput>({
			url: "/textbook/queryTextbookDetail",
			method: "get",
			params: { textbookId },
			requestType: "query",
		});
	},
	/**
	 * 添加教材
	 */
	addTextbook(data: AddTextbookInput) {
		return axiosUtil.request({
			url: "/textbook/addTextbook",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑教材
	 */
	editTextbook(data: EditTextbookInput) {
		return axiosUtil.request({
			url: "/textbook/editTextbook",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	/**
	 * 删除教材
	 */
	deleteTextbook(data: TextbookIdInput) {
		return axiosUtil.request({
			url: "/textbook/deleteTextbook",
			method: "post",
			data,
			requestType: "delete",
		});
	},
	/**
	 * 获取册分页列表
	 */
	queryVolumePaged(data: QueryVolumePagedInput) {
		return axiosUtil.request<PagedResult<QueryVolumePagedOutput>>({
			url: "/textbook/queryVolumePaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取册详情
	 */
	queryVolumeDetail(volumeId: number) {
		return axiosUtil.request<QueryTextbookDetailOutput>({
			url: "/textbook/queryVolumeDetail",
			method: "get",
			params: { volumeId },
			requestType: "query",
		});
	},
	/**
	 * 添加册
	 */
	addVolume(data: AddVolumeInput) {
		return axiosUtil.request({
			url: "/textbook/addVolume",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑册
	 */
	editVolume(data: EditVolumeInput) {
		return axiosUtil.request({
			url: "/textbook/editVolume",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	/**
	 * 删除册
	 */
	deleteVolume(data: VolumeIdInput) {
		return axiosUtil.request({
			url: "/textbook/deleteVolume",
			method: "post",
			data,
			requestType: "delete",
		});
	},
	/**
	 * 获取课程分页列表
	 */
	queryLessonPaged(data: QueryLessonPagedInput) {
		return axiosUtil.request<PagedResult<QueryLessonPagedOutput>>({
			url: "/textbook/queryLessonPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取课程详情
	 */
	queryLessonDetail(lessonId: number) {
		return axiosUtil.request<QueryTextbookDetailOutput>({
			url: "/textbook/queryLessonDetail",
			method: "get",
			params: { lessonId },
			requestType: "query",
		});
	},
	/**
	 * 添加课程
	 */
	addLesson(data: AddLessonInput) {
		return axiosUtil.request({
			url: "/textbook/addLesson",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑课程
	 */
	editLesson(data: EditLessonInput) {
		return axiosUtil.request({
			url: "/textbook/editLesson",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	/**
	 * 删除课程
	 */
	deleteLesson(data: LessonIdInput) {
		return axiosUtil.request({
			url: "/textbook/deleteLesson",
			method: "post",
			data,
			requestType: "delete",
		});
	},
};
