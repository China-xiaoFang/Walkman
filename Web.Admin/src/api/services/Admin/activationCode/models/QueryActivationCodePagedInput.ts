import { PagedInput } from "fast-element-plus";
import { ActivationCodeStatusEnum } from "@/api/enums/ActivationCodeStatusEnum";

export interface QueryActivationCodePagedInput extends PagedInput {
	textbookId?: number;
	status?: ActivationCodeStatusEnum;
	accountId?: number;
}
