import { axiosUtil } from "@fast-china/axios";
import { TextbookListOutput } from "./models/TextbookListOutput";
import { VolumeListOutput } from "./models/VolumeListOutput";
import { LessonListOutput } from "./models/LessonListOutput";
import { PlaylistInput } from "./models/PlaylistInput";
import { PlaylistOutput } from "./models/PlaylistOutput";
import { SaveProgressInput } from "./models/SaveProgressInput";
import { PlayProgressOutput } from "./models/PlayProgressOutput";
import { UseActivationCodeInput } from "./models/UseActivationCodeInput";
import { ActivationStatusOutput } from "./models/ActivationStatusOutput";
import { AudioTypeOutput } from "./models/AudioTypeOutput";
import { PronunciationTypeOutput } from "./models/PronunciationTypeOutput";

/**
 * Fast.Admin.Service.Play.PlayService 播放服务Api
 */
export const playApi = {
	/**
	 * 获取教材列表
	 */
	getTextbookList() {
		return axiosUtil.request<TextbookListOutput[]>({
			url: "/play/getTextbookList",
			method: "get",
			requestType: "query",
		});
	},
	/**
	 * 获取册列表
	 */
	getVolumeList(textbookId: number) {
		return axiosUtil.request<VolumeListOutput[]>({
			url: "/play/getVolumeList",
			method: "get",
			params: { textbookId },
			requestType: "query",
		});
	},
	/**
	 * 获取课程列表
	 */
	getLessonList(volumeId: number) {
		return axiosUtil.request<LessonListOutput[]>({
			url: "/play/getLessonList",
			method: "get",
			params: { volumeId },
			requestType: "query",
		});
	},
	/**
	 * 获取播放列表
	 */
	getPlaylist(data: PlaylistInput) {
		return axiosUtil.request<PlaylistOutput[]>({
			url: "/play/getPlaylist",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 保存播放进度
	 */
	saveProgress(data: SaveProgressInput) {
		return axiosUtil.request({
			url: "/play/saveProgress",
			method: "post",
			data,
			requestType: "edit",
			showCodeMessage: false,
		});
	},
	/**
	 * 获取播放进度
	 */
	getPlayProgress(lessonId: number, audioTypeId: number, pronunciationTypeId: number) {
		return axiosUtil.request<PlayProgressOutput>({
			url: "/play/getPlayProgress",
			method: "get",
			params: { lessonId, audioTypeId, pronunciationTypeId },
			requestType: "query",
		});
	},
	/**
	 * 获取激活状态
	 */
	getActivationStatus() {
		return axiosUtil.request<ActivationStatusOutput>({
			url: "/play/getActivationStatus",
			method: "get",
			requestType: "query",
		});
	},
	/**
	 * 使用激活码
	 */
	useActivationCode(data: UseActivationCodeInput) {
		return axiosUtil.request({
			url: "/play/useActivationCode",
			method: "post",
			data,
			requestType: "edit",
			loading: true,
			loadingText: "激活中...",
		});
	},
	/**
	 * 音频类型选择器
	 */
	audioTypeSelector() {
		return axiosUtil.request<AudioTypeOutput[]>({
			url: "/audio/audioTypeSelector",
			method: "get",
			requestType: "query",
		});
	},
	/**
	 * 发音类型选择器
	 */
	pronunciationTypeSelector() {
		return axiosUtil.request<PronunciationTypeOutput[]>({
			url: "/audio/pronunciationTypeSelector",
			method: "get",
			requestType: "query",
		});
	},
};
