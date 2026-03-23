import { Fragment, computed, defineComponent, onMounted, reactive, ref } from "vue";
import { ElDropdownItem, ElMessage, ElMessageBox, dayjs } from "element-plus";
import { FaTable, faTableEmits, faTableProps } from "fast-element-plus";
import { clickUtil, makeSlots, useEmits, useExpose, useProps, useRender, withDefineType } from "@fast-china/utils";
import { isString } from "lodash";
import { tableApi } from "@/api/services/Admin/table";
import { useApp, useConfig } from "@/stores";
import type { FaTableColumnCtx, FaTableInstance, FaTableSlots } from "fast-element-plus";
import type { VNode } from "vue";

export default defineComponent({
	name: "FastTable",
	props: {
		...faTableProps,
		/** @description 列配置按钮 */
		columnSettingBtn: {
			type: Boolean,
			default: true,
		},
	},
	emits: {
		...faTableEmits,
	},
	slots: makeSlots<FaTableSlots>(),
	setup(props, { attrs, slots, emit, expose }) {
		const faTableRef = ref<FaTableInstance>();

		const appStore = useApp();
		const configStore = useConfig();

		const state = reactive({
			loading: false,
			loadingText: "加载中...",
			/** 表格列 */
			columns: withDefineType<FaTableColumnCtx[]>([]),
			/** 是否存在缓存列 */
			existCacheColumns: false,
		});

		/** type: 1 字符串，2 数字，4 Boolean，8 方法 */
		type FaTableColumnLocalCtx = {
			otherAdvancedConfig?: { prop: string; type: number }[];
			searchAdvancedConfig?: { prop: string; type: number }[];
		};

		/** 处理列类型 */
		const handleColumnType = (localColumns: (FaTableColumnCtx & FaTableColumnLocalCtx)[]): (FaTableColumnCtx & FaTableColumnLocalCtx)[] => {
			const handleFunctionArgs = (
				functionStr: string
			): {
				args: string[];
				body: string;
			} => {
				// 去掉字符串开头和结尾的多余空白字符，包括换行符
				const trimmedStr = functionStr.trim();

				// 正则表达式用于匹配箭头函数的参数和函数体，包括支持解构参数
				// eslint-disable-next-line regexp/no-super-linear-backtracking
				const arrowFunctionMatch = trimmedStr.match(/^\s*\(?([^)]*?)\)?\s*=>\s*\{([\s\S]*?)\}\s*;?$/);

				if (arrowFunctionMatch) {
					const args = arrowFunctionMatch[1]
						.split(",")
						.map((arg) => arg.trim())
						.filter((arg) => arg);
					const body = arrowFunctionMatch[2].trim();
					return { args, body };
				}

				return { args: [], body: "" };
			};

			for (let i = 0; i < localColumns.length; i++) {
				if (localColumns[i]?.otherAdvancedConfig?.length > 0) {
					localColumns[i]?.otherAdvancedConfig
						.filter((f) => f.type === 8)
						.forEach((advKey: { prop: string; type: number }) => {
							const { args, body } = handleFunctionArgs(localColumns[i][advKey.prop]);
							localColumns[i][advKey.prop] = new Function(...args, body);
						});
					delete localColumns[i].otherAdvancedConfig;
				}
				if (localColumns[i]?.searchAdvancedConfig?.length > 0) {
					localColumns[i]?.searchAdvancedConfig
						.filter((f) => f.type === 8)
						.forEach((advKey: { prop: string; type: number }) => {
							const { args, body } = handleFunctionArgs(localColumns[i].search.props[advKey.prop]);
							localColumns[i].search.props[advKey.prop] = new Function(...args, body);
						});
					delete localColumns[i].searchAdvancedConfig;
				}
			}

			return localColumns;
		};

		/** 同步表格列配置 */
		const syncColumnsCache = async (showConfirm = true): Promise<void> => {
			if (props.columns) return;

			async function localRequest(): Promise<void> {
				await faTableRef.value.doLoading(async () => {
					try {
						await tableApi.syncUserTableConfig({
							tableKey: props.tableKey,
						});
						appStore.deleteTableColumns(props.tableKey);
						ElMessage.success("同步成功");
						// eslint-disable-next-line no-use-before-define
						await loadTableColumns();
					} catch (error) {
						ElMessage.error("同步失败");
						throw error;
					}
				}, "同步列配置中...");
			}

			if (showConfirm) {
				await ElMessageBox.confirm("确认同步列缓存配置？此操作无法撤销。", {
					type: "warning",
					async beforeClose() {
						await localRequest();
					},
				});
			} else {
				await localRequest();
			}
		};

		/** 清除表格列缓存 */
		const clearColumnsCache = (): void => {
			if (props.columns) return;
			ElMessageBox.confirm("确认重置列缓存配置？此操作无法撤销。", {
				type: "warning",
				async beforeClose() {
					await faTableRef.value.doLoading(async () => {
						try {
							await tableApi.clearUserTableConfig({ tableKey: props.tableKey });
							appStore.deleteTableColumns(props.tableKey);
							ElMessage.success("重置成功");
							// eslint-disable-next-line no-use-before-define
							await loadTableColumns();
						} catch (error) {
							ElMessage.error("重置列配置失败");
							throw error;
						}
					}, "重置列配置中...");
				},
			});
		};

		/** 保存表格列配置 */
		const saveColumnsCache = async (columns: FaTableColumnCtx[]): Promise<void> => {
			if (props.columns) return;
			await faTableRef.value.doLoading(async () => {
				try {
					await tableApi.saveUserTableConfig({
						tableKey: props.tableKey,
						columns: columns.map((m) => ({
							columnId: m.columnId,
							label: m.label,
							fixed: isString(m.fixed) ? m.fixed : "",
							autoWidth: m.autoWidth,
							width: Number(m.width),
							smallWidth: Number(m.smallWidth),
							order: m.order,
							show: m.show,
							copy: m.copy,
							sortable: m.sortable,
							searchLabel: m.search?.label,
							searchOrder: m.search?.order,
						})),
					});
					// 这里已经成功缓存列了
					state.existCacheColumns = true;
					appStore.setTableColumns(props.tableKey, columns);
					appStore.deleteTableColumns(props.tableKey);
					ElMessage.success("保存列配置成功");
				} catch (error) {
					ElMessage.error("保存列配置失败");
					throw error;
				}
			}, "保存列配置中...");
		};

		/** 处理列 */
		const handleColumns = (columns: FaTableColumnCtx[]): FaTableColumnCtx[] => {
			return columns.map((col) => {
				const result = { ...col };

				if (result._children?.length) {
					result._children = handleColumns(result._children);
				}

				if (result.enum && isString(result.enum)) {
					const enumDict = appStore.getDictionary(result.enum);
					if (enumDict) {
						result.enum = enumDict;
					}
				}

				return result;
			});
		};

		/** 加载表格列 */
		const loadTableColumns = async (): Promise<void> => {
			let columns: FaTableColumnCtx[] = [];
			if (props.columns) {
				columns = props.columns;
			} else {
				const cacheColumns = appStore.getTableColumns(props.tableKey, false);
				if (cacheColumns) {
					columns = cacheColumns;
				} else {
					state.loading = true;
					state.loadingText = "加载列配置中...";
					try {
						const apiRes = await tableApi.queryTableColumnConfig(props.tableKey);
						// 判断是否存在改变
						if (apiRes.change) {
							const lastUpdateTime = apiRes.updatedTime ?? new Date();
							ElMessage.info(
								`当前列配置于 '${dayjs(lastUpdateTime).format("YYYY-MM-DD")}' 已发生改变，为确保数据准确性，正在同步缓存配置，请稍后...`
							);
							// 同步表格列配置
							await syncColumnsCache(false);
							return;
						} else {
							state.existCacheColumns = apiRes.cache;
							columns = handleColumnType(apiRes.cache ? apiRes.cacheColumns : apiRes.columns);
							appStore.setTableColumns(props.tableKey, columns);
						}
					} catch (error) {
						ElMessage.error("加载列配置失败");
						throw error;
					} finally {
						state.loading = false;
					}
				}
			}

			state.columns = handleColumns(columns);
		};

		const doRender = async (): Promise<void> => {
			state.columns = [];
			await clickUtil.debounceAsync(async () => {
				await loadTableColumns();
				faTableRef.value?.doRender();
			}, 300);
		};

		onMounted(async () => {
			await loadTableColumns();
		});

		const tableProps = useProps(props, faTableProps, [
			"columns",
			"hideImage",
			"collapsedSearch",
			"advancedSearchDrawer",
			"dataSearchRange",
			"columnSettingBtn",
			"columnsChange",
		]);

		const tableEmits = useEmits(faTableEmits, emit);

		const tableSlot = {
			...slots,
			columnSetting: (): VNode => (
				<Fragment>
					<ElDropdownItem title="同步列配置" disabled={!state.existCacheColumns} onClick={syncColumnsCache}>
						同步列配置
					</ElDropdownItem>
					<ElDropdownItem title="重置列配置" disabled={!state.existCacheColumns} onClick={clearColumnsCache}>
						重置列配置
					</ElDropdownItem>
				</Fragment>
			),
		};

		useRender(() => (
			<FaTable
				{...tableProps.value}
				{...tableEmits.value}
				vSlots={tableSlot}
				ref={faTableRef}
				vLoading={state.loading}
				element-loading-text={state.loadingText}
				columns={state.columns}
				searchForm={props.searchForm && configStore.tableLayout.showSearch}
				hideImage={configStore.tableLayout.hideImage}
				collapsedSearch={configStore.tableLayout.defaultCollapsedSearch}
				advancedSearchDrawer={configStore.tableLayout.advancedSearchDrawer}
				dataSearchRange={configStore.tableLayout.dataSearchRange}
				columnSettingBtn={props.columnSettingBtn && !props.columns}
				columnsChange={saveColumnsCache}
			/>
		));

		return useExpose(expose, {
			/** @description 用于多选表格，清空用户的选择 */
			clearSelection: computed(() => faTableRef.value?.clearSelection),
			/** @description 返回当前选中的行 */
			getSelectionRows: computed(() => faTableRef.value?.getSelectionRows),
			/** @description 用于多选表格，切换某一行的选中状态， 如果使用了第二个参数，则可直接设置这一行选中与否 */
			toggleRowSelection: computed(() => faTableRef.value?.toggleRowSelection),
			/** @description 用于多选表格，切换全选和全不选 */
			toggleAllSelection: computed(() => faTableRef.value?.toggleAllSelection),
			/** @description 用于可扩展的表格或树表格，如果某行被扩展，则切换。 使用第二个参数，您可以直接设置该行应该被扩展或折叠。 */
			toggleRowExpansion: computed(() => faTableRef.value?.toggleRowExpansion),
			/** @description 用于单选表格，设定某一行为选中行， 如果调用时不加参数，则会取消目前高亮行的选中状态。 */
			setCurrentRow: computed(() => faTableRef.value?.setCurrentRow),
			/** @description 用于清空排序条件，数据会恢复成未排序的状态 */
			clearSort: computed(() => faTableRef.value?.clearSort),
			/** @description 传入由columnKey 组成的数组以清除指定列的过滤条件。 如果没有参数，清除所有过滤器 */
			clearFilter: computed(() => faTableRef.value?.clearFilter),
			/** @description 对 Table 进行重新布局。 当表格可见性变化时，您可能需要调用此方法以获得正确的布局 */
			doLayout: computed(() => faTableRef.value?.doLayout),
			/** @description 手动排序表格。 参数 prop 属性指定排序列，order 指定排序顺序。 */
			sort: computed(() => faTableRef.value?.sort),
			/** @description 滚动到一组特定坐标 */
			scrollTo: computed(() => faTableRef.value?.scrollTo),
			/** @description 设置垂直滚动位置 */
			setScrollTop: computed(() => faTableRef.value?.setScrollTop),
			/** @description 设置水平滚动位置 */
			setScrollLeft: computed(() => faTableRef.value?.setScrollLeft),
			/** @description 获取表列的 context */
			columns: computed(() => faTableRef.value?.columns),
			/** @description 适用于 lazy Table, 需要设置 rowKey, 更新 key children */
			updateKeyChildren: computed(() => faTableRef.value?.updateKeyChildren),
			/** @description 加载状态 */
			loading: computed(() => faTableRef.value?.loading),
			/** @description 表格数据 */
			tableData: computed(() => faTableRef.value?.tableData),
			/** @description 分页数据 */
			tablePagination: computed(() => faTableRef.value?.tablePagination),
			/** @description 搜索参数 */
			searchParam: computed(() => faTableRef.value?.searchParam),
			/** @description 选中状态 */
			selected: computed(() => faTableRef.value?.selected),
			/** @description 选中数据列表 */
			selectedList: computed(() => faTableRef.value?.selectedList),
			/** @description 选中数据 rowKey 列表 */
			selectedListIds: computed(() => faTableRef.value?.selectedListIds),
			/** @description 部分选中数据 rowKey 列表 */
			indeterminateSelectedListIds: computed(() => faTableRef.value?.indeterminateSelectedListIds),
			/** @description 表格宽度 */
			tableWidth: computed(() => faTableRef.value?.tableWidth),
			/** @description 表格高度 */
			tableHeight: computed(() => faTableRef.value?.tableHeight),
			/** @description 部分选中（样式不一样而已），用于多选表格，切换某一行的选中状态， 如果使用了第二个参数，则可直接设置这一行选中与否 */
			toggleRowIndeterminateSelection: computed(() => faTableRef.value?.toggleRowIndeterminateSelection),
			/** @description 异步方法，刷新表格 */
			refresh: computed(() => faTableRef.value?.refresh),
			/** @description 异步方法，重置表格 */
			reset: computed(() => faTableRef.value?.reset),
			/** @description 对 Table 进行重新渲染。当 TableKey 发生变化的时候可以通过此方法重新渲染表格 */
			doRender,
		});
	},
});
