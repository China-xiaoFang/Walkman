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
			<FaFormItem prop="textbookId" label="所属教材">
				<el-select v-model="state.formData.textbookId" placeholder="请选择教材" filterable>
					<el-option
						v-for="item in state.textbookList"
						:key="item.textbookId"
						:label="item.textbookName"
						:value="item.textbookId"
					/>
				</el-select>
			</FaFormItem>
			<FaFormItem prop="volumeName" label="册名称">
				<el-input v-model="state.formData.volumeName" maxlength="50" placeholder="请输入册名称" />
			</FaFormItem>
			<FaFormItem prop="coverUrl" label="封面地址">
				<el-input v-model="state.formData.coverUrl" maxlength="500" placeholder="请输入封面图片URL" />
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
import { AddVolumeInput } from "@/api/services/Admin/textbook/models/AddVolumeInput";
import { EditVolumeInput } from "@/api/services/Admin/textbook/models/EditVolumeInput";
import type { FaDialogInstance, FaFormInstance } from "fast-element-plus";

defineOptions({
	name: "WalkmanVolumeEdit",
});

const emit = defineEmits(["ok"]);

const faDialogRef = ref<FaDialogInstance>();
const faFormRef = ref<FaFormInstance>();

const state = reactive({
	formData: withDefineType<EditVolumeInput & AddVolumeInput>({}),
	formRules: withDefineType<FormRules>({
		textbookId: [{ required: true, message: "请选择所属教材", trigger: "change" }],
		volumeName: [{ required: true, message: "请输入册名称", trigger: "blur" }],
		sort: [{ required: true, message: "请输入排序", trigger: "blur" }],
	}),
	formDisabled: false,
	dialogState: withDefineType<IPageStateType>("detail"),
	dialogTitle: "册",
	textbookList: withDefineType<Array<{ textbookId: number; textbookName: string }>>([]),
});

const loadTextbookList = async () => {
	const res = await textbookApi.queryTextbookPaged({ pageIndex: 1, pageSize: 999 });
	state.textbookList = (res.rows || []).map((item) => ({
		textbookId: item.textbookId,
		textbookName: item.textbookName,
	}));
};

const handleConfirm = () => {
	faDialogRef.value.close(async () => {
		await faFormRef.value.validateScrollToField();
		switch (state.dialogState) {
			case "add":
				await textbookApi.addVolume(state.formData);
				ElMessage.success("新增成功！");
				break;
			case "edit":
				await textbookApi.editVolume(state.formData);
				ElMessage.success("保存成功！");
				break;
		}
		emit("ok");
	});
};

const detail = (volumeId: number) => {
	faDialogRef.value.open(async () => {
		state.formDisabled = true;
		await loadTextbookList();
		const apiRes = await textbookApi.queryVolumeDetail(volumeId);
		state.formData = apiRes;
		state.dialogTitle = `册详情 - ${apiRes.volumeName}`;
	});
};

const add = () => {
	faDialogRef.value.open(async () => {
		state.dialogState = "add";
		state.dialogTitle = "添加册";
		state.formDisabled = false;
		await loadTextbookList();
		state.formData = {
			sort: 1,
		};
	});
};

const edit = (volumeId: number) => {
	faDialogRef.value.open(async () => {
		state.dialogState = "edit";
		state.formDisabled = false;
		await loadTextbookList();
		const apiRes = await textbookApi.queryVolumeDetail(volumeId);
		state.formData = apiRes;
		state.dialogTitle = `编辑册 - ${apiRes.volumeName}`;
	});
};

defineExpose({
	element: faDialogRef,
	detail,
	add,
	edit,
});
</script>
