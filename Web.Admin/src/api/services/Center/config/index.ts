import { axiosUtil } from "@fast-china/axios";
import { PagedInput, PagedResult } from "fast-element-plus";
import { QueryConfigPagedOutput } from "./models/QueryConfigPagedOutput";
import { QueryConfigDetailOutput } from "./models/QueryConfigDetailOutput";
import { AddConfigInput } from "./models/AddConfigInput";
import { EditConfigInput } from "./models/EditConfigInput";
import { DeleteConfigCacheInput } from "./models/DeleteConfigCacheInput";

/**
 * Fast.Admin.Service.Config.ConfigService 配置服务Api
 */
export const configApi = {
  /**
   * 获取配置分页列表
   */
  queryConfigPaged(data: PagedInput) {
    return axiosUtil.request<PagedResult<QueryConfigPagedOutput>>({
      url: "/config/queryConfigPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
  /**
   * 获取配置详情
   */
  queryConfigDetail(configId: number) {
    return axiosUtil.request<QueryConfigDetailOutput>({
      url: "/config/queryConfigDetail",
      method: "get",
      params: {
        configId,
      },
      requestType: "query",
    });
  },
  /**
   * 添加配置
   */
  addConfig(data: AddConfigInput) {
    return axiosUtil.request({
      url: "/config/addConfig",
      method: "post",
      data,
      requestType: "add",
    });
  },
  /**
   * 编辑配置
   */
  editConfig(data: EditConfigInput) {
    return axiosUtil.request({
      url: "/config/editConfig",
      method: "post",
      data,
      requestType: "edit",
    });
  },
  /**
   * 删除配置缓存
   */
  deleteConfigCache(data: DeleteConfigCacheInput) {
    return axiosUtil.request({
      url: "/config/deleteConfigCache",
      method: "post",
      data,
      requestType: "delete",
    });
  },
  /**
   * 删除所有配置缓存
   */
  deleteAllConfigCache() {
    return axiosUtil.request({
      url: "/config/deleteAllConfigCache",
      method: "post",
      requestType: "delete",
    });
  },
};
