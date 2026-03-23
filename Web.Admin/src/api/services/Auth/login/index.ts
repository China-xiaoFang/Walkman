import { axiosUtil } from "@fast-china/axios";
import { LoginOutput } from "./models/LoginOutput";
import { LoginInput } from "./models/LoginInput";
import { ClientRegisterInput } from "./models/ClientRegisterInput";
import { ClientLoginOutput } from "./models/ClientLoginOutput";
import { WeChatClientLoginInput } from "./models/WeChatClientLoginInput";

/**
 * Fast.Admin.Service.Login.LoginService 登录服务Api
 */
export const loginApi = {
  /**
   * 登录
   */
  login(data: LoginInput) {
    return axiosUtil.request<LoginOutput>({
      url: "/login",
      method: "post",
      data,
      requestType: "auth",
    });
  },
  /**
   * 客户端注册
   */
  clientRegister(data: ClientRegisterInput) {
    return axiosUtil.request({
      url: "/clientRegister",
      method: "post",
      data,
      requestType: "auth",
    });
  },
  /**
   * 客户端登录
   */
  clientLogin(data: LoginInput) {
    return axiosUtil.request<ClientLoginOutput>({
      url: "/clientLogin",
      method: "post",
      data,
      requestType: "auth",
    });
  },
  /**
   * 微信客户端登录
   */
  weChatClientLogin(data: WeChatClientLoginInput) {
    return axiosUtil.request<ClientLoginOutput>({
      url: "/weChatClientLogin",
      method: "post",
      data,
      requestType: "auth",
    });
  },
  /**
   * 退出登录
   */
  logout() {
    return axiosUtil.request({
      url: "/logout",
      method: "post",
      requestType: "auth",
    });
  },
};
