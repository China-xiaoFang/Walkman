<template>
	<el-color-picker v-bind="$attrs" v-model="modelValue" :predefine="predefineColorList" />
</template>

<script lang="ts" setup>
import { useVModel } from "@vueuse/core";
import { defaultThemeColor } from "@/stores";

defineOptions({
	name: "ColorPicker",
});

const props = defineProps<{
	modelValue?: string;
}>();

const emit = defineEmits({
	"update:modelValue": (_value: string) => true,
});

const predefineColorList = [
	defaultThemeColor,
	"#4488FE",
	"#2A3A93",
	"#EB4537", // 红色
	"#FF8225", // 橙色
	"#FAC230", // 黄色
	"#55AF7B", // 绿色
	"#FF4191", // 粉色
	"#4286F3", // 蓝色
	"#AD6DEF", // 紫色
];

const modelValue = useVModel(props, "modelValue", emit, { passive: false });
</script>
