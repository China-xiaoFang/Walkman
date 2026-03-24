import { axiosUtil } from "@fast-china/axios";
import { QueryWeChatUserPagedOutput } from "./models/QueryWeChatUserPagedOutput";
import { QueryWeChatUserPagedInput } from "./models/QueryWeChatUserPagedInput";
import { QueryWeChatUserDetailOutput } from "./models/QueryWeChatUserDetailOutput";
import { EditWeChatUserInput } from "./models/EditWeChatUserInput";
import { WeChatCode2SessionOutput } from "./models/WeChatCode2SessionOutput";
import { WeChatCode2SessionInput } from "./models/WeChatCode2SessionInput";
import { WeChatCode2PhoneNumberOutput } from "./models/WeChatCode2PhoneNumberOutput";
import { WeChatCode2PhoneNumberInput } from "./models/WeChatCode2PhoneNumberInput";

/**
 * Fast.Admin.Service.WeChat.WeChatService 微信服务Api
 */
export const weChatApi = {
  /**
   * 获取微信用户分页列表
   */
  queryWeChatUserPaged(data: QueryWeChatUserPagedInput) {
    return axiosUtil.request<PagedResult<QueryWeChatUserPagedOutput>>({
      url: "/weChat/queryWeChatUserPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
  /**
   * 获取微信用户详情
   */
  queryWeChatUserDetail() {
    return axiosUtil.request<QueryWeChatUserDetailOutput>({
      url: "/weChat/queryWeChatUserDetail",
      method: "get",
      requestType: "query",
    });
  },
  /**
   * 编辑微信用户
   */
  editWeChatUser(data: EditWeChatUserInput) {
    return axiosUtil.request({
      url: "/weChat/editWeChatUser",
      method: "post",
      data,
      requestType: "edit",
    });
  },
  /**
   * 换取微信用户身份信息
   */
  weChatCode2Session(data: WeChatCode2SessionInput) {
    return axiosUtil.request<WeChatCode2SessionOutput>({
      url: "/weChat/weChatCode2Session",
      method: "post",
      data,
      requestType: "auth",
    });
  },
  /**
   * 换取微信用户手机号
   */
  weChatCode2PhoneNumber(data: WeChatCode2PhoneNumberInput) {
    return axiosUtil.request<WeChatCode2PhoneNumberOutput>({
      url: "/weChat/weChatCode2PhoneNumber",
      method: "post",
      data,
      requestType: "auth",
    });
  },
};
