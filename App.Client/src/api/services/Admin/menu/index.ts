import { axiosUtil } from "@fast-china/axios";
import { QueryMenuPagedOutput } from "./models/QueryMenuPagedOutput";
import { QueryMenuPagedInput } from "./models/QueryMenuPagedInput";
import { QueryMenuDetailOutput } from "./models/QueryMenuDetailOutput";
import { AddMenuInput } from "./models/AddMenuInput";
import { EditMenuInput } from "./models/EditMenuInput";
import { MenuIdInput } from "./models/MenuIdInput";

/**
 * Fast.Admin.Service.Menu.MenuService 菜单服务Api
 */
export const menuApi = {
  /**
   * 菜单选择器
   */
  menuSelector() {
    return axiosUtil.request<ElSelectorOutput<number>[]>({
      url: "/menu/menuSelector",
      method: "get",
      requestType: "query",
    });
  },
  /**
   * 获取菜单列表
   */
  queryMenuPaged(data: QueryMenuPagedInput) {
    return axiosUtil.request<QueryMenuPagedOutput[]>({
      url: "/menu/queryMenuPaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
  /**
   * 获取菜单详情
   */
  queryMenuDetail(menuId: number) {
    return axiosUtil.request<QueryMenuDetailOutput>({
      url: "/menu/queryMenuDetail",
      method: "get",
      params: {
        menuId,
      },
      requestType: "query",
    });
  },
  /**
   * 添加菜单
   */
  addMenu(data: AddMenuInput) {
    return axiosUtil.request({
      url: "/menu/addMenu",
      method: "post",
      data,
      requestType: "add",
    });
  },
  /**
   * 编辑菜单
   */
  editMenu(data: EditMenuInput) {
    return axiosUtil.request({
      url: "/menu/editMenu",
      method: "post",
      data,
      requestType: "edit",
    });
  },
  /**
   * 删除菜单
   */
  deleteMenu(data: MenuIdInput) {
    return axiosUtil.request({
      url: "/menu/deleteMenu",
      method: "post",
      data,
      requestType: "delete",
    });
  },
  /**
   * 菜单更改状态
   */
  changeStatus(data: MenuIdInput) {
    return axiosUtil.request({
      url: "/menu/changeStatus",
      method: "post",
      data,
      requestType: "edit",
    });
  },
};
