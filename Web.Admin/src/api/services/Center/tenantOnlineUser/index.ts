import { axiosUtil } from "@fast-china/axios";
import type { PagedResult } from "fast-element-plus";
import type { ForceOfflineInput } from "./models/ForceOfflineInput";
import type { QueryTenantOnlineUserPagedInput } from "./models/QueryTenantOnlineUserPagedInput";
import type { TenantOnlineUserModel } from "./models/TenantOnlineUserModel";

/**
 * 在线用户服务Api
 */
export const tenantOnlineUserApi = {
	/**
	 * 获取在线用户分页列表
	 */
	queryTenantOnlineUserPaged(data: QueryTenantOnlineUserPagedInput): Promise<PagedResult<TenantOnlineUserModel>> {
		return axiosUtil.request<PagedResult<TenantOnlineUserModel>>({
			url: "/tenantOnlineUser/queryTenantOnlineUserPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 强制下线
	 */
	forceOffline(data: ForceOfflineInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/tenantOnlineUser/forceOffline",
			method: "post",
			data,
			requestType: "query",
		});
	},
};
