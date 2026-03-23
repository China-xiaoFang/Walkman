import { HttpRequestActionEnum } from "@/api/enums/HttpRequestActionEnum";
import { HttpRequestMethodEnum } from "@/api/enums/HttpRequestMethodEnum";

/**
 * Fast.AdminLog.Entity.RequestLogModel 请求日志Model类
 */
export interface RequestLogModel {
  /**
   * 记录Id
   */
  recordId?: number;
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
   * 是否执行成功
   */
  isSuccess?: boolean;
  /**
   * 
   */
  operationAction?: HttpRequestActionEnum;
  /**
   * 操作名称
   */
  operationName?: string;
  /**
   * 类名
   */
  className?: string;
  /**
   * 方法名
   */
  methodName?: string;
  /**
   * 请求地址
   */
  location?: string;
  /**
   * 
   */
  requestMethod?: HttpRequestMethodEnum;
  /**
   * 请求参数
   */
  param?: string;
  /**
   * 返回结果
   */
  result?: string;
  /**
   * 耗时（毫秒）
   */
  elapsedTime?: number;
  /**
   * 操作者用户Id
   */
  createdUserId?: number;
  /**
   * 操作者用户名称
   */
  createdUserName?: string;
  /**
   * 操作时间
   */
  createdTime?: Date;
  /**
   * 
   */
  device?: string;
  /**
   * 
   */
  os?: string;
  /**
   * 
   */
  browser?: string;
  /**
   * 
   */
  province?: string;
  /**
   * 
   */
  city?: string;
  /**
   * 
   */
  ip?: string;
  /**
   * 
   */
  departmentId?: number;
  /**
   * 
   */
  departmentName?: string;
}

