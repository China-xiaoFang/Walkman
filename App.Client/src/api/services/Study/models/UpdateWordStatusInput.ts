import { WordStatusEnum } from "@/api/enums/WordStatusEnum";

export interface UpdateWordStatusInput {
	wordId?: number;
	status?: WordStatusEnum;
}
