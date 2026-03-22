/**
 * 保存播放进度输入
 */
export interface SaveProgressInput {
	/** 课程Id */
	lessonId?: number;
	/** 音频类型Id */
	audioTypeId?: number;
	/** 发音类型Id */
	pronunciationTypeId?: number;
	/** 播放进度（秒） */
	progress?: number;
	/** 音频总时长（秒） */
	duration?: number;
}
