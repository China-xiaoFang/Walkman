<template>
	<FaInputDialogPage
		v-bind="$attrs"
		row-key="employeeId"
		label-key="employeeName"
		:request-api="employeeApi.queryEmployeePaged"
		v-model="modelValue"
		v-model:label="employeeName"
		placeholder="请选择职员"
		@change="handleChange"
	>
		<FaTableColumn prop="idPhoto" label="头像" fixed type="image" width="50" small-width="50" />
		<FaTableColumn prop="employeeName" label="名称" fixed width="200" small-width="180" sortable />
		<FaTableColumn prop="employeeNo" label="工号" width="150" small-width="130" sortable />
		<FaTableColumn prop="mobile" label="手机" width="150" small-width="130" sortable />
		<FaTableColumn prop="departmentName" label="部门" width="150" small-width="130" sortable />
		<FaTableColumn prop="positionName" label="职位" width="150" small-width="130" sortable />
		<FaTableColumn prop="jobLevelName" label="职级" width="150" small-width="130" sortable />
	</FaInputDialogPage>
</template>

<script lang="ts" setup>
import { useVModel } from "@vueuse/core";
import { employeeApi } from "@/api/services/Admin/employee";
import type { QueryEmployeePagedOutput } from "@/api/services/Admin/employee/models/QueryEmployeePagedOutput";

defineOptions({
	name: "EmployeeDialogSelect",
});

const props = defineProps<{
	modelValue?: string;
	employeeName?: string;
	employeeNo?: string;
	mobile?: string;
}>();

const emit = defineEmits({
	"update:modelValue": (_value: string) => true,
	"update:employeeName": (_value: string) => true,
	"update:employeeNo": (_value: string) => true,
	"update:mobile": (_value: string) => true,
	change: (_data: QueryEmployeePagedOutput) => true,
});

const modelValue = useVModel(props, "modelValue", emit, { passive: false });
const employeeName = useVModel(props, "employeeName", emit, { passive: false });
const employeeNo = useVModel(props, "employeeNo", emit, { passive: true });
const mobile = useVModel(props, "mobile", emit, { passive: true });

const handleChange = (data: QueryEmployeePagedOutput) => {
	if (data) {
		employeeNo.value = data.employeeNo;
		mobile.value = data.mobile;
		emit("change", data);
	} else {
		employeeNo.value = undefined;
		mobile.value = undefined;
		emit("change", undefined);
	}
};
</script>
