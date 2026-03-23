import { PagedInput } from "fast-element-plus";
import { AppEnvironmentEnum } from "@/api/enums/AppEnvironmentEnum";

/**
 * Fast.Admin.Service.OnlineUser.Dto.QueryOnlineUserPagedInput 获取在线用户分页列表输入
 */
export interface QueryOnlineUserPagedInput extends PagedInput  {
  /**
   * 
   */
  deviceType?: AppEnvironmentEnum;
  /**
   * 账号Id
   */
  accountId?: number;
  /**
   * 职员Id
   */
  employeeId?: number;
  /**
   * 
   */
  readonly isOrderBy?: boolean;
}

