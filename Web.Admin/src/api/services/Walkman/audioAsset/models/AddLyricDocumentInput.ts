/**
 * 添加歌词文档输入
 */
export interface AddLyricDocumentInput {
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
}

