<template>
	<view v-if="props.loading" :class="['fa-loading', { 'fa-loading-mask': props.mask }]">
		<view class="fa-loading__box">
			<view class="box-icon" />
			<view class="box-text">{{ props.text }}</view>
		</view>
	</view>
</template>

<script setup lang="ts">
defineOptions({
	name: "Loading",
	options: {
		virtualHost: true,
		addGlobalClass: true,
		styleIsolation: "shared",
	},
});

const props = defineProps({
	/** @description 加载 */
	loading: Boolean,
	/** @description 加载文字 */
	text: {
		type: String,
		default: "加载中...",
	},
	/** @description 是否遮罩层，一般用于组件内部 */
	mask: Boolean,
});
</script>

<style scoped lang="scss">
@use "./index.scss";
</style>
