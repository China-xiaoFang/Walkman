import { FaTableColumnAdvancedCtx } from "./FaTableColumnAdvancedCtx";

/**
 * Fast.Admin.Service.Table.Dto.FaTableColumnCtx FastElementPlus FaTable 列上下文
 */
export interface FaTableColumnCtx {
  /**
   * 表格列Id
   */
  columnId?: number;
  /**
   * 绑定字段
   */
  prop?: string;
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
   * 排序字段
   */
  sortableField?: string;
  /**
   * 列类型
   */
  type?: string;
  /**
   * 链接
   */
  link?: boolean;
  /**
   * 点击事件名称
   */
  clickEmit?: string;
  /**
   * 标签
   */
  tag?: boolean;
  /**
   * 字典名称
   */
  enum?: string;
  /**
   * 日期格式化
   */
  dateFix?: boolean;
  /**
   * 日期格式化
   */
  dateFormat?: string;
  /**
   * 权限标识
   */
  authTag?: Array<string>;
  /**
   * 数据删除
   */
  dataDeleteField?: string;
  /**
   * 插槽名称
   */
  slot?: string;
  /**
   * 其他配置
   */
  otherConfig?: Array<FaTableColumnAdvancedCtx>;
  /**
   * 纯搜索
   */
  pureSearch?: boolean;
  /**
   * 搜索项
   */
  searchEl?: string;
  /**
   * 搜索项Key
   */
  searchKey?: string;
  /**
   * 搜索项名称
   */
  searchLabel?: string;
  /**
   * 搜索项排序
   */
  searchOrder?: number;
  /**
   * 搜索项插槽
   */
  searchSlot?: string;
  /**
   * 搜索配置
   */
  searchConfig?: Array<FaTableColumnAdvancedCtx>;
}

