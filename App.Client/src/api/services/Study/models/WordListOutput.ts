import { WordStatusEnum } from "@/api/enums/WordStatusEnum";

/**
 * 单词列表输出
 */
export interface WordListOutput {
	/** 单词Id */
	wordId?: number;
	/** 课程Id */
	lessonId?: number;
	/** 英文单词 */
	english?: string;
	/** 中文释义 */
	chinese?: string;
	/** 音标 */
	phonetic?: string;
	/** 音频地址 */
	audioUrl?: string;
	/** 例句（英文） */
	exampleSentence?: string;
	/** 例句（中文） */
	exampleSentenceCn?: string;
	/** 单词状态 */
	status?: WordStatusEnum;
}
