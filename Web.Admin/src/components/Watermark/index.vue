<template>
	<el-watermark id="watermark" :rotate="-38" :gap="[-50, -50]" :offset="[-10, 0]" :width="200" :height="300" :font="font" :content="content">
		<slot />
	</el-watermark>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useGlobalSize } from "element-plus";
import { useApp, useConfig, useUserInfo } from "@/stores";

defineOptions({
	name: "Watermark",
});

const _globalSize = useGlobalSize();
const appStore = useApp();
const configStore = useConfig();
const userInfoStore = useUserInfo();

/** 随主题和全局组件尺寸变化的文字样式 */
const font = computed(() => {
	return {
		fontSize: _globalSize.value === "small" ? 12 : 14,
		color: configStore.layout.isDark ? "rgba(255,255,255,0.2)" : "rgba(0,0,0,0.2)",
	};
});
/** 优先展示租户名称，并在路由初始化后附加当前用户名称 */
const content = computed(() => {
	let watermarkContent = [appStore.appName];
	if (userInfoStore.tenantName) {
		watermarkContent = [userInfoStore.tenantName];
	}
	if (userInfoStore.asyncRouterGen) {
		watermarkContent.push(userInfoStore.employeeName || userInfoStore.nickName);
	}

	return watermarkContent;
});
</script>
