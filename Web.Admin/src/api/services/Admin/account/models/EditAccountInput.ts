import { GenderEnum } from "@/api/enums/GenderEnum";

/**
 * Fast.Admin.Service.Account.Dto.EditAccountInput 编辑账号输入
 */
export interface EditAccountInput {
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
  /**
   * 电话
   */
  phone?: string;
  /**
   * 
   */
  sex?: GenderEnum;
  /**
   * 生日
   */
  birthday?: Date;
  /**
   * 
   */
  rowVersion?: number;
}

