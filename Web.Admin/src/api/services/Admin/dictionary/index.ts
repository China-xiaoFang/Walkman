import { axiosUtil } from "@fast-china/axios";
import { ElSelectorOutput, FaTableEnumColumnCtx, PagedInput, PagedResult } from "fast-element-plus";
import { QueryDictionaryPagedOutput } from "./models/QueryDictionaryPagedOutput";
import { QueryDictionaryDetailOutput } from "./models/QueryDictionaryDetailOutput";
import { AddDictionaryInput } from "./models/AddDictionaryInput";
import { EditDictionaryInput } from "./models/EditDictionaryInput";
import { DictionaryIdInput } from "./models/DictionaryIdInput";

/**
 * Fast.Admin.Service.Dictionary.DictionaryService 字典服务Api
 */
export const dictionaryApi = {
  /**
   * 获取字典
   */
  queryDictionary() {
    return axiosUtil.request<Record<string, FaTableEnumColumnCtx[]>>({
      url: "/dictionary/queryDictionary",
      method: "get",
      requestType: "query",
    });
  },
  /**
   * 字典分页选择器
   */
  selectorPaged(data: PagedInput) {
    return axiosUtil.request<PagedResult<ElSelectorOutput<number>>>({
      url: "/dictionary/selectorPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
  /**
   * 获取字典分页列表
   */
  queryDictionaryPaged(data: PagedInput) {
    return axiosUtil.request<PagedResult<QueryDictionaryPagedOutput>>({
      url: "/dictionary/queryDictionaryPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
  /**
   * 获取字典详情
   */
  queryDictionaryDetail(dictionaryId: number) {
    return axiosUtil.request<QueryDictionaryDetailOutput>({
      url: "/dictionary/queryDictionaryDetail",
      method: "get",
      params: {
        dictionaryId,
      },
      requestType: "query",
    });
  },
  /**
   * 添加字典
   */
  addDictionary(data: AddDictionaryInput) {
    return axiosUtil.request({
      url: "/dictionary/addDictionary",
      method: "post",
      data,
      requestType: "add",
    });
  },
  /**
   * 编辑字典
   */
  editDictionary(data: EditDictionaryInput) {
    return axiosUtil.request({
      url: "/dictionary/editDictionary",
      method: "post",
      data,
      requestType: "edit",
    });
  },
  /**
   * 删除字典
   */
  deleteDictionary(data: DictionaryIdInput) {
    return axiosUtil.request({
      url: "/dictionary/deleteDictionary",
      method: "post",
      data,
      requestType: "delete",
    });
  },
};
