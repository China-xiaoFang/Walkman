import { PagedInput } from "fast-element-plus";
import { QuestionTypeEnum } from "@/api/enums/QuestionTypeEnum";

/**
 * Fast.Admin.Service.Question.Dto.QueryQuestionPagedInput 获取题目分页列表输入
 */
export interface QueryQuestionPagedInput extends PagedInput {
	lessonId?: number;
	questionType?: QuestionTypeEnum;
}
