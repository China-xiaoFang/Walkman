import { kebabCase } from "@uni-helper/vite-plugin-uni-components";
import type { ComponentResolveResult, ComponentResolver } from "@uni-helper/vite-plugin-uni-components";

export const ZPagingResolver = (): ComponentResolver => {
	return {
		type: "component",
		resolve(name: string): ComponentResolveResult {
			if (!/^(?!ZPagingRefresh|ZPagingLoadMore)ZPaging.*/.test(name)) return;
			const compName = kebabCase(name);
			return {
				as: name,
				name: "default",
				from: `z-paging/components/${compName}/${compName}.vue`,
			};
		},
	};
};
