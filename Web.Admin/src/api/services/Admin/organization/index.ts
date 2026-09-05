import { axiosUtil } from "@fast-china/axios";
import type { ElSelectorOutput } from "fast-element-plus";
import type { AddOrganizationInput } from "./models/AddOrganizationInput";
import type { EditOrganizationInput } from "./models/EditOrganizationInput";
import type { OrganizationIdInput } from "./models/OrganizationIdInput";
import type { QueryOrganizationDetailOutput } from "./models/QueryOrganizationDetailOutput";

/**
 * 机构服务Api
 */
export const organizationApi = {
	/**
	 * 机构选择器
	 */
	organizationSelector(): Promise<ElSelectorOutput<string>[]> {
		return axiosUtil.request<ElSelectorOutput<string>[]>({
			url: "/organization/organizationSelector",
			method: "get",
			requestType: "query",
		});
	},
	/**
	 * 获取机构详情
	 */
	queryOrganizationDetail(orgId: string): Promise<QueryOrganizationDetailOutput> {
		return axiosUtil.request<QueryOrganizationDetailOutput>({
			url: "/organization/queryOrganizationDetail",
			method: "get",
			params: {
				orgId,
			},
			requestType: "query",
		});
	},
	/**
	 * 添加机构
	 */
	addOrganization(data: AddOrganizationInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/organization/addOrganization",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑机构
	 */
	editOrganization(data: EditOrganizationInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/organization/editOrganization",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	/**
	 * 删除机构
	 */
	deleteOrganization(data: OrganizationIdInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/organization/deleteOrganization",
			method: "post",
			data,
			requestType: "delete",
		});
	},
};
