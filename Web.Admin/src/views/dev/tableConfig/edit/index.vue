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
			<FaFormItem prop="tableName" label="表格名称">
				<el-input v-model="state.formData.tableName" maxlength="50" placeholder="请输入表格名称" />
			</FaFormItem>
			<FaFormItem prop="remark" label="备注">
				<el-input v-model="state.formData.remark" type="textarea" maxlength="200" placeholder="请输入备注" />
			</FaFormItem>
		</FaForm>
	</FaDialog>
</template>

<script lang="ts" setup>
import { reactive, ref } from "vue";
import { ElMessage, type FormRules } from "element-plus";
import { FaDialog } from "fast-element-plus";
import { withDefineType } from "@fast-china/utils";
import { tableApi } from "@/api/services/Admin/table";
import type { AddTableConfigInput } from "@/api/services/Admin/table/models/AddTableConfigInput";
import type { EditTableConfigInput } from "@/api/services/Admin/table/models/EditTableConfigInput";
import type { FaDialogInstance, FaFormInstance } from "fast-element-plus";

defineOptions({
	name: "DevTableConfigEdit",
});

const emit = defineEmits(["ok"]);

const faDialogRef = ref<FaDialogInstance>();
const faFormRef = ref<FaFormInstance>();

const state = reactive({
	formData: withDefineType<EditTableConfigInput & AddTableConfigInput>({}),
	formRules: withDefineType<FormRules>({
		tableName: [{ required: true, message: "请输入表格名称", trigger: "blur" }],
	}),
	formDisabled: false,
	dialogState: withDefineType<IPageStateType>("detail"),
	dialogTitle: "表格",
	copyTableId: withDefineType<number>(undefined),
});

const handleConfirm = () => {
	faDialogRef.value.close(async () => {
		await faFormRef.value.validateScrollToField();
		switch (state.dialogState) {
			case "add":
				await tableApi.addTableConfig(state.formData);
				ElMessage.success("新增成功！");
				break;
			case "edit":
				await tableApi.editTableConfig(state.formData);
				ElMessage.success("保存成功！");
				break;
			case "copy":
				await tableApi.copyTableConfig({
					...state.formData,
					tableId: state.copyTableId,
				});
				ElMessage.success("复制成功");
				break;
		}
		emit("ok");
	});
};

const detail = (tableId: number) => {
	faDialogRef.value.open(async () => {
		state.formDisabled = true;
		const apiRes = await tableApi.queryTableConfigDetail(tableId);
		state.formData = apiRes;
		state.dialogTitle = `表格详情 - ${apiRes.tableName}`;
	});
};

const add = () => {
	faDialogRef.value.open(() => {
		state.dialogState = "add";
		state.dialogTitle = "添加表格";
		state.formDisabled = false;
		state.formData = {};
	});
};

const edit = (tableId: number) => {
	faDialogRef.value.open(async () => {
		state.dialogState = "edit";
		state.formDisabled = false;
		const apiRes = await tableApi.queryTableConfigDetail(tableId);
		state.formData = apiRes;
		state.dialogTitle = `编辑表格 - ${apiRes.tableName}`;
	});
};

const copy = (tableId: number) => {
	faDialogRef.value.open(async () => {
		state.copyTableId = tableId;
		state.dialogState = "copy";
		state.formDisabled = false;
		state.dialogTitle = "复制表格配置";
	});
};

// 暴露给父组件的参数和方法(外部需要什么，都可以从这里暴露出去)
defineExpose({
	element: faDialogRef,
	detail,
	add,
	edit,
	copy,
});
</script>
