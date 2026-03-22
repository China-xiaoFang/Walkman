<template>
	<view class="page ranking-page">
		<view class="ranking-header">
			<wd-icon name="trophy" size="60rpx" color="#ff9900" />
			<text class="ranking-title">学习排行榜</text>
		</view>

		<view class="ranking-list">
			<view v-for="item in state.rankingList" :key="item.accountId" class="ranking-item" :class="{ 'top-three': item.rank <= 3 }">
				<view class="rank-no" :class="'rank-' + item.rank">{{ item.rank }}</view>
				<view class="rank-info">
					<text class="rank-name">{{ item.nickName || '匿名用户' }}</text>
					<text class="rank-detail">学习 {{ formatDuration(item.totalDuration) }} · {{ item.totalLessons }}课 · {{ item.totalWords }}词</text>
				</view>
				<view class="rank-score">
					<text class="score-value">{{ item.totalScore }}</text>
					<text class="score-label">积分</text>
				</view>
			</view>
		</view>

		<view v-if="state.rankingList.length === 0 && !state.loading" class="empty-tip">
			<text>暂无排行数据</text>
		</view>
	</view>
</template>

<script setup lang="ts">
import { onLoad, onPullDownRefresh } from "@dcloudio/uni-app";
import { reactive } from "vue";
import { studyApi } from "@/api/services/Study";
import type { RankingOutput } from "@/api/services/Study/models/RankingOutput";

definePage({
	name: "WalkmanRanking",
	layout: "layout",
	style: { navigationBarTitleText: "排行榜", enablePullDownRefresh: true },
});

const state = reactive({ rankingList: [] as RankingOutput[], loading: false });

const formatDuration = (seconds: number) => {
	if (!seconds) return "0分钟";
	const h = Math.floor(seconds / 3600);
	const m = Math.floor((seconds % 3600) / 60);
	return h > 0 ? `${h}小时${m}分钟` : `${m}分钟`;
};

const loadRanking = async () => {
	try {
		state.loading = true;
		state.rankingList = await studyApi.getRanking();
	} catch { state.rankingList = []; } finally { state.loading = false; }
};

onLoad(async () => { await loadRanking(); });
onPullDownRefresh(async () => { await loadRanking(); uni.stopPullDownRefresh(); });
</script>

<style scoped lang="scss">
@import "./index.scss";
</style>
