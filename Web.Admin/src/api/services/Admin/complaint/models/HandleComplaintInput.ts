/**
 * Fast.Admin.Service.Complaint.Dto.HandleComplaintInput 处理投诉输入
 */
export interface HandleComplaintInput {
  /**
   * 投诉Id
   */
  complaintId?: number;
  /**
   * 处理描述
   */
  handleDescription?: string;
  /**
   * 备注
   */
  remark?: string;
  /**
   * 
   */
  rowVersion?: number;
}

