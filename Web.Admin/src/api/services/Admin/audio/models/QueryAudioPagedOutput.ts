export interface QueryAudioPagedOutput {
	audioId?: number;
	lessonId?: number;
	lessonName?: string;
	audioTypeId?: number;
	audioTypeName?: string;
	pronunciationTypeId?: number;
	pronunciationTypeName?: string;
	audioUrl?: string;
	duration?: number;
	sort?: number;
	remark?: string;
	departmentName?: string;
	createdUserName?: string;
	createdTime?: Date;
	updatedUserName?: string;
	updatedTime?: Date;
	rowVersion?: number;
}
