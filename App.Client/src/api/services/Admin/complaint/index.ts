import { axiosUtil } from "@fast-china/axios";
import { QueryComplaintPagedOutput } from "./models/QueryComplaintPagedOutput";
import { QueryComplaintPagedInput } from "./models/QueryComplaintPagedInput";
import { AddComplaintInput } from "./models/AddComplaintInput";
import { HandleComplaintInput } from "./models/HandleComplaintInput";

/**
 * Fast.Admin.Service.Complaint.ComplaintService 投诉服务Api
 */
export const complaintApi = {
  /**
   * 获取投诉工单分页列表
   */
  queryComplaintPaged(data: QueryComplaintPagedInput) {
    return axiosUtil.request<PagedResult<QueryComplaintPagedOutput>>({
      url: "/complaint/queryComplaintPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
  /**
   * 获取投诉详情
   */
  queryComplaintDetail(complaintId: number) {
    return axiosUtil.request<QueryComplaintPagedOutput>({
      url: "/complaint/queryComplaintDetail",
      method: "get",
      params: {
        complaintId,
      },
      requestType: "query",
    });
  },
  /**
   * 添加投诉
   */
  addComplaint(data: AddComplaintInput) {
    return axiosUtil.request({
      url: "/complaint/addComplaint",
      method: "post",
      data,
      requestType: "add",
    });
  },
  /**
   * 处理投诉
   */
  handleComplaint(data: HandleComplaintInput) {
    return axiosUtil.request({
      url: "/complaint/handleComplaint",
      method: "post",
      data,
      requestType: "edit",
    });
  },
};
