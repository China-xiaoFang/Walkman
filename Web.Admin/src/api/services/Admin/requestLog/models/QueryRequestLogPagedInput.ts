import { PagedInput } from "fast-element-plus";
import { HttpRequestActionEnum } from "@/api/enums/HttpRequestActionEnum";
import { HttpRequestMethodEnum } from "@/api/enums/HttpRequestMethodEnum";

/**
 * Fast.Admin.Service.RequestLog.Dto.QueryRequestLogPagedInput 获取请求日志分页列表输入
 */
export interface QueryRequestLogPagedInput extends PagedInput  {
  /**
   * 账号Id
   */
  accountId?: number;
  /**
   * 是否执行成功
   */
  isSuccess?: boolean;
  /**
   * 
   */
  operationAction?: HttpRequestActionEnum;
  /**
   * 
   */
  requestMethod?: HttpRequestMethodEnum;
  /**
   * 
   */
  readonly isOrderBy?: boolean;
}

