import { axiosUtil } from "@fast-china/axios";
import type { ElSelectorOutput, PagedInput, PagedResult } from "fast-element-plus";
import type { AddPositionInput } from "./models/AddPositionInput";
import type { EditPositionInput } from "./models/EditPositionInput";
import type { PositionIdInput } from "./models/PositionIdInput";
import type { QueryPositionDetailOutput } from "./models/QueryPositionDetailOutput";
import type { QueryPositionPagedOutput } from "./models/QueryPositionPagedOutput";

/**
 * 职位服务Api
 */
export const positionApi = {
	/**
	 * 职位选择器
	 */
	positionSelector(): Promise<ElSelectorOutput<string>[]> {
		return axiosUtil.request<ElSelectorOutput<string>[]>({
			url: "/position/positionSelector",
			method: "get",
			requestType: "query",
		});
	},
	/**
	 * 获取职位分页列表
	 */
	queryPositionPaged(data: PagedInput): Promise<PagedResult<QueryPositionPagedOutput>> {
		return axiosUtil.request<PagedResult<QueryPositionPagedOutput>>({
			url: "/position/queryPositionPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取职位详情
	 */
	queryPositionDetail(positionId: string): Promise<QueryPositionDetailOutput> {
		return axiosUtil.request<QueryPositionDetailOutput>({
			url: "/position/queryPositionDetail",
			method: "get",
			params: {
				positionId,
			},
			requestType: "query",
		});
	},
	/**
	 * 添加职位
	 */
	addPosition(data: AddPositionInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/position/addPosition",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑职位
	 */
	editPosition(data: EditPositionInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/position/editPosition",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	/**
	 * 删除职位
	 */
	deletePosition(data: PositionIdInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/position/deletePosition",
			method: "post",
			data,
			requestType: "delete",
		});
	},
};
