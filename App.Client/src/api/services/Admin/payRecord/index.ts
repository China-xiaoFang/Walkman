import { axiosUtil } from "@fast-china/axios";
import { PayRecordModel } from "./models/PayRecordModel";

/**
 * Fast.Admin.Service.PayRecord.PayRecordService 支付记录服务Api
 */
export const payRecordApi = {
  /**
   * 获取支付记录分页列表
   */
  queryPayRecordPaged(data: PagedInput) {
    return axiosUtil.request<PagedResult<PayRecordModel>>({
      url: "/payRecord/queryPayRecordPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
};
