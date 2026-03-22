/**
 * Fast.Admin.Service.Textbook.Dto.EditTextbookInput 编辑教材输入
 */
export interface EditTextbookInput {
	textbookId?: number;
	textbookName?: string;
	coverUrl?: string;
	description?: string;
	sort?: number;
	remark?: string;
	rowVersion?: number;
}
