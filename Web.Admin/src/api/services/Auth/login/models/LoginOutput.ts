import { LoginStatusEnum } from "@/api/enums/LoginStatusEnum";

/**
 * Fast.Admin.Service.Login.Dto.LoginOutput 登录输出
 */
export interface LoginOutput {
  /**
   * 
   */
  status?: LoginStatusEnum;
  /**
   * 消息
   */
  message?: string;
  /**
   * 昵称
   */
  nickName?: string;
  /**
   * 头像
   */
  avatar?: string;
}

