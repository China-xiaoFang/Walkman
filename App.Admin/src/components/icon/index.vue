<template>
	<wd-icon
		v-bind="props"
		:custom-class="`fa-icon fa-icon-${props.name} ${props.customClass}`"
		class-prefix="iconfont"
		@click="(event: MouseEvent) => emit('click', event)"
	/>
</template>

<script setup lang="ts">
import { baseProps } from "@wot-ui/ui/common/props";
import "./iconfont.css";

defineOptions({
	name: "Icon",
	options: {
		virtualHost: true,
		addGlobalClass: true,
		styleIsolation: "shared",
	},
});

const props = defineProps({
	...baseProps,
	/** @description 阿里矢量图标名称 */
	name: {
		type: String,
		required: true,
	},
	/** @description 图标的字体大小 */
	size: {
		type: [String, Number],
	},
	/** @description 图标的颜色 */
	color: String,
});

const emit = defineEmits<{
	/** @description 点击事件 */
	click: [event: MouseEvent];
}>();
</script>
