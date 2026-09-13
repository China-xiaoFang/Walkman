<template>
	<el-cascader
		v-bind="$attrs"
		v-model="modelValue"
		:options="state.regionList"
		placeholder="请选择地区"
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
	name: "RegionSelect",
});

const emit = defineEmits<{
	change: [value: ElSelectorOutput<string> | undefined];
}>();

const modelValue = defineModel<string>();
const provinceName = defineModel<string>("provinceName");
const cityName = defineModel<string>("cityName");
const districtName = defineModel<string>("districtName");

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
		const districtInfo = cityInfo?.children?.find((f) => f.value === value[2]);
		districtName.value = districtInfo?.label;
		modelValue.value = value[2];
		emit("change", districtInfo);
	} else {
		provinceName.value = null;
		cityName.value = null;
		districtName.value = null;
		modelValue.value = null;
		emit("change", undefined);
	}
};

onMounted(async () => {
	state.regionList = await regionApi.regionSelector();
});
</script>
