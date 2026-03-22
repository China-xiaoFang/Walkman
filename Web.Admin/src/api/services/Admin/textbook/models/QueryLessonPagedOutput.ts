/**
 * Fast.Admin.Service.Textbook.Dto.QueryLessonPagedOutput 获取课程分页列表输出
 */
export interface QueryLessonPagedOutput {
	lessonId?: number;
	volumeId?: number;
	volumeName?: string;
	lessonName?: string;
	lessonNo?: number;
	sort?: number;
	remark?: string;
	departmentName?: string;
	createdUserName?: string;
	createdTime?: Date;
	updatedUserName?: string;
	updatedTime?: Date;
	rowVersion?: number;
}
