/**
 * Fast.Admin.Service.WeChat.Dto.WeChatCode2PhoneNumberOutput 换取微信用户手机号输出
 */
export interface WeChatCode2PhoneNumberOutput {
  /**
   * 用户纯手机号码
   */
  purePhoneNumber?: string;
  /**
   * 用户手机号码
   */
  phoneNumber?: string;
  /**
   * 用户手机号码区号
   */
  countryCode?: string;
}

