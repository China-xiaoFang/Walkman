import { AppEnvironmentEnum } from "@/api/enums/AppEnvironmentEnum";

/**
 * Fast.Admin.Entity.OnlineUserModel 在线用户表Model类
 */
export interface OnlineUserModel {
  /**
   * 连接Id
   */
  connectionId?: string;
  /**
   * 
   */
  deviceType?: AppEnvironmentEnum;
  /**
   * 设备Id
   */
  deviceId?: string;
  /**
   * 账号Id
   */
  accountId?: number;
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
   * 职员Id
   */
  employeeId?: number;
  /**
   * 工号
   */
  employeeNo?: string;
  /**
   * 姓名
   */
  employeeName?: string;
  /**
   * 部门Id
   */
  departmentId?: number;
  /**
   * 部门名称
   */
  departmentName?: string;
  /**
   * 是否超级管理员
   */
  isSuperAdmin?: boolean;
  /**
   * 是否管理员
   */
  isAdmin?: boolean;
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
   * 是否在线
   */
  isOnline?: boolean;
  /**
   * 下线时间
   */
  offlineTime?: Date;
}

