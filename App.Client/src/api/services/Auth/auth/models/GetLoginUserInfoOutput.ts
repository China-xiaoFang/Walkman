import { RoleTypeEnum } from "@/api/enums/RoleTypeEnum";
import { DataScopeTypeEnum } from "@/api/enums/DataScopeTypeEnum";
import { AuthMenuInfoDto } from "./AuthMenuInfoDto";

/**
 * Fast.Admin.Service.Auth.Dto.GetLoginUserInfoOutput 获取登录用户信息输出
 */
export interface GetLoginUserInfoOutput {
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
   * 角色名称集合
   */
  roleNameList?: Array<string>;
  /**
   * 
   */
  roleType?: RoleTypeEnum;
  /**
   * 
   */
  dataScopeType?: DataScopeTypeEnum;
  /**
   * 按钮编码集合
   */
  buttonCodeList?: Array<string>;
  /**
   * 菜单集合
   */
  menuList?: Array<AuthMenuInfoDto>;
}

