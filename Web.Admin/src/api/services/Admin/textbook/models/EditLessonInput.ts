/**
 * Fast.Admin.Service.Textbook.Dto.EditLessonInput 编辑课程输入
 */
export interface EditLessonInput {
	lessonId?: number;
	volumeId?: number;
	lessonName?: string;
	lessonNo?: number;
	sort?: number;
	remark?: string;
	rowVersion?: number;
}
