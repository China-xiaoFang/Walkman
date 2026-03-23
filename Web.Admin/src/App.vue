<template>
	<el-config-provider v-bind="elConfigProviderProps">
		<RouterView></RouterView>
	</el-config-provider>
</template>

<script lang="ts" setup>
import { reactive, watch } from "vue";
import { type componentSizes } from "element-plus";
import { withDefineType } from "@fast-china/utils";
import { useWindowSize } from "@vueuse/core";
import zhCn from "element-plus/es/locale/lang/zh-cn";
import { RouterView } from "vue-router";
import { useConfig } from "@/stores";

defineOptions({
	name: "App",
});

const windowSize = useWindowSize();
const configStore = useConfig();

const elConfigProviderProps = reactive({
	// 语言
	locale: zhCn,
	button: {
		// 自动插入空格
		autoInsertSpace: false,
	},
	emptyValues: [undefined, null],
	valueOnClear: null,
	size: withDefineType<(typeof componentSizes)[number]>("default"),
});

// 初始化主题
configStore.initTheme();

/** 自动大小 */
const handleAutoSize = () => {
	const html = document.documentElement;
	if (windowSize.width.value > 1366) {
		// 1366 以上使用默认
		elConfigProviderProps.size = "default";
		html.classList.remove("small");
		html.classList.add("default");
		configStore.layout.layoutSize = "default";
		configStore.setDefaultLayoutSize();
	} else {
		elConfigProviderProps.size = "small";
		html.classList.remove("default");
		html.classList.add("small");
		configStore.layout.layoutSize = "small";
		configStore.setSmallLayoutSize();
	}
};

/** 监听页面宽度，设置组件大小 */
watch(
	() => [windowSize.width.value, windowSize.height.value],
	() => {
		configStore.layout.autoSize && handleAutoSize();
	},
	{ immediate: true }
);

/** 监听是否自动大小 */
watch(
	() => [configStore.layout.autoSize, configStore.layout.layoutSize],
	() => {
		if (configStore.layout.autoSize) {
			handleAutoSize();
		} else {
			const html = document.documentElement;
			if (configStore.layout.layoutSize === "default") {
				elConfigProviderProps.size = "default";
				html.classList.remove("small");
				html.classList.remove("large");
				html.classList.add("default");
				configStore.setDefaultLayoutSize();
			} else if (configStore.layout.layoutSize === "small") {
				elConfigProviderProps.size = "small";
				html.classList.remove("default");
				html.classList.remove("large");
				html.classList.add("small");
				configStore.setSmallLayoutSize();
			} else if (configStore.layout.layoutSize === "large") {
				elConfigProviderProps.size = "large";
				html.classList.remove("default");
				html.classList.remove("small");
				html.classList.add("large");
				configStore.setDefaultLayoutSize();
			}
		}
	},
	{ immediate: true }
);
</script>
