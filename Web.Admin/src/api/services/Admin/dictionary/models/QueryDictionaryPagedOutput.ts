import { DictionaryValueTypeEnum } from "@/api/enums/DictionaryValueTypeEnum";
import { CommonStatusEnum } from "@/api/enums/CommonStatusEnum";

/**
 * Fast.Admin.Service.Dictionary.Dto.QueryDictionaryPagedOutput 获取字典分页列表输出
 */
export interface QueryDictionaryPagedOutput {
  /**
   * 字典Id
   */
  dictionaryId?: number;
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
   * Flags枚举
   */
  hasFlags?: boolean;
  /**
   * 
   */
  status?: CommonStatusEnum;
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
  createdTime?: Date;
  /**
   * 
   */
  updatedUserName?: string;
  /**
   * 
   */
  updatedTime?: Date;
  /**
   * 
   */
  rowVersion?: number;
}

