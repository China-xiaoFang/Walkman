import { axiosUtil } from "@fast-china/axios";
import { ExceptionLogModel } from "./models/ExceptionLogModel";
import { QueryExceptionLogPagedInput } from "./models/QueryExceptionLogPagedInput";

/**
 * Fast.Admin.Service.ExceptionLog.ExceptionLogService 异常日志服务Api
 */
export const exceptionLogApi = {
  /**
   * 获取异常日志分页列表
   */
  queryExceptionLogPaged(data: QueryExceptionLogPagedInput) {
    return axiosUtil.request<PagedResult<ExceptionLogModel>>({
      url: "/exceptionLog/queryExceptionLogPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
};
