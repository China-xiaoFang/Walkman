<template>
	<view class="page player-page">
		<!-- 顶部信息 -->
		<view class="track-info" v-if="playerStore.currentTrack">
			<text class="track-name">{{ playerStore.currentTrack.lessonName }}</text>
			<text class="track-type">
				{{ playerStore.currentTrack.audioTypeName }} · {{ playerStore.currentTrack.pronunciationTypeName }}
			</text>
		</view>

		<!-- 封面区域 -->
		<view class="cover-area">
			<view class="cover-disc" :class="{ 'is-playing': playerStore.isPlaying }">
				<view class="cover-inner">
					<wd-icon name="music" size="120rpx" color="#fff" />
				</view>
			</view>
		</view>

		<!-- 进度条 -->
		<view class="progress-area">
			<text class="time-text">{{ playerStore.formatTime(playerStore.currentProgress) }}</text>
			<wd-slider
				class="progress-slider"
				v-model="sliderValue"
				:min="0"
				:max="playerStore.currentTrack?.duration || 0"
				:step="1"
				hideLabel
				@dragend="handleSeek"
			/>
			<text class="time-text">{{ playerStore.formatTime(playerStore.currentTrack?.duration || 0) }}</text>
		</view>

		<!-- 切换类型 -->
		<view class="type-switch">
			<view class="type-group">
				<text class="type-label">音频：</text>
				<wd-tag
					v-for="item in playerStore.audioTypes"
					:key="item.value"
					:type="playerStore.currentAudioTypeId === item.value ? 'primary' : 'default'"
					size="small"
					round
					@click="handleSwitchAudioType(item.value)"
				>
					{{ item.label }}
				</wd-tag>
			</view>
			<view class="type-group">
				<text class="type-label">发音：</text>
				<wd-tag
					v-for="item in playerStore.pronunciationTypes"
					:key="item.value"
					:type="playerStore.currentPronunciationTypeId === item.value ? 'primary' : 'default'"
					size="small"
					round
					@click="handleSwitchPronunciationType(item.value)"
				>
					{{ item.label }}
				</wd-tag>
			</view>
		</view>

		<!-- 控制按钮 -->
		<view class="controls">
			<view class="control-btn" :class="{ disabled: !playerStore.hasPrev }" @click="playerStore.prev()">
				<wd-icon name="skip-back-fill" size="64rpx" />
			</view>
			<view class="control-btn play-btn" @click="playerStore.togglePlay()">
				<wd-icon :name="playerStore.isPlaying ? 'pause-circle-fill' : 'play-circle-fill'" size="120rpx" color="var(--wot-color-theme)" />
			</view>
			<view class="control-btn" :class="{ disabled: !playerStore.hasNext }" @click="playerStore.next()">
				<wd-icon name="skip-forward-fill" size="64rpx" />
			</view>
		</view>

		<!-- 播放列表 -->
		<view class="playlist-area">
			<view class="playlist-header">
				<text class="playlist-title">播放列表 ({{ playerStore.playlist.length }})</text>
			</view>
			<scroll-view scroll-y class="playlist-scroll">
				<view
					v-for="(item, index) in playerStore.playlist"
					:key="item.lessonId"
					class="playlist-item"
					:class="{
						'is-active': index === playerStore.currentIndex,
						'is-locked': !item.isAccessible,
					}"
					@click="playerStore.playAt(index)"
				>
					<text class="playlist-item-name">{{ item.lessonNo }}. {{ item.lessonName }}</text>
					<wd-icon v-if="index === playerStore.currentIndex && playerStore.isPlaying" name="volume" size="32rpx" color="var(--wot-color-theme)" />
					<wd-tag v-else-if="!item.isAccessible" type="danger" size="small">需激活</wd-tag>
				</view>
			</scroll-view>
		</view>
	</view>
</template>

<script setup lang="ts">
import { onLoad, onUnload } from "@dcloudio/uni-app";
import { computed, watch } from "vue";
import { usePlayer } from "@/stores";

definePage({
	name: "WalkmanPlayer",
	layout: "layout",
	style: {
		navigationBarTitleText: "播放器",
	},
});

const playerStore = usePlayer();

const sliderValue = computed({
	get: () => playerStore.currentProgress,
	set: () => {},
});

const handleSeek = (e: any) => {
	const value = typeof e === "number" ? e : e?.detail?.value || e?.value || 0;
	playerStore.seekTo(value);
};

const handleSwitchAudioType = async (audioTypeId: number) => {
	if (audioTypeId === playerStore.currentAudioTypeId) return;
	await playerStore.switchAudioType(audioTypeId);
	if (playerStore.playlist.length > 0) {
		playerStore.playAt(0);
	}
};

const handleSwitchPronunciationType = async (pronunciationTypeId: number) => {
	if (pronunciationTypeId === playerStore.currentPronunciationTypeId) return;
	await playerStore.switchPronunciationType(pronunciationTypeId);
	if (playerStore.playlist.length > 0) {
		playerStore.playAt(0);
	}
};

onLoad(async (options: any) => {
	const volumeId = Number(options?.volumeId || 0);
	const lessonId = Number(options?.lessonId || 0);

	// 加载选择器数据
	await playerStore.loadSelectors();

	// 设置默认音频/发音类型
	if (!playerStore.currentAudioTypeId && playerStore.audioTypes.length > 0) {
		playerStore.currentAudioTypeId = playerStore.audioTypes[0].value;
	}
	if (!playerStore.currentPronunciationTypeId && playerStore.pronunciationTypes.length > 0) {
		playerStore.currentPronunciationTypeId = playerStore.pronunciationTypes[0].value;
	}

	if (volumeId && playerStore.currentAudioTypeId && playerStore.currentPronunciationTypeId) {
		// 加载播放列表
		await playerStore.loadPlaylist(
			volumeId,
			playerStore.currentAudioTypeId,
			playerStore.currentPronunciationTypeId,
			lessonId || undefined
		);

		// 开始播放第一首
		if (playerStore.playlist.length > 0) {
			playerStore.initAudioContext();
			playerStore.playAt(0);
		}
	}
});

onUnload(() => {
	// 保存进度但不销毁播放器（允许后台播放）
	playerStore.saveCurrentProgress();
});
</script>

<style scoped lang="scss">
@import "./index.scss";
</style>
