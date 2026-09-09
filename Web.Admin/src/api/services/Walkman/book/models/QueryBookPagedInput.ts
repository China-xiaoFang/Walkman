import type { PagedInput } from "fast-element-plus";
import type { CommonStatusEnum } from "@/api/enums/CommonStatusEnum";

/**
 * 获取教材分页列表输入
 */
export interface QueryBookPagedInput extends PagedInput  {
	/**
	 * 
	 */
	status?: CommonStatusEnum;
	/**
	 * 
	 */
	readonly isOrderBy?: boolean;
}

