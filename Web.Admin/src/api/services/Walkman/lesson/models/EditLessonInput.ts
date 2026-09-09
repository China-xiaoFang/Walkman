/**
 * 编辑课程输入
 */
export interface EditLessonInput {
	/**
	 * 课程Id
	 */
	lessonId?: string;
	/**
	 * 教材Id
	 */
	bookId?: string;
	/**
	 * 课程标题
	 */
	lessonTitle?: string;
	/**
	 * 课程编号
	 */
	lessonNumber?: number;
	/**
	 * 英文
	 */
	english?: string;
	/**
	 * 中文
	 */
	chinese?: string;
	/**
	 * 备注
	 */
	remark?: string;
	/**
	 * 
	 */
	rowVersion?: string;
}

