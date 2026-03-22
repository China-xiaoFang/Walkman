/**
 * Fast.Admin.Service.Textbook.Dto.AddVolumeInput 添加册输入
 */
export interface AddVolumeInput {
	textbookId?: number;
	volumeName?: string;
	coverUrl?: string;
	sort?: number;
	remark?: string;
}
