import { CommonStatusEnum } from "@/api/enums/CommonStatusEnum";
import { GenderEnum } from "@/api/enums/GenderEnum";

/**
 * Fast.Admin.Service.Account.Dto.QueryAccountPagedOutput 获取账号分页列表输出
 */
export interface QueryAccountPagedOutput {
  /**
   * 账号Id
   */
  accountId?: number;
  /**
   * 手机
   */
  mobile?: string;
  /**
   * 
   */
  status?: CommonStatusEnum;
  /**
   * 昵称
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
   * 生日
   */
  birthday?: Date;
  /**
   * 初次登录设备
   */
  firstLoginDevice?: string;
  /**
   * 初次登录操作系统（版本）
   */
  firstLoginOS?: string;
  /**
   * 初次登录浏览器（版本）
   */
  firstLoginBrowser?: string;
  /**
   * 初次登录省份
   */
  firstLoginProvince?: string;
  /**
   * 初次登录城市
   */
  firstLoginCity?: string;
  /**
   * 初次登录Ip
   */
  firstLoginIp?: string;
  /**
   * 初次登录时间
   */
  firstLoginTime?: Date;
  /**
   * 最后登录设备
   */
  lastLoginDevice?: string;
  /**
   * 最后登录操作系统（版本）
   */
  lastLoginOS?: string;
  /**
   * 最后登录浏览器（版本）
   */
  lastLoginBrowser?: string;
  /**
   * 最后登录省份
   */
  lastLoginProvince?: string;
  /**
   * 最后登录城市
   */
  lastLoginCity?: string;
  /**
   * 最后登录Ip
   */
  lastLoginIp?: string;
  /**
   * 最后登录时间
   */
  lastLoginTime?: Date;
  /**
   * 密码错误次数
   */
  passwordErrorTime?: number;
  /**
   * 锁定开始时间
   */
  lockStartTime?: Date;
  /**
   * 锁定结束时间
   */
  lockEndTime?: Date;
  /**
   * 是否锁定
   */
  isLock?: boolean;
  /**
   * 创建时间
   */
  createdTime?: Date;
  /**
   * 更新时间
   */
  updatedTime?: Date;
  /**
   * 更新版本控制字段
   */
  rowVersion?: number;
}

