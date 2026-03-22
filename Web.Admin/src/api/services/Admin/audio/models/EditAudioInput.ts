export interface EditAudioInput {
	audioId?: number;
	lessonId?: number;
	audioTypeId?: number;
	pronunciationTypeId?: number;
	audioUrl?: string;
	duration?: number;
	sort?: number;
	remark?: string;
	rowVersion?: number;
}
