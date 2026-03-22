/**
 * 激活状态输出
 */
export interface ActivationStatusOutput {
	/** 是否已激活 */
	isActivated?: boolean;
	/** 已激活的教材Id列表 */
	activatedTextbookIds?: number[];
}
