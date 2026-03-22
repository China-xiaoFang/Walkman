import { PagedInput } from "fast-element-plus";

/**
 * Fast.Admin.Service.Textbook.Dto.QueryLessonPagedInput 获取课程分页列表输入
 */
export interface QueryLessonPagedInput extends PagedInput {
	volumeId?: number;
}
