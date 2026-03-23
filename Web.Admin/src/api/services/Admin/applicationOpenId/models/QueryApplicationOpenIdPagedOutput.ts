import { AppEnvironmentEnum } from "@/api/enums/AppEnvironmentEnum";
import { EnvironmentTypeEnum } from "@/api/enums/EnvironmentTypeEnum";

/**
 * Fast.Admin.Service.ApplicationOpenId.Dto.QueryApplicationOpenIdPagedOutput 获取应用标识分页列表输出
 */
export interface QueryApplicationOpenIdPagedOutput {
  /**
   * 记录Id
   */
  recordId?: number;
  /**
   * 应用标识
   */
  openId?: string;
  /**
   * 
   */
  appType?: AppEnvironmentEnum;
  /**
   * 
   */
  environmentType?: EnvironmentTypeEnum;
  /**
   * 微信商户号
   */
  weChatMerchantNo?: string;
  /**
   * 支付宝商户号
   */
  alipayMerchantNo?: string;
  /**
   * 微信 AccessToken 刷新时间
   */
  weChatAccessTokenRefreshTime?: Date;
  /**
   * 微信 JsApi Ticket 刷新时间
   */
  weChatJsApiTicketRefreshTime?: Date;
  /**
   * 备注
   */
  remark?: string;
  /**
   * 
   */
  departmentName?: string;
  /**
   * 
   */
  createdUserName?: string;
  /**
   * 
   */
  createdTime?: Date;
  /**
   * 
   */
  updatedUserName?: string;
  /**
   * 
   */
  updatedTime?: Date;
  /**
   * 
   */
  rowVersion?: number;
}

