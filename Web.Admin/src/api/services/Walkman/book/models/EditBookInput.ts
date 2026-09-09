import type { CommonStatusEnum } from "@/api/enums/CommonStatusEnum";

/**
 * 编辑教材输入
 */
export interface EditBookInput {
	/**
	 * 教材Id
	 */
	bookId?: string;
	/**
	 * 教材名称
	 */
	bookName?: string;
	/**
	 * 教材简介
	 */
	description?: string;
	/**
	 * 封面地址
	 */
	coverUrl?: string;
	/**
	 * 
	 */
	status?: CommonStatusEnum;
	/**
	 * 排序
	 */
	sort?: number;
	/**
	 * 备注
	 */
	remark?: string;
	/**
	 * 
	 */
	rowVersion?: string;
}

