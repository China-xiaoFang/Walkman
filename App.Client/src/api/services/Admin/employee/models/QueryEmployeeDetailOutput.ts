import { EmployeeStatusEnum } from "@/api/enums/EmployeeStatusEnum";
import { GenderEnum } from "@/api/enums/GenderEnum";
import { NationEnum } from "@/api/enums/NationEnum";
import { IdTypeEnum } from "@/api/enums/IdTypeEnum";
import { EducationLevelEnum } from "@/api/enums/EducationLevelEnum";
import { PoliticalStatusEnum } from "@/api/enums/PoliticalStatusEnum";
import { AcademicQualificationsEnum } from "@/api/enums/AcademicQualificationsEnum";
import { AcademicSystemEnum } from "@/api/enums/AcademicSystemEnum";
import { DegreeEnum } from "@/api/enums/DegreeEnum";
import { EmployeeOrgModel } from "./EmployeeOrgModel";
import { EmployeeRoleModel } from "./EmployeeRoleModel";

/**
 * Fast.Admin.Service.Employee.Dto.QueryEmployeeDetailOutput 获取职员详情输出
 */
export interface QueryEmployeeDetailOutput {
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
   * 离职原因
   */
  resignReason?: string;
  /**
   * 
   */
  nation?: NationEnum;
  /**
   * 籍贯
   */
  nativePlace?: string;
  /**
   * 家庭地址
   */
  familyAddress?: string;
  /**
   * 通信地址
   */
  mailingAddress?: string;
  /**
   * 生日
   */
  birthday?: Date;
  /**
   * 
   */
  idType?: IdTypeEnum;
  /**
   * 证件号码
   */
  idNumber?: string;
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
   * 家庭电话
   */
  familyPhone?: string;
  /**
   * 办公电话
   */
  officePhone?: string;
  /**
   * 紧急联系人
   */
  emergencyContact?: string;
  /**
   * 紧急联系电话
   */
  emergencyPhone?: string;
  /**
   * 紧急联系地址
   */
  emergencyAddress?: string;
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
   * 机构信息
   */
  orgList?: Array<EmployeeOrgModel>;
  /**
   * 角色信息
   */
  roleList?: Array<EmployeeRoleModel>;
}

