import { axiosUtil } from "@fast-china/axios";
import { WordListOutput } from "./models/WordListOutput";
import { LessonContentOutput } from "./models/LessonContentOutput";
import { ExerciseOutput } from "./models/ExerciseOutput";
import { SubmitExerciseInput } from "./models/SubmitExerciseInput";
import { ExerciseResultOutput } from "./models/ExerciseResultOutput";
import { UpdateWordStatusInput } from "./models/UpdateWordStatusInput";
import { RecordLearningInput } from "./models/RecordLearningInput";
import { RankingOutput } from "./models/RankingOutput";
import { LearningStatsOutput } from "./models/LearningStatsOutput";

/**
 * Fast.Admin.Service.Study.StudyService 学习服务Api
 */
export const studyApi = {
	/**
	 * 获取单词列表
	 */
	getWordList(lessonId: number) {
		return axiosUtil.request<WordListOutput[]>({
			url: "/study/getWordList",
			method: "get",
			params: { lessonId },
			requestType: "query",
		});
	},
	/**
	 * 获取课程内容
	 */
	getLessonContent(lessonId: number) {
		return axiosUtil.request<LessonContentOutput>({
			url: "/study/getLessonContent",
			method: "get",
			params: { lessonId },
			requestType: "query",
		});
	},
	/**
	 * 获取练习题目
	 */
	getExercises(lessonId: number) {
		return axiosUtil.request<ExerciseOutput[]>({
			url: "/study/getExercises",
			method: "get",
			params: { lessonId },
			requestType: "query",
		});
	},
	/**
	 * 提交练习答案
	 */
	submitExercise(data: SubmitExerciseInput) {
		return axiosUtil.request<ExerciseResultOutput>({
			url: "/study/submitExercise",
			method: "post",
			data,
			requestType: "edit",
			loading: true,
			loadingText: "提交中...",
		});
	},
	/**
	 * 更新单词状态
	 */
	updateWordStatus(data: UpdateWordStatusInput) {
		return axiosUtil.request({
			url: "/study/updateWordStatus",
			method: "post",
			data,
			requestType: "edit",
			showCodeMessage: false,
		});
	},
	/**
	 * 记录学习
	 */
	recordLearning(data: RecordLearningInput) {
		return axiosUtil.request({
			url: "/study/recordLearning",
			method: "post",
			data,
			requestType: "edit",
			showCodeMessage: false,
		});
	},
	/**
	 * 获取排行榜
	 */
	getRanking() {
		return axiosUtil.request<RankingOutput[]>({
			url: "/study/getRanking",
			method: "get",
			requestType: "query",
		});
	},
	/**
	 * 获取学习统计
	 */
	getLearningStats() {
		return axiosUtil.request<LearningStatsOutput>({
			url: "/study/getLearningStats",
			method: "get",
			requestType: "query",
		});
	},
	/**
	 * 获取每日签到
	 */
	getDailyCheckIn() {
		return axiosUtil.request<boolean>({
			url: "/study/getDailyCheckIn",
			method: "get",
			requestType: "query",
		});
	},
};
