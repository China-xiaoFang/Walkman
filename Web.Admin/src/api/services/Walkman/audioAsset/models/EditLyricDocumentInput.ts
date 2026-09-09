/**
 * 编辑歌词文档输入
 */
export interface EditLyricDocumentInput {
	/**
	 * 记录Id
	 */
	recordId?: string;
	/**
	 * 英文
	 */
	english?: string;
	/**
	 * 中文
	 */
	chinese?: string;
	/**
	 * 开始时间
	 */
	startTime?: string;
	/**
	 * 结束时间
	 */
	endTime?: string;
	/**
	 * 
	 */
	rowVersion?: string;
}

