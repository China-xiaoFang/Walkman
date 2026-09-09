import type { PagedInput } from "fast-element-plus";
import type { AudioTypeEnum } from "@/api/enums/AudioTypeEnum";

/**
 * 获取音频资源分页列表输入
 */
export interface QueryAudioAssetPagedInput extends PagedInput  {
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
	 * 
	 */
	readonly isOrderBy?: boolean;
}

