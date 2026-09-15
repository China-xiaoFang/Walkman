<template>
	<wd-config-provider
		:custom-class="`fa-layout fa-layout__${theme}`"
		:theme="theme"
		:theme-vars="configStore.layout.themeVars"
		:custom-style="configStore.layout.themeStyle"
	>
		<FaWatermark v-if="state.watermark" />
		<view :class="['fa-main', { 'fa-main__tabBar': state.isTabBar, 'fa-main__page-scroll': state.pageScroll === false }]" :style="mainStyle">
			<slot v-if="state.rendered" />
			<!-- 页脚 -->
			<FaFooter v-if="showFooter" />
			<!-- 底部导航栏 -->
			<FaTabBar v-if="state.isTabBar" />
		</view>
		<!-- 消息通知 -->
		<wd-notify selector="#fast_notify" />
		<!-- 轻提示 -->
		<wd-toast selector="#fast_toast" />
		<!-- 消息弹窗 -->
		<wd-dialog selector="#fast_dialog" />
		<!-- 蒙版层加载 -->
		<FaLoading :loading="!wdHookState.loading?.fullscreen && wdHookState.loading?.state" :text="wdHookState.loading?.text" />
		<!-- 全屏加载 -->
		<FaLoadingPage :loading="wdHookState.loading?.fullscreen && wdHookState.loading?.state" :text="wdHookState.loading?.text" />
		<!-- 遮罩层 -->
		<FaOverlay :visible="wdHookState.overlay?.state" :transparent="wdHookState.overlay?.transparent" />
	</wd-config-provider>
</template>

<script setup lang="ts">
import { onHide, onLoad, onShow } from "@dcloudio/uni-app";
import { type WatchStopHandle, computed, nextTick, onBeforeMount, reactive, watch } from "vue";
import { useDialog } from "@wot-ui/ui/components/wd-dialog";
import { useNotify } from "@wot-ui/ui/components/wd-notify";
import { useToast } from "@wot-ui/ui/components/wd-toast";
import { useRoute } from "uni-mini-router";
import { wdHookState } from "@/hooks";
import { useConfig } from "@/stores";

defineOptions({
	name: "Layout",
	options: {
		virtualHost: true,
		addGlobalClass: true,
		styleIsolation: "shared",
	},
});

const configStore = useConfig();

let wdNotifyWatch: WatchStopHandle;
const uNotify = useNotify("#fast_notify");
let wdToastWatch: WatchStopHandle;
const uToast = useToast("#fast_toast");
let wdDialogWatch: WatchStopHandle;
const uDialog = useDialog("#fast_dialog");

const state = reactive({
	/** 页面滚动 */
	pageScroll: true,
	/** 显示页脚 */
	footer: true,
	/** 显示水印 */
	watermark: true,
	/** 导航页 */
	isTabBar: false,
	/** 背景颜色 */
	backgroundColor: "var(--wot-filled-bottom)",
	/** 渲染结束 */
	rendered: false,
});

/** 主题 */
const theme = computed(() => (configStore.layout.isDark ? "dark" : "light"));

/** 显示页脚 */
const showFooter = computed(() => configStore.layout.footer && state.footer !== false);

/** 主页面样式 */
const mainStyle = computed(() => {
	const style: Record<string, string> = {};
	let height = "var(--fa-window-height, 100vh)";
	if (showFooter.value) height += " - var(--fa-footer-height, 40px)";
	if (state.isTabBar) {
		height += " - var(--wot-tabbar-height, 55px)";
	}
	style["--main-height"] = `calc(${height})`;
	if (state.backgroundColor) {
		style["background-color"] = state.backgroundColor;
	} else {
		style["background-color"] = "var(--wot-filled-bottom)";
	}
	return style;
});

/** 处理监听 */
const handleWatch = () => {
	wdNotifyWatch ||= watch(
		() => wdHookState.wdNotify,
		(newValue) => {
			if (!newValue?.type) return;
			if (newValue.type === "closeNotify") uNotify.closeNotify();
			else uNotify.showNotify(newValue.options);
			wdHookState.wdNotify = undefined;
		}
	);

	wdToastWatch ||= watch(
		() => wdHookState.wdToast,
		(newValue) => {
			if (!newValue?.type) return;
			if (newValue.type === "close") uToast.close();
			else uToast[newValue.type](newValue.options);
			wdHookState.wdToast = undefined;
		}
	);

	wdDialogWatch ||= watch(
		() => wdHookState.wdMessageBox,
		(newValue) => {
			if (!newValue?.type) return;
			if (newValue.type === "close") {
				uDialog.close();
			} else {
				uDialog[newValue.type](newValue.options).then(newValue.then).catch(newValue.catch);
			}
			wdHookState.wdMessageBox = undefined;
		}
	);
};

onLoad(() => {
	handleWatch();
	nextTick(() => {
		state.rendered = true;
	});
});
onShow(handleWatch);
onHide(() => {
	/** 页面隐藏，取消监听 */
	wdNotifyWatch && wdNotifyWatch();
	wdNotifyWatch = undefined;
	wdToastWatch && wdToastWatch();
	wdToastWatch = undefined;
	wdDialogWatch && wdDialogWatch();
	wdDialogWatch = undefined;
});

onBeforeMount(() => {
	const { pageScroll, footer, watermark, isTabBar, backgroundColor } = useRoute();
	state.pageScroll = pageScroll ?? true;
	state.footer = footer ?? true;
	state.watermark = watermark ?? true;
	state.isTabBar = isTabBar ?? false;
	state.backgroundColor = backgroundColor ?? "var(--wot-filled-bottom)";
});
</script>

<style scoped lang="scss">
@use "./layout.scss";
</style>
