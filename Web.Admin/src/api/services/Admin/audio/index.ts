import { axiosUtil } from "@fast-china/axios";
import { ElSelectorOutput, PagedResult } from "fast-element-plus";
import { QueryAudioTypePagedOutput } from "./models/QueryAudioTypePagedOutput";
import { QueryAudioTypePagedInput } from "./models/QueryAudioTypePagedInput";
import { AddAudioTypeInput } from "./models/AddAudioTypeInput";
import { EditAudioTypeInput } from "./models/EditAudioTypeInput";
import { AudioTypeIdInput } from "./models/AudioTypeIdInput";
import { QueryPronunciationTypePagedOutput } from "./models/QueryPronunciationTypePagedOutput";
import { QueryPronunciationTypePagedInput } from "./models/QueryPronunciationTypePagedInput";
import { AddPronunciationTypeInput } from "./models/AddPronunciationTypeInput";
import { EditPronunciationTypeInput } from "./models/EditPronunciationTypeInput";
import { PronunciationTypeIdInput } from "./models/PronunciationTypeIdInput";
import { QueryAudioPagedOutput } from "./models/QueryAudioPagedOutput";
import { QueryAudioPagedInput } from "./models/QueryAudioPagedInput";
import { AddAudioInput } from "./models/AddAudioInput";
import { EditAudioInput } from "./models/EditAudioInput";
import { AudioIdInput } from "./models/AudioIdInput";

/**
 * Fast.Admin.Service.Audio.AudioService 音频服务Api
 */
export const audioApi = {
	/**
	 * 音频类型选择器
	 */
	audioTypeSelector() {
		return axiosUtil.request<ElSelectorOutput<number>[]>({
			url: "/audio/audioTypeSelector",
			method: "get",
			requestType: "query",
		});
	},
	/**
	 * 获取音频类型分页列表
	 */
	queryAudioTypePaged(data: QueryAudioTypePagedInput) {
		return axiosUtil.request<PagedResult<QueryAudioTypePagedOutput>>({
			url: "/audio/queryAudioTypePaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取音频类型详情
	 */
	queryAudioTypeDetail(audioTypeId: number) {
		return axiosUtil.request<QueryAudioTypePagedOutput>({
			url: "/audio/queryAudioTypeDetail",
			method: "get",
			params: { audioTypeId },
			requestType: "query",
		});
	},
	/**
	 * 添加音频类型
	 */
	addAudioType(data: AddAudioTypeInput) {
		return axiosUtil.request({
			url: "/audio/addAudioType",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑音频类型
	 */
	editAudioType(data: EditAudioTypeInput) {
		return axiosUtil.request({
			url: "/audio/editAudioType",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	/**
	 * 删除音频类型
	 */
	deleteAudioType(data: AudioTypeIdInput) {
		return axiosUtil.request({
			url: "/audio/deleteAudioType",
			method: "post",
			data,
			requestType: "delete",
		});
	},
	/**
	 * 发音类型选择器
	 */
	pronunciationTypeSelector() {
		return axiosUtil.request<ElSelectorOutput<number>[]>({
			url: "/audio/pronunciationTypeSelector",
			method: "get",
			requestType: "query",
		});
	},
	/**
	 * 获取发音类型分页列表
	 */
	queryPronunciationTypePaged(data: QueryPronunciationTypePagedInput) {
		return axiosUtil.request<PagedResult<QueryPronunciationTypePagedOutput>>({
			url: "/audio/queryPronunciationTypePaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取发音类型详情
	 */
	queryPronunciationTypeDetail(pronunciationTypeId: number) {
		return axiosUtil.request<QueryPronunciationTypePagedOutput>({
			url: "/audio/queryPronunciationTypeDetail",
			method: "get",
			params: { pronunciationTypeId },
			requestType: "query",
		});
	},
	/**
	 * 添加发音类型
	 */
	addPronunciationType(data: AddPronunciationTypeInput) {
		return axiosUtil.request({
			url: "/audio/addPronunciationType",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑发音类型
	 */
	editPronunciationType(data: EditPronunciationTypeInput) {
		return axiosUtil.request({
			url: "/audio/editPronunciationType",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	/**
	 * 删除发音类型
	 */
	deletePronunciationType(data: PronunciationTypeIdInput) {
		return axiosUtil.request({
			url: "/audio/deletePronunciationType",
			method: "post",
			data,
			requestType: "delete",
		});
	},
	/**
	 * 获取音频分页列表
	 */
	queryAudioPaged(data: QueryAudioPagedInput) {
		return axiosUtil.request<PagedResult<QueryAudioPagedOutput>>({
			url: "/audio/queryAudioPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取音频详情
	 */
	queryAudioDetail(audioId: number) {
		return axiosUtil.request<QueryAudioPagedOutput>({
			url: "/audio/queryAudioDetail",
			method: "get",
			params: { audioId },
			requestType: "query",
		});
	},
	/**
	 * 添加音频
	 */
	addAudio(data: AddAudioInput) {
		return axiosUtil.request({
			url: "/audio/addAudio",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑音频
	 */
	editAudio(data: EditAudioInput) {
		return axiosUtil.request({
			url: "/audio/editAudio",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	/**
	 * 删除音频
	 */
	deleteAudio(data: AudioIdInput) {
		return axiosUtil.request({
			url: "/audio/deleteAudio",
			method: "post",
			data,
			requestType: "delete",
		});
	},
};
