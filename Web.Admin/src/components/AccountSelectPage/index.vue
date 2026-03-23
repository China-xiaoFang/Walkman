<template>
	<FaSelectPage
		v-bind="$attrs"
		:requestApi="accountApi.accountSelector"
		v-model="modelValue"
		v-model:label="mobile"
		placeholder="请选择账号"
		clearable
		moreDetail
		@change="(value) => emit('change', value)"
	>
		<template #default="data">
			<div style="display: flex; justify-content: space-between; align-items: center; gap: 8px; width: 100%">
				<FaAvatar :src="data.data.avatar" thumb size="small" />
				<div style="flex: 1">
					<span>{{ data.label }}</span>
					<span style="display: flex; justify-content: space-between; width: 100%">
						<span style="font-size: var(--el-font-size-extra-small); padding-right: 8px">{{ data.data?.nickName }}</span>
						<span style="font-size: var(--el-font-size-extra-small)">{{ data.data?.mobile }}</span>
					</span>
				</div>
			</div>
		</template>
	</FaSelectPage>
</template>

<script lang="ts" setup>
import { useVModel } from "@vueuse/core";
import { accountApi } from "@/api/services/Admin/account";
import type { ElSelectorOutput } from "fast-element-plus";

defineOptions({
	name: "AccountSelectPage",
});

const props = withDefaults(
	defineProps<{
		modelValue?: number | string;
		mobile?: string;
		accountKey?: string;
	}>(),
	{}
);

const emit = defineEmits({
	"update:modelValue": (value: number | string) => true,
	"update:mobile": (value: string) => true,
	change: (value: ElSelectorOutput<number | string>) => true,
});

const modelValue = useVModel(props, "modelValue", emit, { passive: false });
const mobile = useVModel(props, "mobile", emit, { passive: false });
</script>
