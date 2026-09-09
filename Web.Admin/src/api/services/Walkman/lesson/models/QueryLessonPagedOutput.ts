/**
 * 获取课程分页列表输出
 */
export interface QueryLessonPagedOutput {
	/**
	 * 课程Id
	 */
	lessonId?: string;
	/**
	 * 教材Id
	 */
	bookId?: string;
	/**
	 * 教材名称
	 */
	bookName?: string;
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

