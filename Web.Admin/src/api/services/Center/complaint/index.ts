import { axiosUtil } from "@fast-china/axios";
import type { PagedResult } from "fast-element-plus";
import type { AddComplaintInput } from "./models/AddComplaintInput";
import type { HandleComplaintInput } from "./models/HandleComplaintInput";
import type { QueryComplaintPagedInput } from "./models/QueryComplaintPagedInput";
import type { QueryComplaintPagedOutput } from "./models/QueryComplaintPagedOutput";

/**
 * 投诉服务Api
 */
export const complaintApi = {
	/**
	 * 获取投诉工单分页列表
	 */
	queryComplaintPaged(data: QueryComplaintPagedInput): Promise<PagedResult<QueryComplaintPagedOutput>> {
		return axiosUtil.request<PagedResult<QueryComplaintPagedOutput>>({
			url: "/complaint/queryComplaintPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取用户投诉分页列表
	 */
	queryTenantComplaintPaged(data: QueryComplaintPagedInput): Promise<PagedResult<QueryComplaintPagedOutput>> {
		return axiosUtil.request<PagedResult<QueryComplaintPagedOutput>>({
			url: "/complaint/queryTenantComplaintPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取投诉详情
	 */
	queryComplaintDetail(complaintId: string): Promise<QueryComplaintPagedOutput> {
		return axiosUtil.request<QueryComplaintPagedOutput>({
			url: "/complaint/queryComplaintDetail",
			method: "get",
			params: {
				complaintId,
			},
			requestType: "query",
		});
	},
	/**
	 * 添加投诉
	 */
	addComplaint(data: AddComplaintInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/complaint/addComplaint",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 处理投诉
	 */
	handleComplaint(data: HandleComplaintInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/complaint/handleComplaint",
			method: "post",
			data,
			requestType: "edit",
		});
	},
};
