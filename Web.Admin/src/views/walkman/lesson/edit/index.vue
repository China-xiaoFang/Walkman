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
			<FaFormItem prop="volumeId" label="所属册">
				<el-select v-model="state.formData.volumeId" placeholder="请选择册" filterable>
					<el-option v-for="item in state.volumeList" :key="item.volumeId" :label="item.volumeName" :value="item.volumeId" />
				</el-select>
			</FaFormItem>
			<FaFormItem prop="lessonName" label="课程名称">
				<el-input v-model="state.formData.lessonName" maxlength="100" placeholder="请输入课程名称" />
			</FaFormItem>
			<FaFormItem prop="lessonNo" label="课程编号">
				<el-input-number v-model="state.formData.lessonNo" :min="1" :max="9999" placeholder="请输入课程编号" />
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
import { AddLessonInput } from "@/api/services/Admin/textbook/models/AddLessonInput";
import { EditLessonInput } from "@/api/services/Admin/textbook/models/EditLessonInput";
import type { FaDialogInstance, FaFormInstance } from "fast-element-plus";

defineOptions({
	name: "WalkmanLessonEdit",
});

const emit = defineEmits(["ok"]);

const faDialogRef = ref<FaDialogInstance>();
const faFormRef = ref<FaFormInstance>();

const state = reactive({
	formData: withDefineType<EditLessonInput & AddLessonInput>({}),
	formRules: withDefineType<FormRules>({
		volumeId: [{ required: true, message: "请选择所属册", trigger: "change" }],
		lessonName: [{ required: true, message: "请输入课程名称", trigger: "blur" }],
		lessonNo: [{ required: true, message: "请输入课程编号", trigger: "blur" }],
		sort: [{ required: true, message: "请输入排序", trigger: "blur" }],
	}),
	formDisabled: false,
	dialogState: withDefineType<IPageStateType>("detail"),
	dialogTitle: "课程",
	volumeList: withDefineType<Array<{ volumeId: number; volumeName: string }>>([]),
});

const loadVolumeList = async () => {
	const res = await textbookApi.queryVolumePaged({ pageIndex: 1, pageSize: 999 });
	state.volumeList = (res.rows || []).map((item) => ({
		volumeId: item.volumeId,
		volumeName: item.volumeName,
	}));
};

const handleConfirm = () => {
	faDialogRef.value.close(async () => {
		await faFormRef.value.validateScrollToField();
		switch (state.dialogState) {
			case "add":
				await textbookApi.addLesson(state.formData);
				ElMessage.success("新增成功！");
				break;
			case "edit":
				await textbookApi.editLesson(state.formData);
				ElMessage.success("保存成功！");
				break;
		}
		emit("ok");
	});
};

const detail = (lessonId: number) => {
	faDialogRef.value.open(async () => {
		state.formDisabled = true;
		await loadVolumeList();
		const apiRes = await textbookApi.queryLessonDetail(lessonId);
		state.formData = apiRes;
		state.dialogTitle = `课程详情 - ${apiRes.lessonName}`;
	});
};

const add = () => {
	faDialogRef.value.open(async () => {
		state.dialogState = "add";
		state.dialogTitle = "添加课程";
		state.formDisabled = false;
		await loadVolumeList();
		state.formData = {
			lessonNo: 1,
			sort: 1,
		};
	});
};

const edit = (lessonId: number) => {
	faDialogRef.value.open(async () => {
		state.dialogState = "edit";
		state.formDisabled = false;
		await loadVolumeList();
		const apiRes = await textbookApi.queryLessonDetail(lessonId);
		state.formData = apiRes;
		state.dialogTitle = `编辑课程 - ${apiRes.lessonName}`;
	});
};

defineExpose({
	element: faDialogRef,
	detail,
	add,
	edit,
});
</script>
