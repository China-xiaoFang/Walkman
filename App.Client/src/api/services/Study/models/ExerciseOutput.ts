import { QuestionTypeEnum } from "@/api/enums/QuestionTypeEnum";

export interface ExerciseOptionOutput {
	questionOptionId?: number;
	label?: string;
	content?: string;
}

export interface ExerciseOutput {
	questionId?: number;
	questionType?: QuestionTypeEnum;
	content?: string;
	score?: number;
	options?: ExerciseOptionOutput[];
}
