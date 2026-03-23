import { EmployeeStatusEnum } from "@/api/enums/EmployeeStatusEnum";
import { GenderEnum } from "@/api/enums/GenderEnum";
import { NationEnum } from "@/api/enums/NationEnum";
import { EducationLevelEnum } from "@/api/enums/EducationLevelEnum";
import { PoliticalStatusEnum } from "@/api/enums/PoliticalStatusEnum";
import { AcademicQualificationsEnum } from "@/api/enums/AcademicQualificationsEnum";
import { AcademicSystemEnum } from "@/api/enums/AcademicSystemEnum";
import { DegreeEnum } from "@/api/enums/DegreeEnum";
import { CommonStatusEnum } from "@/api/enums/CommonStatusEnum";

/**
 * Fast.Admin.Service.Employee.Dto.QueryEmployeePagedOutput 获取职员分页列表输出
 */
export interface QueryEmployeePagedOutput {
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
   * 手机
   */
  mobile?: string;
  /**
   * 
   */
  status?: EmployeeStatusEnum;
  /**
   * 邮箱
   */
  email?: string;
  /**
   * 
   */
  sex?: GenderEnum;
  /**
   * 证件照
   */
  idPhoto?: string;
  /**
   * 入职日期
   */
  entryDate?: Date;
  /**
   * 离职日期
   */
  resignDate?: Date;
  /**
   * 
   */
  nation?: NationEnum;
  /**
   * 籍贯
   */
  nativePlace?: string;
  /**
   * 生日
   */
  birthday?: Date;
  /**
   * 
   */
  educationLevel?: EducationLevelEnum;
  /**
   * 
   */
  politicalStatus?: PoliticalStatusEnum;
  /**
   * 毕业学院
   */
  graduationCollege?: string;
  /**
   * 
   */
  academicQualifications?: AcademicQualificationsEnum;
  /**
   * 
   */
  academicSystem?: AcademicSystemEnum;
  /**
   * 
   */
  degree?: DegreeEnum;
  /**
   * 备注
   */
  remark?: string;
  /**
   * 创建者用户名称
   */
  createdUserName?: string;
  /**
   * 创建时间
   */
  createdTime?: Date;
  /**
   * 更新者用户名称
   */
  updatedUserName?: string;
  /**
   * 更新时间
   */
  updatedTime?: Date;
  /**
   * 更新版本控制字段
   */
  rowVersion?: number;
  /**
   * 机构Id
   */
  orgId?: number;
  /**
   * 机构名称
   */
  orgName?: string;
  /**
   * 机构名称
   */
  orgNames?: Array<string>;
  /**
   * 部门Id
   */
  departmentId?: number;
  /**
   * 部门名称
   */
  departmentName?: string;
  /**
   * 部门名称
   */
  departmentNames?: Array<string>;
  /**
   * 职位Id
   */
  positionId?: number;
  /**
   * 职位名称
   */
  positionName?: string;
  /**
   * 职级Id
   */
  jobLevelId?: number;
  /**
   * 职级名称
   */
  jobLevelName?: string;
  /**
   * 是否为负责人
   */
  isPrincipal?: boolean;
  /**
   * 角色名称
   */
  roleNames?: string;
  /**
   * 
   */
  accountStatus?: CommonStatusEnum;
  /**
   * 账号手机
   */
  accountMobile?: string;
  /**
   * 账号昵称
   */
  accountNickName?: string;
  /**
   * 最后登录时间
   */
  lastLoginTime?: Date;
}

