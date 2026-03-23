/**
 * Fast.Admin.Service.Config.Dto.QueryConfigPagedOutput 获取配置分页列表输出
 */
export interface QueryConfigPagedOutput {
  /**
   * 配置Id
   */
  configId?: number;
  /**
   * 配置编码
   */
  configCode?: string;
  /**
   * 配置名称
   */
  configName?: string;
  /**
   * 配置值
   */
  configValue?: string;
  /**
   * 备注
   */
  remark?: string;
  /**
   * 
   */
  departmentName?: string;
  /**
   * 
   */
  createdUserName?: string;
  /**
   * 
   */
  createdTime?: Date;
  /**
   * 
   */
  updatedUserName?: string;
  /**
   * 
   */
  updatedTime?: Date;
  /**
   * 
   */
  rowVersion?: number;
}

