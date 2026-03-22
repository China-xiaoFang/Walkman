<template>
	<FaDialog
		ref="faDialogRef"
		width="500"
		title="批量生成激活码"
		confirmButtonText="生成"
		@confirm-click="handleConfirm"
		@close="faFormRef.resetFields()"
	>
		<FaForm ref="faFormRef" :model="state.formData" :rules="state.formRules" cols="1">
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
			<FaFormItem prop="count" label="生成数量">
				<el-input-number v-model="state.formData.count" :min="1" :max="1000" placeholder="请输入生成数量" />
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
import { activationCodeApi } from "@/api/services/Admin/activationCode";
import { GenerateActivationCodeInput } from "@/api/services/Admin/activationCode/models/GenerateActivationCodeInput";
import { textbookApi } from "@/api/services/Admin/textbook";
import type { FaDialogInstance, FaFormInstance } from "fast-element-plus";

defineOptions({
	name: "WalkmanActivationCodeGenerate",
});

const emit = defineEmits(["ok"]);

const faDialogRef = ref<FaDialogInstance>();
const faFormRef = ref<FaFormInstance>();

const state = reactive({
	formData: withDefineType<GenerateActivationCodeInput>({}),
	formRules: withDefineType<FormRules>({
		textbookId: [{ required: true, message: "请选择所属教材", trigger: "change" }],
		count: [{ required: true, message: "请输入生成数量", trigger: "blur" }],
	}),
	textbookList: withDefineType<Array<{ textbookId: number; textbookName: string }>>([]),
});

const handleConfirm = () => {
	faDialogRef.value.close(async () => {
		await faFormRef.value.validateScrollToField();
		await activationCodeApi.generateActivationCode(state.formData);
		ElMessage.success("生成成功！");
		emit("ok");
	});
};

const open = () => {
	faDialogRef.value.open(async () => {
		const res = await textbookApi.queryTextbookPaged({ pageIndex: 1, pageSize: 999 });
		state.textbookList = (res.rows || []).map((item) => ({
			textbookId: item.textbookId,
			textbookName: item.textbookName,
		}));
		state.formData = { count: 10 };
	});
};

defineExpose({ element: faDialogRef, open });
</script>
