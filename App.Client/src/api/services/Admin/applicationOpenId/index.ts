import { axiosUtil } from "@fast-china/axios";
import { QueryApplicationOpenIdPagedOutput } from "./models/QueryApplicationOpenIdPagedOutput";
import { QueryApplicationOpenIdPagedInput } from "./models/QueryApplicationOpenIdPagedInput";
import { QueryApplicationOpenIdDetailOutput } from "./models/QueryApplicationOpenIdDetailOutput";
import { AddApplicationOpenIdInput } from "./models/AddApplicationOpenIdInput";
import { EditApplicationOpenIdInput } from "./models/EditApplicationOpenIdInput";
import { RecordIdInput } from "./models/RecordIdInput";

/**
 * Fast.Admin.Service.ApplicationOpenId.ApplicationOpenIdService 应用标识服务Api
 */
export const applicationOpenIdApi = {
  /**
   * 获取应用标识分页列表
   */
  queryApplicationOpenIdPaged(data: QueryApplicationOpenIdPagedInput) {
    return axiosUtil.request<PagedResult<QueryApplicationOpenIdPagedOutput>>({
      url: "/applicationOpenId/queryApplicationOpenIdPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
  /**
   * 获取应用标识详情
   */
  queryApplicationOpenIdDetail(recordId: number) {
    return axiosUtil.request<QueryApplicationOpenIdDetailOutput>({
      url: "/applicationOpenId/queryApplicationOpenIdDetail",
      method: "get",
      params: {
        recordId,
      },
      requestType: "query",
    });
  },
  /**
   * 添加应用标识
   */
  addApplicationOpenId(data: AddApplicationOpenIdInput) {
    return axiosUtil.request({
      url: "/applicationOpenId/addApplicationOpenId",
      method: "post",
      data,
      requestType: "add",
    });
  },
  /**
   * 编辑应用标识
   */
  editApplicationOpenId(data: EditApplicationOpenIdInput) {
    return axiosUtil.request({
      url: "/applicationOpenId/editApplicationOpenId",
      method: "post",
      data,
      requestType: "edit",
    });
  },
  /**
   * 删除应用标识
   */
  deleteApplicationOpenId(data: RecordIdInput) {
    return axiosUtil.request({
      url: "/applicationOpenId/deleteApplicationOpenId",
      method: "post",
      data,
      requestType: "delete",
    });
  },
};
