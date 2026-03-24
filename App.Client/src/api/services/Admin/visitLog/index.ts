import { axiosUtil } from "@fast-china/axios";
import { VisitLogModel } from "./models/VisitLogModel";
import { QueryVisitLogPagedInput } from "./models/QueryVisitLogPagedInput";

/**
 * Fast.Admin.Service.VisitLog.VisitLogService 访问日志服务Api
 */
export const visitLogApi = {
  /**
   * 获取访问日志分页列表
   */
  queryVisitLogPaged(data: QueryVisitLogPagedInput) {
    return axiosUtil.request<PagedResult<VisitLogModel>>({
      url: "/visitLog/queryVisitLogPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
  /**
   * 删除90天前的访问日志
   */
  deleteVisitLog() {
    return axiosUtil.request({
      url: "/visitLog/deleteVisitLog",
      method: "post",
      requestType: "delete",
    });
  },
};
