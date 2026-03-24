import { GenderEnum } from "@/api/enums/GenderEnum";

/**
 * Fast.Admin.Service.Login.Dto.ClientRegisterInput 客户端注册
 */
export interface ClientRegisterInput {
  /**
   * 手机
   */
  mobile?: string;
  /**
   * 密码
   */
  password?: string;
  /**
   * 验证码
   */
  verifyCode?: string;
  /**
   * 昵称
   */
  nickName?: string;
  /**
   * 头像
   */
  avatar?: string;
  /**
   * 电话
   */
  phone?: string;
  /**
   * 
   */
  sex?: GenderEnum;
}

