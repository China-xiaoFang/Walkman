import { axiosUtil } from "@fast-china/axios";
import type { PagedResult } from "fast-element-plus";
import type { GenerateActivationCodeInput } from "./models/GenerateActivationCodeInput";
import type { QueryActivationCodeDetailOutput } from "./models/QueryActivationCodeDetailOutput";
import type { QueryActivationCodePagedInput } from "./models/QueryActivationCodePagedInput";
import type { QueryActivationCodePagedOutput } from "./models/QueryActivationCodePagedOutput";
import type { UserActivateInput } from "./models/UserActivateInput";

/**
 * 激活码服务Api
 */
export const activationCodeApi = {
	/**
	 * 用户激活
	 */
	userActivate(data: UserActivateInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/activationCode/userActivate",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	/**
	 * 获取激活码分页列表
	 */
	queryActivationCodePaged(data: QueryActivationCodePagedInput): Promise<PagedResult<QueryActivationCodePagedOutput>> {
		return axiosUtil.request<PagedResult<QueryActivationCodePagedOutput>>({
			url: "/activationCode/queryActivationCodePaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取激活码详情
	 */
	queryActivationCodeDetail(activationCodeId: string): Promise<QueryActivationCodeDetailOutput> {
		return axiosUtil.request<QueryActivationCodeDetailOutput>({
			url: "/activationCode/queryActivationCodeDetail",
			method: "get",
			params: {
				activationCodeId,
			},
			requestType: "query",
		});
	},
	/**
	 * 批量生成激活码
	 */
	generateActivationCode(data: GenerateActivationCodeInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/activationCode/generateActivationCode",
			method: "post",
			data,
			requestType: "add",
		});
	},
};
