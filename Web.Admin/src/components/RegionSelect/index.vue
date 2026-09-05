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
import { useVModel } from "@vueuse/core";
import { onMounted, reactive } from "vue";
import { withDefineType } from "@fast-china/utils";
import { regionApi } from "@/api/services/Center/region";
import type { CascaderValue } from "element-plus";
import type { ElSelectorOutput } from "fast-element-plus";

defineOptions({
	name: "RegionSelect",
});

const props = defineProps<{
	modelValue?: string;
	provinceName?: string;
	cityName?: string;
	districtName?: string;
}>();

const emit = defineEmits({
	"update:modelValue": (_value: string) => true,
	"update:provinceName": (_value: string) => true,
	"update:cityName": (_value: string) => true,
	"update:districtName": (_value: string) => true,
	change: (_value: ElSelectorOutput<string>) => true,
});

const modelValue = useVModel(props, "modelValue", emit);
const provinceName = useVModel(props, "provinceName", emit, { passive: true });
const cityName = useVModel(props, "cityName", emit, { passive: true });
const districtName = useVModel(props, "districtName", emit, { passive: true });

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
		emit("update:modelValue", value[2]);
		emit("change", districtInfo);
	} else {
		provinceName.value = null;
		cityName.value = null;
		districtName.value = null;
		emit("update:modelValue", null);
		emit("change", undefined);
	}
};

onMounted(async () => {
	state.regionList = await regionApi.regionSelector();
});
</script>
