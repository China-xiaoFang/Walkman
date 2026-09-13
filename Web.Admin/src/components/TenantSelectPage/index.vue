<template>
	<FaSelectPage
		v-bind="$attrs"
		:request-api="tenantApi.tenantSelector"
		v-model="modelValue"
		v-model:label="tenantName"
		placeholder="请选择租户"
		clearable
		more-detail
		@change="handleChange"
	>
		<template #default="data">
			<div style="display: flex; justify-content: space-between; align-items: center; gap: 8px; width: 100%">
				<FaAvatar v-if="data.data?.logoUrl" :src="data.data.logoUrl" thumb size="small" />
				<div style="flex: 1">
					<span>{{ data.label }}</span>
					<span style="display: flex; justify-content: space-between; width: 100%">
						<span style="font-size: var(--el-font-size-extra-small); padding-right: 8px">{{ data.data?.tenantNo }}</span>
						<Tag name="EditionEnum" :value="data.data?.edition" size="small" />
					</span>
				</div>
			</div>
		</template>
	</FaSelectPage>
</template>

<script lang="ts" setup>
import { tenantApi } from "@/api/services/Center/tenant";
import Tag from "../Tag/index.vue";
import type { ElSelectorOutput } from "fast-element-plus";

defineOptions({
	name: "TenantSelectPage",
});

const emit = defineEmits<{
	change: [value: ElSelectorOutput | undefined];
}>();

const modelValue = defineModel<string>();
const tenantName = defineModel<string>("tenantName");
const tenantNo = defineModel<string>("tenantNo");
const tenantCode = defineModel<string>("tenantCode");

const handleChange = (data: ElSelectorOutput | ElSelectorOutput[]) => {
	if (Array.isArray(data)) return;
	if (data) {
		tenantNo.value = data.data?.tenantNo;
		tenantCode.value = data.data?.tenantCode;
		emit("change", data);
	} else {
		tenantNo.value = undefined;
		tenantCode.value = undefined;
		emit("change", undefined);
	}
};
</script>
