import { axiosUtil } from "@fast-china/axios";
import { PagedResult } from "fast-element-plus";
import { SqlTimeoutLogModel } from "./models/SqlTimeoutLogModel";
import { QuerySqlTimeoutLogPagedInput } from "./models/QuerySqlTimeoutLogPagedInput";

/**
 * Fast.Admin.Service.SqlTimeoutLog.SqlTimeoutLogModelService Sql超时日志服务Api
 */
export const sqlTimeoutLogApi = {
  /**
   * 获取Sql超时日志分页列表
   */
  querySqlTimeoutLogPaged(data: QuerySqlTimeoutLogPagedInput) {
    return axiosUtil.request<PagedResult<SqlTimeoutLogModel>>({
      url: "/sqlTimeoutLog/querySqlTimeoutLogPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
};
