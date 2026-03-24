import { reactive, ref, toRefs } from "vue";
import { Local, base64Util, consoleError } from "@fast-china/utils";
import { isNil } from "lodash-unified";
import { defineStore } from "pinia";
import { loginApi } from "@/api/services/Auth/login";
import { CommonRoute } from "@/common";
import { useToast } from "@/hooks";
import { closeWebSocket } from "@/signalR";
import { useApp } from "../app";
import type { ClientLoginOutput } from "@/api/services/Auth/login/models/ClientLoginOutput";
import type { WeChatClientLoginInput } from "@/api/services/Auth/login/models/WeChatClientLoginInput";
import type { AxiosResponse } from "axios";

type IState = {
	/** Token */
	token: string;
	/** Refresh Token */
	refreshToken: string;
	/** 激活的 TabBar 页面 */
	activeTabBar: string;
	/** 授权登录弹窗 */
	authLoginPopup: boolean;
};

export const useUserInfo = defineStore(
	"userInfo",
	() => {
		const state = reactive<IState & Required<Omit<ClientLoginOutput, "status" | "message">>>({
			token: "",
			refreshToken: "",
			activeTabBar: CommonRoute.Home,
			authLoginPopup: false,
			openId: "",
			unionId: "",
			mobile: "",
			nickName: "",
			avatar: "",
		});

		/** 是否存在用户信息 */
		const hasUserInfo = ref(false);

		/** WebSocket 是否连接 */
		const hasWebSocket = ref(false);

		const tabBars = reactive<ITabBar[]>([
			{
				path: CommonRoute.Home,
				icon: "home",
				title: "首页",
			},
			{
				path: CommonRoute.My,
				icon: "my",
				title: "个人中心",
			},
		]);

		/** 设置用户信息 */
		const setUserInfo = (uInfo: ClientLoginOutput): void => {
			Object.keys(uInfo).forEach((key) => {
				state[key] = uInfo[key];
			});
		};

		/** 删除 Token */
		const removeToken = (): void => {
			state.token = "";
			state.refreshToken = "";
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
		const resolveToken = (token: string = null, refreshToken: string = null): { token: string; refreshToken: string; tokenData: any } => {
			token ??= state.token;
			refreshToken ??= state.refreshToken;
			if (token) {
				const jwtToken = decodeURIComponent(encodeURIComponent(base64Util.atob(token.replace(/_/g, "/").replace(/-/g, "+").split(".")[1])));
				const jwtTokenData = JSON.parse(jwtToken);
				// 获取 Token 的过期时间
				const exp = new Date(jwtTokenData.exp * 1000);
				if (new Date() >= exp) {
					return { token: `Bearer ${token}`, refreshToken: `Bearer ${refreshToken}`, tokenData: jwtTokenData };
				}
				return { token: `Bearer ${token}`, refreshToken: null, tokenData: jwtTokenData };
			}
			return { token: null, refreshToken: null, tokenData: null };
		};

		/** 登录 */
		const login = async (input: WeChatClientLoginInput = {}): Promise<void> => {
			return new Promise((resolve, reject) => {
				// #ifdef MP-WEIXIN
				uni.login({
					success: async (res) => {
						try {
							const apiRes = await loginApi.weChatClientLogin({
								...input,
								weChatCode: res.code,
							});
							setUserInfo(apiRes);
							// 确保用户添加完成
							hasUserInfo.value = true;
							return resolve();
						} catch {
							return reject();
						}
					},
					fail: (error) => {
						useToast.warning("授权失败，无法获取您的信息。请重新授权以继续使用我们的服务。");
						return reject();
					},
				});
				// #endif

				// #ifndef MP-WEIXIN
				return resolve();
				// #endif
			});
		};

		const logoutClear = (): void => {
			removeToken();
			// 删除 HTTP 缓存数据
			Local.removeByPrefix("HTTP_CACHE_");
		};

		/** 退出登录 */
		const logout = async (): Promise<void> => {
			try {
				// 关闭 WebSocket 连接
				await closeWebSocket();
			} catch (error) {
				consoleError("Logout", error);
			}
			try {
				await loginApi.logout();
			} catch (error) {
				consoleError("Logout", error);
			} finally {
				hasUserInfo.value = false;
				logoutClear();
			}
		};

		/** 刷新应用 */
		const refreshApp = async (): Promise<void> => {
			// 删除 HTTP 缓存数据
			Local.removeByPrefix("HTTP_CACHE_");

			// 刷新字典
			const appStore = useApp();
			await appStore.setDictionary();
		};

		/** 授权登录检查 */
		const authLoginCheck = (): boolean => {
			if (isNil(state.mobile)) {
				state.authLoginPopup = true;
				return false;
			}

			return true;
		};

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
			refreshApp,
			authLoginCheck,
		};
	},
	{
		persist: {
			key: "store-user-info",
			// 这里是配置 pinia 只需要持久化 state 中的 Token 即可，而不是整个 store
			paths: ["token", "refreshToken", "openId", "unionId", "mobile", "nickName", "avatar"],
		},
	}
);
