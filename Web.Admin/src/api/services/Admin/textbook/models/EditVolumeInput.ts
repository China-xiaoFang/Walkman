/**
 * Fast.Admin.Service.Textbook.Dto.EditVolumeInput 编辑册输入
 */
export interface EditVolumeInput {
	volumeId?: number;
	textbookId?: number;
	volumeName?: string;
	coverUrl?: string;
	sort?: number;
	remark?: string;
	rowVersion?: number;
}
