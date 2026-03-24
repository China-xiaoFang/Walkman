import { axiosUtil } from "@fast-china/axios";
import { SqlExceptionLogModel } from "./models/SqlExceptionLogModel";
import { QuerySqlExceptionLogPagedInput } from "./models/QuerySqlExceptionLogPagedInput";

/**
 * Fast.Admin.Service.SqlExceptionLog.SqlExceptionLogModelService Sql异常日志服务Api
 */
export const sqlExceptionLogApi = {
  /**
   * 获取Sql异常日志分页列表
   */
  querySqlExceptionLogPaged(data: QuerySqlExceptionLogPagedInput) {
    return axiosUtil.request<PagedResult<SqlExceptionLogModel>>({
      url: "/sqlExceptionLog/querySqlExceptionLogPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
};
