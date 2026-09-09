<template>
	<FaDialog
		ref="faDialogRef"
		width="800"
		:title="state.dialogTitle"
		:show-confirm-button="!state.formDisabled"
		:show-before-close="!state.formDisabled"
		confirm-button-text="保存"
		@confirm-click="handleConfirm"
		@close="faFormRef.resetFields()"
	>
		<FaForm ref="faFormRef" :model="state.formData" :rules="state.formRules" :disabled="state.formDisabled" cols="2">
			<FaFormItem prop="bookName" label="教材名称" span="2">
				<el-input v-model="state.formData.bookName" maxlength="50" placeholder="请输入教材名称" />
			</FaFormItem>
			<FaFormItem v-if="state.dialogState !== 'add'" prop="status" label="状态">
				<RadioGroup name="CommonStatusEnum" v-model="state.formData.status" />
			</FaFormItem>
			<FaFormItem prop="description" label="教材简介" span="2">
				<el-input v-model="state.formData.description" type="textarea" :rows="4" maxlength="1000" placeholder="请输入教材简介" />
			</FaFormItem>
			<FaFormItem prop="remark" label="备注" span="2">
				<el-input v-model="state.formData.remark" type="textarea" :rows="3" maxlength="200" placeholder="请输入备注" />
			</FaFormItem>
			<FaFormItem prop="sort" label="排序" tips="从小到大">
				<el-input-number v-model="state.formData.sort" :min="1" :max="9999" placeholder="请输入排序" />
			</FaFormItem>
			<FaFormItem prop="coverUrl" label="封面">
				<FaUploadImage v-model="state.formData.coverUrl" :upload-api="fileApi.uploadCover" />
			</FaFormItem>
		</FaForm>
	</FaDialog>
</template>

<script lang="ts" setup>
import { reactive, useTemplateRef } from "vue";
import { ElMessage } from "element-plus";
import { withDefineType } from "@fast-china/utils";
import { CommonStatusEnum } from "@/api/enums/CommonStatusEnum";
import { fileApi } from "@/api/services/File";
import { bookApi } from "@/api/services/Walkman/book";
import type { FormRules } from "element-plus";
import type { FaDialogInstance, FaFormInstance } from "fast-element-plus";
import type { AddBookInput } from "@/api/services/Walkman/book/models/AddBookInput";
import type { EditBookInput } from "@/api/services/Walkman/book/models/EditBookInput";

defineOptions({ name: "WalkmanBookEdit" });

const emit = defineEmits(["ok"]);
const faDialogRef = useTemplateRef<FaDialogInstance>("faDialogRef");
const faFormRef = useTemplateRef<FaFormInstance>("faFormRef");

const state = reactive({
	formData: withDefineType<EditBookInput & AddBookInput>({}),
	formRules: withDefineType<FormRules<EditBookInput & AddBookInput>>({
		bookName: [{ required: true, message: "请输入教材名称", trigger: "blur" }],
		sort: [{ required: true, message: "请输入排序", trigger: "blur" }],
	}),
	formDisabled: false,
	dialogState: withDefineType<IPageStateType>("detail"),
	dialogTitle: "教材",
});

const handleConfirm = () => {
	void faDialogRef.value.close(async () => {
		await faFormRef.value.validateScrollToField();
		if (state.dialogState === "add") {
			await bookApi.addBook(state.formData);
			ElMessage.success("新增成功！");
		} else if (state.dialogState === "edit") {
			await bookApi.editBook(state.formData);
			ElMessage.success("保存成功！");
		}
		emit("ok");
	});
};

const detail = (bookId: string) => {
	void faDialogRef.value.open(async () => {
		state.formDisabled = true;
		const apiRes = await bookApi.queryBookDetail(bookId);
		state.formData = apiRes;
		state.dialogTitle = `教材详情 - ${apiRes.bookName}`;
	});
};

const add = () => {
	void faDialogRef.value.open(() => {
		state.dialogState = "add";
		state.formDisabled = false;
		state.dialogTitle = "添加教材";
		state.formData = { status: CommonStatusEnum.Enable, sort: 1 };
	});
};

const edit = (bookId: string) => {
	void faDialogRef.value.open(async () => {
		state.dialogState = "edit";
		state.formDisabled = false;
		const apiRes = await bookApi.queryBookDetail(bookId);
		state.formData = apiRes;
		state.dialogTitle = `编辑教材 - ${apiRes.bookName}`;
	});
};

defineExpose({ element: faDialogRef, detail, add, edit });
</script>
