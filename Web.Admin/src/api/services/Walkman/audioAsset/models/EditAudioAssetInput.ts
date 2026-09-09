import type { EditLyricDocumentInput } from "./EditLyricDocumentInput";
import type { AudioTypeEnum } from "@/api/enums/AudioTypeEnum";

/**
 * 编辑音频资源输入
 */
export interface EditAudioAssetInput {
	/**
	 * 音频资源Id
	 */
	audioAssetId?: string;
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
	lyricDocumentList?: EditLyricDocumentInput[];
	/**
	 * 
	 */
	rowVersion?: string;
}

