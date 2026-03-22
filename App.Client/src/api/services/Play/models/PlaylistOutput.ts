/**
 * 播放列表项输出
 */
export interface PlaylistOutput {
	/** 课程Id */
	lessonId?: number;
	/** 课程名称 */
	lessonName?: string;
	/** 课程编号 */
	lessonNo?: number;
	/** 音频Id */
	audioId?: number;
	/** 音频地址 */
	audioUrl?: string;
	/** 音频时长（秒） */
	duration?: number;
	/** 音频类型Id */
	audioTypeId?: number;
	/** 音频类型名称 */
	audioTypeName?: string;
	/** 发音类型Id */
	pronunciationTypeId?: number;
	/** 发音类型名称 */
	pronunciationTypeName?: string;
	/** 是否免费 */
	isFree?: boolean;
	/** 是否可访问 */
	isAccessible?: boolean;
	/** 播放进度（秒） */
	progress?: number;
}
