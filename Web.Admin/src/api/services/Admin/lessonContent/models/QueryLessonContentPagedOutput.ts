/**
 * Fast.Admin.Service.LessonContent.Dto.QueryLessonContentPagedOutput 获取课程内容分页列表输出
 */
export interface QueryLessonContentPagedOutput {
	lessonContentId?: number;
	lessonId?: number;
	lessonName?: string;
	englishText?: string;
	chineseText?: string;
	grammarPoints?: string;
	knowledgePoints?: string;
	createdUserName?: string;
	createdTime?: Date;
	updatedUserName?: string;
	updatedTime?: Date;
	rowVersion?: number;
}
