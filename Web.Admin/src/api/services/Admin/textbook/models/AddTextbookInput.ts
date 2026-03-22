/**
 * Fast.Admin.Service.Textbook.Dto.AddTextbookInput 添加教材输入
 */
export interface AddTextbookInput {
	textbookName?: string;
	coverUrl?: string;
	description?: string;
	sort?: number;
	remark?: string;
}
