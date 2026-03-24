import { AppEnvironmentEnum } from "@/api/enums/AppEnvironmentEnum";
import { EnvironmentTypeEnum } from "@/api/enums/EnvironmentTypeEnum";

/**
 * Fast.Admin.Service.ApplicationOpenId.Dto.QueryApplicationOpenIdPagedInput 获取应用标识分页列表输入
 */
export interface QueryApplicationOpenIdPagedInput extends PagedInput  {
  /**
   * 
   */
  appType?: AppEnvironmentEnum;
  /**
   * 
   */
  environmentType?: EnvironmentTypeEnum;
  /**
   * 
   */
  readonly isOrderBy?: boolean;
}

