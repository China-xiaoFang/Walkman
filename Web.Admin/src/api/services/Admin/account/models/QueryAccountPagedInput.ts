import { PagedInput } from "fast-element-plus";
import { CommonStatusEnum } from "@/api/enums/CommonStatusEnum";
import { GenderEnum } from "@/api/enums/GenderEnum";

/**
 * Fast.Admin.Service.Account.Dto.QueryAccountPagedInput 获取账号分页列表输入
 */
export interface QueryAccountPagedInput extends PagedInput  {
  /**
   * 手机
   */
  mobile?: string;
  /**
   * 
   */
  status?: CommonStatusEnum;
  /**
   * 电话
   */
  phone?: string;
  /**
   * 
   */
  sex?: GenderEnum;
  /**
   * 初次登录城市
   */
  firstLoginCity?: string;
  /**
   * 初次登录Ip
   */
  firstLoginIp?: string;
  /**
   * 最后登录城市
   */
  lastLoginCity?: string;
  /**
   * 最后登录Ip
   */
  lastLoginIp?: string;
  /**
   * 是否锁定
   */
  isLock?: boolean;
  /**
   * 
   */
  readonly isOrderBy?: boolean;
}

