<template>
	<view class="page lesson-detail-page">
		<!-- 课程标题 -->
		<view class="lesson-header">
			<text class="lesson-title">{{ state.lessonName }}</text>
			<view class="lesson-check-in" v-if="state.hasCheckedIn">
				<wd-icon name="check-circle-fill" size="32rpx" color="#52c41a" />
				<text>今日已学</text>
			</view>
		</view>

		<!-- 功能标签页 -->
		<wd-tabs v-model="state.activeTab" sticky>
			<wd-tab title="听力" name="listen">
				<view class="tab-content">
					<view class="listen-entry" @click="handleGoPlayer">
						<view class="entry-icon">
							<wd-icon name="play-circle-fill" size="80rpx" color="var(--wot-color-theme)" />
						</view>
						<text class="entry-title">开始听力练习</text>
						<text class="entry-desc">播放课程音频，支持多种音频类型</text>
					</view>
				</view>
			</wd-tab>

			<wd-tab title="阅读" name="read">
				<view class="tab-content">
					<template v-if="state.lessonContent">
						<view class="reading-section">
							<view class="section-header">
								<text class="section-title">课文原文</text>
								<wd-button size="small" type="text" @click="state.showTranslation = !state.showTranslation">
									{{ state.showTranslation ? '隐藏翻译' : '显示翻译' }}
								</wd-button>
							</view>
							<view class="reading-text english-text">
								<text>{{ state.lessonContent.englishText }}</text>
							</view>
							<view v-if="state.showTranslation" class="reading-text chinese-text">
								<text>{{ state.lessonContent.chineseText }}</text>
							</view>
						</view>

						<view v-if="state.lessonContent.grammarPoints" class="reading-section">
							<view class="section-header">
								<text class="section-title">语法要点</text>
							</view>
							<view class="reading-text grammar-text">
								<text>{{ state.lessonContent.grammarPoints }}</text>
							</view>
						</view>

						<view v-if="state.lessonContent.knowledgePoints" class="reading-section">
							<view class="section-header">
								<text class="section-title">知识点</text>
							</view>
							<view class="reading-text knowledge-text">
								<text>{{ state.lessonContent.knowledgePoints }}</text>
							</view>
						</view>
					</template>
					<view v-else class="empty-tip">
						<text>暂无课文内容</text>
					</view>
				</view>
			</wd-tab>

			<wd-tab title="单词" name="word">
				<view class="tab-content">
					<view class="word-stats" v-if="state.wordList.length > 0">
						<text>共 {{ state.wordList.length }} 个单词</text>
						<text class="mastered-count">已掌握 {{ state.masteredCount }}/{{ state.wordList.length }}</text>
					</view>
					<view class="word-list">
						<view
							v-for="item in state.wordList"
							:key="item.wordId"
							class="word-item"
							@click="handleWordClick(item)"
						>
							<view class="word-main">
								<view class="word-english-row">
									<text class="word-english">{{ item.english }}</text>
									<text v-if="item.phonetic" class="word-phonetic">{{ item.phonetic }}</text>
								</view>
								<text class="word-chinese">{{ item.chinese }}</text>
							</view>
							<view class="word-actions">
								<view class="play-btn" @click.stop="handlePlayWord(item)">
									<wd-icon name="volume" size="36rpx" color="var(--wot-color-theme)" />
								</view>
								<wd-tag
									:type="item.status === 3 ? 'success' : item.status === 2 ? 'warning' : 'default'"
									size="small"
									@click.stop="handleToggleWordStatus(item)"
								>
									{{ item.status === 3 ? '已掌握' : item.status === 2 ? '学习中' : '新词' }}
								</wd-tag>
							</view>
						</view>
					</view>
					<view v-if="state.wordList.length === 0" class="empty-tip">
						<text>暂无单词数据</text>
					</view>
				</view>
			</wd-tab>

			<wd-tab title="练习" name="exercise">
				<view class="tab-content">
					<view class="exercise-entry" @click="handleGoExercise">
						<wd-icon name="edit" size="80rpx" color="#ff9900" />
						<text class="entry-title">课后练习</text>
						<text class="entry-desc">检验学习成果，巩固所学知识</text>
					</view>
					<view class="exercise-entry" @click="handleGoTest">
						<wd-icon name="certificate" size="80rpx" color="#ee0a24" />
						<text class="entry-title">课程测试</text>
						<text class="entry-desc">系统测评，获取成绩排名</text>
					</view>
				</view>
			</wd-tab>
		</wd-tabs>

		<!-- 单词详情弹窗 -->
		<wd-popup v-model="state.showWordDetail" position="bottom" :style="{ height: '60vh' }" round>
			<view class="word-detail-popup" v-if="state.selectedWord">
				<view class="word-detail-header">
					<text class="word-detail-english">{{ state.selectedWord.english }}</text>
					<view class="play-btn" @click="handlePlayWord(state.selectedWord)">
						<wd-icon name="volume" size="48rpx" color="var(--wot-color-theme)" />
					</view>
				</view>
				<text v-if="state.selectedWord.phonetic" class="word-detail-phonetic">{{ state.selectedWord.phonetic }}</text>
				<text class="word-detail-chinese">{{ state.selectedWord.chinese }}</text>
				<view v-if="state.selectedWord.exampleSentence" class="word-detail-example">
					<text class="example-label">例句：</text>
					<text class="example-en">{{ state.selectedWord.exampleSentence }}</text>
					<text v-if="state.selectedWord.exampleSentenceCn" class="example-cn">{{ state.selectedWord.exampleSentenceCn }}</text>
				</view>
				<view class="word-detail-actions">
					<wd-button type="error" size="small" plain @click="handleSetWordStatus(state.selectedWord, 1)">标为新词</wd-button>
					<wd-button type="warning" size="small" plain @click="handleSetWordStatus(state.selectedWord, 2)">学习中</wd-button>
					<wd-button type="success" size="small" plain @click="handleSetWordStatus(state.selectedWord, 3)">已掌握</wd-button>
				</view>
			</view>
		</wd-popup>
	</view>
</template>

<script setup lang="ts">
import { onLoad, onUnload } from "@dcloudio/uni-app";
import { reactive, computed } from "vue";
import { useRouter } from "uni-mini-router";
import { studyApi } from "@/api/services/Study";
import { LearningTypeEnum } from "@/api/enums/LearningTypeEnum";
import { WordStatusEnum } from "@/api/enums/WordStatusEnum";
import type { WordListOutput } from "@/api/services/Study/models/WordListOutput";
import type { LessonContentOutput } from "@/api/services/Study/models/LessonContentOutput";

definePage({
	name: "WalkmanLessonDetail",
	layout: "layout",
	style: {
		navigationBarTitleText: "课程详情",
	},
});

const router = useRouter();

let wordAudioContext: UniApp.InnerAudioContext | null = null;
let startTime = 0;

const state = reactive({
	lessonId: 0,
	lessonName: "",
	volumeId: 0,
	activeTab: "listen",
	showTranslation: false,
	showWordDetail: false,
	hasCheckedIn: false,
	lessonContent: null as LessonContentOutput | null,
	wordList: [] as WordListOutput[],
	selectedWord: null as WordListOutput | null,
	masteredCount: computed(() => state.wordList.filter((w) => w.status === WordStatusEnum.Mastered).length),
});

/** 跳转到播放器 */
const handleGoPlayer = () => {
	router.push({
		path: "/pages/walkman/player/index",
		query: {
			volumeId: state.volumeId,
			lessonId: state.lessonId,
		},
	});
};

/** 跳转到练习 */
const handleGoExercise = () => {
	router.push({
		path: "/pages/walkman/exercise/index",
		query: {
			lessonId: state.lessonId,
			lessonName: state.lessonName,
			type: LearningTypeEnum.Exercise,
		},
	});
};

/** 跳转到测试 */
const handleGoTest = () => {
	router.push({
		path: "/pages/walkman/exercise/index",
		query: {
			lessonId: state.lessonId,
			lessonName: state.lessonName,
			type: LearningTypeEnum.Test,
		},
	});
};

/** 播放单词音频 */
const handlePlayWord = (word: WordListOutput) => {
	if (!word.audioUrl) {
		uni.showToast({ title: "暂无音频", icon: "none" });
		return;
	}
	if (!wordAudioContext) {
		wordAudioContext = uni.createInnerAudioContext();
	}
	wordAudioContext.src = word.audioUrl;
	wordAudioContext.play();
};

/** 点击单词显示详情 */
const handleWordClick = (word: WordListOutput) => {
	state.selectedWord = word;
	state.showWordDetail = true;
};

/** 切换单词状态 */
const handleToggleWordStatus = async (word: WordListOutput) => {
	const nextStatus =
		word.status === WordStatusEnum.Mastered
			? WordStatusEnum.New
			: word.status === WordStatusEnum.Learning
				? WordStatusEnum.Mastered
				: WordStatusEnum.Learning;
	await handleSetWordStatus(word, nextStatus);
};

/** 设置单词状态 */
const handleSetWordStatus = async (word: WordListOutput, status: WordStatusEnum) => {
	try {
		await studyApi.updateWordStatus({ wordId: word.wordId, status });
		word.status = status;
		state.showWordDetail = false;
	} catch {
		// handled by framework
	}
};

/** 加载课程内容 */
const loadLessonContent = async () => {
	try {
		state.lessonContent = await studyApi.getLessonContent(state.lessonId);
	} catch {
		state.lessonContent = null;
	}
};

/** 加载单词列表 */
const loadWordList = async () => {
	try {
		state.wordList = await studyApi.getWordList(state.lessonId);
	} catch {
		state.wordList = [];
	}
};

/** 检查今日签到 */
const checkDailyCheckIn = async () => {
	try {
		state.hasCheckedIn = await studyApi.getDailyCheckIn();
	} catch {
		state.hasCheckedIn = false;
	}
};

onLoad(async (options: any) => {
	state.lessonId = Number(options?.lessonId || 0);
	state.lessonName = options?.lessonName || "";
	state.volumeId = Number(options?.volumeId || 0);
	if (state.lessonName) {
		uni.setNavigationBarTitle({ title: state.lessonName });
	}
	startTime = Date.now();

	await Promise.all([loadLessonContent(), loadWordList(), checkDailyCheckIn()]);
});

onUnload(() => {
	// Record learning time
	const duration = Math.floor((Date.now() - startTime) / 1000);
	if (duration > 5 && state.lessonId) {
		studyApi.recordLearning({
			lessonId: state.lessonId,
			learningType: LearningTypeEnum.Read,
			duration,
		});
	}
	// Destroy audio context
	if (wordAudioContext) {
		wordAudioContext.destroy();
		wordAudioContext = null;
	}
});
</script>

<style scoped lang="scss">
@import "./index.scss";
</style>
