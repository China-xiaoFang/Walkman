import { AppEnvironmentEnum } from "@/api/enums/AppEnvironmentEnum";
import { EnvironmentTypeEnum } from "@/api/enums/EnvironmentTypeEnum";

/**
 * Fast.Admin.Service.ApplicationOpenId.Dto.EditApplicationOpenIdInput 编辑应用标识输入
 */
export interface EditApplicationOpenIdInput {
  /**
   * 记录Id
   */
  recordId?: number;
  /**
   * 应用Id
   */
  appId?: number;
  /**
   * 应用标识
   */
  openId?: string;
  /**
   * 
   */
  appType?: AppEnvironmentEnum;
  /**
   * 开放平台密钥
   */
  openSecret?: string;
  /**
   * 
   */
  environmentType?: EnvironmentTypeEnum;
  /**
   * 微信商户号Id
   */
  weChatMerchantId?: number;
  /**
   * 微信商户号
   */
  weChatMerchantNo?: string;
  /**
   * 支付宝商户号Id
   */
  alipayMerchantId?: number;
  /**
   * 支付宝商户号
   */
  alipayMerchantNo?: string;
  /**
   * 备注
   */
  remark?: string;
  /**
   * 
   */
  rowVersion?: number;
}

