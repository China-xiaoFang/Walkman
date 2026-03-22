import { QuestionTypeEnum } from "@/api/enums/QuestionTypeEnum";
import { AddQuestionOptionInput } from "./AddQuestionInput";

/**
 * Fast.Admin.Service.Question.Dto.EditQuestionInput 编辑题目输入
 */
export interface EditQuestionInput {
	questionId?: number;
	lessonId?: number;
	questionType?: QuestionTypeEnum;
	content?: string;
	answer?: string;
	explanation?: string;
	score?: number;
	sort?: number;
	options?: AddQuestionOptionInput[];
	rowVersion?: number;
}
