/**
 * Fast.Admin.Service.Table.Dto.QueryTableColumnConfigOutput 获取表格列配置输出
 */
export interface QueryTableColumnConfigOutput {
  /**
   * 表格Key
   */
  tableKey?: string;
  /**
   * 原始列
   */
  columns?: Array<any>;
  /**
   * 缓存列
   */
  cacheColumns?: Array<any>;
  /**
   * 更新时间
   */
  updatedTime?: Date;
  /**
   * 是否存在改变
   */
  change?: boolean;
  /**
   * 是否存在缓存
   */
  cache?: boolean;
}

