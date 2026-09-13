<template>
	<el-watermark id="watermark" v-bind="watermarkProps">
		<slot />
	</el-watermark>
</template>

<script setup lang="ts">
import { computed, reactive } from "vue";
import { useGlobalSize } from "element-plus";
import { withDefineType } from "@fast-china/utils";
import { useApp, useConfig, useUserInfo } from "@/stores";

defineOptions({
	name: "Watermark",
});

const _globalSize = useGlobalSize();
const appStore = useApp();
const configStore = useConfig();
const userInfoStore = useUserInfo();

/** 根据当前主题、组件尺寸和登录信息生成水印配置 */
const watermarkProps = reactive({
	/** 水印旋转角度 */
	rotate: -38,
	/** 相邻水印之间的水平和垂直间距 */
	gap: withDefineType<[number, number]>([-50, -50]),
	/** 水印相对于容器起点的偏移量 */
	offset: withDefineType<[number, number]>([-10, 0]),
	/** 单个水印的布局宽度 */
	width: 200,
	/** 单个水印的布局高度 */
	height: 300,
	/** 随主题和全局组件尺寸变化的文字样式 */
	font: computed(() => {
		return {
			fontSize: _globalSize.value === "small" ? 12 : 14,
			color: configStore.layout.isDark ? "rgba(255,255,255,0.2)" : "rgba(0,0,0,0.2)",
		};
	}),
	/** 优先展示租户名称，并在路由初始化后附加当前用户名称 */
	content: computed(() => {
		let watermarkContent = [appStore.appName];
		if (userInfoStore.tenantName) {
			watermarkContent = [userInfoStore.tenantName];
		}
		if (userInfoStore.asyncRouterGen) {
			watermarkContent.push(userInfoStore.employeeName || userInfoStore.nickName);
		}

		return watermarkContent;
	}),
});
</script>
