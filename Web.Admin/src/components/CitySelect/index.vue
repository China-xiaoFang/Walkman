<template>
	<el-cascader
		v-bind="$attrs"
		v-model="modelValue"
		:options="state.regionList"
		placeholder="请选择城市"
		filterable
		clearable
		:props="{ checkOnClickNode: true, emitPath: true }"
		@change="handleChange"
	/>
</template>

<script lang="ts" setup>
import { onMounted, reactive } from "vue";
import { withDefineType } from "@fast-china/utils";
import { regionApi } from "@/api/services/Center/region";
import type { CascaderValue } from "element-plus";
import type { ElSelectorOutput } from "fast-element-plus";

defineOptions({
	name: "CitySelect",
});

const emit = defineEmits<{
	change: [value: ElSelectorOutput<string> | undefined];
}>();

const modelValue = defineModel<string>();
const provinceName = defineModel<string>("provinceName");
const cityName = defineModel<string>("cityName");

const state = reactive({
	regionList: withDefineType<ElSelectorOutput<string>[]>([]),
});

const handleChange = (val: CascaderValue) => {
	const value = val as string[];
	if (value && value.length > 0) {
		const provinceInfo = state.regionList.find((f) => f.value === value[0]);
		provinceName.value = provinceInfo.label;
		const cityInfo = provinceInfo?.children?.find((f) => f.value === value[1]);
		cityName.value = cityInfo?.label;
		modelValue.value = value[1];
		emit("change", cityInfo);
	} else {
		provinceName.value = null;
		cityName.value = null;
		modelValue.value = null;
		emit("change", undefined);
	}
};

onMounted(async () => {
	state.regionList = await regionApi.citySelector();
});
</script>
