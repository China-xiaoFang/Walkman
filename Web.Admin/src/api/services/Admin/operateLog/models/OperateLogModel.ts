import { OperateLogTypeEnum } from "@/api/enums/OperateLogTypeEnum";

/**
 * Fast.AdminLog.Entity.OperateLogModel 操作日志Model类
 */
export interface OperateLogModel {
  /**
   * 记录Id
   */
  recordId?: number;
  /**
   * 工号
   */
  employeeNo?: string;
  /**
   * 手机
   */
  mobile?: string;
  /**
   * 标题
   */
  title?: string;
  /**
   * 
   */
  operateType?: OperateLogTypeEnum;
  /**
   * 业务Id
   */
  bizId?: number;
  /**
   * 业务编码
   */
  bizNo?: string;
  /**
   * 描述
   */
  description?: string;
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

