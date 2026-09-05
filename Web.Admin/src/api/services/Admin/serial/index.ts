import { axiosUtil } from "@fast-china/axios";
import type { PagedInput, PagedResult } from "fast-element-plus";
import type { AddSerialRuleInput } from "./models/AddSerialRuleInput";
import type { EditSerialRuleInput } from "./models/EditSerialRuleInput";
import type { QuerySerialRuleDetailOutput } from "./models/QuerySerialRuleDetailOutput";
import type { QuerySerialRulePagedOutput } from "./models/QuerySerialRulePagedOutput";

/**
 * 序号规则服务Api
 */
export const serialApi = {
	/**
	 * 获取序号规则分页列表
	 */
	querySerialRulePaged(data: PagedInput): Promise<PagedResult<QuerySerialRulePagedOutput>> {
		return axiosUtil.request<PagedResult<QuerySerialRulePagedOutput>>({
			url: "/serial/querySerialRulePaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取序号规则详情
	 */
	querySerialRuleDetail(serialRuleId: string): Promise<QuerySerialRuleDetailOutput> {
		return axiosUtil.request<QuerySerialRuleDetailOutput>({
			url: "/serial/querySerialRuleDetail",
			method: "get",
			params: {
				serialRuleId,
			},
			requestType: "query",
		});
	},
	/**
	 * 添加序号规则
	 */
	addSerialRule(data: AddSerialRuleInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/serial/addSerialRule",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑序号规则
	 */
	editSerialRule(data: EditSerialRuleInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/serial/editSerialRule",
			method: "post",
			data,
			requestType: "edit",
		});
	},
};
