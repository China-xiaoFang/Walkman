<template>
	<FaSelect
		v-bind="$attrs"
		:request-api="applicationApi.applicationSelector"
		v-model="modelValue"
		v-model:label="appName"
		placeholder="请选择应用"
		clearable
		more-detail
		@change="(data) => emit('change', data)"
	>
		<template #default="data">
			<div style="display: flex; justify-content: space-between; align-items: center; gap: 8px; width: 100%">
				<FaAvatar v-if="data.data?.logoUrl" :src="data.data.logoUrl" thumb size="small" />
				<div style="flex: 1">
					<span>{{ data.label }}</span>
					<span style="display: flex; justify-content: space-between; width: 100%">
						<span style="font-size: var(--el-font-size-extra-small); padding-right: 8px">{{ data.data?.appNo }}</span>
						<Tag name="EditionEnum" :value="data.data?.edition" size="small" />
					</span>
				</div>
			</div>
		</template>
	</FaSelect>
</template>

<script lang="ts" setup>
import { useVModel } from "@vueuse/core";
import { applicationApi } from "@/api/services/Center/application";
import type { ElSelectorOutput } from "fast-element-plus";

defineOptions({
	name: "ApplicationSelect",
});

const props = defineProps<{
	modelValue?: string;
	appName?: string;
}>();

const emit = defineEmits({
	"update:modelValue": (_value: string) => true,
	"update:appName": (_value: string) => true,
	change: (_value: ElSelectorOutput) => true,
});

const modelValue = useVModel(props, "modelValue", emit, { passive: false });
const appName = useVModel(props, "appName", emit, { passive: false });
</script>
