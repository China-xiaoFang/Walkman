<template>
	<FaDialog
		ref="faDialogRef"
		width="700"
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
			<FaFormItem prop="questionType" label="题目类型">
				<RadioGroup name="QuestionTypeEnum" v-model="state.formData.questionType" />
			</FaFormItem>
			<FaFormItem prop="content" label="题目内容">
				<el-input type="textarea" v-model="state.formData.content" :rows="3" maxlength="2000" placeholder="请输入题目内容" />
			</FaFormItem>
			<FaFormItem prop="answer" label="答案">
				<el-input v-model="state.formData.answer" maxlength="500" placeholder="请输入答案" />
			</FaFormItem>
			<FaFormItem prop="explanation" label="解析">
				<el-input type="textarea" v-model="state.formData.explanation" :rows="2" maxlength="1000" placeholder="请输入解析" />
			</FaFormItem>
			<FaFormItem prop="score" label="分值">
				<el-input-number v-model="state.formData.score" :min="0" :max="100" placeholder="请输入分值" />
			</FaFormItem>
			<FaFormItem prop="sort" label="排序" tips="从小到大">
				<el-input-number v-model="state.formData.sort" :min="1" :max="9999" placeholder="请输入排序" />
			</FaFormItem>

			<!-- 选项区域：仅单选题和多选题显示 -->
			<FaFormItem
				v-if="state.formData.questionType === QuestionTypeEnum.SingleChoice || state.formData.questionType === QuestionTypeEnum.MultipleChoice"
				prop="options"
				label="选项列表"
			>
				<div style="width: 100%">
					<div v-for="(option, index) in state.formData.options" :key="index" style="display: flex; align-items: center; margin-bottom: 8px; gap: 8px">
						<el-input v-model="option.label" placeholder="标签" style="width: 80px" maxlength="10" />
						<el-input v-model="option.content" placeholder="选项内容" style="flex: 1" maxlength="500" />
						<el-checkbox v-model="option.isCorrect" :disabled="state.formDisabled">正确</el-checkbox>
						<el-button v-if="!state.formDisabled" type="danger" :icon="Minus" circle size="small" @click="removeOption(index)" />
					</div>
					<el-button v-if="!state.formDisabled" type="primary" :icon="Plus" size="small" @click="addOption">添加选项</el-button>
				</div>
			</FaFormItem>
		</FaForm>
	</FaDialog>
</template>

<script lang="ts" setup>
import { reactive, ref } from "vue";
import { ElMessage, type FormRules } from "element-plus";
import { Plus, Minus } from "@element-plus/icons-vue";
import { withDefineType } from "@fast-china/utils";
import { questionApi } from "@/api/services/Admin/question";
import { textbookApi } from "@/api/services/Admin/textbook";
import { AddQuestionInput, AddQuestionOptionInput } from "@/api/services/Admin/question/models/AddQuestionInput";
import { EditQuestionInput } from "@/api/services/Admin/question/models/EditQuestionInput";
import { QuestionTypeEnum } from "@/api/enums/QuestionTypeEnum";
import type { FaDialogInstance, FaFormInstance } from "fast-element-plus";

defineOptions({
	name: "WalkmanQuestionEdit",
});

const emit = defineEmits(["ok"]);

const faDialogRef = ref<FaDialogInstance>();
const faFormRef = ref<FaFormInstance>();

const state = reactive({
	formData: withDefineType<EditQuestionInput & AddQuestionInput>({}),
	formRules: withDefineType<FormRules>({
		lessonId: [{ required: true, message: "请选择所属课程", trigger: "change" }],
		questionType: [{ required: true, message: "请选择题目类型", trigger: "change" }],
		content: [{ required: true, message: "请输入题目内容", trigger: "blur" }],
		score: [{ required: true, message: "请输入分值", trigger: "blur" }],
	}),
	formDisabled: false,
	dialogState: withDefineType<IPageStateType>("detail"),
	dialogTitle: "题目",
	lessonList: withDefineType<Array<{ lessonId: number; lessonName: string }>>([]),
});

const loadLessonList = async () => {
	const res = await textbookApi.queryLessonPaged({ pageIndex: 1, pageSize: 999 });
	state.lessonList = (res.rows || []).map((item) => ({
		lessonId: item.lessonId,
		lessonName: item.lessonName,
	}));
};

const addOption = () => {
	if (!state.formData.options) {
		state.formData.options = [];
	}
	state.formData.options.push({ label: "", content: "", isCorrect: false, sort: state.formData.options.length + 1 });
};

const removeOption = (index: number) => {
	state.formData.options?.splice(index, 1);
};

const handleConfirm = () => {
	faDialogRef.value.close(async () => {
		await faFormRef.value.validateScrollToField();
		switch (state.dialogState) {
			case "add":
				await questionApi.addQuestion(state.formData);
				ElMessage.success("新增成功！");
				break;
			case "edit":
				await questionApi.editQuestion(state.formData);
				ElMessage.success("保存成功！");
				break;
		}
		emit("ok");
	});
};

const detail = (questionId: number) => {
	faDialogRef.value.open(async () => {
		state.formDisabled = true;
		await loadLessonList();
		const apiRes = await questionApi.queryQuestionDetail(questionId);
		state.formData = apiRes;
		state.dialogTitle = `题目详情`;
	});
};

const add = () => {
	faDialogRef.value.open(async () => {
		state.dialogState = "add";
		state.dialogTitle = "添加题目";
		state.formDisabled = false;
		await loadLessonList();
		state.formData = { score: 1, sort: 1, options: [] };
	});
};

const edit = (questionId: number) => {
	faDialogRef.value.open(async () => {
		state.dialogState = "edit";
		state.formDisabled = false;
		await loadLessonList();
		const apiRes = await questionApi.queryQuestionDetail(questionId);
		state.formData = apiRes;
		state.dialogTitle = `编辑题目`;
	});
};

defineExpose({ element: faDialogRef, detail, add, edit });
</script>
