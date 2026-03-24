import { GenderEnum } from "@/api/enums/GenderEnum";

/**
 * Fast.Admin.Service.WeChat.Dto.WeChatCode2SessionOutput 换取微信用户身份信息输出
 */
export interface WeChatCode2SessionOutput {
  /**
   * 唯一用户标识
   */
  openId?: string;
  /**
   * 统一用户标识
   */
  unionId?: string;
  /**
   * 小程序登录凭证
   */
  sessionKey?: string;
  /**
   * 微信昵称
   */
  nickName?: string;
  /**
   * 头像
   */
  avatar?: string;
  /**
   * 
   */
  sex?: GenderEnum;
  /**
   * 国家
   */
  country?: string;
  /**
   * 省份
   */
  province?: string;
  /**
   * 城市
   */
  city?: string;
  /**
   * 语言
   */
  language?: string;
}

