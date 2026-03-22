/**
 * Fast.Admin.Service.Word.Dto.AddWordInput 添加单词输入
 */
export interface AddWordInput {
	lessonId?: number;
	english?: string;
	chinese?: string;
	phonetic?: string;
	audioUrl?: string;
	exampleSentence?: string;
	exampleSentenceCn?: string;
	sort?: number;
}
