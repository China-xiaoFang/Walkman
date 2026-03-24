import { PasswordOperationTypeEnum } from "@/api/enums/PasswordOperationTypeEnum";

/**
 * Fast.Admin.Service.PasswordRecord.Dto.QueryPasswordRecordPagedInput 获取密码记录分页列表输入
 */
export interface QueryPasswordRecordPagedInput extends PagedInput  {
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
  readonly isOrderBy?: boolean;
}

