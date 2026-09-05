<template>
	<FaSelectPage
		v-bind="$attrs"
		:request-api="accountApi.accountSelector"
		v-model="modelValue"
		v-model:label="mobile"
		placeholder="请选择账号"
		clearable
		more-detail
		@change="handleChange"
	>
		<template #default="data">
			<div style="display: flex; justify-content: space-between; align-items: center; gap: 8px; width: 100%">
				<FaAvatar :src="data.data?.avatar" thumb size="small" />
				<div style="flex: 1">
					<span>{{ data.label }}</span>
					<span style="display: flex; justify-content: space-between; width: 100%">
						<span style="font-size: var(--el-font-size-extra-small); padding-right: 8px">{{ data.data?.nickName }}</span>
						<span style="font-size: var(--el-font-size-extra-small)">{{ data.data?.email }}</span>
					</span>
				</div>
			</div>
		</template>
	</FaSelectPage>
</template>

<script lang="ts" setup>
import { useVModel } from "@vueuse/core";
import { accountApi } from "@/api/services/Center/account";
import type { ElSelectorOutput } from "fast-element-plus";

defineOptions({
	name: "AccountSelectPage",
});

const props = defineProps<{
	modelValue?: string;
	mobile?: string;
	email?: string;
	accountKey?: string;
}>();

const emit = defineEmits({
	"update:modelValue": (_value: string) => true,
	"update:mobile": (_value: string) => true,
	"update:email": (_value: string) => true,
	"update:accountKey": (_value: string) => true,
	change: (_value: ElSelectorOutput) => true,
});

const modelValue = useVModel(props, "modelValue", emit, { passive: false });
const mobile = useVModel(props, "mobile", emit, { passive: false });
const email = useVModel(props, "email", emit, { passive: true });
const accountKey = useVModel(props, "accountKey", emit, { passive: true });

const handleChange = (data: ElSelectorOutput | ElSelectorOutput[]) => {
	if (Array.isArray(data)) return;
	if (data) {
		email.value = data.data?.email;
		accountKey.value = data.data?.accountKey;
		emit("change", data);
	} else {
		email.value = undefined;
		accountKey.value = undefined;
		emit("change", undefined);
	}
};
</script>
