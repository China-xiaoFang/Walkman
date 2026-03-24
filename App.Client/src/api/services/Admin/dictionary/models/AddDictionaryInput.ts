import { DictionaryValueTypeEnum } from "@/api/enums/DictionaryValueTypeEnum";
import { AddDictionaryItemInput } from "./AddDictionaryItemInput";

/**
 * Fast.Admin.Service.Dictionary.Dto.AddDictionaryInput 添加字典输入
 */
export interface AddDictionaryInput {
  /**
   * 字典Key
   */
  dictionaryKey?: string;
  /**
   * 字典名称
   */
  dictionaryName?: string;
  /**
   * 
   */
  valueType?: DictionaryValueTypeEnum;
  /**
   * 备注
   */
  remark?: string;
  /**
   * 字典项集合
   */
  dictionaryItemList?: Array<AddDictionaryItemInput>;
}

