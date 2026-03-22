/**
 * Fast.Admin.Service.LessonContent.Dto.EditLessonContentInput 编辑课程内容输入
 */
export interface EditLessonContentInput {
	lessonContentId?: number;
	lessonId?: number;
	englishText?: string;
	chineseText?: string;
	grammarPoints?: string;
	knowledgePoints?: string;
	rowVersion?: number;
}
