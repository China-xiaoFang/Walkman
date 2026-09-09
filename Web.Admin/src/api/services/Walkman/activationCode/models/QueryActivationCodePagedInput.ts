import type { PagedInput } from "fast-element-plus";

/**
 * 获取激活码分页列表输入
 */
export interface QueryActivationCodePagedInput extends PagedInput  {
	/**
	 * 是否已使用
	 */
	isUsed?: boolean;
	/**
	 * 
	 */
	readonly isOrderBy?: boolean;
}

