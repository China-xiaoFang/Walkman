export interface AddAudioInput {
	lessonId?: number;
	audioTypeId?: number;
	pronunciationTypeId?: number;
	audioUrl?: string;
	duration?: number;
	sort?: number;
	remark?: string;
}
