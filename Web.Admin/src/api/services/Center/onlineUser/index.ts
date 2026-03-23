import { axiosUtil } from "@fast-china/axios";
import { PagedResult } from "fast-element-plus";
import { OnlineUserModel } from "./models/OnlineUserModel";
import { QueryTenantOnlineUserPagedInput } from "./models/QueryTenantOnlineUserPagedInput";
import { ForceOfflineInput } from "./models/ForceOfflineInput";

/**
 * Fast.Admin.Service.OnlineUser.OnlineUserService 在线用户服务Api
 */
export const onlineUserApi = {
  /**
   * 获取在线用户分页列表
   */
  queryTenantOnlineUserPaged(data: QueryTenantOnlineUserPagedInput) {
    return axiosUtil.request<PagedResult<OnlineUserModel>>({
      url: "/onlineUser/queryTenantOnlineUserPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
  /**
   * 强制下线
   */
  forceOffline(data: ForceOfflineInput) {
    return axiosUtil.request({
      url: "/onlineUser/forceOffline",
      method: "post",
      data,
      requestType: "query",
    });
  },
};
