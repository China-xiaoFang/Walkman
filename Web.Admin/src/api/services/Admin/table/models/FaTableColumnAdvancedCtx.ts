import { ColumnAdvancedTypeEnum } from "@/api/enums/ColumnAdvancedTypeEnum";

/**
 * Fast.Shared.FaTableColumnAdvancedCtx FastElementPlus FaTable 列高级选项上下文
 */
export interface FaTableColumnAdvancedCtx {
  /**
   * 字段名称
   */
  prop?: string;
  /**
   * 
   */
  type?: ColumnAdvancedTypeEnum;
  /**
   * 值
   */
  value?: string;
}

