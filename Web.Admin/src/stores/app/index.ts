import { ref } from "vue";
import { consoleError } from "@fast-china/utils";
import { defineStore } from "pinia";
import { dictionaryApi } from "@/api/services/Admin/dictionary";
import type { FaTableColumnCtx, FaTableEnumColumnCtx } from "fast-element-plus";

export type ILoginComponent = "ClassicLogin" | "ModernLogin" | "SimpleLogin" | "SplitLogin";

export const useApp = defineStore(
	"app",
	() => {
		/** 字典 */
		const dictionary = ref<Map<string, FaTableEnumColumnCtx[]>>(new Map());

		/** 表格列 */
		const tableColumns = ref<Map<string, FaTableColumnCtx[]>>(new Map());

		/** Launch */
		const launch = async (): Promise<void> => {
			try {
				// 处理数据字典
				dictionary.value.clear();
				const _dictionary = await dictionaryApi.queryDictionary();
				Object.entries(_dictionary).forEach(([key, value]) => {
					dictionary.value.set(key, value);
				});
			} catch {
				consoleError("App", "字典加载失败");
			}
		};

		/** 获取字典 */
		const getDictionary = (key: string, throwError = true): FaTableEnumColumnCtx[] => {
			if (!dictionary.value.has(key)) {
				if (throwError) {
					consoleError("app", `字典 [${key}] 不存在`);
				}
				return;
			}
			return dictionary.value.get(key);
		};

		/** 获取表格列 */
		const getTableColumns = (tableKey: string, throwError = true): FaTableColumnCtx[] => {
			if (!tableColumns.value.has(tableKey)) {
				if (throwError) {
					consoleError("app", `表格列 [${tableKey}] 不存在`);
				}
				return;
			}
			return tableColumns.value.get(tableKey);
		};

		/** 设置或更新表格列 */
		const setTableColumns = (tableKey: string, columns: FaTableColumnCtx[]): void => {
			if (tableColumns.value.has(tableKey)) {
				tableColumns.value.delete(tableKey);
			}
			tableColumns.value.set(tableKey, columns);
		};

		/** 删除表格列 */
		const deleteTableColumns = (tableKey: string): void => {
			if (tableColumns.value.has(tableKey)) {
				tableColumns.value.delete(tableKey);
			}
		};

		return {
			launch,
			getDictionary,
			getTableColumns,
			setTableColumns,
			deleteTableColumns,
		};
	},
	{
		persist: {
			key: "store-app",
		},
	}
);
