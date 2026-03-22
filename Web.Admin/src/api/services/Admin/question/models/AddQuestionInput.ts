import { QuestionTypeEnum } from "@/api/enums/QuestionTypeEnum";

/**
 * Fast.Admin.Service.Question.Dto.AddQuestionOptionInput 添加题目选项输入
 */
export interface AddQuestionOptionInput {
	label?: string;
	content?: string;
	isCorrect?: boolean;
	sort?: number;
}

/**
 * Fast.Admin.Service.Question.Dto.AddQuestionInput 添加题目输入
 */
export interface AddQuestionInput {
	lessonId?: number;
	questionType?: QuestionTypeEnum;
	content?: string;
	answer?: string;
	explanation?: string;
	score?: number;
	sort?: number;
	options?: AddQuestionOptionInput[];
}
