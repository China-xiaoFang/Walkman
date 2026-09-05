import { axiosUtil } from "@fast-china/axios";
import type { PagedInput, PagedResult } from "fast-element-plus";
import type { AddSysSerialRuleInput } from "./models/AddSysSerialRuleInput";
import type { EditSysSerialRuleInput } from "./models/EditSysSerialRuleInput";
import type { QuerySysSerialRuleDetailOutput } from "./models/QuerySysSerialRuleDetailOutput";
import type { QuerySysSerialRulePagedOutput } from "./models/QuerySysSerialRulePagedOutput";

/**
 * 系统序号规则服务Api
 */
export const sysSerialApi = {
	/**
	 * 获取系统序号规则分页列表
	 */
	querySysSerialRulePaged(data: PagedInput): Promise<PagedResult<QuerySysSerialRulePagedOutput>> {
		return axiosUtil.request<PagedResult<QuerySysSerialRulePagedOutput>>({
			url: "/sysSerial/querySysSerialRulePaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取系统序号规则详情
	 */
	querySysSerialRuleDetail(serialRuleId: string): Promise<QuerySysSerialRuleDetailOutput> {
		return axiosUtil.request<QuerySysSerialRuleDetailOutput>({
			url: "/sysSerial/querySysSerialRuleDetail",
			method: "get",
			params: {
				serialRuleId,
			},
			requestType: "query",
		});
	},
	/**
	 * 添加系统序号规则
	 */
	addSysSerialRule(data: AddSysSerialRuleInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/sysSerial/addSysSerialRule",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑系统序号规则
	 */
	editSysSerialRule(data: EditSysSerialRuleInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/sysSerial/editSysSerialRule",
			method: "post",
			data,
			requestType: "edit",
		});
	},
};
