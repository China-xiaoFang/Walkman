export interface AnswerResultItem {
	questionId?: number;
	isCorrect?: boolean;
	correctAnswer?: string;
	userAnswer?: string;
	explanation?: string;
}

export interface ExerciseResultOutput {
	totalScore?: number;
	score?: number;
	correctCount?: number;
	totalCount?: number;
	results?: AnswerResultItem[];
}
