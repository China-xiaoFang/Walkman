<template>
	<FaSelect
		v-bind="$attrs"
		:requestApi="regionApi.provinceSelector"
		v-model="modelValue"
		v-model:label="provinceName"
		placeholder="请选择省份"
		moreDetail
		@change="(value) => emit('change', value)"
	/>
</template>

<script lang="ts" setup>
import { useVModel } from "@vueuse/core";
import { regionApi } from "@/api/services/Admin/region";
import type { ElSelectorOutput } from "fast-element-plus";

defineOptions({
	name: "ProvinceSelect",
});

const props = withDefaults(
	defineProps<{
		modelValue?: number | string;
		provinceName?: string;
	}>(),
	{}
);

const emit = defineEmits({
	"update:modelValue": (value: number | string) => true,
	"update:provinceName": (value: string) => true,
	change: (value: ElSelectorOutput<number | string>) => true,
});

const modelValue = useVModel(props, "modelValue", emit, { passive: false });
const provinceName = useVModel(props, "provinceName", emit, { passive: false });
</script>
