/**
 * Fast.AdminLog.Entity.SqlExceptionLogModel Sql异常日志Model类
 */
export interface SqlExceptionLogModel {
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
   * 文件名称
   */
  fileName?: string;
  /**
   * 文件行数
   */
  fileLine?: number;
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
   * 纯Sql，参数化之后的Sql
   */
  pureSql?: string;
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

