import { axiosUtil } from "@fast-china/axios";
import type { ElSelectorOutput, PagedInput, PagedResult } from "fast-element-plus";
import type { AddTenantInput } from "./models/AddTenantInput";
import type { EditTenantInput } from "./models/EditTenantInput";
import type { QueryTenantDetailOutput } from "./models/QueryTenantDetailOutput";
import type { QueryTenantPagedInput } from "./models/QueryTenantPagedInput";
import type { QueryTenantPagedOutput } from "./models/QueryTenantPagedOutput";
import type { TenantIdInput } from "./models/TenantIdInput";

/**
 * 租户服务Api
 */
export const tenantApi = {
	/**
	 * 租户选择器
	 */
	tenantSelector(data: PagedInput): Promise<PagedResult<ElSelectorOutput<string>>> {
		return axiosUtil.request<PagedResult<ElSelectorOutput<string>>>({
			url: "/tenant/tenantSelector",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取租户分页列表
	 */
	queryTenantPaged(data: QueryTenantPagedInput): Promise<PagedResult<QueryTenantPagedOutput>> {
		return axiosUtil.request<PagedResult<QueryTenantPagedOutput>>({
			url: "/tenant/queryTenantPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取租户详情
	 */
	queryTenantDetail(tenantId: string): Promise<QueryTenantDetailOutput> {
		return axiosUtil.request<QueryTenantDetailOutput>({
			url: "/tenant/queryTenantDetail",
			method: "get",
			params: {
				tenantId,
			},
			requestType: "query",
		});
	},
	/**
	 * 添加租户
	 */
	addTenant(data: AddTenantInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/tenant/addTenant",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑租户
	 */
	editTenant(data: EditTenantInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/tenant/editTenant",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	/**
	 * 租户更改状态
	 */
	changeStatus(data: TenantIdInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/tenant/changeStatus",
			method: "post",
			data,
			requestType: "edit",
		});
	},
};
