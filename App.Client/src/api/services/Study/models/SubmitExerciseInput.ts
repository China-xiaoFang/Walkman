import { LearningTypeEnum } from "@/api/enums/LearningTypeEnum";

export interface AnswerItem {
	questionId?: number;
	answer?: string;
}

export interface SubmitExerciseInput {
	lessonId?: number;
	learningType?: LearningTypeEnum;
	answers?: AnswerItem[];
	duration?: number;
}
