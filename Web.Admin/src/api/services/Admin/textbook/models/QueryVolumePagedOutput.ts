/**
 * Fast.Admin.Service.Textbook.Dto.QueryVolumePagedOutput 获取册分页列表输出
 */
export interface QueryVolumePagedOutput {
	volumeId?: number;
	textbookId?: number;
	textbookName?: string;
	volumeName?: string;
	coverUrl?: string;
	sort?: number;
	remark?: string;
	departmentName?: string;
	createdUserName?: string;
	createdTime?: Date;
	updatedUserName?: string;
	updatedTime?: Date;
	rowVersion?: number;
}
