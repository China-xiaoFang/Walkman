import { defineStore } from "pinia";
import { reactive, ref, toRefs } from "vue";
import { Local, decodeBase64Url, logger } from "@fast-china/utils";
import { DataScopeTypeEnum } from "@/api/enums/DataScopeTypeEnum";
import { LoginStatusEnum } from "@/api/enums/LoginStatusEnum";
import { authApi } from "@/api/services/Auth/auth";
import { loginApi } from "@/api/services/Auth/login";
import { CommonRoute } from "@/common";
import { useMessageBox, useToast } from "@/hooks";
import { closeWebSocket } from "@/signalR";
import { useApp } from "../app";
import type { AxiosResponse } from "axios";
import type { GetLoginUserInfoOutput } from "@/api/services/Auth/auth/models/GetLoginUserInfoOutput";
import type { LoginOutput } from "@/api/services/Auth/login/models/LoginOutput";

type IState = {
	/** Token */
	token: string;
	/** Refresh Token */
	refreshToken: string;
	/** 激活的 TabBar 页面 */
	activeTabBar: string;
};

export const useUserInfo = defineStore(
	"userInfo",
	() => {
		const state = reactive<IState & Required<GetLoginUserInfoOutput>>({
			token: "",
			refreshToken: "",
			activeTabBar: CommonRoute.Workbench,
			accountId: undefined,
			accountKey: "",
			mobile: "",
			nickName: "",
			avatar: "",
			identityVerification: false,
			tenantNo: "",
			tenantName: "",
			shortName: "",
			tenantCode: "",
			logoUrl: "",
			userKey: "",
			employeeId: undefined,
			employeeNo: "",
			employeeName: "",
			departmentId: null,
			departmentName: "",
			isSuperAdmin: false,
			isAdmin: false,
			roleNameList: [],
			buttonCodeList: [],
			roleType: null,
			dataScopeType: DataScopeTypeEnum.All,
			menuList: [],
		});

		/** 是否存在用户信息 */
		const hasUserInfo = ref(false);

		/** WebSocket 是否连接 */
		const hasWebSocket = ref(false);

		/** 底部导航配置，后续可根据用户权限动态调整 */
		const tabBars = reactive<ITabBar[]>([
			{ path: CommonRoute.Home, icon: "home", title: "首页" },
			{ path: CommonRoute.Workbench, icon: "workbench", title: "工作台" },
			{ path: CommonRoute.My, icon: "my", title: "我的" },
		]);

		/** 设置用户信息 */
		const setUserInfo = (uInfo: GetLoginUserInfoOutput) => {
			Object.assign(state, uInfo);
		};

		/** 删除 Token */
		const removeToken = () => {
			state.token = "";
			state.refreshToken = "";
			// 删除Token后，不用校验账号
			state.identityVerification = false;
		};

		/** 设置 Token */
		const setToken = (axiosResponse: AxiosResponse) => {
			if (!axiosResponse) return;
			// 从请求头部中获取 Token
			const token = (axiosResponse.headers.get as (headerName: string) => string)("access-token");
			// 从请求头部中获取 Refresh Token
			const refreshToken = (axiosResponse.headers.get as (headerName: string) => string)("x-access-token");
			// 判断是否为无效 Token
			if (token === "invalid_token") {
				// 删除 Token
				removeToken();
			} else if (token && refreshToken && refreshToken !== "invalid_token") {
				// 设置 Token
				state.token = token;
				state.refreshToken = refreshToken;
			}
		};

		/**
		 * 获取 Token
		 * @description 从缓存中获取
		 */
		const getToken = () => {
			return { token: state.token, refreshToken: state.refreshToken };
		};

		/**
		 * 解析Token
		 * @description 如果Token过期，会解析不出来
		 * @param token 可以传入，也可以直接获取 pinia 中的
		 * @param refreshToken 可以传入，也可以直接获取 pinia 中的
		 */
		const resolveToken = (token: string = null, refreshToken: string = null) => {
			token ??= state.token;
			refreshToken ??= state.refreshToken;
			if (token) {
				const jwtToken = decodeURIComponent(encodeURIComponent(decodeBase64Url(token.replace(/_/g, "/").replace(/-/g, "+").split(".")[1])));
				const jwtTokenData = JSON.parse(jwtToken) as { exp?: number };
				// 获取 Token 的过期时间
				const expired = Date.now() >= jwtTokenData.exp * 1000;
				if (expired) {
					return { token: `Bearer ${token}`, refreshToken: `Bearer ${refreshToken}`, tokenData: jwtTokenData };
				}
				return { token: `Bearer ${token}`, refreshToken: null, tokenData: jwtTokenData };
			}
			return { token: null, refreshToken: null, tokenData: null };
		};

		/** 登录 */
		const login = (loginRes: LoginOutput) => {
			if (!loginRes || loginRes.status !== LoginStatusEnum.Success) return;
			// 优先缓存一些数据
			state.accountKey = loginRes.accountKey;
			state.nickName = loginRes.nickName;
			state.avatar = loginRes.avatar;
			const userRes = loginRes.tenantList[0];
			state.userKey = userRes.userKey;
			state.tenantName = userRes.tenantName;
			state.employeeNo = userRes.employeeNo;
			state.employeeName = userRes.employeeName;
			useToast.success("登录成功");
			// 确保 getLoginUser 获取用户信息
			hasUserInfo.value = false;
			// 跳转到工作台
			uni.switchTab({ url: CommonRoute.Workbench });
		};

		const logoutClear = () => {
			removeToken();
			// 删除 HTTP 缓存数据
			Local.removeByPrefix("HTTP_CACHE_");

			const currentPage = getCurrentPages().at(-1) as { route?: string; $page?: { fullPath?: string } };
			const currentPath = currentPage?.route ? `/${currentPage.route.replace(/^\/+/, "")}` : "";
			// 排除登录页面报错的问题
			if (currentPath === CommonRoute.Login) {
				uni.navigateTo({ url: CommonRoute.Login });
			} else {
				const fullPath = currentPage?.$page?.fullPath || currentPath;
				const redirect = fullPath ? `?redirect=${encodeURIComponent(fullPath)}` : "";
				uni.reLaunch({ url: `${CommonRoute.Login}${redirect}` });
			}
		};

		/** 退出登录 */
		const logout = async (
			data: {
				/**
				 * 1 强制下线
				 * 2 其他地方登录
				 */
				type: 1 | 2;
				message: string;
			} = null
		) => {
			if (data?.type === 2) {
				useMessageBox.alert(data?.message);
				try {
					// 关闭 WebSocket 连接
					closeWebSocket();
				} catch (error) {
					logger.error("Logout", error);
				}
				logoutClear();
			} else {
				try {
					// 关闭 WebSocket 连接
					closeWebSocket();
				} catch (error) {
					logger.error("Logout", error);
				}
				try {
					await loginApi.logout();
				} catch (error) {
					logger.error("Logout", error);
				} finally {
					logoutClear();
					if (data !== null) {
						useMessageBox.alert(data?.message);
					}
				}
			}
		};

		/** 刷新用户信息 */
		const refreshUserInfo = async () => {
			const apiRes = await authApi.getLoginUserInfo();
			setUserInfo(apiRes);
		};

		/** 刷新应用 */
		const refreshApp = async () => {
			// 删除 HTTP 缓存数据
			Local.removeByPrefix("HTTP_CACHE_");

			// 刷新用户信息
			await refreshUserInfo();

			// 刷新字典
			await useApp().setDictionary();
		};

		/** 切换登录 @description 调用此方法下次才不会 tryLogin */
		const switchLogin = () => {
			// 确保下次会自动刷新用户信息
			hasUserInfo.value = false;
			logoutClear();
		};

		/** 获取微信Code */
		const getWeChatCode = () =>
			new Promise<string>((resolve) => {
				// #ifdef MP-WEIXIN
				uni.login({
					success: ({ code }) => resolve(code),
					fail: () => {
						useToast.warning("授权失败，无法获取您的信息。请重新授权以继续使用我们的服务。");
						resolve("");
					},
				});
				// #endif

				// #ifndef MP-WEIXIN
				resolve("");
				// #endif
			});

		return {
			...toRefs(state),
			hasUserInfo,
			hasWebSocket,
			tabBars,
			setUserInfo,
			removeToken,
			setToken,
			getToken,
			resolveToken,
			login,
			logout,
			logoutClear,
			refreshUserInfo,
			refreshApp,
			switchLogin,
			getWeChatCode,
		};
	},
	{
		persist: {
			key: "store-user-info",
			// 这里是配置 pinia 只需要持久化 state 中的 Token 即可，而不是整个 store
			pick: [
				"token",
				"refreshToken",
				"accountKey",
				"nickName",
				"avatar",
				"tenantName",
				"userKey",
				"employeeNo",
				"employeeName",
				"departmentName",
				"isSuperAdmin",
				"isAdmin",
			],
		},
	}
);
