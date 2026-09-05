<template>
	<FaSelect
		v-bind="$attrs"
		:request-api="() => merchantApi.merchantSelector(props.merchantType)"
		v-model="modelValue"
		v-model:label="merchantNo"
		placeholder="请选择商户号"
		clearable
		more-detail
		@change="(data) => emit('change', data)"
	>
		<template #default="data">
			<span>{{ data.data?.merchantName }}</span>
			<span style="display: flex; justify-content: space-between; width: 100%">
				<span style="font-size: var(--el-font-size-extra-small); padding-right: 8px">{{ data.label }}</span>
				<Tag name="PaymentChannelEnum" :value="data.data?.merchantType" size="small" />
			</span>
		</template>
	</FaSelect>
</template>

<script lang="ts" setup>
import { useVModel } from "@vueuse/core";
import { merchantApi } from "@/api/services/Center/merchant";
import type { ElSelectorOutput } from "fast-element-plus";
import type { PaymentChannelEnum } from "@/api/enums/PaymentChannelEnum";

defineOptions({
	name: "MerchantSelect",
});

const props = defineProps<{
	modelValue?: string;
	merchantNo?: string;
	merchantType?: PaymentChannelEnum;
}>();

const emit = defineEmits({
	"update:modelValue": (_value: string) => true,
	"update:merchantNo": (_value: string) => true,
	change: (_value: ElSelectorOutput) => true,
});

const modelValue = useVModel(props, "modelValue", emit, { passive: false });
const merchantNo = useVModel(props, "merchantNo", emit, { passive: false });
</script>
