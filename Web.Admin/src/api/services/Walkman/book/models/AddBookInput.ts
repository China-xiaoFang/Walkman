/**
 * 添加教材输入
 */
export interface AddBookInput {
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
	 * 排序
	 */
	sort?: number;
	/**
	 * 备注
	 */
	remark?: string;
}

