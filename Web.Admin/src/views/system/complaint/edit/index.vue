<template>
	<FaDialog ref="faDialogRef" width="500" :title="state.dialogTitle" @confirm-click="handleConfirm" @close="faFormRef.resetFields()">
		<FaForm ref="faFormRef" :model="state.formData" :rules="state.formRules">
			<FaFormItem prop="handleDescription" label="处理描述">
				<el-input type="textarea" v-model="state.formData.handleDescription" :rows="2" maxlength="200" placeholder="请输入处理描述" />
			</FaFormItem>
			<FaFormItem prop="remark" label="备注">
				<el-input type="textarea" v-model="state.formData.remark" :rows="2" maxlength="200" placeholder="请输入备注" />
			</FaFormItem>
		</FaForm>
	</FaDialog>
</template>

<script lang="ts" setup>
import { reactive, ref } from "vue";
import { ElMessage, type FormRules } from "element-plus";
import { withDefineType } from "@fast-china/utils";
import { complaintApi } from "@/api/services/Admin/complaint";
import { HandleComplaintInput } from "@/api/services/Admin/complaint/models/HandleComplaintInput";
import type { FaDialogInstance, FaFormInstance } from "fast-element-plus";

defineOptions({
	name: "SystemComplaintEdit",
});

const emit = defineEmits(["ok"]);

const faDialogRef = ref<FaDialogInstance>();
const faFormRef = ref<FaFormInstance>();

const state = reactive({
	formData: withDefineType<HandleComplaintInput>({}),
	formRules: withDefineType<FormRules>({
		handleDescription: [{ required: true, message: "请输入处理描述", trigger: "blur" }],
	}),
	dialogState: withDefineType<IPageStateType>("detail"),
	dialogTitle: "处理投诉",
});

const handleConfirm = () => {
	faDialogRef.value.close(async () => {
		await faFormRef.value.validateScrollToField();
		await complaintApi.handleComplaint(state.formData);
		ElMessage.success("新增成功！");
		emit("ok");
	});
};

const open = (complaintId: number) => {
	faDialogRef.value.open(async () => {
		const apiRes = await complaintApi.queryComplaintDetail(complaintId);
		state.formData = {
			complaintId: apiRes.complaintId,
			rowVersion: apiRes.rowVersion,
		};
		state.dialogTitle = `处理投诉 - ${apiRes.nickName}`;
	});
};

// 暴露给父组件的参数和方法(外部需要什么，都可以从这里暴露出去)
defineExpose({
	element: faDialogRef,
	open,
});
</script>
