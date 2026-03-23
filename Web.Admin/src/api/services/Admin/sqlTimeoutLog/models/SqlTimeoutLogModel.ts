/**
 * Fast.AdminLog.Entity.SqlTimeoutLogModel Sql超时日志Model类
 */
export interface SqlTimeoutLogModel {
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
   * 超时秒数
   */
  timeoutSeconds?: number;
  /**
   * 纯Sql，参数化之后的Sql
   */
  pureSql?: string;
  /**
   * 超时时间
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

