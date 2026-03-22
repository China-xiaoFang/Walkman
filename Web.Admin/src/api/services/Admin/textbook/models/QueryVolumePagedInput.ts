import { PagedInput } from "fast-element-plus";

/**
 * Fast.Admin.Service.Textbook.Dto.QueryVolumePagedInput 获取册分页列表输入
 */
export interface QueryVolumePagedInput extends PagedInput {
	textbookId?: number;
}
