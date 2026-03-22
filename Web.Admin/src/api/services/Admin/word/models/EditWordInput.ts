/**
 * Fast.Admin.Service.Word.Dto.EditWordInput 编辑单词输入
 */
export interface EditWordInput {
	wordId?: number;
	lessonId?: number;
	english?: string;
	chinese?: string;
	phonetic?: string;
	audioUrl?: string;
	exampleSentence?: string;
	exampleSentenceCn?: string;
	sort?: number;
	rowVersion?: number;
}
