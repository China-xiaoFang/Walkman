/**
 * Fast.Admin.Service.Textbook.Dto.QueryTextbookPagedOutput 获取教材分页列表输出
 */
export interface QueryTextbookPagedOutput {
	textbookId?: number;
	textbookName?: string;
	coverUrl?: string;
	description?: string;
	sort?: number;
	remark?: string;
	departmentName?: string;
	createdUserName?: string;
	createdTime?: Date;
	updatedUserName?: string;
	updatedTime?: Date;
	rowVersion?: number;
}
