<template>
	<FaDialog
		ref="faDialogRef"
		width="500"
		title="生成激活码"
		confirm-button-text="生成"
		@confirm-click="handleConfirm"
		@close="faFormRef.resetFields()"
	>
		<FaForm ref="faFormRef" :model="state.formData" :rules="state.formRules">
			<FaFormItem prop="count" label="生成数量" tips="单次最多生成 500 个">
				<el-input-number v-model="state.formData.count" :min="1" :max="500" />
			</FaFormItem>
		</FaForm>
	</FaDialog>
</template>

<script lang="ts" setup>
import { reactive, useTemplateRef } from "vue";
import { ElMessage } from "element-plus";
import { withDefineType } from "@fast-china/utils";
import { activationCodeApi } from "@/api/services/Walkman/activationCode";
import type { FormRules } from "element-plus";
import type { FaDialogInstance, FaFormInstance } from "fast-element-plus";
import type { GenerateActivationCodeInput } from "@/api/services/Walkman/activationCode/models/GenerateActivationCodeInput";

defineOptions({ name: "WalkmanActivationCodeGenerate" });

const emit = defineEmits(["ok"]);
const faDialogRef = useTemplateRef<FaDialogInstance>("faDialogRef");
const faFormRef = useTemplateRef<FaFormInstance>("faFormRef");
const state = reactive({
	formData: withDefineType<GenerateActivationCodeInput>({ count: 1 }),
	formRules: withDefineType<FormRules<GenerateActivationCodeInput>>({
		count: [{ required: true, message: "请输入生成数量", trigger: "blur" }],
	}),
});

const handleConfirm = () => {
	void faDialogRef.value.close(async () => {
		await faFormRef.value.validateScrollToField();
		await activationCodeApi.generateActivationCode(state.formData);
		ElMessage.success("生成成功！");
		emit("ok");
	});
};

const open = () => {
	void faDialogRef.value.open(() => {
		state.formData = { count: 1 };
	});
};

defineExpose({ element: faDialogRef, open });
</script>
