/**
 * Fast.Shared.DataScopeTypeEnum 数据范围类型枚举
 */
export enum DataScopeTypeEnum {
  /**
   * 全部数据
   */
  All = 1,
  /**
   * 本机构及以下数据
   */
  OrgWithChild = 2,
  /**
   * 本部门及以下数据
   */
  DeptWithChild = 4,
  /**
   * 本部门数据
   */
  Dept = 8,
  /**
   * 仅本人数据
   */
  Self = 16,
}
