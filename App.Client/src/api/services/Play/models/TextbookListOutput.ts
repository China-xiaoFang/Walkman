/**
 * 教材列表输出
 */
export interface TextbookListOutput {
	/** 教材Id */
	textbookId?: number;
	/** 教材名称 */
	textbookName?: string;
	/** 封面地址 */
	coverUrl?: string;
	/** 描述 */
	description?: string;
}
