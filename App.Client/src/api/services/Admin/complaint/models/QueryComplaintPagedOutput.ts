import { ComplaintTypeEnum } from "@/api/enums/ComplaintTypeEnum";

/**
 * Fast.Admin.Service.Complaint.Dto.QueryComplaintPagedOutput 获取投诉分页列表输出
 */
export interface QueryComplaintPagedOutput {
  /**
   * 投诉Id
   */
  complaintId?: number;
  /**
   * 应用标识
   */
  openId?: string;
  /**
   * 昵称
   */
  nickName?: string;
  /**
   * 
   */
  complaintType?: ComplaintTypeEnum;
  /**
   * 手机
   */
  mobile?: string;
  /**
   * 联系电话
   */
  contactPhone?: string;
  /**
   * 联系邮箱
   */
  contactEmail?: string;
  /**
   * 投诉描述
   */
  description?: string;
  /**
   * 附件图片
   */
  attachmentImages?: Array<string>;
  /**
   * 处理时间
   */
  handleTime?: Date;
  /**
   * 处理描述
   */
  handleDescription?: string;
  /**
   * 备注
   */
  remark?: string;
  /**
   * 创建时间
   */
  createdTime?: Date;
  /**
   * 更新版本控制字段
   */
  rowVersion?: number;
}

