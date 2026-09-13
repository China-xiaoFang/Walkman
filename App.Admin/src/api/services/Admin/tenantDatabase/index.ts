import { axiosUtil } from "@fast-china/axios";
import type { InitDatabaseInput } from "./models/InitDatabaseInput";

/**
 * 租户数据库自定义初始化逻辑Api
 */
export const tenantDatabaseApi = {
	/**
	 * 初始化数据库
	 */
	initDatabase(data: InitDatabaseInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/tenantDatabase/initDatabase",
			method: "post",
			data,
			requestType: "submit",
		});
	},
};
