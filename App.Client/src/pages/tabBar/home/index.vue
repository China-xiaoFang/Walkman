<template>
	<view class="page">
		<FaSearchInput
			class="mb20"
			search
			:filter="false"
			v-model="state.searchValue"
			@confirm="handleSearch()"
			@clear="handleSearch"
			@search="handleSearch"
		/>
	</view>
</template>

<script setup lang="ts">
import { onLoad, onPullDownRefresh, onShow } from "@dcloudio/uni-app";
import { reactive, watch } from "vue";
import { useUserInfo } from "@/stores";

definePage({
	name: "Home",
	layout: "layout",
	isTabBar: true,
	style: {
		navigationBarTitleText: "首页",
		enablePullDownRefresh: true,
	},
});

const userInfoStore = useUserInfo();

const state = reactive({
	/** 搜素值 */
	searchValue: "",
});

/** 搜素 */
const handleSearch = () => {
	if (!state.searchValue) return;
};

/** 加载页面 */
const loadPage = async () => {};

/** 页面刷新 */
const loadRefresh = async () => {};

onLoad(async () => {
	watch(
		() => userInfoStore.hasUserInfo,
		async (newVal) => {
			if (newVal) {
				await loadPage();
			}
		},
		{
			immediate: true,
		}
	);
});

onShow(async () => {
	watch(
		() => userInfoStore.hasUserInfo,
		async (newVal) => {
			if (newVal) {
				await loadRefresh();
			}
		},
		{
			immediate: true,
		}
	);
});

onPullDownRefresh(async () => {
	await loadPage();
	await loadRefresh();
	uni.stopPullDownRefresh();
});
</script>

<style scoped lang="scss">
@import "./index.scss";
</style>
