/**
 * Fast.Admin.Service.SqlExecutionLog.Dto.QuerySqlExecutionLogPagedInput 获取Sql执行日志分页列表输入
 */
export interface QuerySqlExecutionLogPagedInput extends PagedInput  {
  /**
   * 账号Id
   */
  accountId?: number;
  /**
   * 
   */
  readonly isOrderBy?: boolean;
}

