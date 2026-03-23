/**
 * 保存用户表格列配置Dto
 */
export interface SaveUserTableColumnConfigDto {
  /**
   * 表格列Id
   */
  columnId?: number;
  /**
   * 名称
   */
  label?: string;
  /**
   * 固定
   */
  fixed?: string;
  /**
   * 自动宽度
   */
  autoWidth?: boolean;
  /**
   * 宽度
   */
  width?: number;
  /**
   * 小的宽度
   */
  smallWidth?: number;
  /**
   * 顺序
   */
  order?: number;
  /**
   * 显示
   */
  show?: boolean;
  /**
   * 复制
   */
  copy?: boolean;
  /**
   * 排序
   */
  sortable?: boolean;
  /**
   * 搜索项名称
   */
  searchLabel?: string;
  /**
   * 搜索项排序
   */
  searchOrder?: number;
}

