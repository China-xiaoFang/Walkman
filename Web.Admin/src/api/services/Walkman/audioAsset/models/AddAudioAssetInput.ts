import type { AddLyricDocumentInput } from "./AddLyricDocumentInput";
import type { AudioTypeEnum } from "@/api/enums/AudioTypeEnum";

/**
 * 添加音频资源输入
 */
export interface AddAudioAssetInput {
	/**
	 * 教材Id
	 */
	bookId?: string;
	/**
	 * 课程Id
	 */
	lessonId?: string;
	/**
	 * 
	 */
	audioType?: AudioTypeEnum;
	/**
	 * 音频地址
	 */
	audioUrl?: string;
	/**
	 * 音频时长
	 */
	audioDuration?: string;
	/**
	 * 歌词文档集合
	 */
	lyricDocumentList?: AddLyricDocumentInput[];
}

