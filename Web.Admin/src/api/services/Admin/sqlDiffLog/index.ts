import { axiosUtil } from "@fast-china/axios";
import { PagedResult } from "fast-element-plus";
import { SqlDiffLogModel } from "./models/SqlDiffLogModel";
import { QuerySqlDiffLogPagedInput } from "./models/QuerySqlDiffLogPagedInput";

/**
 * Fast.Admin.Service.SqlDiffLog.SqlDiffLogService Sql差异日志服务Api
 */
export const sqlDiffLogApi = {
  /**
   * 获取Sql差异日志分页列表
   */
  querySqlDiffLogPaged(data: QuerySqlDiffLogPagedInput) {
    return axiosUtil.request<PagedResult<SqlDiffLogModel>>({
      url: "/sqlDiffLog/querySqlDiffLogPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
  /**
   * 删除90天前的Sql差异日志
   */
  deleteSqlDiffLog() {
    return axiosUtil.request({
      url: "/sqlDiffLog/deleteSqlDiffLog",
      method: "post",
      requestType: "delete",
    });
  },
};
