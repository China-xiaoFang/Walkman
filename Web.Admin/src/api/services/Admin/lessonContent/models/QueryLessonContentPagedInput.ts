import { PagedInput } from "fast-element-plus";

/**
 * Fast.Admin.Service.LessonContent.Dto.QueryLessonContentPagedInput 获取课程内容分页列表输入
 */
export interface QueryLessonContentPagedInput extends PagedInput {
	lessonId?: number;
}
