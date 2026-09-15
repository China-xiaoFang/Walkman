<template>
	<view v-if="props.loading" class="fa-loading-page">
		<view class="fa-loading-page__box">
			<view class="box-icon" />
			<view class="box-text">{{ props.text }}</view>
		</view>
	</view>
</template>

<script setup lang="ts">
defineOptions({
	name: "LoadingPage",
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
});
</script>

<style scoped lang="scss">
@use "./index.scss";
</style>
