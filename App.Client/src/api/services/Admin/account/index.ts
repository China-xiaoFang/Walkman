import { axiosUtil } from "@fast-china/axios";
import { QueryAccountPagedOutput } from "./models/QueryAccountPagedOutput";
import { QueryAccountPagedInput } from "./models/QueryAccountPagedInput";
import { QueryAccountDetailOutput } from "./models/QueryAccountDetailOutput";
import { EditAccountInput } from "./models/EditAccountInput";
import { ChangePasswordInput } from "./models/ChangePasswordInput";
import { AccountIdInput } from "./models/AccountIdInput";

/**
 * Fast.Admin.Service.Account.AccountService 账号服务Api
 */
export const accountApi = {
  /**
   * 账号选择器
   */
  accountSelector(data: PagedInput) {
    return axiosUtil.request<PagedResult<ElSelectorOutput<number>>>({
      url: "/account/accountSelector",
      method: "post",
      data,
      requestType: "query",
    });
  },
  /**
   * 获取账号分页列表
   */
  queryAccountPaged(data: QueryAccountPagedInput) {
    return axiosUtil.request<PagedResult<QueryAccountPagedOutput>>({
      url: "/account/queryAccountPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
  /**
   * 获取账号详情
   */
  queryAccountDetail(accountId: number) {
    return axiosUtil.request<QueryAccountDetailOutput>({
      url: "/account/queryAccountDetail",
      method: "get",
      params: {
        accountId,
      },
      requestType: "query",
    });
  },
  /**
   * 获取编辑账号详情
   */
  queryEditAccountDetail() {
    return axiosUtil.request<EditAccountInput>({
      url: "/account/queryEditAccountDetail",
      method: "get",
      requestType: "query",
    });
  },
  /**
   * 编辑账号
   */
  editAccount(data: EditAccountInput) {
    return axiosUtil.request({
      url: "/account/editAccount",
      method: "post",
      data,
      requestType: "edit",
    });
  },
  /**
   * 账号修改密码
   */
  changePassword(data: ChangePasswordInput) {
    return axiosUtil.request({
      url: "/account/changePassword",
      method: "post",
      data,
      requestType: "edit",
    });
  },
  /**
   * 账号解除锁定
   */
  unlock(data: AccountIdInput) {
    return axiosUtil.request({
      url: "/account/unlock",
      method: "post",
      data,
      requestType: "edit",
    });
  },
  /**
   * 账号重置密码
   */
  resetPassword(data: AccountIdInput) {
    return axiosUtil.request({
      url: "/account/resetPassword",
      method: "post",
      data,
      requestType: "edit",
    });
  },
  /**
   * 账号更改状态
   */
  changeStatus(data: AccountIdInput) {
    return axiosUtil.request({
      url: "/account/changeStatus",
      method: "post",
      data,
      requestType: "edit",
    });
  },
};
