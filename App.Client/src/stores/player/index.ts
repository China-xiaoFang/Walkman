import { reactive, ref, computed } from "vue";
import { defineStore } from "pinia";
import { consoleLog } from "@fast-china/utils";
import { playApi } from "@/api/services/Play";
import type { PlaylistOutput } from "@/api/services/Play/models/PlaylistOutput";
import type { AudioTypeOutput } from "@/api/services/Play/models/AudioTypeOutput";
import type { PronunciationTypeOutput } from "@/api/services/Play/models/PronunciationTypeOutput";

/**
 * 播放器状态管理
 */
export const usePlayer = defineStore(
	"player",
	() => {
		/** 播放列表 */
		const playlist = ref<PlaylistOutput[]>([]);

		/** 当前播放位置索引 */
		const currentIndex = ref(0);

		/** 当前播放进度（秒） */
		const currentProgress = ref(0);

		/** 是否正在播放 */
		const isPlaying = ref(false);

		/** 音频类型列表 */
		const audioTypes = ref<AudioTypeOutput[]>([]);

		/** 发音类型列表 */
		const pronunciationTypes = ref<PronunciationTypeOutput[]>([]);

		/** 当前音频类型Id */
		const currentAudioTypeId = ref<number>(0);

		/** 当前发音类型Id */
		const currentPronunciationTypeId = ref<number>(0);

		/** 当前册Id */
		const currentVolumeId = ref<number>(0);

		/** 音频上下文 */
		let innerAudioContext: UniApp.InnerAudioContext | null = null;

		/** 进度保存定时器 */
		let progressTimer: ReturnType<typeof setInterval> | null = null;

		/** 当前播放项 */
		const currentTrack = computed(() => {
			if (playlist.value.length === 0 || currentIndex.value < 0 || currentIndex.value >= playlist.value.length) {
				return null;
			}
			return playlist.value[currentIndex.value];
		});

		/** 是否有下一曲 */
		const hasNext = computed(() => {
			return currentIndex.value < playlist.value.length - 1;
		});

		/** 是否有上一曲 */
		const hasPrev = computed(() => {
			return currentIndex.value > 0;
		});

		/** 格式化时间 mm:ss */
		const formatTime = (seconds: number): string => {
			const min = Math.floor(seconds / 60);
			const sec = Math.floor(seconds % 60);
			return `${min.toString().padStart(2, "0")}:${sec.toString().padStart(2, "0")}`;
		};

		/** 初始化音频上下文 */
		const initAudioContext = () => {
			if (innerAudioContext) {
				innerAudioContext.destroy();
			}
			innerAudioContext = uni.createInnerAudioContext();
			innerAudioContext.autoplay = false;

			// 播放结束事件
			innerAudioContext.onEnded(() => {
				consoleLog("Player", "播放结束");
				saveCurrentProgress();
				if (hasNext.value) {
					next();
				} else {
					isPlaying.value = false;
					stopProgressTimer();
				}
			});

			// 播放错误事件
			innerAudioContext.onError((err) => {
				consoleLog("Player", "播放错误", err);
				isPlaying.value = false;
				stopProgressTimer();
			});

			// 时间更新事件
			innerAudioContext.onTimeUpdate(() => {
				if (innerAudioContext) {
					currentProgress.value = Math.floor(innerAudioContext.currentTime || 0);
				}
			});

			// 播放事件
			innerAudioContext.onPlay(() => {
				isPlaying.value = true;
				startProgressTimer();
			});

			// 暂停事件
			innerAudioContext.onPause(() => {
				isPlaying.value = false;
				stopProgressTimer();
				saveCurrentProgress();
			});
		};

		/** 加载播放列表 */
		const loadPlaylist = async (volumeId: number, audioTypeId: number, pronunciationTypeId: number, startLessonId?: number) => {
			currentVolumeId.value = volumeId;
			currentAudioTypeId.value = audioTypeId;
			currentPronunciationTypeId.value = pronunciationTypeId;

			const data = await playApi.getPlaylist({
				volumeId,
				audioTypeId,
				pronunciationTypeId,
				startLessonId,
			});
			playlist.value = data || [];
			currentIndex.value = 0;
			currentProgress.value = 0;

			consoleLog("Player", `加载播放列表，共${playlist.value.length}首`);
		};

		/** 加载音频类型和发音类型 */
		const loadSelectors = async () => {
			const [atRes, ptRes] = await Promise.all([playApi.audioTypeSelector(), playApi.pronunciationTypeSelector()]);
			audioTypes.value = atRes || [];
			pronunciationTypes.value = ptRes || [];
		};

		/** 播放指定索引的曲目 */
		const playAt = (index: number) => {
			if (index < 0 || index >= playlist.value.length) return;
			const track = playlist.value[index];
			if (!track.isAccessible) {
				uni.showToast({ title: "该课程需要激活后才能播放", icon: "none" });
				return;
			}

			currentIndex.value = index;
			currentProgress.value = track.progress || 0;

			if (!innerAudioContext) {
				initAudioContext();
			}

			innerAudioContext!.src = track.audioUrl;

			// 如果有历史进度，从历史位置继续
			if (track.progress > 0) {
				innerAudioContext!.startTime = track.progress;
			}

			innerAudioContext!.play();
			consoleLog("Player", `播放: ${track.lessonName}`);
		};

		/** 播放/暂停切换 */
		const togglePlay = () => {
			if (!innerAudioContext || !currentTrack.value) return;

			if (isPlaying.value) {
				innerAudioContext.pause();
			} else {
				innerAudioContext.play();
			}
		};

		/** 下一曲 */
		const next = () => {
			if (!hasNext.value) return;
			saveCurrentProgress();
			playAt(currentIndex.value + 1);
		};

		/** 上一曲 */
		const prev = () => {
			if (!hasPrev.value) return;
			saveCurrentProgress();
			playAt(currentIndex.value - 1);
		};

		/** 跳转到指定位置（秒） */
		const seekTo = (seconds: number) => {
			if (!innerAudioContext) return;
			innerAudioContext.seek(seconds);
			currentProgress.value = seconds;
		};

		/** 保存当前播放进度 */
		const saveCurrentProgress = async () => {
			const track = currentTrack.value;
			if (!track || currentProgress.value <= 0) return;

			try {
				await playApi.saveProgress({
					lessonId: track.lessonId,
					audioTypeId: track.audioTypeId,
					pronunciationTypeId: track.pronunciationTypeId,
					progress: currentProgress.value,
					duration: track.duration,
				});
			} catch {
				// 静默失败，不影响播放体验
			}
		};

		/** 开始进度保存定时器（每30秒保存一次） */
		const startProgressTimer = () => {
			stopProgressTimer();
			progressTimer = setInterval(() => {
				saveCurrentProgress();
			}, 30000);
		};

		/** 停止进度保存定时器 */
		const stopProgressTimer = () => {
			if (progressTimer) {
				clearInterval(progressTimer);
				progressTimer = null;
			}
		};

		/** 切换音频类型 */
		const switchAudioType = async (audioTypeId: number) => {
			saveCurrentProgress();
			stop();
			await loadPlaylist(currentVolumeId.value, audioTypeId, currentPronunciationTypeId.value);
		};

		/** 切换发音类型 */
		const switchPronunciationType = async (pronunciationTypeId: number) => {
			saveCurrentProgress();
			stop();
			await loadPlaylist(currentVolumeId.value, currentAudioTypeId.value, pronunciationTypeId);
		};

		/** 停止播放 */
		const stop = () => {
			if (innerAudioContext) {
				innerAudioContext.stop();
			}
			isPlaying.value = false;
			stopProgressTimer();
		};

		/** 销毁播放器 */
		const destroy = () => {
			stop();
			if (innerAudioContext) {
				innerAudioContext.destroy();
				innerAudioContext = null;
			}
			playlist.value = [];
			currentIndex.value = 0;
			currentProgress.value = 0;
		};

		return {
			// 状态
			playlist,
			currentIndex,
			currentProgress,
			isPlaying,
			audioTypes,
			pronunciationTypes,
			currentAudioTypeId,
			currentPronunciationTypeId,
			currentVolumeId,
			// 计算属性
			currentTrack,
			hasNext,
			hasPrev,
			// 方法
			formatTime,
			initAudioContext,
			loadPlaylist,
			loadSelectors,
			playAt,
			togglePlay,
			next,
			prev,
			seekTo,
			saveCurrentProgress,
			switchAudioType,
			switchPronunciationType,
			stop,
			destroy,
		};
	},
	{
		persist: {
			key: "store-player",
			paths: ["currentAudioTypeId", "currentPronunciationTypeId", "currentVolumeId"],
		},
	}
);
