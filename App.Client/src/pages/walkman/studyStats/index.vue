<template>
	<view class="page stats-page">
		<view class="stats-header">
			<view class="stats-days">
				<text class="days-number">{{ state.stats?.continuousDays || 0 }}</text>
				<text class="days-label">连续学习天数</text>
			</view>
		</view>

		<view class="stats-grid" v-if="state.stats">
			<view class="stats-item">
				<text class="stats-value">{{ formatDuration(state.stats.totalDuration) }}</text>
				<text class="stats-label">总学习时长</text>
			</view>
			<view class="stats-item">
				<text class="stats-value">{{ state.stats.totalLessons }}</text>
				<text class="stats-label">已学课程</text>
			</view>
			<view class="stats-item">
				<text class="stats-value">{{ state.stats.totalWords }}</text>
				<text class="stats-label">已学单词</text>
			</view>
			<view class="stats-item">
				<text class="stats-value">{{ state.stats.masteredWords }}</text>
				<text class="stats-label">已掌握</text>
			</view>
			<view class="stats-item">
				<text class="stats-value">{{ state.stats.totalExercises }}</text>
				<text class="stats-label">练习次数</text>
			</view>
			<view class="stats-item">
				<text class="stats-value">{{ state.stats.averageScore || 0 }}</text>
				<text class="stats-label">平均分</text>
			</view>
		</view>

		<!-- 词汇进度 -->
		<view class="section-card" v-if="state.stats">
			<text class="section-title">词汇进度</text>
			<wd-progress :percentage="wordProgress" :color="'#52c41a'" />
			<text class="progress-desc">已掌握 {{ state.stats.masteredWords }} / {{ state.stats.totalWords }} 个单词</text>
		</view>

		<!-- 近期学习 -->
		<view class="section-card" v-if="state.stats?.dailyRecords?.length > 0">
			<text class="section-title">近30天学习记录</text>
			<view class="daily-list">
				<view v-for="item in state.stats.dailyRecords" :key="item.date" class="daily-item">
					<text class="daily-date">{{ formatDate(item.date) }}</text>
					<text class="daily-info">{{ item.lessonCount }}课 · {{ item.wordCount }}词 · {{ Math.floor(item.duration / 60) }}分钟</text>
				</view>
			</view>
		</view>

		<view v-if="!state.stats && !state.loading" class="empty-tip"><text>暂无学习数据</text></view>
	</view>
</template>

<script setup lang="ts">
import { onLoad, onPullDownRefresh } from "@dcloudio/uni-app";
import { reactive, computed } from "vue";
import { studyApi } from "@/api/services/Study";
import type { LearningStatsOutput } from "@/api/services/Study/models/LearningStatsOutput";

definePage({
	name: "WalkmanStudyStats",
	layout: "layout",
	style: { navigationBarTitleText: "学习统计", enablePullDownRefresh: true },
});

const state = reactive({ stats: null as LearningStatsOutput | null, loading: false });

const wordProgress = computed(() => {
	if (!state.stats || !state.stats.totalWords) return 0;
	return Math.round((state.stats.masteredWords / state.stats.totalWords) * 100);
});

const formatDuration = (seconds: number) => {
	if (!seconds) return "0分";
	const h = Math.floor(seconds / 3600);
	const m = Math.floor((seconds % 3600) / 60);
	return h > 0 ? `${h}时${m}分` : `${m}分`;
};

const formatDate = (date: string) => {
	if (!date) return "";
	return date.substring(5, 10);
};

const loadStats = async () => {
	try {
		state.loading = true;
		state.stats = await studyApi.getLearningStats();
	} catch { state.stats = null; } finally { state.loading = false; }
};

onLoad(async () => { await loadStats(); });
onPullDownRefresh(async () => { await loadStats(); uni.stopPullDownRefresh(); });
</script>

<style scoped lang="scss">
@import "./index.scss";
</style>
