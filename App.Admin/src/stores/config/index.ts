import { defineStore } from "pinia";
import { reactive } from "vue";
import { addCssUnit, formatHexColor, mixHexColors, parseHexColor, pickHigherContrastColor, serializeStyle, withDefineType } from "@fast-china/utils";
import { CommonUniApp } from "@/common";
import { useToast } from "@/hooks";
import { useApp } from "../app";
import type { ConfigProviderThemeVars } from "@wot-ui/ui/components/wd-config-provider/types";

export const useConfig = defineStore(
	"config",
	() => {
		/** 布局配置 */
		const layout = reactive({
			/** 头部导航栏高度 */
			navBarHeight: 48,
			/** 底部标签栏高度 */
			tabBarHeight: 55,
			/** 页脚高度 */
			footerHeight: 40,
			/** 主题颜色 */
			themeColor: "",
			/** 主题样式 */
			themeStyle: "",
			/** Wot UI 主题变量 */
			themeVars: withDefineType<ConfigProviderThemeVars>({}),
			/** 导航栏是否跟随主题色自动切换 */
			autoThemeNavBar: false,
			/** 跟随系统设置，自动切换浅色/深色模式 */
			autoThemMode: true,
			/** 是否深色模式 */
			isDark: false,
			/** 是否置灰模式 */
			isGrey: false,
			/** 是否色弱模式 */
			isWeak: false,
			/** 是否显示页脚 */
			footer: true,
		});

		/** 表格配置 */
		const tableLayout = reactive({
			/** Table隐藏图片 */
			hideImage: true,
			/** Table默认时间搜索范围 */
			dataSearchRange: withDefineType<FaTableDataRange>("Past3D"),
		});

		/** 设置主题色 */
		const setTheme = (color?: string) => {
			if (!color) {
				color = useApp().themeColor;
				useToast.success(`主题颜色已重置为 ${color}`);
			}
			let navbarBgColor = "#f8f8f8";
			let navbarFrontColor = "#000000";
			let pageBackgroundColor = "#f2f3f5";
			if (layout.isDark) {
				navbarBgColor = "#141414";
				navbarFrontColor = "#ffffff";
				pageBackgroundColor = "#0a0a0a";
			} else if (layout.autoThemeNavBar) {
				navbarBgColor = color;
				navbarFrontColor = pickHigherContrastColor(color) === "#ffffff" ? "#ffffff" : "#000000";
			}

			uni.setNavigationBarColor({ frontColor: navbarFrontColor, backgroundColor: navbarBgColor });
			uni.setBackgroundColor({
				backgroundColor: pageBackgroundColor,
				backgroundColorTop: pageBackgroundColor,
				backgroundColorBottom: pageBackgroundColor,
			});
			uni.setBackgroundTextStyle({ textStyle: layout.isDark ? "light" : "dark" });

			// #ifdef APP-PLUS
			plus.navigator.setStatusBarStyle(navbarFrontColor === "#ffffff" ? "light" : "dark");
			// #endif

			const appStore = useApp();
			// 头部胶囊padding
			// eslint-disable-next-line no-useless-assignment
			let navbarCapsulePadding = CommonUniApp.navbarCapsuleMenuButtonPadding;
			// #ifdef MP-WEIXIN
			navbarCapsulePadding = appStore.windowInfo.windowWidth - (appStore.menuButton?.right ?? 0);
			// #endif

			const styles: Record<string, string> = {
				"--fa-window-width": `${appStore.windowInfo.windowWidth}px`,
				// 如果存在安全距离，页面 100vh 会导致失效，所以这里通过动态计算计算页面剩余高度
				"--fa-window-height": `calc(100vh - ${appStore.windowInfo.safeAreaInsets.bottom}px)`,
				// 状态栏高度
				"--fa-status-bar-height": `${appStore.windowInfo.statusBarHeight}px`,
				// 头部胶囊按钮边距
				"--fa-navbar-capsule-padding": `${navbarCapsulePadding}px`,
				// 页脚高度，如果不存在底部安全区域，则默认 + 10px
				"--fa-footer-height": `calc(${addCssUnit(layout.footerHeight)} + ${appStore.windowInfo.safeAreaInsets.bottom > 0 ? "0px" : "10px"})`,
				// 安全距离
				"--fa-area-inset-left": `${appStore.windowInfo.safeAreaInsets.left}px`,
				"--fa-area-inset-right": `${appStore.windowInfo.safeAreaInsets.right}px`,
				"--fa-area-inset-top": `${appStore.windowInfo.safeAreaInsets.top}px`,
				"--fa-area-inset-bottom": `${appStore.windowInfo.safeAreaInsets.bottom}px`,
				// 头部 navbar 背景颜色
				"--wot-navbar-bg": navbarBgColor,
				// 头部 navbar 高度
				"--wot-navbar-height": addCssUnit(layout.navBarHeight),
				// 头部胶囊按钮宽度
				"--wot-navbar-capsule-width": `${appStore.menuButton?.width ?? 0}px`,
				// 头部胶囊按钮高度，-2 边框
				"--wot-navbar-capsule-height": `${appStore.menuButton.height - 2}px`,
				/* 导航栏文字颜色 */
				"--wot-navbar-color": navbarFrontColor,
				// 底部 tabbar 高度
				"--wot-tabbar-height": addCssUnit(layout.tabBarHeight),
			};

			const themeVars: ConfigProviderThemeVars = {
				primary6: color,
				feedbackAccent: formatHexColor({ ...parseHexColor(color), alpha: 0.08 }),
			};
			for (let i = 1; i <= 10; i++) {
				if (i === 6) continue;
				let mixColor = i < 6 ? "#ffffff" : "#000000";
				if (layout.isDark) mixColor = i < 6 ? "#000000" : "#ffffff";
				Object.assign(themeVars, { [`primary${i}`]: mixHexColors(color, mixColor, Math.abs(6 - i) / 6) });
			}
			layout.themeVars = themeVars;

			// 判断是否为置灰模式
			if (layout.isGrey) {
				styles["filter"] = "grayscale(1)";
			}

			// 判断是否为色弱模式
			if (layout.isWeak) {
				styles["filter"] = "invert(80%)";
			}

			layout.themeColor = color;
			layout.themeStyle = serializeStyle(styles);
		};

		/** 切换深色模式 */
		const switchDark = () => {
			if (layout.isDark) {
				// #ifdef APP-PLUS
				plus.nativeUI.setUIStyle("dark");
				// #endif
			} else {
				// #ifdef APP-PLUS
				plus.nativeUI.setUIStyle("light");
				// #endif
			}
			setTheme(layout.themeColor);
		};

		/** 切换跟随系统变化自动设置浅色/深色模式 */
		const switchAutoThemMode = () => {
			if (!layout.autoThemMode) return;
			// #ifdef APP-PLUS
			plus.nativeUI.setUIStyle("auto");
			// #endif

			// 判断是否启用深色模式
			if (uni.getAppBaseInfo().theme === "dark") {
				layout.isDark = true;
			} else {
				layout.isDark = false;
			}
			setTheme(layout.themeColor);
		};

		/** 切换置灰或色弱模式 */
		const switchGreyOrWeak = (type: "grey" | "weak", value: boolean) => {
			layout.isGrey = type === "grey" && value;
			layout.isWeak = type === "weak" && value;
			setTheme(layout.themeColor);
		};

		/** 初始化主题 */
		const initTheme = () => {
			const appStore = useApp();
			if (appStore.isIphone) {
				layout.navBarHeight = CommonUniApp.iosNavBarHeight;
			} else {
				layout.navBarHeight = CommonUniApp.androidNavBarHeight;
			}
			switchAutoThemMode();
			uni.onThemeChange(switchAutoThemMode);
			switchDark();
			if (layout.isGrey) switchGreyOrWeak("grey", true);
			if (layout.isWeak) switchGreyOrWeak("weak", true);
		};

		/** 重置 */
		const reset = () => {
			layout.tabBarHeight = 55;
			layout.footerHeight = 40;
			layout.themeColor = useApp().themeColor;
			layout.autoThemeNavBar = false;
			layout.autoThemMode = true;
			layout.isDark = false;
			layout.isGrey = false;
			layout.isWeak = false;
			layout.footer = true;
			initTheme();
			tableLayout.hideImage = true;
			tableLayout.dataSearchRange = "Past3D";
		};

		return {
			layout,
			tableLayout,
			setTheme,
			switchAutoThemMode,
			switchDark,
			switchGreyOrWeak,
			initTheme,
			reset,
		};
	},
	{
		persist: {
			key: "store-config",
		},
	}
);
