import { PagedInput } from "fast-element-plus";
import { VisitTypeEnum } from "@/api/enums/VisitTypeEnum";

/**
 * Fast.Admin.Service.VisitLog.Dto.QueryVisitLogPagedInput 获取访问日志分页列表输入
 */
export interface QueryVisitLogPagedInput extends PagedInput  {
  /**
   * 账号Id
   */
  accountId?: number;
  /**
   * 
   */
  visitType?: VisitTypeEnum;
  /**
   * 
   */
  readonly isOrderBy?: boolean;
}

