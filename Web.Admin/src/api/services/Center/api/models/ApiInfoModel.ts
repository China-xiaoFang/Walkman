import { HttpRequestMethodEnum } from "@/api/enums/HttpRequestMethodEnum";
import { HttpRequestActionEnum } from "@/api/enums/HttpRequestActionEnum";

/**
 * Fast.Admin.Entity.ApiInfoModel 接口信息表Model类
 */
export interface ApiInfoModel {
  /**
   * 接口Id
   */
  apiId?: number;
  /**
   * 服务名称
   */
  serviceName?: string;
  /**
   * 分组名称
   */
  groupName?: string;
  /**
   * 分组标题
   */
  groupTitle?: string;
  /**
   * 版本
   */
  version?: string;
  /**
   * 分组描述
   */
  description?: string;
  /**
   * 模块名称
   */
  moduleName?: string;
  /**
   * 接口地址
   */
  apiUrl?: string;
  /**
   * 接口名称
   */
  apiName?: string;
  /**
   * 
   */
  method?: HttpRequestMethodEnum;
  /**
   * 
   */
  action?: HttpRequestActionEnum;
  /**
   * 是否检测授权
   */
  hasAuth?: boolean;
  /**
   * 是否检测权限
   */
  hasPermission?: boolean;
  /**
   * 标签
   */
  tags?: Array<string>;
  /**
   * 排序
   */
  sort?: number;
  /**
   * 创建时间
   */
  createdTime?: Date;
  /**
   * 更新时间
   */
  updatedTime?: Date;
}

