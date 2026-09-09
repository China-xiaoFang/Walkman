import type { CommonStatusEnum } from "@/api/enums/CommonStatusEnum";

/**
 * 获取教材分页列表输出
 */
export interface QueryBookPagedOutput {
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
	departmentName?: string;
	/**
	 * 
	 */
	createdUserName?: string;
	/**
	 * 
	 */
	createdTime?: string;
	/**
	 * 
	 */
	updatedUserName?: string;
	/**
	 * 
	 */
	updatedTime?: string;
	/**
	 * 
	 */
	rowVersion?: string;
}

