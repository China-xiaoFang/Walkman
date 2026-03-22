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
		<FaForm ref="faFormRef" :model="state.formData" :rules="state.formRules" :disabled="state.formDisabled" cols="1">
			<FaFormItem prop="pronunciationTypeName" label="类型名称">
				<el-input v-model="state.formData.pronunciationTypeName" maxlength="50" placeholder="请输入发音类型名称" />
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
import { AddPronunciationTypeInput } from "@/api/services/Admin/audio/models/AddPronunciationTypeInput";
import { EditPronunciationTypeInput } from "@/api/services/Admin/audio/models/EditPronunciationTypeInput";
import type { FaDialogInstance, FaFormInstance } from "fast-element-plus";

defineOptions({
	name: "WalkmanPronunciationTypeEdit",
});

const emit = defineEmits(["ok"]);

const faDialogRef = ref<FaDialogInstance>();
const faFormRef = ref<FaFormInstance>();

const state = reactive({
	formData: withDefineType<EditPronunciationTypeInput & AddPronunciationTypeInput>({}),
	formRules: withDefineType<FormRules>({
		pronunciationTypeName: [{ required: true, message: "请输入发音类型名称", trigger: "blur" }],
		sort: [{ required: true, message: "请输入排序", trigger: "blur" }],
	}),
	formDisabled: false,
	dialogState: withDefineType<IPageStateType>("detail"),
	dialogTitle: "发音类型",
});

const handleConfirm = () => {
	faDialogRef.value.close(async () => {
		await faFormRef.value.validateScrollToField();
		switch (state.dialogState) {
			case "add":
				await audioApi.addPronunciationType(state.formData);
				ElMessage.success("新增成功！");
				break;
			case "edit":
				await audioApi.editPronunciationType(state.formData);
				ElMessage.success("保存成功！");
				break;
		}
		emit("ok");
	});
};

const detail = (pronunciationTypeId: number) => {
	faDialogRef.value.open(async () => {
		state.formDisabled = true;
		const apiRes = await audioApi.queryPronunciationTypeDetail(pronunciationTypeId);
		state.formData = apiRes;
		state.dialogTitle = `发音类型详情 - ${apiRes.pronunciationTypeName}`;
	});
};

const add = () => {
	faDialogRef.value.open(async () => {
		state.dialogState = "add";
		state.dialogTitle = "添加发音类型";
		state.formDisabled = false;
		state.formData = { sort: 1 };
	});
};

const edit = (pronunciationTypeId: number) => {
	faDialogRef.value.open(async () => {
		state.dialogState = "edit";
		state.formDisabled = false;
		const apiRes = await audioApi.queryPronunciationTypeDetail(pronunciationTypeId);
		state.formData = apiRes;
		state.dialogTitle = `编辑发音类型 - ${apiRes.pronunciationTypeName}`;
	});
};

defineExpose({ element: faDialogRef, detail, add, edit });
</script>
