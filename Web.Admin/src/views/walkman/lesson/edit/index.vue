<template>
	<FaDialog
		ref="faDialogRef"
		width="900"
		:title="state.dialogTitle"
		:show-confirm-button="!state.formDisabled"
		:show-before-close="!state.formDisabled"
		confirm-button-text="保存"
		@confirm-click="handleConfirm"
		@close="faFormRef.resetFields()"
	>
		<FaForm ref="faFormRef" :model="state.formData" :rules="state.formRules" :disabled="state.formDisabled" cols="2">
			<FaFormItem prop="bookId" label="教材" span="2">
				<FaSelectPage
					:request-api="bookApi.bookSelector"
					v-model="state.formData.bookId"
					v-model:label="state.formData.bookName"
					placeholder="请选择教材"
					filterable
					clearable
				/>
			</FaFormItem>
			<FaFormItem prop="lessonTitle" label="课程标题">
				<el-input v-model="state.formData.lessonTitle" maxlength="100" placeholder="请输入课程标题" />
			</FaFormItem>
			<FaFormItem prop="lessonNumber" label="课程编号">
				<el-input-number v-model="state.formData.lessonNumber" :min="1" :max="9999" placeholder="请输入课程编号" />
			</FaFormItem>
			<FaFormItem prop="english" label="英文" span="2">
				<el-input v-model="state.formData.english" type="textarea" :rows="4" maxlength="100" placeholder="请输入英文内容" />
			</FaFormItem>
			<FaFormItem prop="chinese" label="中文" span="2">
				<el-input v-model="state.formData.chinese" type="textarea" :rows="4" maxlength="100" placeholder="请输入中文内容" />
			</FaFormItem>
			<FaFormItem prop="remark" label="备注" span="2">
				<el-input v-model="state.formData.remark" type="textarea" :rows="2" maxlength="200" placeholder="请输入备注" />
			</FaFormItem>
		</FaForm>
	</FaDialog>
</template>

<script lang="ts" setup>
import { reactive, useTemplateRef } from "vue";
import { ElMessage } from "element-plus";
import { withDefineType } from "@fast-china/utils";
import { bookApi } from "@/api/services/Walkman/book";
import { lessonApi } from "@/api/services/Walkman/lesson";
import type { FormRules } from "element-plus";
import type { FaDialogInstance, FaFormInstance } from "fast-element-plus";
import type { AddLessonInput } from "@/api/services/Walkman/lesson/models/AddLessonInput";
import type { EditLessonInput } from "@/api/services/Walkman/lesson/models/EditLessonInput";
import type { QueryLessonDetailOutput } from "@/api/services/Walkman/lesson/models/QueryLessonDetailOutput";

defineOptions({ name: "WalkmanLessonEdit" });

const emit = defineEmits(["ok"]);
const faDialogRef = useTemplateRef<FaDialogInstance>("faDialogRef");
const faFormRef = useTemplateRef<FaFormInstance>("faFormRef");

const state = reactive({
	formData: withDefineType<EditLessonInput & AddLessonInput & QueryLessonDetailOutput>({}),
	formRules: withDefineType<FormRules<EditLessonInput & AddLessonInput>>({
		bookId: [{ required: true, message: "请选择教材", trigger: "change" }],
		lessonTitle: [{ required: true, message: "请输入课程标题", trigger: "blur" }],
		lessonNumber: [{ required: true, message: "请输入课程编号", trigger: "blur" }],
		english: [{ required: true, message: "请输入英文内容", trigger: "blur" }],
		chinese: [{ required: true, message: "请输入中文内容", trigger: "blur" }],
	}),
	formDisabled: false,
	dialogState: withDefineType<IPageStateType>("detail"),
	dialogTitle: "课程",
});

const handleConfirm = () => {
	void faDialogRef.value.close(async () => {
		await faFormRef.value.validateScrollToField();
		if (state.dialogState === "add") {
			await lessonApi.addLesson(state.formData);
			ElMessage.success("新增成功！");
		} else if (state.dialogState === "edit") {
			await lessonApi.editLesson(state.formData);
			ElMessage.success("保存成功！");
		}
		emit("ok");
	});
};

const detail = (lessonId: string) => {
	void faDialogRef.value.open(async () => {
		state.formDisabled = true;
		const apiRes = await lessonApi.queryLessonDetail(lessonId);
		state.formData = apiRes;
		state.dialogTitle = `课程详情 - ${apiRes.lessonTitle}`;
	});
};

const add = () => {
	void faDialogRef.value.open(() => {
		state.dialogState = "add";
		state.formDisabled = false;
		state.dialogTitle = "添加课程";
		state.formData = { lessonNumber: 1 };
	});
};

const edit = (lessonId: string) => {
	void faDialogRef.value.open(async () => {
		state.dialogState = "edit";
		state.formDisabled = false;
		const apiRes = await lessonApi.queryLessonDetail(lessonId);
		state.formData = apiRes;
		state.dialogTitle = `编辑课程 - ${apiRes.lessonTitle}`;
	});
};

defineExpose({ element: faDialogRef, detail, add, edit });
</script>
