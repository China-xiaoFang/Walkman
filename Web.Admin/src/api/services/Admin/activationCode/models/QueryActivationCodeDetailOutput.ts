import { ActivationCodeStatusEnum } from "@/api/enums/ActivationCodeStatusEnum";

export interface QueryActivationCodeDetailOutput {
	activationCodeId?: number;
	code?: string;
	textbookId?: number;
	textbookName?: string;
	accountId?: number;
	status?: ActivationCodeStatusEnum;
	usedTime?: Date;
	remark?: string;
	departmentName?: string;
	createdUserName?: string;
	createdTime?: Date;
	updatedUserName?: string;
	updatedTime?: Date;
	rowVersion?: number;
}
