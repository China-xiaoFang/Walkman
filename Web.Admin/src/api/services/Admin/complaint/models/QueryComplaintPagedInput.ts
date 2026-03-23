import { PagedInput } from "fast-element-plus";
import { ComplaintTypeEnum } from "@/api/enums/ComplaintTypeEnum";

/**
 * Fast.Admin.Service.Complaint.Dto.QueryComplaintPagedInput 获取投诉分页列表输入
 */
export interface QueryComplaintPagedInput extends PagedInput  {
  /**
   * 
   */
  complaintType?: ComplaintTypeEnum;
  /**
   * 
   */
  readonly isOrderBy?: boolean;
}

