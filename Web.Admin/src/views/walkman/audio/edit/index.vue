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
			<FaFormItem prop="audioTypeId" label="音频类型">
				<el-select v-model="state.formData.audioTypeId" placeholder="请选择音频类型" filterable>
					<el-option v-for="item in state.audioTypeList" :key="item.value" :label="item.label" :value="item.value" />
				</el-select>
			</FaFormItem>
			<FaFormItem prop="pronunciationTypeId" label="发音类型">
				<el-select v-model="state.formData.pronunciationTypeId" placeholder="请选择发音类型" filterable>
					<el-option v-for="item in state.pronunciationTypeList" :key="item.value" :label="item.label" :value="item.value" />
				</el-select>
			</FaFormItem>
			<FaFormItem prop="audioUrl" label="音频地址">
				<el-input v-model="state.formData.audioUrl" maxlength="500" placeholder="请输入音频CDN地址" />
			</FaFormItem>
			<FaFormItem prop="duration" label="时长（秒）">
				<el-input-number v-model="state.formData.duration" :min="0" :max="99999" placeholder="请输入时长" />
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
import { audioApi } from "@/api/services/Admin/audio";
import { textbookApi } from "@/api/services/Admin/textbook";
import { AddAudioInput } from "@/api/services/Admin/audio/models/AddAudioInput";
import { EditAudioInput } from "@/api/services/Admin/audio/models/EditAudioInput";
import type { ElSelectorOutput, FaDialogInstance, FaFormInstance } from "fast-element-plus";

defineOptions({
	name: "WalkmanAudioEdit",
});

const emit = defineEmits(["ok"]);

const faDialogRef = ref<FaDialogInstance>();
const faFormRef = ref<FaFormInstance>();

const state = reactive({
	formData: withDefineType<EditAudioInput & AddAudioInput>({}),
	formRules: withDefineType<FormRules>({
		lessonId: [{ required: true, message: "请选择所属课程", trigger: "change" }],
		audioTypeId: [{ required: true, message: "请选择音频类型", trigger: "change" }],
		pronunciationTypeId: [{ required: true, message: "请选择发音类型", trigger: "change" }],
		audioUrl: [{ required: true, message: "请输入音频地址", trigger: "blur" }],
	}),
	formDisabled: false,
	dialogState: withDefineType<IPageStateType>("detail"),
	dialogTitle: "音频",
	lessonList: withDefineType<Array<{ lessonId: number; lessonName: string }>>([]),
	audioTypeList: withDefineType<ElSelectorOutput<number>[]>([]),
	pronunciationTypeList: withDefineType<ElSelectorOutput<number>[]>([]),
});

const loadSelectors = async () => {
	const [lessonRes, audioTypes, pronunciationTypes] = await Promise.all([
		textbookApi.queryLessonPaged({ pageIndex: 1, pageSize: 999 }),
		audioApi.audioTypeSelector(),
		audioApi.pronunciationTypeSelector(),
	]);
	state.lessonList = (lessonRes.rows || []).map((item) => ({
		lessonId: item.lessonId,
		lessonName: item.lessonName,
	}));
	state.audioTypeList = audioTypes;
	state.pronunciationTypeList = pronunciationTypes;
};

const handleConfirm = () => {
	faDialogRef.value.close(async () => {
		await faFormRef.value.validateScrollToField();
		switch (state.dialogState) {
			case "add":
				await audioApi.addAudio(state.formData);
				ElMessage.success("新增成功！");
				break;
			case "edit":
				await audioApi.editAudio(state.formData);
				ElMessage.success("保存成功！");
				break;
		}
		emit("ok");
	});
};

const detail = (audioId: number) => {
	faDialogRef.value.open(async () => {
		state.formDisabled = true;
		await loadSelectors();
		const apiRes = await audioApi.queryAudioDetail(audioId);
		state.formData = apiRes;
		state.dialogTitle = `音频详情`;
	});
};

const add = () => {
	faDialogRef.value.open(async () => {
		state.dialogState = "add";
		state.dialogTitle = "添加音频";
		state.formDisabled = false;
		await loadSelectors();
		state.formData = { duration: 0, sort: 1 };
	});
};

const edit = (audioId: number) => {
	faDialogRef.value.open(async () => {
		state.dialogState = "edit";
		state.formDisabled = false;
		await loadSelectors();
		const apiRes = await audioApi.queryAudioDetail(audioId);
		state.formData = apiRes;
		state.dialogTitle = `编辑音频`;
	});
};

defineExpose({ element: faDialogRef, detail, add, edit });
</script>
