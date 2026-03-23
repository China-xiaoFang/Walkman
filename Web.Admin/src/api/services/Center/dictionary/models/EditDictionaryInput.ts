import { DictionaryValueTypeEnum } from "@/api/enums/DictionaryValueTypeEnum";
import { CommonStatusEnum } from "@/api/enums/CommonStatusEnum";
import { EditDictionaryItemInput } from "./EditDictionaryItemInput";

/**
 * Fast.Admin.Service.Dictionary.Dto.EditDictionaryInput 编辑字典输入
 */
export interface EditDictionaryInput {
  /**
   * 字典Id
   */
  dictionaryId?: number;
  /**
   * 字典名称
   */
  dictionaryName?: string;
  /**
   * 
   */
  valueType?: DictionaryValueTypeEnum;
  /**
   * 
   */
  status?: CommonStatusEnum;
  /**
   * 备注
   */
  remark?: string;
  /**
   * 字典项集合
   */
  dictionaryItemList?: Array<EditDictionaryItemInput>;
  /**
   * 
   */
  rowVersion?: number;
}

