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
			<FaFormItem prop="lessonId" label="所属课程">
				<el-select v-model="state.formData.lessonId" placeholder="请选择课程" filterable>
					<el-option v-for="item in state.lessonList" :key="item.lessonId" :label="item.lessonName" :value="item.lessonId" />
				</el-select>
			</FaFormItem>
			<FaFormItem prop="english" label="英文">
				<el-input v-model="state.formData.english" maxlength="200" placeholder="请输入英文" />
			</FaFormItem>
			<FaFormItem prop="chinese" label="中文">
				<el-input v-model="state.formData.chinese" maxlength="200" placeholder="请输入中文" />
			</FaFormItem>
			<FaFormItem prop="phonetic" label="音标">
				<el-input v-model="state.formData.phonetic" maxlength="200" placeholder="请输入音标" />
			</FaFormItem>
			<FaFormItem prop="audioUrl" label="音频地址">
				<el-input v-model="state.formData.audioUrl" maxlength="500" placeholder="请输入音频CDN地址" />
			</FaFormItem>
			<FaFormItem prop="exampleSentence" label="例句">
				<el-input type="textarea" v-model="state.formData.exampleSentence" :rows="2" maxlength="500" placeholder="请输入例句" />
			</FaFormItem>
			<FaFormItem prop="exampleSentenceCn" label="例句翻译">
				<el-input type="textarea" v-model="state.formData.exampleSentenceCn" :rows="2" maxlength="500" placeholder="请输入例句翻译" />
			</FaFormItem>
			<FaFormItem prop="sort" label="排序" tips="从小到大">
				<el-input-number v-model="state.formData.sort" :min="1" :max="9999" placeholder="请输入排序" />
			</FaFormItem>
		</FaForm>
	</FaDialog>
</template>

<script lang="ts" setup>
import { reactive, ref } from "vue";
import { ElMessage, type FormRules } from "element-plus";
import { withDefineType } from "@fast-china/utils";
import { wordApi } from "@/api/services/Admin/word";
import { textbookApi } from "@/api/services/Admin/textbook";
import { AddWordInput } from "@/api/services/Admin/word/models/AddWordInput";
import { EditWordInput } from "@/api/services/Admin/word/models/EditWordInput";
import type { FaDialogInstance, FaFormInstance } from "fast-element-plus";

defineOptions({
	name: "WalkmanWordEdit",
});

const emit = defineEmits(["ok"]);

const faDialogRef = ref<FaDialogInstance>();
const faFormRef = ref<FaFormInstance>();

const state = reactive({
	formData: withDefineType<EditWordInput & AddWordInput>({}),
	formRules: withDefineType<FormRules>({
		lessonId: [{ required: true, message: "请选择所属课程", trigger: "change" }],
		english: [{ required: true, message: "请输入英文", trigger: "blur" }],
		chinese: [{ required: true, message: "请输入中文", trigger: "blur" }],
	}),
	formDisabled: false,
	dialogState: withDefineType<IPageStateType>("detail"),
	dialogTitle: "单词",
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
				await wordApi.addWord(state.formData);
				ElMessage.success("新增成功！");
				break;
			case "edit":
				await wordApi.editWord(state.formData);
				ElMessage.success("保存成功！");
				break;
		}
		emit("ok");
	});
};

const detail = (wordId: number) => {
	faDialogRef.value.open(async () => {
		state.formDisabled = true;
		await loadLessonList();
		const apiRes = await wordApi.queryWordDetail(wordId);
		state.formData = apiRes;
		state.dialogTitle = `单词详情 - ${apiRes.english}`;
	});
};

const add = () => {
	faDialogRef.value.open(async () => {
		state.dialogState = "add";
		state.dialogTitle = "添加单词";
		state.formDisabled = false;
		await loadLessonList();
		state.formData = { sort: 1 };
	});
};

const edit = (wordId: number) => {
	faDialogRef.value.open(async () => {
		state.dialogState = "edit";
		state.formDisabled = false;
		await loadLessonList();
		const apiRes = await wordApi.queryWordDetail(wordId);
		state.formData = apiRes;
		state.dialogTitle = `编辑单词 - ${apiRes.english}`;
	});
};

defineExpose({ element: faDialogRef, detail, add, edit });
</script>
