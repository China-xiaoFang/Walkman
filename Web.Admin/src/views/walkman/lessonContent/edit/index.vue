<template>
	<FaDialog
		ref="faDialogRef"
		width="800"
		:title="state.dialogTitle"
		:showConfirmButton="!state.formDisabled"
		:showBeforeClose="!state.formDisabled"
		confirmButtonText="保存"
		@confirm-click="handleConfirm"
		@close="faFormRef.resetFields()"
	>
		<FaForm ref="faFormRef" :model="state.formData" :rules="state.formRules" :disabled="state.formDisabled" cols="1">
			<FaFormItem prop="lessonId" label="所属课程">
				<el-select v-model="state.formData.lessonId" placeholder="请选择课程" filterable>
					<el-option v-for="item in state.lessonList" :key="item.lessonId" :label="item.lessonName" :value="item.lessonId" />
				</el-select>
			</FaFormItem>
			<FaFormItem prop="englishText" label="英文内容">
				<el-input type="textarea" v-model="state.formData.englishText" :rows="8" maxlength="5000" placeholder="请输入英文内容" />
			</FaFormItem>
			<FaFormItem prop="chineseText" label="中文内容">
				<el-input type="textarea" v-model="state.formData.chineseText" :rows="8" maxlength="5000" placeholder="请输入中文内容" />
			</FaFormItem>
			<FaFormItem prop="grammarPoints" label="语法要点">
				<el-input type="textarea" v-model="state.formData.grammarPoints" :rows="4" maxlength="2000" placeholder="请输入语法要点" />
			</FaFormItem>
			<FaFormItem prop="knowledgePoints" label="知识要点">
				<el-input type="textarea" v-model="state.formData.knowledgePoints" :rows="4" maxlength="2000" placeholder="请输入知识要点" />
			</FaFormItem>
		</FaForm>
	</FaDialog>
</template>

<script lang="ts" setup>
import { reactive, ref } from "vue";
import { ElMessage, type FormRules } from "element-plus";
import { withDefineType } from "@fast-china/utils";
import { lessonContentApi } from "@/api/services/Admin/lessonContent";
import { textbookApi } from "@/api/services/Admin/textbook";
import { AddLessonContentInput } from "@/api/services/Admin/lessonContent/models/AddLessonContentInput";
import { EditLessonContentInput } from "@/api/services/Admin/lessonContent/models/EditLessonContentInput";
import type { FaDialogInstance, FaFormInstance } from "fast-element-plus";

defineOptions({
	name: "WalkmanLessonContentEdit",
});

const emit = defineEmits(["ok"]);

const faDialogRef = ref<FaDialogInstance>();
const faFormRef = ref<FaFormInstance>();

const state = reactive({
	formData: withDefineType<EditLessonContentInput & AddLessonContentInput>({}),
	formRules: withDefineType<FormRules>({
		lessonId: [{ required: true, message: "请选择所属课程", trigger: "change" }],
		englishText: [{ required: true, message: "请输入英文内容", trigger: "blur" }],
		chineseText: [{ required: true, message: "请输入中文内容", trigger: "blur" }],
	}),
	formDisabled: false,
	dialogState: withDefineType<IPageStateType>("detail"),
	dialogTitle: "课文内容",
	lessonList: withDefineType<Array<{ lessonId: number; lessonName: string }>>([]),
});

const loadLessonList = async () => {
	const res = await textbookApi.queryLessonPaged({ pageIndex: 1, pageSize: 999 });
	state.lessonList = (res.rows || []).map((item) => ({
		lessonId: item.lessonId,
		lessonName: item.lessonName,
	}));
};

const handleConfirm = () => {
	faDialogRef.value.close(async () => {
		await faFormRef.value.validateScrollToField();
		switch (state.dialogState) {
			case "add":
				await lessonContentApi.addLessonContent(state.formData);
				ElMessage.success("新增成功！");
				break;
			case "edit":
				await lessonContentApi.editLessonContent(state.formData);
				ElMessage.success("保存成功！");
				break;
		}
		emit("ok");
	});
};

const detail = (lessonContentId: number) => {
	faDialogRef.value.open(async () => {
		state.formDisabled = true;
		await loadLessonList();
		const apiRes = await lessonContentApi.queryLessonContentDetail(lessonContentId);
		state.formData = apiRes;
		state.dialogTitle = `课文内容详情`;
	});
};

const add = () => {
	faDialogRef.value.open(async () => {
		state.dialogState = "add";
		state.dialogTitle = "添加课文内容";
		state.formDisabled = false;
		await loadLessonList();
		state.formData = {};
	});
};

const edit = (lessonContentId: number) => {
	faDialogRef.value.open(async () => {
		state.dialogState = "edit";
		state.formDisabled = false;
		await loadLessonList();
		const apiRes = await lessonContentApi.queryLessonContentDetail(lessonContentId);
		state.formData = apiRes;
		state.dialogTitle = `编辑课文内容`;
	});
};

defineExpose({ element: faDialogRef, detail, add, edit });
</script>
