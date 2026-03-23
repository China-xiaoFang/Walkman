import { FaTableColumnCtx } from "./FaTableColumnCtx";

/**
 * Fast.Admin.Service.Table.Dto.EditTableColumnConfigInput 编辑表格列配置输入
 */
export interface EditTableColumnConfigInput {
  /**
   * 表格Id
   */
  tableId?: number;
  /**
   * 表格列
   */
  columns?: Array<FaTableColumnCtx>;
  /**
   * 
   */
  rowVersion?: number;
}

