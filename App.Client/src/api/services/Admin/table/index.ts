import { axiosUtil } from "@fast-china/axios";
import { QueryTableConfigPagedOutput } from "./models/QueryTableConfigPagedOutput";
import { QueryTableConfigDetailOutput } from "./models/QueryTableConfigDetailOutput";
import { AddTableConfigInput } from "./models/AddTableConfigInput";
import { EditTableConfigInput } from "./models/EditTableConfigInput";
import { TableIdInput } from "./models/TableIdInput";
import { CopyTableConfigInput } from "./models/CopyTableConfigInput";
import { FaTableColumnCtx } from "./models/FaTableColumnCtx";
import { EditTableColumnConfigInput } from "./models/EditTableColumnConfigInput";
import { QueryTableColumnConfigOutput } from "./models/QueryTableColumnConfigOutput";
import { SyncUserTableConfigInput } from "./models/SyncUserTableConfigInput";
import { SaveUserTableConfigInput } from "./models/SaveUserTableConfigInput";

/**
 * Fast.Admin.Service.Table.TableService 表格服务Api
 */
export const tableApi = {
  /**
   * 获取表格配置分页列表
   */
  queryTableConfigPaged(data: PagedInput) {
    return axiosUtil.request<PagedResult<QueryTableConfigPagedOutput>>({
      url: "/table/queryTableConfigPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
  /**
   * 获取表格配置详情
   */
  queryTableConfigDetail(tableId: number) {
    return axiosUtil.request<QueryTableConfigDetailOutput>({
      url: "/table/queryTableConfigDetail",
      method: "get",
      params: {
        tableId,
      },
      requestType: "query",
    });
  },
  /**
   * 添加表格配置
   */
  addTableConfig(data: AddTableConfigInput) {
    return axiosUtil.request({
      url: "/table/addTableConfig",
      method: "post",
      data,
      requestType: "add",
    });
  },
  /**
   * 编辑表格配置
   */
  editTableConfig(data: EditTableConfigInput) {
    return axiosUtil.request({
      url: "/table/editTableConfig",
      method: "post",
      data,
      requestType: "edit",
    });
  },
  /**
   * 删除表格配置
   */
  deleteTableConfig(data: TableIdInput) {
    return axiosUtil.request({
      url: "/table/deleteTableConfig",
      method: "post",
      data,
      requestType: "delete",
    });
  },
  /**
   * 复制表格配置
   */
  copyTableConfig(data: CopyTableConfigInput) {
    return axiosUtil.request({
      url: "/table/copyTableConfig",
      method: "post",
      data,
      requestType: "edit",
    });
  },
  /**
   * 获取表格列配置详情
   */
  queryTableColumnConfigDetail(tableId: number) {
    return axiosUtil.request<FaTableColumnCtx[]>({
      url: "/table/queryTableColumnConfigDetail",
      method: "get",
      params: {
        tableId,
      },
      requestType: "query",
    });
  },
  /**
   * 编辑表格列配置
   */
  editTableColumnConfig(data: EditTableColumnConfigInput) {
    return axiosUtil.request({
      url: "/table/editTableColumnConfig",
      method: "post",
      data,
      requestType: "edit",
    });
  },
  /**
   * 获取表格列配置
   */
  queryTableColumnConfig(tableKey: string) {
    return axiosUtil.request<QueryTableColumnConfigOutput>({
      url: "/table/queryTableColumnConfig",
      method: "get",
      params: {
        tableKey,
      },
      requestType: "query",
    });
  },
  /**
   * 同步用户表格配置
   */
  syncUserTableConfig(data: SyncUserTableConfigInput) {
    return axiosUtil.request({
      url: "/table/syncUserTableConfig",
      method: "post",
      data,
      requestType: "edit",
    });
  },
  /**
   * 保存用户表格配置
   */
  saveUserTableConfig(data: SaveUserTableConfigInput) {
    return axiosUtil.request({
      url: "/table/saveUserTableConfig",
      method: "post",
      data,
      requestType: "edit",
    });
  },
  /**
   * 清除用户表格配置
   */
  clearUserTableConfig(data: SyncUserTableConfigInput) {
    return axiosUtil.request({
      url: "/table/clearUserTableConfig",
      method: "post",
      data,
      requestType: "delete",
    });
  },
};
