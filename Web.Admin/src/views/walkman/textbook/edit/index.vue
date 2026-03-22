<template>
	<FaDialog
		ref="faDialogRef"
		width="600"
		:title="state.dialogTitle"
		:showConfirmButton="!state.formDisabled"
		:showBeforeClose="!state.formDisabled"
		confirmButtonText="保存"
		@confirm-click="handleConfirm"
		@close="faFormRef.resetFields()"
	>
		<FaForm ref="faFormRef" :model="state.formData" :rules="state.formRules" :disabled="state.formDisabled" cols="1">
			<FaFormItem prop="textbookName" label="教材名称">
				<el-input v-model="state.formData.textbookName" maxlength="50" placeholder="请输入教材名称" />
			</FaFormItem>
			<FaFormItem prop="coverUrl" label="封面地址">
				<el-input v-model="state.formData.coverUrl" maxlength="500" placeholder="请输入封面图片URL" />
			</FaFormItem>
			<FaFormItem prop="description" label="描述">
				<el-input type="textarea" v-model="state.formData.description" :rows="3" maxlength="500" placeholder="请输入描述" />
			</FaFormItem>
			<FaFormItem prop="sort" label="排序" tips="从小到大">
				<el-input-number v-model="state.formData.sort" :min="1" :max="9999" placeholder="请输入排序" />
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
import { textbookApi } from "@/api/services/Admin/textbook";
import { AddTextbookInput } from "@/api/services/Admin/textbook/models/AddTextbookInput";
import { EditTextbookInput } from "@/api/services/Admin/textbook/models/EditTextbookInput";
import type { FaDialogInstance, FaFormInstance } from "fast-element-plus";

defineOptions({
	name: "WalkmanTextbookEdit",
});

const emit = defineEmits(["ok"]);

const faDialogRef = ref<FaDialogInstance>();
const faFormRef = ref<FaFormInstance>();

const state = reactive({
	formData: withDefineType<EditTextbookInput & AddTextbookInput>({}),
	formRules: withDefineType<FormRules>({
		textbookName: [{ required: true, message: "请输入教材名称", trigger: "blur" }],
		sort: [{ required: true, message: "请输入排序", trigger: "blur" }],
	}),
	formDisabled: false,
	dialogState: withDefineType<IPageStateType>("detail"),
	dialogTitle: "教材",
});

const handleConfirm = () => {
	faDialogRef.value.close(async () => {
		await faFormRef.value.validateScrollToField();
		switch (state.dialogState) {
			case "add":
				await textbookApi.addTextbook(state.formData);
				ElMessage.success("新增成功！");
				break;
			case "edit":
				await textbookApi.editTextbook(state.formData);
				ElMessage.success("保存成功！");
				break;
		}
		emit("ok");
	});
};

const detail = (textbookId: number) => {
	faDialogRef.value.open(async () => {
		state.formDisabled = true;
		const apiRes = await textbookApi.queryTextbookDetail(textbookId);
		state.formData = apiRes;
		state.dialogTitle = `教材详情 - ${apiRes.textbookName}`;
	});
};

const add = () => {
	faDialogRef.value.open(async () => {
		state.dialogState = "add";
		state.dialogTitle = "添加教材";
		state.formDisabled = false;
		state.formData = {
			sort: 1,
		};
	});
};

const edit = (textbookId: number) => {
	faDialogRef.value.open(async () => {
		state.dialogState = "edit";
		state.formDisabled = false;
		const apiRes = await textbookApi.queryTextbookDetail(textbookId);
		state.formData = apiRes;
		state.dialogTitle = `编辑教材 - ${apiRes.textbookName}`;
	});
};

defineExpose({
	element: faDialogRef,
	detail,
	add,
	edit,
});
</script>
