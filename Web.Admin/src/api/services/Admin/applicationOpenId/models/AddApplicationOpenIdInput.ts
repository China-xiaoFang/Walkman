import { AppEnvironmentEnum } from "@/api/enums/AppEnvironmentEnum";
import { EnvironmentTypeEnum } from "@/api/enums/EnvironmentTypeEnum";

/**
 * Fast.Admin.Service.ApplicationOpenId.Dto.AddApplicationOpenIdInput 添加应用标识输入
 */
export interface AddApplicationOpenIdInput {
  /**
   * 应用标识
   */
  openId?: string;
  /**
   * 应用Id
   */
  appId?: number;
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
}

