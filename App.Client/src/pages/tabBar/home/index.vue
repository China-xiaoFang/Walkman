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
		<wd-swiper
			v-if="appStore.bannerImages?.length > 0"
			customClass="mb20"
			:list="appStore.bannerImages"
			autoplay
			v-model:current="state.swiperCurrent"
			:indicator="{ type: 'dots-bar' }"
		/>

		<!-- 快捷入口 -->
		<view class="quick-entry">
			<view class="entry-item" @click="router.push({ path: '/pages/walkman/ranking/index' })">
				<wd-icon name="trophy" size="48rpx" color="#ff9900" />
				<text>排行榜</text>
			</view>
			<view class="entry-item" @click="router.push({ path: '/pages/walkman/studyStats/index' })">
				<wd-icon name="chart" size="48rpx" color="#667eea" />
				<text>学习统计</text>
			</view>
			<view class="entry-item" @click="router.push({ path: '/pages/walkman/activation/index' })">
				<wd-icon name="gift" size="48rpx" color="#ee0a24" />
				<text>激活码</text>
			</view>
		</view>

		<!-- 教材列表 -->
		<view class="section-title">教材列表</view>
		<view class="textbook-list">
			<view
				v-for="item in state.textbookList"
				:key="item.textbookId"
				class="textbook-card"
				@click="handleTextbookClick(item)"
			>
				<FaImage
					v-if="item.coverUrl"
					width="200rpx"
					height="260rpx"
					:src="item.coverUrl"
					:hideImage="false"
				/>
				<view v-else class="textbook-cover-placeholder">
					<text>{{ item.textbookName?.substring(0, 1) }}</text>
				</view>
				<view class="textbook-info">
					<text class="textbook-name">{{ item.textbookName }}</text>
					<text v-if="item.description" class="textbook-desc">{{ item.description }}</text>
				</view>
			</view>
		</view>

		<view v-if="state.textbookList.length === 0 && !state.loading" class="empty-tip">
			<text>暂无教材数据</text>
		</view>
	</view>
</template>

<script setup lang="ts">
import { onLoad, onPullDownRefresh, onShow } from "@dcloudio/uni-app";
import { reactive, watch } from "vue";
import { useRouter } from "uni-mini-router";
import { useApp, useUserInfo } from "@/stores";
import { playApi } from "@/api/services/Play";
import type { TextbookListOutput } from "@/api/services/Play/models/TextbookListOutput";

definePage({
	name: "Home",
	layout: "layout",
	isTabBar: true,
	style: {
		navigationBarTitleText: "首页",
		enablePullDownRefresh: true,
	},
});

const appStore = useApp();
const userInfoStore = useUserInfo();
const router = useRouter();

const state = reactive({
	swiperCurrent: 0,
	searchValue: "",
	textbookList: [] as TextbookListOutput[],
	loading: false,
});

const handleSearch = () => {
	if (!state.searchValue) return;
};

/** 点击教材 */
const handleTextbookClick = (item: TextbookListOutput) => {
	router.push({
		path: "/pages/walkman/volume/index",
		query: {
			textbookId: item.textbookId,
			textbookName: item.textbookName,
		},
	});
};

/** 加载教材列表 */
const loadTextbooks = async () => {
	try {
		state.loading = true;
		state.textbookList = await playApi.getTextbookList();
	} catch {
		state.textbookList = [];
	} finally {
		state.loading = false;
	}
};

const loadPage = async () => {
	await loadTextbooks();
};

const loadRefresh = async () => {};

onLoad(async () => {
	watch(
		() => appStore.appName,
		(newVal) => {
			if (newVal) {
				uni.setNavigationBarTitle({ title: appStore.appName });
			}
		},
		{ immediate: true }
	);
	watch(
		() => userInfoStore.hasUserInfo,
		async (newVal) => {
			if (newVal) await loadPage();
		},
		{ immediate: true }
	);
});

onShow(async () => {
	watch(
		() => userInfoStore.hasUserInfo,
		async (newVal) => {
			if (newVal) await loadRefresh();
		},
		{ immediate: true }
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
