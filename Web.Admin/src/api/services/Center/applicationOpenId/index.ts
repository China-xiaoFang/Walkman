import { axiosUtil } from "@fast-china/axios";
import type { PagedResult } from "fast-element-plus";
import type { AddApplicationOpenIdInput } from "./models/AddApplicationOpenIdInput";
import type { EditApplicationOpenIdInput } from "./models/EditApplicationOpenIdInput";
import type { QueryApplicationOpenIdDetailOutput } from "./models/QueryApplicationOpenIdDetailOutput";
import type { QueryApplicationOpenIdPagedInput } from "./models/QueryApplicationOpenIdPagedInput";
import type { QueryApplicationOpenIdPagedOutput } from "./models/QueryApplicationOpenIdPagedOutput";
import type { RecordIdInput } from "./models/RecordIdInput";

/**
 * 应用标识服务Api
 */
export const applicationOpenIdApi = {
	/**
	 * 获取应用标识分页列表
	 */
	queryApplicationOpenIdPaged(data: QueryApplicationOpenIdPagedInput): Promise<PagedResult<QueryApplicationOpenIdPagedOutput>> {
		return axiosUtil.request<PagedResult<QueryApplicationOpenIdPagedOutput>>({
			url: "/applicationOpenId/queryApplicationOpenIdPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取应用标识详情
	 */
	queryApplicationOpenIdDetail(recordId: string): Promise<QueryApplicationOpenIdDetailOutput> {
		return axiosUtil.request<QueryApplicationOpenIdDetailOutput>({
			url: "/applicationOpenId/queryApplicationOpenIdDetail",
			method: "get",
			params: {
				recordId,
			},
			requestType: "query",
		});
	},
	/**
	 * 添加应用标识
	 */
	addApplicationOpenId(data: AddApplicationOpenIdInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/applicationOpenId/addApplicationOpenId",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑应用标识
	 */
	editApplicationOpenId(data: EditApplicationOpenIdInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/applicationOpenId/editApplicationOpenId",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	/**
	 * 删除应用标识
	 */
	deleteApplicationOpenId(data: RecordIdInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/applicationOpenId/deleteApplicationOpenId",
			method: "post",
			data,
			requestType: "delete",
		});
	},
};
