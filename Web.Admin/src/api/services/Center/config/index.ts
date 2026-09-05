import { axiosUtil } from "@fast-china/axios";
import type { PagedInput, PagedResult } from "fast-element-plus";
import type { AddConfigInput } from "./models/AddConfigInput";
import type { DeleteConfigCacheInput } from "./models/DeleteConfigCacheInput";
import type { EditConfigInput } from "./models/EditConfigInput";
import type { QueryConfigDetailOutput } from "./models/QueryConfigDetailOutput";
import type { QueryConfigPagedOutput } from "./models/QueryConfigPagedOutput";

/**
 * 配置服务Api
 */
export const configApi = {
	/**
	 * 获取配置分页列表
	 */
	queryConfigPaged(data: PagedInput): Promise<PagedResult<QueryConfigPagedOutput>> {
		return axiosUtil.request<PagedResult<QueryConfigPagedOutput>>({
			url: "/config/queryConfigPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取配置详情
	 */
	queryConfigDetail(configId: string): Promise<QueryConfigDetailOutput> {
		return axiosUtil.request<QueryConfigDetailOutput>({
			url: "/config/queryConfigDetail",
			method: "get",
			params: {
				configId,
			},
			requestType: "query",
		});
	},
	/**
	 * 添加配置
	 */
	addConfig(data: AddConfigInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/config/addConfig",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑配置
	 */
	editConfig(data: EditConfigInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/config/editConfig",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	/**
	 * 删除配置缓存
	 */
	deleteConfigCache(data: DeleteConfigCacheInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/config/deleteConfigCache",
			method: "post",
			data,
			requestType: "delete",
		});
	},
	/**
	 * 删除所有配置缓存
	 */
	deleteAllConfigCache(): Promise<void> {
		return axiosUtil.request<void>({
			url: "/config/deleteAllConfigCache",
			method: "post",
			requestType: "delete",
		});
	},
};
