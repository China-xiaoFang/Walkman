/**
 * Fast.AdminLog.Entity.ExceptionLogModel 异常日志Model类
 */
export interface ExceptionLogModel {
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
   * 类名
   */
  className?: string;
  /**
   * 方法名
   */
  methodName?: string;
  /**
   * 异常信息
   */
  message?: string;
  /**
   * 异常源
   */
  source?: string;
  /**
   * 异常堆栈信息
   */
  stackTrace?: string;
  /**
   * 参数对象
   */
  paramsObj?: string;
  /**
   * 异常时间
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
  /**
   * 
   */
  createdUserId?: number;
  /**
   * 
   */
  createdUserName?: string;
}

