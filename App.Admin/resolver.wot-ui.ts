import { kebabCase } from "@uni-helper/vite-plugin-uni-components";
import type { ComponentResolveResult, ComponentResolver } from "@uni-helper/vite-plugin-uni-components";

export const WotResolver = (): ComponentResolver => {
	return {
		type: "component",
		resolve(name: string): ComponentResolveResult {
			if (!/^Wd[A-Z]/.test(name)) return;
			const compName = kebabCase(name);
			return {
				as: name,
				name: "default",
				from: `@wot-ui/ui/components/${compName}/${compName}.vue`,
			};
		},
	};
};
