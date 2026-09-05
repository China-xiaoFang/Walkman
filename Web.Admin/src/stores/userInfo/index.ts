import { defineStore } from "pinia";
import { reactive, ref, toRefs } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { Local, logger } from "@fast-china/utils";
import { DataScopeTypeEnum } from "@/api/enums/DataScopeTypeEnum";
import { authApi } from "@/api/services/Auth/auth";
import { loginApi } from "@/api/services/Auth/login";
import router from "@/router";
import { closeWebSocket } from "@/signalR";
import type { AxiosResponse } from "axios";
import type { GetLoginUserInfoOutput } from "@/api/services/Auth/auth/models/GetLoginUserInfoOutput";

type IState = {
	/** Token */
	token: string;
	/** Refresh Token */
	refreshToken: string;
};

export const useUserInfo = defineStore(
	"userInfo",
	() => {
		const state = reactive<IState & Required<GetLoginUserInfoOutput>>({
			token: "",
			refreshToken: "",
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

		/** 动态生成路由 */
		const asyncRouterGen = ref(false);

		/** WebSocket 是否连接 */
		const hasWebSocket = ref(false);

		/** 设置用户信息 */
		const setUserInfo = (uInfo: GetLoginUserInfoOutput): void => {
			Object.assign(state, uInfo);
		};

		/** 删除 Token */
		const removeToken = (): void => {
			state.token = "";
			state.refreshToken = "";
			// 删除Token后，不用校验账号
			state.identityVerification = false;
		};

		/** 设置 Token */
		const setToken = (axiosResponse: AxiosResponse): void => {
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
		const getToken = (): { token: string; refreshToken: string } => {
			return { token: state.token, refreshToken: state.refreshToken };
		};

		/**
		 * 解析Token
		 * @description 如果Token过期，会解析不出来
		 * @param token 可以传入，也可以直接获取 pinia 中的
		 * @param refreshToken 可以传入，也可以直接获取 pinia 中的
		 */
		const resolveToken = (
			token: string = null,
			refreshToken: string = null
		): {
			token: string;
			refreshToken: string;
			tokenData: unknown;
		} => {
			token ??= state.token;
			refreshToken ??= state.refreshToken;
			if (token) {
				const jwtToken = decodeURIComponent(encodeURIComponent(window.atob(token.replace(/_/g, "/").replace(/-/g, "+").split(".")[1])));
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
		const login = (): void => {
			ElMessage.success("登录成功");
			// 确保 getLoginUser 获取用户信息
			asyncRouterGen.value = false;
			// 进入系统
			void router.push("/");
		};

		const logoutClear = (): void => {
			removeToken();
			// 删除 HTTP 缓存数据
			Local.removeByPrefix("HTTP_CACHE_");
			// 排除登录页面报错的问题
			if (router.currentRoute.value.path === "/login") {
				void router.push({ path: "/login" });
			} else {
				void router.push({ path: "/login", query: { redirect: encodeURIComponent(router.currentRoute.value.fullPath) } });
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
		): Promise<void> => {
			if (data?.type === 2) {
				void ElMessageBox.alert(data?.message, {
					type: "warning",
				});
				try {
					// 关闭 WebSocket 连接
					await closeWebSocket();
				} catch (error) {
					logger.error("Logout", "发生异常", error);
				}
				logoutClear();
			} else {
				try {
					// 关闭 WebSocket 连接
					await closeWebSocket();
				} catch (error) {
					logger.error("Logout", "发生异常", error);
				}
				try {
					await loginApi.logout();
				} catch (error) {
					logger.error("Logout", "发生异常", error);
				} finally {
					logoutClear();
					if (data !== null) {
						void ElMessageBox.alert(data?.message, {
							type: "warning",
						});
					}
				}
			}
		};

		/** 刷新用户信息 */
		const refreshUserInfo = async (): Promise<void> => {
			const apiRes = await authApi.getLoginUserInfo();
			setUserInfo(apiRes);
		};

		return {
			...toRefs(state),
			asyncRouterGen,
			hasWebSocket,
			setUserInfo,
			removeToken,
			setToken,
			getToken,
			resolveToken,
			login,
			logout,
			logoutClear,
			refreshUserInfo,
		};
	},
	{
		persist: {
			key: "store-user-info",
			// 这里是配置 pinia 只需要持久化 state 中的 Token 即可，而不是整个 store
			pick: ["token", "refreshToken"],
		},
	}
);
