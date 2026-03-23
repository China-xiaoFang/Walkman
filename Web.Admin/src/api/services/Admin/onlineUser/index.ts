import { axiosUtil } from "@fast-china/axios";
import { PagedResult } from "fast-element-plus";
import { OnlineUserModel } from "./models/OnlineUserModel";
import { QueryOnlineUserPagedInput } from "./models/QueryOnlineUserPagedInput";
import { ForceOfflineInput } from "./models/ForceOfflineInput";

/**
 * Fast.Admin.Service.OnlineUser.OnlineUserService 在线用户服务Api
 */
export const onlineUserApi = {
  /**
   * 获取在线用户分页列表
   */
  queryOnlineUserPaged(data: QueryOnlineUserPagedInput) {
    return axiosUtil.request<PagedResult<OnlineUserModel>>({
      url: "/onlineUser/queryOnlineUserPaged",
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
