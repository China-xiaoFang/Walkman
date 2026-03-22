<template>
	<view class="page">
		<view class="lesson-list">
			<view
				v-for="(item, index) in state.lessonList"
				:key="item.lessonId"
				class="lesson-item"
				:class="{ 'is-locked': !item.isAccessible }"
				@click="handleLessonClick(item, index)"
			>
				<view class="lesson-no">{{ item.lessonNo }}</view>
				<view class="lesson-content">
					<text class="lesson-name">{{ item.lessonName }}</text>
					<view class="lesson-tags">
						<wd-tag v-if="item.isFree" type="success" size="small">免费</wd-tag>
						<wd-tag v-if="!item.isAccessible" type="danger" size="small">需激活</wd-tag>
					</view>
				</view>
				<view class="lesson-action">
					<wd-icon v-if="item.isAccessible" name="play-circle-fill" size="48rpx" color="var(--wot-color-theme)" />
					<wd-icon v-else name="lock" size="48rpx" color="#ccc" />
				</view>
			</view>
		</view>

		<view v-if="state.lessonList.length === 0 && !state.loading" class="empty-tip">
			<text>暂无课程数据</text>
		</view>

		<!-- 底部激活码入口 -->
		<view class="activation-bar" @click="handleActivation">
			<wd-icon name="gift" size="36rpx" />
			<text>输入激活码解锁全部课程</text>
			<wd-icon name="arrow-right" size="32rpx" />
		</view>
	</view>
</template>

<script setup lang="ts">
import { onLoad, onPullDownRefresh } from "@dcloudio/uni-app";
import { reactive } from "vue";
import { useRouter } from "uni-mini-router";
import { playApi } from "@/api/services/Play";
import type { LessonListOutput } from "@/api/services/Play/models/LessonListOutput";

definePage({
	name: "WalkmanLesson",
	layout: "layout",
	style: {
		navigationBarTitleText: "课程列表",
		enablePullDownRefresh: true,
	},
});

const router = useRouter();

const state = reactive({
	volumeId: 0,
	volumeName: "",
	lessonList: [] as LessonListOutput[],
	loading: false,
});

const handleLessonClick = (item: LessonListOutput, index: number) => {
	if (!item.isAccessible) {
		uni.showToast({ title: "该课程需要激活后才能播放", icon: "none" });
		return;
	}
	router.push({
		path: "/pages/walkman/lessonDetail/index",
		query: {
			volumeId: state.volumeId,
			lessonId: item.lessonId,
			lessonName: item.lessonName,
		},
	});
};

const handleActivation = () => {
	router.push({ path: "/pages/walkman/activation/index" });
};

const loadLessons = async () => {
	if (!state.volumeId) return;
	try {
		state.loading = true;
		state.lessonList = await playApi.getLessonList(state.volumeId);
	} catch {
		state.lessonList = [];
	} finally {
		state.loading = false;
	}
};

onLoad(async (options: any) => {
	state.volumeId = Number(options?.volumeId || 0);
	state.volumeName = options?.volumeName || "";
	if (state.volumeName) {
		uni.setNavigationBarTitle({ title: state.volumeName });
	}
	await loadLessons();
});

onPullDownRefresh(async () => {
	await loadLessons();
	uni.stopPullDownRefresh();
});
</script>

<style scoped lang="scss">
@import "./index.scss";
</style>
