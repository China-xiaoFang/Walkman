/**
 * 课程列表输出
 */
export interface LessonListOutput {
	/** 课程Id */
	lessonId?: number;
	/** 册Id */
	volumeId?: number;
	/** 课程名称 */
	lessonName?: string;
	/** 课程编号 */
	lessonNo?: number;
	/** 是否免费 */
	isFree?: boolean;
	/** 是否可访问 */
	isAccessible?: boolean;
}
