import { GenderEnum } from "@/api/enums/GenderEnum";
import { NationEnum } from "@/api/enums/NationEnum";
import { IdTypeEnum } from "@/api/enums/IdTypeEnum";
import { EducationLevelEnum } from "@/api/enums/EducationLevelEnum";
import { PoliticalStatusEnum } from "@/api/enums/PoliticalStatusEnum";
import { AcademicQualificationsEnum } from "@/api/enums/AcademicQualificationsEnum";
import { AcademicSystemEnum } from "@/api/enums/AcademicSystemEnum";
import { DegreeEnum } from "@/api/enums/DegreeEnum";
import { EmployeeRoleModel } from "./EmployeeRoleModel";

/**
 * Fast.Admin.Service.Employee.Dto.AddEmployeeInput 添加职员输入
 */
export interface AddEmployeeInput {
  /**
   * 姓名
   */
  employeeName?: string;
  /**
   * 手机
   */
  mobile?: string;
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
   * 机构Id
   */
  orgId?: number;
  /**
   * 机构名称
   */
  orgName?: string;
  /**
   * 部门Id
   */
  departmentId?: number;
  /**
   * 部门名称
   */
  departmentName?: string;
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
   * 角色信息
   */
  roleList?: Array<EmployeeRoleModel>;
}

