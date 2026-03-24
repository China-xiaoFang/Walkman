import { axiosUtil } from "@fast-china/axios";
import { ApiInfoModel } from "./models/ApiInfoModel";

/**
 * Fast.Admin.Service.Api.ApiService ApiApi
 */
export const apiApi = {
  /**
   * 获取接口分页列表
   */
  queryApiPaged(data: PagedInput) {
    return axiosUtil.request<PagedResult<ApiInfoModel>>({
      url: "/api/queryApiPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
};
