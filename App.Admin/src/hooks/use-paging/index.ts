import { reactive, ref, shallowRef } from "vue";
import useZPaging from "z-paging/components/z-paging/js/hooks/useZPaging.js";
import type { Reactive, Ref, ShallowRef } from "vue";
import type { ZPagingProps } from "z-paging/types/comps/z-paging";

type UsePagingResult<TResult, TInput> = {
	/** @description ZPaging 组件实例 */
	paging: Ref<ZPagingRef<TResult>>;
	/** @description 搜索参数 */
	searchParam: Reactive<PagedInput & TInput>;
	/** @description 分页数据集合 */
	pagedList: ShallowRef<TResult[]>;
	/** @description 分页查询 */
	pagingQuery: ZPagingProps["onQuery"];
	/** @description 虚拟列表数据改变回调 */
	pagingVirtualListChange: (list: TResult[]) => void;
	/** @description 刷新列表 */
	pagingRefresh: {
		(): void;
		(pageIndex: number): void;
	};
};

export const usePaging = <TResult, TInput extends Record<string, unknown> = Record<string, never>>(
	requestApi: (params?: PagedInput & TInput) => Promise<PagedResult<TResult>>,
	pageScroll?: boolean
): UsePagingResult<TResult, TInput> => {
	/** @description ZPaging 组件实例 */
	const zPagingRef = ref<ZPagingRef<TResult>>();

	/** @description 搜索参数 */
	const searchParam = reactive<PagedInput & TInput>({} as PagedInput & TInput);

	/** @description 分页数据集合 */
	const pagedList = shallowRef<TResult[]>([]);

	/** @description 分页查询 */
	const pagingQuery: ZPagingProps["onQuery"] = (pageNo, pageSize) => {
		requestApi({
			pageIndex: pageNo,
			pageSize,
			...(searchParam as TInput),
		})
			.then((apiRes) => zPagingRef.value.complete(apiRes.rows))
			.catch(() => zPagingRef.value.complete(false));
	};

	/** @description 虚拟列表数据改变回调 */
	const pagingVirtualListChange: ZPagingProps["onVirtualListChange"] = (list: TResult[]) => {
		pagedList.value = list;
	};

	/** @description 刷新列表 */
	function pagingRefresh(): void;
	function pagingRefresh(pageIndex?: number) {
		if (pageIndex) {
			zPagingRef.value?.refreshToPage(pageIndex);
		} else {
			zPagingRef.value?.reload();
		}
	}

	// 如果是页面滚动则引入 mixins
	if (pageScroll) {
		useZPaging(zPagingRef);
	}

	return {
		/** @description 这里必须用 'paging' */
		paging: zPagingRef,
		searchParam,
		pagedList,
		pagingQuery,
		pagingVirtualListChange,
		pagingRefresh,
	};
};
