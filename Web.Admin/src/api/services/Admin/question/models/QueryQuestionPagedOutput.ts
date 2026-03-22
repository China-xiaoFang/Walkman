import { QuestionTypeEnum } from "@/api/enums/QuestionTypeEnum";

/**
 * Fast.Admin.Service.Question.Dto.QueryQuestionPagedOutput 获取题目分页列表输出
 */
export interface QueryQuestionPagedOutput {
	questionId?: number;
	lessonId?: number;
	lessonName?: string;
	questionType?: QuestionTypeEnum;
	content?: string;
	answer?: string;
	explanation?: string;
	score?: number;
	sort?: number;
	createdUserName?: string;
	createdTime?: Date;
	updatedUserName?: string;
	updatedTime?: Date;
	rowVersion?: number;
}
