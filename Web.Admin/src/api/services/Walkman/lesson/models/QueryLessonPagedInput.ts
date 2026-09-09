import type { PagedInput } from "fast-element-plus";

/**
 * 获取课程分页列表输入
 */
export interface QueryLessonPagedInput extends PagedInput  {
	/**
	 * 教材Id
	 */
	bookId?: string;
	/**
	 * 
	 */
	readonly isOrderBy?: boolean;
}

