import { axiosUtil } from "@fast-china/axios";
import type { ElSelectorOutput, PagedResult } from "fast-element-plus";
import type { AddMerchantInput } from "./models/AddMerchantInput";
import type { EditMerchantInput } from "./models/EditMerchantInput";
import type { MerchantIdInput } from "./models/MerchantIdInput";
import type { QueryMerchantDetailOutput } from "./models/QueryMerchantDetailOutput";
import type { QueryMerchantPagedInput } from "./models/QueryMerchantPagedInput";
import type { QueryMerchantPagedOutput } from "./models/QueryMerchantPagedOutput";
import type { PaymentChannelEnum } from "@/api/enums/PaymentChannelEnum";

/**
 * 商户号服务Api
 */
export const merchantApi = {
	/**
	 * 商户号选择器
	 */
	merchantSelector(merchantType: PaymentChannelEnum): Promise<ElSelectorOutput<string>[]> {
		return axiosUtil.request<ElSelectorOutput<string>[]>({
			url: "/merchant/merchantSelector",
			method: "get",
			params: {
				merchantType,
			},
			requestType: "query",
		});
	},
	/**
	 * 获取商户号分页列表
	 */
	queryMerchantPaged(data: QueryMerchantPagedInput): Promise<PagedResult<QueryMerchantPagedOutput>> {
		return axiosUtil.request<PagedResult<QueryMerchantPagedOutput>>({
			url: "/merchant/queryMerchantPaged",
			method: "post",
			data,
			requestType: "query",
		});
	},
	/**
	 * 获取商户号详情
	 */
	queryMerchantDetail(merchantId: string): Promise<QueryMerchantDetailOutput> {
		return axiosUtil.request<QueryMerchantDetailOutput>({
			url: "/merchant/queryMerchantDetail",
			method: "get",
			params: {
				merchantId,
			},
			requestType: "query",
		});
	},
	/**
	 * 添加商户号
	 */
	addMerchant(data: AddMerchantInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/merchant/addMerchant",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑商户号
	 */
	editMerchant(data: EditMerchantInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/merchant/editMerchant",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	/**
	 * 删除商户号
	 */
	deleteMerchant(data: MerchantIdInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/merchant/deleteMerchant",
			method: "post",
			data,
			requestType: "delete",
		});
	},
};
