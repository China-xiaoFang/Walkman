import { axiosUtil } from "@fast-china/axios";
import { RefundRecordModel } from "./models/RefundRecordModel";

/**
 * Fast.Admin.Service.RefundRecord.RefundRecordService 退款记录服务Api
 */
export const refundRecordApi = {
  /**
   * 获取退款记录分页列表
   */
  queryRefundRecordPaged(data: PagedInput) {
    return axiosUtil.request<PagedResult<RefundRecordModel>>({
      url: "/refundRecord/queryRefundRecordPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
};
