<template>
	<FaDialog
		ref="faDialogRef"
		width="500"
		:title="state.dialogTitle"
		:showConfirmButton="!state.formDisabled"
		:showBeforeClose="!state.formDisabled"
		confirmButtonText="保存"
		@confirm-click="handleConfirm"
		@close="faFormRef.resetFields()"
	>
		<FaForm ref="faFormRef" :model="state.formData" :rules="state.formRules" :disabled="state.formDisabled">
			<FaFormItem prop="prefix" label="前缀">
				<el-input v-model="state.formData.prefix" maxlength="5" placeholder="请输入前缀" />
			</FaFormItem>
			<FaFormItem v-if="state.dialogState === 'add'" prop="ruleType" label="规则类型">
				<RadioGroup name="SerialRuleTypeEnum" v-model="state.formData.ruleType" />
			</FaFormItem>
			<FaFormItem prop="dateType" label="时间类型">
				<RadioGroup name="SerialDateTypeEnum" v-model="state.formData.dateType" />
			</FaFormItem>
			<FaFormItem prop="spacer" label="分隔符">
				<RadioGroup name="SerialSpacerEnum" v-model="state.formData.spacer" />
			</FaFormItem>
			<FaFormItem prop="length" label="长度">
				<el-input-number v-model="state.formData.length" :min="1" :max="6" placeholder="请输入长度" />
			</FaFormItem>
		</FaForm>
	</FaDialog>
</template>

<script lang="ts" setup>
import { reactive, ref } from "vue";
import { ElMessage, type FormRules } from "element-plus";
import { withDefineType } from "@fast-china/utils";
import { SerialDateTypeEnum } from "@/api/enums/SerialDateTypeEnum";
import { SerialRuleTypeEnum } from "@/api/enums/SerialRuleTypeEnum";
import { SerialSpacerEnum } from "@/api/enums/SerialSpacerEnum";
import { serialApi } from "@/api/services/Admin/serial";
import type { AddSerialRuleInput } from "@/api/services/Admin/serial/models/AddSerialRuleInput";
import type { EditSerialRuleInput } from "@/api/services/Admin/serial/models/EditSerialRuleInput";
import type { FaDialogInstance, FaFormInstance } from "fast-element-plus";

defineOptions({
	name: "DevSerialEdit",
});

const emit = defineEmits(["ok"]);

const faDialogRef = ref<FaDialogInstance>();
const faFormRef = ref<FaFormInstance>();

const state = reactive({
	formData: withDefineType<EditSerialRuleInput & AddSerialRuleInput>({}),
	formRules: withDefineType<FormRules>({
		length: [{ required: true, message: "请输入长度", trigger: "blur" }],
	}),
	formDisabled: false,
	dialogState: withDefineType<IPageStateType>("detail"),
	dialogTitle: "序号",
});

const handleConfirm = () => {
	faDialogRef.value.close(async () => {
		await faFormRef.value.validateScrollToField();
		switch (state.dialogState) {
			case "add":
				await serialApi.addSerialRule(state.formData);
				ElMessage.success("新增成功！");
				break;
			case "edit":
				await serialApi.editSerialRule(state.formData);
				ElMessage.success("保存成功！");
				break;
		}
		emit("ok");
	});
};

const detail = (serialRuleId: number) => {
	faDialogRef.value.open(async () => {
		state.formDisabled = true;
		const apiRes = await serialApi.querySerialRuleDetail(serialRuleId);
		state.formData = apiRes;
		state.dialogTitle = `序号详情`;
	});
};

const add = () => {
	faDialogRef.value.open(() => {
		state.dialogState = "add";
		state.dialogTitle = "添加序号";
		state.formDisabled = false;
		state.formData = {
			ruleType: SerialRuleTypeEnum.EmployeeNo,
			dateType: SerialDateTypeEnum.Year,
			spacer: SerialSpacerEnum.None,
		};
	});
};

const edit = (serialRuleId: number) => {
	faDialogRef.value.open(async () => {
		state.dialogState = "edit";
		state.formDisabled = false;
		const apiRes = await serialApi.querySerialRuleDetail(serialRuleId);
		state.formData = apiRes;
		state.dialogTitle = `编辑序号`;
	});
};

// 暴露给父组件的参数和方法(外部需要什么，都可以从这里暴露出去)
defineExpose({
	element: faDialogRef,
	detail,
	add,
	edit,
});
</script>
