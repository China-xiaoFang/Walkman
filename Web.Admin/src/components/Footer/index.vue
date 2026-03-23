<template>
	<template v-if="globalSize === 'small'">
		<div>
			<a href="http://fastdotnet.com" target="_blank">Copyright © {{ new Date().getFullYear() }} Fast All rights reserved.</a>
			<el-text>v{{ state.appVersion }}</el-text>
		</div>
		<div v-if="state.publicSecurityCode">
			<a :href="`https://beian.mps.gov.cn/#/query/webSearch?code=${state.publicSecurityCode?.replace(/\D+/g, '')}`" target="_blank">
				<img src="https://www.beian.gov.cn/img/new/gongan.png" />
				{{ state.publicSecurityCode }}
			</a>
			<a v-if="state.icpText" href="https://beian.miit.gov.cn/" target="_blank">{{ state.icpText }}</a>
		</div>
	</template>
	<template v-else>
		<a href="http://fastdotnet.com" target="_blank">Copyright © {{ new Date().getFullYear() }} Fast All rights reserved.</a>
		<a
			v-if="state.publicSecurityCode"
			:href="`https://beian.mps.gov.cn/#/query/webSearch?code=${state.publicSecurityCode?.replace(/\D+/g, '')}`"
			target="_blank"
		>
			<img src="https://www.beian.gov.cn/img/new/gongan.png" />
			{{ state.publicSecurityCode }}
		</a>
		<a v-if="state.icpText" href="https://beian.miit.gov.cn/" target="_blank">{{ state.icpText }}</a>
		<el-text>v{{ state.appVersion }}</el-text>
	</template>
</template>

<script lang="ts" setup>
import { reactive } from "vue";
import { useGlobalSize } from "element-plus";

defineOptions({
	// eslint-disable-next-line vue/no-reserved-component-names
	name: "Footer",
});

const globalSize = useGlobalSize();

const state = reactive({
	appVersion: import.meta.env.VITE_APP_VERSION,
	publicSecurityCode: "甘公网安备 62090202000584号",
	icpText: "陇ICP备2020003856号",
});
</script>

<style scoped lang="scss">
div {
	display: flex;
	align-items: center;
	justify-content: center;
	overflow: hidden;
}
a,
.el-text {
	display: flex;
	align-items: center;
	gap: 3px;
	color: var(--el-text-color-secondary);
	text-decoration: none;
	letter-spacing: 0.5px;
	padding-right: 10px;
}
a:hover {
	color: var(--el-text-color-primary);
}
</style>
