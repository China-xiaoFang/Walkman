import { axiosUtil } from "@fast-china/axios";
import type { ElSelectorOutput, PagedInput, PagedResult } from "fast-element-plus";
import type { AddJobLevelInput } from "./models/AddJobLevelInput";
import type { EditJobLevelInput } from "./models/EditJobLevelInput";
import type { JobLevelIdInput } from "./models/JobLevelIdInput";
import type { QueryJobLevelDetailOutput } from "./models/QueryJobLevelDetailOutput";
import type { QueryJobLevelPagedOutput } from "./models/QueryJobLevelPagedOutput";

/**
 * 职级服务Api
 */
export const jobLevelApi = {
	/**
	 * 职级选择器
	 */
	jobLevelSelector(): Promise<ElSelectorOutput<string>[]> {
		return axiosUtil.request<ElSelectorOutput<string>[]>({
			url: "/jobLevel/jobLevelSelector",
			method: "get",
			requestType: "query",
		});
	},
	/**
	 * 获取职级分页列表
	 */
	queryJobLevelPaged(data: PagedInput): Promise<PagedResult<QueryJobLevelPagedOutput>> {
		return axiosUtil.request<PagedResult<QueryJobLevelPagedOutput>>({
			url: "/jobLevel/queryJobLevelPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取职级详情
	 */
	queryJobLevelDetail(jobLevelId: string): Promise<QueryJobLevelDetailOutput> {
		return axiosUtil.request<QueryJobLevelDetailOutput>({
			url: "/jobLevel/queryJobLevelDetail",
			method: "get",
			params: {
				jobLevelId,
			},
			requestType: "query",
		});
	},
	/**
	 * 添加职级
	 */
	addJobLevel(data: AddJobLevelInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/jobLevel/addJobLevel",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑职级
	 */
	editJobLevel(data: EditJobLevelInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/jobLevel/editJobLevel",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	/**
	 * 删除职级
	 */
	deleteJobLevel(data: JobLevelIdInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/jobLevel/deleteJobLevel",
			method: "post",
			data,
			requestType: "delete",
		});
	},
};
