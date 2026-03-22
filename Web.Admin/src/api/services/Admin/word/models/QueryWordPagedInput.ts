import { PagedInput } from "fast-element-plus";

/**
 * Fast.Admin.Service.Word.Dto.QueryWordPagedInput 获取单词分页列表输入
 */
export interface QueryWordPagedInput extends PagedInput {
	lessonId?: number;
}
