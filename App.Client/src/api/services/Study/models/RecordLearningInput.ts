import { LearningTypeEnum } from "@/api/enums/LearningTypeEnum";

export interface RecordLearningInput {
	lessonId?: number;
	learningType?: LearningTypeEnum;
	duration?: number;
}
