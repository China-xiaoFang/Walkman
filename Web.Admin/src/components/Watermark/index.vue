<template>
	<el-watermark id="watermark" v-bind="watermarkProps">
		<slot />
	</el-watermark>
</template>

<script setup lang="ts">
import { computed, reactive } from "vue";
import { useGlobalSize } from "element-plus";
import { withDefineType } from "@fast-china/utils";
import { useConfig, useUserInfo } from "@/stores";

defineOptions({
	name: "Watermark",
});

const _globalSize = useGlobalSize();
const configStore = useConfig();
const userInfoStore = useUserInfo();

const watermarkProps = reactive({
	rotate: -38,
	gap: withDefineType<[number, number]>([-50, -50]),
	offset: withDefineType<[number, number]>([-10, 0]),
	width: 200,
	height: 300,
	font: computed(() => {
		return {
			fontSize: _globalSize.value === "small" ? 12 : 14,
			color: configStore.layout.isDark ? "rgba(255,255,255,0.2)" : "rgba(0,0,0,0.2)",
		};
	}),
	content: computed(() => {
		const watermarkContent = ["随身听"];
		if (userInfoStore.asyncRouterGen) {
			watermarkContent.push(userInfoStore.employeeName || userInfoStore.nickName);
		}

		return watermarkContent;
	}),
});
</script>
