<template>
	<FaSelect
		v-bind="$attrs"
		:request-api="regionApi.provinceSelector"
		v-model="modelValue"
		v-model:label="provinceName"
		placeholder="请选择省份"
		more-detail
		@change="(data) => emit('change', data)"
	/>
</template>

<script lang="ts" setup>
import { useVModel } from "@vueuse/core";
import { regionApi } from "@/api/services/Center/region";
import type { ElSelectorOutput } from "fast-element-plus";

defineOptions({
	name: "ProvinceSelect",
});

const props = defineProps<{
	modelValue?: string;
	provinceName?: string;
}>();

const emit = defineEmits({
	"update:modelValue": (_value: string) => true,
	"update:provinceName": (_value: string) => true,
	change: (_value: ElSelectorOutput) => true,
});

const modelValue = useVModel(props, "modelValue", emit, { passive: false });
const provinceName = useVModel(props, "provinceName", emit, { passive: false });
</script>
