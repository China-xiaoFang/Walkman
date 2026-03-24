import { DiffLogTypeEnum } from "@/api/enums/DiffLogTypeEnum";

/**
 * Fast.AdminLog.Entity.SqlDiffLogModel Sql差异日志Model类
 */
export interface SqlDiffLogModel {
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
   * 
   */
  diffType?: DiffLogTypeEnum;
  /**
   * 表名称
   */
  tableName?: string;
  /**
   * 表描述
   */
  tableDescription?: string;
  /**
   * 旧的列信息
   */
  beforeColumnList?: Array<any>;
  /**
   * 新的列信息
   */
  afterColumnList?: Array<any>;
  /**
   * 执行秒数
   */
  executeSeconds?: number;
  /**
   * 纯Sql，参数化之后的Sql
   */
  pureSql?: string;
  /**
   * 差异时间
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

