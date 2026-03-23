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
import { CascaderValue } from "element-plus";
import { withDefineType } from "@fast-china/utils";
import { useVModel } from "@vueuse/core";
import { regionApi } from "@/api/services/Admin/region";
import type { ElSelectorOutput } from "fast-element-plus";

defineOptions({
	name: "CitySelect",
});

const props = withDefaults(
	defineProps<{
		modelValue?: number | string;
		provinceName?: string;
		cityName?: string;
	}>(),
	{}
);

const emit = defineEmits({
	"update:modelValue": (value: number | string) => true,
	"update:provinceName": (value: string) => true,
	"update:cityName": (value: string) => true,
	change: (value: ElSelectorOutput<number | string>) => true,
});

const modelValue = useVModel(props, "modelValue", emit);
const provinceName = useVModel(props, "provinceName", emit, { passive: true });
const cityName = useVModel(props, "cityName", emit, { passive: true });

const state = reactive({
	regionList: withDefineType<ElSelectorOutput<number | string>[]>([]),
});

const handleChange = (val: CascaderValue) => {
	const value = val as number[];
	if (value && value.length > 0) {
		const provinceInfo = state.regionList.find((f) => f.value === value[0]);
		provinceName.value = provinceInfo.label;
		const cityInfo = provinceInfo?.children?.find((f) => f.value === value[1]);
		cityName.value = cityInfo?.label;
		emit("update:modelValue", value[1]);
		emit("change", cityInfo);
	} else {
		provinceName.value = null;
		cityName.value = null;
		emit("update:modelValue", null);
		emit("change", undefined);
	}
};

onMounted(async () => {
	state.regionList = await regionApi.citySelector();
});
</script>
