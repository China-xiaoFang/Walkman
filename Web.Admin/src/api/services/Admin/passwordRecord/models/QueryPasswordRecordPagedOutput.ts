import { PasswordOperationTypeEnum } from "@/api/enums/PasswordOperationTypeEnum";
import { PasswordTypeEnum } from "@/api/enums/PasswordTypeEnum";

/**
 * Fast.Admin.Service.PasswordRecord.Dto.QueryPasswordRecordPagedOutput 获取密码记录分页列表输出
 */
export interface QueryPasswordRecordPagedOutput {
  /**
   * 记录Id
   */
  recordId?: number;
  /**
   * 账号Id
   */
  accountId?: number;
  /**
   * 
   */
  operationType?: PasswordOperationTypeEnum;
  /**
   * 
   */
  type?: PasswordTypeEnum;
  /**
   * 密码
   */
  password?: string;
  /**
   * 创建时间
   */
  createdTime?: Date;
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

