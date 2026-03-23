import { ElMessage, ElNotification } from "element-plus";
import { consoleError, dateUtil, envUtil, stringUtil } from "@fast-china/utils";
import { useTitle } from "@vueuse/core";
import NProgress from "nprogress";
import { createRouter, createWebHistory } from "vue-router";
import { initWebSocket } from "@/signalR";
import { useUserInfo } from "@/stores";
import { defaultRoute } from "./modules/defaultRoute";
import { handleDynamicRoute } from "./utils";
import "nprogress/nprogress.css";

const router = createRouter({
	history: createWebHistory(import.meta.env.VITE_PUBLIC_PATH),
	routes: defaultRoute,
});

const defaultRoutePath = defaultRoute.map((m) => m.path);

/** 配置 NProgress */
NProgress.configure({
	// 动画方式
	easing: "ease",
	// 递增进度条的速度
	speed: 500,
	// 是否显示加载ico
	showSpinner: true,
	// 自动递增间隔
	trickleSpeed: 200,
	// 初始化时的最小百分比
	minimum: 0.3,
});

/** 路由加载前 */
router.beforeEach(async (to, from, next) => {
	// 开启进度条
	NProgress.start();

	if (import.meta.env.VITE_ENABLE_MOBILE !== "true" && to.path != "/mobileBlocked" && envUtil.isMobile()) {
		next({ path: "/mobileBlocked" });
		return;
	}

	const userInfoStore = useUserInfo();

	// 判断是否存在Token
	if (!userInfoStore.token) {
		// 判断当前页面是否需要登录
		if (!to.meta.noLogin) {
			ElMessage.warning("请登录");
			// 如果去的路由和来的路由一致，则携带来的路由的参数
			if (from.path === to.path) {
				next({ path: "/login", query: from.query });
			}
			// 如果是默认路由，则不处理重定向
			else if (defaultRoutePath.includes(to.path)) {
				next({ path: "/login" });
			} else {
				next({ path: "/login", query: { redirect: encodeURIComponent(to?.redirectedFrom?.fullPath ?? to.fullPath) } });
			}
			return;
		}
	} else {
		// 判断 pinia 中的动态路由生成的状态，必须存在Token才加载
		if (!userInfoStore.asyncRouterGen) {
			try {
				// 刷新用户信息
				await userInfoStore.refreshUserInfo();

				// 加载动态路由
				handleDynamicRoute();

				// 确保路由添加完成
				userInfoStore.asyncRouterGen = true;

				// 初始化 WebSocket
				initWebSocket();

				// 延迟 0.5 秒显示欢迎信息
				setTimeout(() => {
					ElNotification({
						title: "欢迎",
						message: `${dateUtil.getGreet()}${userInfoStore.employeeName}`,
						type: "success",
						duration: 1500,
					});
				}, 500);

				// 由于新添加的路由在本次不存在，所以进行重定向
				next({ ...(to.redirectedFrom ?? to), replace: true });
				return;
			} catch (error) {
				next(false);
				consoleError("InitRoute", error);
				// 退出登录
				userInfoStore.logout();
				return;
			}
		}

		// 判断是否存在重定向路径，如果有则跳转
		const redirect = decodeURIComponent((from.query.redirect as string) || "");
		if (redirect && redirect != to.fullPath) {
			delete from.query.redirect;
			const _query = stringUtil.getUrlParams(redirect);
			// 设置 replace: true, 因此导航将不会留下历史记录
			next({ path: redirect, replace: true, query: _query });
			return;
		}

		// 判断登录后是否禁止查看该页面
		if (to.meta.authForbidView) {
			// 重定向到首页
			next({ path: "/" });
			return;
		}
	}

	// 刷新页面标题
	const title = useTitle();
	if (to.meta.title) {
		title.value = `${to.meta.title} - ${userInfoStore.employeeName || "Fast随身听"}`;
	} else {
		title.value = `${userInfoStore.employeeName || "Fast随身听"}`;
	}

	next();
});

/** 路由加载后 */
router.afterEach(() => {
	NProgress.done();
});

export default router;

export * from "./utils";
