import { axiosUtil } from "@fast-china/axios";
import { PagedResult } from "fast-element-plus";
import { RequestLogModel } from "./models/RequestLogModel";
import { QueryRequestLogPagedInput } from "./models/QueryRequestLogPagedInput";

/**
 * Fast.Admin.Service.RequestLog.RequestLogService 请求日志服务Api
 */
export const requestLogApi = {
  /**
   * 获取请求日志分页列表
   */
  queryRequestLogPaged(data: QueryRequestLogPagedInput) {
    return axiosUtil.request<PagedResult<RequestLogModel>>({
      url: "/requestLog/queryRequestLogPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
  /**
   * 删除90天前的请求日志
   */
  deleteRequestLog() {
    return axiosUtil.request({
      url: "/requestLog/deleteRequestLog",
      method: "post",
      requestType: "delete",
    });
  },
};
