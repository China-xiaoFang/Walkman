export interface DailyLearningOutput {
	date?: string;
	duration?: number;
	lessonCount?: number;
	wordCount?: number;
}

export interface LearningStatsOutput {
	totalDuration?: number;
	totalLessons?: number;
	totalWords?: number;
	masteredWords?: number;
	learningWords?: number;
	totalExercises?: number;
	totalTests?: number;
	averageScore?: number;
	continuousDays?: number;
	dailyRecords?: DailyLearningOutput[];
}
