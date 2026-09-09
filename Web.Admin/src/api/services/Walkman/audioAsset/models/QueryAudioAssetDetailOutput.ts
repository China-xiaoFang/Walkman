import type { QueryLyricDocumentOutput } from "./QueryLyricDocumentOutput";
import type { AudioTypeEnum } from "@/api/enums/AudioTypeEnum";

/**
 * 获取音频资源详情输出
 */
export interface QueryAudioAssetDetailOutput {
	/**
	 * 音频资源Id
	 */
	audioAssetId?: string;
	/**
	 * 教材Id
	 */
	bookId?: string;
	/**
	 * 教材名称
	 */
	bookName?: string;
	/**
	 * 课程Id
	 */
	lessonId?: string;
	/**
	 * 课程标题
	 */
	lessonTitle?: string;
	/**
	 * 课程编号
	 */
	lessonNumber?: number;
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
	lyricDocumentList?: QueryLyricDocumentOutput[];
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

