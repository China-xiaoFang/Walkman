/**
 * 歌词文档输出
 */
export interface QueryLyricDocumentOutput {
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
	 * 更新版本控制字段
	 */
	rowVersion?: string;
}

