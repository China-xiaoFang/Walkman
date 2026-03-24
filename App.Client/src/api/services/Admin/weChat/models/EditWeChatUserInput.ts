import { GenderEnum } from "@/api/enums/GenderEnum";

/**
 * Fast.Admin.Service.WeChat.Dto.EditWeChatUserInput 编辑微信用户输入
 */
export interface EditWeChatUserInput {
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
   * 
   */
  rowVersion?: number;
}

