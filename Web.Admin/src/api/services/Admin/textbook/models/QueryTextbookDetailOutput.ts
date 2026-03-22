/**
 * Fast.Admin.Service.Textbook.Dto.QueryTextbookDetailOutput 获取教材详情输出
 */
export interface QueryTextbookDetailOutput {
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
