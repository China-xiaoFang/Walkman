<template>
	<FaSelectPage
		v-bind="$attrs"
		:request-api="employeeApi.employeeSelector"
		v-model="modelValue"
		v-model:label="employeeName"
		placeholder="请选择职员"
		clearable
		more-detail
		@change="handleChange"
	>
		<template #default="data">
			<div style="display: flex; justify-content: space-between; align-items: center; gap: 8px; width: 100%">
				<FaAvatar :src="data.data?.idPhoto" thumb size="small" />
				<div style="flex: 1">
					<span>{{ data.label }}</span>
					<span style="display: flex; justify-content: space-between; width: 100%">
						<span style="font-size: var(--el-font-size-extra-small); padding-right: 8px">{{ data.data?.employeeNo }}</span>
						<span style="font-size: var(--el-font-size-extra-small)">{{ data.data?.mobile }}</span>
					</span>
				</div>
			</div>
		</template>
	</FaSelectPage>
</template>

<script lang="ts" setup>
import { useVModel } from "@vueuse/core";
import { employeeApi } from "@/api/services/Admin/employee";
import type { ElSelectorOutput } from "fast-element-plus";

defineOptions({
	name: "EmployeeSelectPage",
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
	change: (_value: ElSelectorOutput) => true,
});

const modelValue = useVModel(props, "modelValue", emit, { passive: false });
const employeeName = useVModel(props, "employeeName", emit, { passive: false });
const employeeNo = useVModel(props, "employeeNo", emit, { passive: true });
const mobile = useVModel(props, "mobile", emit, { passive: true });

const handleChange = (data: ElSelectorOutput | ElSelectorOutput[]) => {
	if (Array.isArray(data)) return;
	if (data) {
		employeeNo.value = data.data?.employeeNo;
		mobile.value = data.data?.mobile;
		emit("change", data);
	} else {
		employeeNo.value = undefined;
		mobile.value = undefined;
		emit("change", undefined);
	}
};
</script>
