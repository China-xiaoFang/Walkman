import { QuestionTypeEnum } from "@/api/enums/QuestionTypeEnum";

/**
 * Fast.Admin.Service.Question.Dto.QueryQuestionOptionOutput 获取题目选项输出
 */
export interface QueryQuestionOptionOutput {
	questionOptionId?: number;
	label?: string;
	content?: string;
	isCorrect?: boolean;
	sort?: number;
}

/**
 * Fast.Admin.Service.Question.Dto.QueryQuestionDetailOutput 获取题目详情输出
 */
export interface QueryQuestionDetailOutput {
	questionId?: number;
	lessonId?: number;
	questionType?: QuestionTypeEnum;
	content?: string;
	answer?: string;
	explanation?: string;
	score?: number;
	sort?: number;
	options?: QueryQuestionOptionOutput[];
	rowVersion?: number;
}
