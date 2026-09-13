<template>
	<FaTreeSelect
		v-bind="$attrs"
		:request-api="() => departmentApi.departmentSelector(props.orgId)"
		v-model="modelValue"
		v-model:label="departmentName"
		:placeholder="props.placeholder || '请选择部门'"
		check-strictly
		filterable
		clearable
		@change="(value) => emit('change', value)"
	/>
</template>

<script lang="ts" setup>
import { departmentApi } from "@/api/services/Admin/department";
import type { ElSelectorOutput } from "fast-element-plus";

defineOptions({
	name: "DepartmentTreeSelect",
});

const props = defineProps<{
	orgId?: string;
	placeholder?: string;
}>();

const emit = defineEmits<{
	change: [value: ElSelectorOutput];
}>();

const modelValue = defineModel<string | string[]>();
const departmentName = defineModel<string>("departmentName");
</script>
