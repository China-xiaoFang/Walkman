import { axiosUtil } from "@fast-china/axios";
import type { PagedResult } from "fast-element-plus";
import type { AddAudioAssetInput } from "./models/AddAudioAssetInput";
import type { AudioAssetIdInput } from "./models/AudioAssetIdInput";
import type { EditAudioAssetInput } from "./models/EditAudioAssetInput";
import type { QueryAudioAssetDetailOutput } from "./models/QueryAudioAssetDetailOutput";
import type { QueryAudioAssetPagedInput } from "./models/QueryAudioAssetPagedInput";
import type { QueryAudioAssetPagedOutput } from "./models/QueryAudioAssetPagedOutput";

/**
 * 音频资源服务Api
 */
export const audioAssetApi = {
	/**
	 * 获取音频资源分页列表
	 */
	queryAudioAssetPaged(data: QueryAudioAssetPagedInput): Promise<PagedResult<QueryAudioAssetPagedOutput>> {
		return axiosUtil.request<PagedResult<QueryAudioAssetPagedOutput>>({
			url: "/audioAsset/queryAudioAssetPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取音频资源详情
	 */
	queryAudioAssetDetail(audioAssetId: string): Promise<QueryAudioAssetDetailOutput> {
		return axiosUtil.request<QueryAudioAssetDetailOutput>({
			url: "/audioAsset/queryAudioAssetDetail",
			method: "get",
			params: {
				audioAssetId,
			},
			requestType: "query",
		});
	},
	/**
	 * 添加音频资源
	 */
	addAudioAsset(data: AddAudioAssetInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/audioAsset/addAudioAsset",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑音频资源
	 */
	editAudioAsset(data: EditAudioAssetInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/audioAsset/editAudioAsset",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	/**
	 * 删除音频资源
	 */
	deleteAudioAsset(data: AudioAssetIdInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/audioAsset/deleteAudioAsset",
			method: "post",
			data,
			requestType: "delete",
		});
	},
};
