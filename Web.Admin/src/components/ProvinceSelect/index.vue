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
import { regionApi } from "@/api/services/Center/region";
import type { ElSelectorOutput } from "fast-element-plus";

defineOptions({
	name: "ProvinceSelect",
});

const emit = defineEmits<{
	change: [value: ElSelectorOutput];
}>();

const modelValue = defineModel<string>();
const provinceName = defineModel<string>("provinceName");
</script>
