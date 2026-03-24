import { WeChatUserTypeEnum } from "@/api/enums/WeChatUserTypeEnum";
import { GenderEnum } from "@/api/enums/GenderEnum";

/**
 * Fast.Admin.Service.WeChat.Dto.QueryWeChatUserPagedInput 获取微信用户分页列表输入
 */
export interface QueryWeChatUserPagedInput extends PagedInput  {
  /**
   * 
   */
  userType?: WeChatUserTypeEnum;
  /**
   * 
   */
  sex?: GenderEnum;
  /**
   * 
   */
  readonly isOrderBy?: boolean;
}

