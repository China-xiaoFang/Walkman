import { axiosUtil } from "@fast-china/axios";
import { ElSelectorOutput, PagedResult } from "fast-element-plus";
import { PaymentChannelEnum } from "@/api/enums/PaymentChannelEnum";
import { QueryMerchantPagedOutput } from "./models/QueryMerchantPagedOutput";
import { QueryMerchantPagedInput } from "./models/QueryMerchantPagedInput";
import { QueryMerchantDetailOutput } from "./models/QueryMerchantDetailOutput";
import { AddMerchantInput } from "./models/AddMerchantInput";
import { EditMerchantInput } from "./models/EditMerchantInput";
import { MerchantIdInput } from "./models/MerchantIdInput";

/**
 * Fast.Admin.Service.Merchant.MerchantService 商户号服务Api
 */
export const merchantApi = {
  /**
   * 商户号选择器
   */
  merchantSelector(merchantType: PaymentChannelEnum) {
    return axiosUtil.request<ElSelectorOutput<number>[]>({
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
  queryMerchantPaged(data: QueryMerchantPagedInput) {
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
  queryMerchantDetail(merchantId: number) {
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
  addMerchant(data: AddMerchantInput) {
    return axiosUtil.request({
      url: "/merchant/addMerchant",
      method: "post",
      data,
      requestType: "add",
    });
  },
  /**
   * 编辑商户号
   */
  editMerchant(data: EditMerchantInput) {
    return axiosUtil.request({
      url: "/merchant/editMerchant",
      method: "post",
      data,
      requestType: "edit",
    });
  },
  /**
   * 删除商户号
   */
  deleteMerchant(data: MerchantIdInput) {
    return axiosUtil.request({
      url: "/merchant/deleteMerchant",
      method: "post",
      data,
      requestType: "delete",
    });
  },
};
