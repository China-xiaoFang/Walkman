/**
 * 获取播放列表输入
 */
export interface PlaylistInput {
	/** 册Id */
	volumeId?: number;
	/** 起始课程Id */
	startLessonId?: number;
	/** 音频类型Id */
	audioTypeId?: number;
	/** 发音类型Id */
	pronunciationTypeId?: number;
}
