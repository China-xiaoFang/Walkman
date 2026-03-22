<template>
	<view class="page exercise-page">
		<!-- 答题中 -->
		<template v-if="!state.submitted">
			<view class="exercise-progress">
				<text>{{ state.currentIndex + 1 }} / {{ state.exercises.length }}</text>
				<wd-progress :percentage="((state.currentIndex + 1) / Math.max(state.exercises.length, 1)) * 100" />
			</view>

			<view class="question-card" v-if="currentQuestion">
				<view class="question-type">
					<wd-tag size="small" type="primary">{{ getQuestionTypeName(currentQuestion.questionType) }}</wd-tag>
					<text class="question-score">{{ currentQuestion.score }}分</text>
				</view>
				<text class="question-content">{{ currentQuestion.content }}</text>

				<!-- 选择题选项 -->
				<view v-if="currentQuestion.questionType === 1 || currentQuestion.questionType === 2" class="options-list">
					<view
						v-for="opt in currentQuestion.options"
						:key="opt.questionOptionId"
						class="option-item"
						:class="{ selected: isSelected(opt.label) }"
						@click="handleSelectOption(opt.label)"
					>
						<view class="option-label">{{ opt.label }}</view>
						<text class="option-content">{{ opt.content }}</text>
					</view>
				</view>

				<!-- 填空/翻译/判断 -->
				<view v-else class="input-area">
					<wd-input
						v-if="currentQuestion.questionType === 4"
						v-model="state.answers[state.currentIndex]"
						placeholder="请输入 True 或 False"
					/>
					<wd-textarea
						v-else
						v-model="state.answers[state.currentIndex]"
						placeholder="请输入答案"
						:maxlength="500"
					/>
				</view>
			</view>

			<view class="exercise-actions">
				<wd-button v-if="state.currentIndex > 0" plain @click="state.currentIndex--">上一题</wd-button>
				<wd-button v-if="state.currentIndex < state.exercises.length - 1" type="primary" @click="state.currentIndex++">下一题</wd-button>
				<wd-button v-else type="success" @click="handleSubmit">提交</wd-button>
			</view>
		</template>

		<!-- 结果页 -->
		<template v-if="state.submitted && state.result">
			<view class="result-card">
				<view class="result-score">
					<text class="score-number">{{ state.result.score }}</text>
					<text class="score-total">/ {{ state.result.totalScore }}</text>
				</view>
				<text class="result-info">答对 {{ state.result.correctCount }} / {{ state.result.totalCount }} 题</text>
			</view>

			<view class="result-list">
				<view
					v-for="(item, index) in state.result.results"
					:key="item.questionId"
					class="result-item"
					:class="{ correct: item.isCorrect, wrong: !item.isCorrect }"
				>
					<view class="result-item-header">
						<text>第{{ index + 1 }}题</text>
						<wd-icon :name="item.isCorrect ? 'check-circle-fill' : 'close-circle-fill'" :color="item.isCorrect ? '#52c41a' : '#ee0a24'" size="36rpx" />
					</view>
					<text class="result-answer">你的答案：{{ item.userAnswer || '未作答' }}</text>
					<text v-if="!item.isCorrect" class="result-correct">正确答案：{{ item.correctAnswer }}</text>
					<text v-if="item.explanation" class="result-explanation">解析：{{ item.explanation }}</text>
				</view>
			</view>

			<view class="result-actions">
				<wd-button type="primary" block @click="handleRetry">重新作答</wd-button>
			</view>
		</template>

		<view v-if="state.exercises.length === 0 && !state.loading" class="empty-tip">
			<text>暂无练习题目</text>
		</view>
	</view>
</template>

<script setup lang="ts">
import { onLoad } from "@dcloudio/uni-app";
import { reactive, computed } from "vue";
import { studyApi } from "@/api/services/Study";
import { LearningTypeEnum } from "@/api/enums/LearningTypeEnum";
import { QuestionTypeEnum } from "@/api/enums/QuestionTypeEnum";
import type { ExerciseOutput } from "@/api/services/Study/models/ExerciseOutput";
import type { ExerciseResultOutput } from "@/api/services/Study/models/ExerciseResultOutput";

definePage({
	name: "WalkmanExercise",
	layout: "layout",
	style: {
		navigationBarTitleText: "练习",
	},
});

const state = reactive({
	lessonId: 0,
	lessonName: "",
	learningType: LearningTypeEnum.Exercise as LearningTypeEnum,
	exercises: [] as ExerciseOutput[],
	answers: [] as string[],
	currentIndex: 0,
	submitted: false,
	result: null as ExerciseResultOutput | null,
	loading: false,
	startTime: 0,
});

const currentQuestion = computed(() => state.exercises[state.currentIndex] || null);

const getQuestionTypeName = (type: QuestionTypeEnum) => {
	const names = { 1: "单选题", 2: "多选题", 3: "填空题", 4: "判断题", 5: "翻译题" };
	return names[type] || "";
};

const isSelected = (label: string) => {
	const answer = state.answers[state.currentIndex] || "";
	return answer.includes(label);
};

const handleSelectOption = (label: string) => {
	const q = currentQuestion.value;
	if (!q) return;
	if (q.questionType === QuestionTypeEnum.SingleChoice) {
		state.answers[state.currentIndex] = label;
	} else {
		const current = state.answers[state.currentIndex] || "";
		const labels = current ? current.split(",") : [];
		const idx = labels.indexOf(label);
		if (idx >= 0) labels.splice(idx, 1);
		else labels.push(label);
		labels.sort();
		state.answers[state.currentIndex] = labels.join(",");
	}
};

const handleSubmit = async () => {
	const duration = Math.floor((Date.now() - state.startTime) / 1000);
	try {
		state.result = await studyApi.submitExercise({
			lessonId: state.lessonId,
			learningType: state.learningType,
			duration,
			answers: state.exercises.map((q, i) => ({
				questionId: q.questionId,
				answer: state.answers[i] || "",
			})),
		});
		state.submitted = true;
	} catch {
		// handled by framework
	}
};

const handleRetry = () => {
	state.submitted = false;
	state.result = null;
	state.answers = new Array(state.exercises.length).fill("");
	state.currentIndex = 0;
	state.startTime = Date.now();
};

const loadExercises = async () => {
	if (!state.lessonId) return;
	try {
		state.loading = true;
		state.exercises = await studyApi.getExercises(state.lessonId);
		state.answers = new Array(state.exercises.length).fill("");
	} catch {
		state.exercises = [];
	} finally {
		state.loading = false;
	}
};

onLoad(async (options: any) => {
	state.lessonId = Number(options?.lessonId || 0);
	state.lessonName = options?.lessonName || "";
	state.learningType = Number(options?.type || LearningTypeEnum.Exercise);
	const title = state.learningType === LearningTypeEnum.Test ? "课程测试" : "课后练习";
	uni.setNavigationBarTitle({ title: state.lessonName ? `${title} - ${state.lessonName}` : title });
	state.startTime = Date.now();
	await loadExercises();
});
</script>

<style scoped lang="scss">
@import "./index.scss";
</style>
