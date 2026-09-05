import { axiosUtil } from "@fast-china/axios";
import type { PagedResult } from "fast-element-plus";
import type { QueryMessageSendRecordDetailOutput } from "./models/QueryMessageSendRecordDetailOutput";
import type { QueryMessageSendRecordPagedInput } from "./models/QueryMessageSendRecordPagedInput";
import type { QueryMessageSendRecordPagedOutput } from "./models/QueryMessageSendRecordPagedOutput";

/**
 * 消息发送记录服务Api
 */
export const messageSendRecordApi = {
	/**
	 * 获取消息发送记录分页列表
	 */
	queryMessageSendRecordPaged(data: QueryMessageSendRecordPagedInput): Promise<PagedResult<QueryMessageSendRecordPagedOutput>> {
		return axiosUtil.request<PagedResult<QueryMessageSendRecordPagedOutput>>({
			url: "/messageSendRecord/queryMessageSendRecordPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取消息发送记录详情
	 */
	queryMessageSendRecordDetail(recordId: string): Promise<QueryMessageSendRecordDetailOutput> {
		return axiosUtil.request<QueryMessageSendRecordDetailOutput>({
			url: "/messageSendRecord/queryMessageSendRecordDetail",
			method: "get",
			params: {
				recordId,
			},
			requestType: "query",
		});
	},
};
