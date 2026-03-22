import { PagedInput } from "fast-element-plus";
export interface QueryAudioPagedInput extends PagedInput {
	lessonId?: number;
	audioTypeId?: number;
	pronunciationTypeId?: number;
}
