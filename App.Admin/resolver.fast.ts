import { existsSync, readdirSync } from "node:fs";
import { resolve } from "node:path";
import type { ComponentResolveResult, ComponentResolver } from "@uni-helper/vite-plugin-uni-components";

const rootDir = resolve(__dirname, "src");

export const FastResolver = (): ComponentResolver => {
	const components = new Map<string, string>();

	// 公共组件
	const componentsDir = resolve(rootDir, "components");
	if (existsSync(componentsDir)) {
		for (const entry of readdirSync(componentsDir, { withFileTypes: true })) {
			if (!entry.isDirectory()) continue;

			const indexPath = resolve(componentsDir, entry.name, "index.vue");
			if (!existsSync(indexPath)) continue;

			components.set(entry.name, `@/components/${entry.name}/index.vue`);
		}
	}

	// 分包组件
	for (const pageEntry of readdirSync(rootDir, { withFileTypes: true })) {
		if (!pageEntry.isDirectory() || !pageEntry.name.startsWith("pages_")) continue;

		const subComponentsDir = resolve(rootDir, pageEntry.name, "components");
		if (!existsSync(subComponentsDir)) continue;

		for (const entry of readdirSync(subComponentsDir, { withFileTypes: true })) {
			if (!entry.isDirectory()) continue;

			const indexPath = resolve(subComponentsDir, entry.name, "index.vue");
			if (!existsSync(indexPath)) continue;

			// 公共组件优先
			if (!components.has(entry.name)) {
				components.set(entry.name, `@/${pageEntry.name}/components/${entry.name}/index.vue`);
			}
		}
	}

	return {
		type: "component",
		resolve(name: string): ComponentResolveResult {
			if (!/^Fa[A-Z]/.test(name)) return;
			// 移除 Fa 前缀，并将首字母转为小写：FaButton => button
			const componentName = name.charAt(2).toLowerCase() + name.slice(3);
			const from = components.get(componentName);
			if (from) {
				return {
					as: name,
					name: "default",
					from,
				};
			}
		},
	};
};
