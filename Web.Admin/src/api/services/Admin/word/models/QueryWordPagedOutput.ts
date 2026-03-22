/**
 * Fast.Admin.Service.Word.Dto.QueryWordPagedOutput 获取单词分页列表输出
 */
export interface QueryWordPagedOutput {
	wordId?: number;
	lessonId?: number;
	lessonName?: string;
	english?: string;
	chinese?: string;
	phonetic?: string;
	audioUrl?: string;
	exampleSentence?: string;
	exampleSentenceCn?: string;
	sort?: number;
	createdUserName?: string;
	createdTime?: Date;
	updatedUserName?: string;
	updatedTime?: Date;
	rowVersion?: number;
}
