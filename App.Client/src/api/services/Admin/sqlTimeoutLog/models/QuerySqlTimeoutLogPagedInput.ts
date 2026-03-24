/**
 * Fast.Admin.Service.SqlTimeoutLog.Dto.QuerySqlTimeoutLogPagedInput 获取Sql超时日志分页列表输入
 */
export interface QuerySqlTimeoutLogPagedInput extends PagedInput  {
  /**
   * 账号Id
   */
  accountId?: number;
  /**
   * 
   */
  readonly isOrderBy?: boolean;
}

