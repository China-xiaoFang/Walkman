import { axiosUtil } from "@fast-china/axios";
import type { ElSelectorOutput } from "fast-element-plus";
import type { AddMenuInput } from "./models/AddMenuInput";
import type { EditMenuInput } from "./models/EditMenuInput";
import type { MenuIdInput } from "./models/MenuIdInput";
import type { QueryMenuDetailOutput } from "./models/QueryMenuDetailOutput";
import type { QueryMenuPagedInput } from "./models/QueryMenuPagedInput";
import type { QueryMenuPagedOutput } from "./models/QueryMenuPagedOutput";

/**
 * 菜单服务Api
 */
export const menuApi = {
	/**
	 * 菜单选择器
	 */
	menuSelector(): Promise<ElSelectorOutput<string>[]> {
		return axiosUtil.request<ElSelectorOutput<string>[]>({
			url: "/menu/menuSelector",
			method: "get",
			requestType: "query",
		});
	},
	/**
	 * 获取菜单列表
	 */
	queryMenuPaged(data: QueryMenuPagedInput): Promise<QueryMenuPagedOutput[]> {
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
	queryMenuDetail(menuId: string): Promise<QueryMenuDetailOutput> {
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
	addMenu(data: AddMenuInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/menu/addMenu",
			method: "post",
			data,
			requestType: "add",
		});
	},
	/**
	 * 编辑菜单
	 */
	editMenu(data: EditMenuInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/menu/editMenu",
			method: "post",
			data,
			requestType: "edit",
		});
	},
	/**
	 * 删除菜单
	 */
	deleteMenu(data: MenuIdInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/menu/deleteMenu",
			method: "post",
			data,
			requestType: "delete",
		});
	},
	/**
	 * 菜单更改状态
	 */
	changeStatus(data: MenuIdInput): Promise<void> {
		return axiosUtil.request<void>({
			url: "/menu/changeStatus",
			method: "post",
			data,
			requestType: "edit",
		});
	},
};
