import { axiosUtil } from "@fast-china/axios";
import { PagedResult } from "fast-element-plus";
import { QueryActivationCodePagedOutput } from "./models/QueryActivationCodePagedOutput";
import { QueryActivationCodePagedInput } from "./models/QueryActivationCodePagedInput";
import { QueryActivationCodeDetailOutput } from "./models/QueryActivationCodeDetailOutput";
import { GenerateActivationCodeInput } from "./models/GenerateActivationCodeInput";

/**
 * Fast.Admin.Service.ActivationCode.ActivationCodeService 激活码服务Api
 */
export const activationCodeApi = {
	/**
	 * 获取激活码分页列表
	 */
	queryActivationCodePaged(data: QueryActivationCodePagedInput) {
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
	queryActivationCodeDetail(activationCodeId: number) {
		return axiosUtil.request<QueryActivationCodeDetailOutput>({
			url: "/activationCode/queryActivationCodeDetail",
			method: "get",
			params: { activationCodeId },
			requestType: "query",
		});
	},
	/**
	 * 批量生成激活码
	 */
	generateActivationCode(data: GenerateActivationCodeInput) {
		return axiosUtil.request({
			url: "/activationCode/generateActivationCode",
			method: "post",
			data,
			requestType: "add",
		});
	},
};
