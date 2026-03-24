import { LoginStatusEnum } from "@/api/enums/LoginStatusEnum";

/**
 * Fast.Admin.Service.Login.Dto.ClientLoginOutput 客户端登录输出
 */
export interface ClientLoginOutput {
  /**
   * 
   */
  status?: LoginStatusEnum;
  /**
   * 消息
   */
  message?: string;
  /**
   * 唯一用户标识
   */
  openId?: string;
  /**
   * 统一用户标识
   */
  unionId?: string;
  /**
   * 手机
   */
  mobile?: string;
  /**
   * 昵称
   */
  nickName?: string;
  /**
   * 头像
   */
  avatar?: string;
}

