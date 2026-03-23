import { PaymentChannelEnum } from "@/api/enums/PaymentChannelEnum";

/**
 * Fast.Admin.Service.Merchant.Dto.AddMerchantInput 添加商户号输入
 */
export interface AddMerchantInput {
  /**
   * 
   */
  merchantType?: PaymentChannelEnum;
  /**
   * 商户名称
   */
  merchantName?: string;
  /**
   * 商户号
   */
  merchantNo?: string;
  /**
   * 商户密钥
   */
  merchantSecret?: string;
  /**
   * 公钥序号
   */
  publicSerialNum?: string;
  /**
   * 公钥
   */
  publicKey?: string;
  /**
   * 证书序号
   */
  certSerialNum?: string;
  /**
   * 证书
   */
  cert?: string;
  /**
   * 证书私钥
   */
  certPrivateKey?: string;
  /**
   * 备注
   */
  remark?: string;
}

