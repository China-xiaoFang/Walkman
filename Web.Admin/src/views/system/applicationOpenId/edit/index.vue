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
			<FaFormItem prop="openId" label="应用标识">
				<el-input v-model="state.formData.openId" maxlength="50" placeholder="请输入应用标识" />
			</FaFormItem>
			<FaFormItem prop="appType" label="应用类型">
				<FaSelect :data="appEnvironmentEnum" v-model="state.formData.appType" />
			</FaFormItem>
			<FaFormItem prop="openSecret" label="开放平台密钥">
				<el-input v-model="state.formData.openSecret" maxlength="50" placeholder="请输入开放平台密钥" />
			</FaFormItem>
			<FaFormItem prop="environmentType" label="环境类型" span="2">
				<RadioGroup name="EnvironmentTypeEnum" v-model="state.formData.environmentType" />
			</FaFormItem>
			<FaFormItem prop="remark" label="备注">
				<el-input type="textarea" v-model="state.formData.remark" :rows="2" maxlength="200" placeholder="请输入备注" />
			</FaFormItem>

			<FaLayoutGridItem span="2">
				<el-divider contentPosition="left">支付相关</el-divider>
			</FaLayoutGridItem>
			<FaFormItem prop="weChatMerchantId" label="微信商户号">
				<MerchantSelect
					:merchantType="PaymentChannelEnum.WeChat"
					v-model="state.formData.weChatMerchantId"
					v-model:merchantNo="state.formData.weChatMerchantNo"
					maxlength="20"
					placeholder="请选择微信商户号"
				/>
			</FaFormItem>
			<FaFormItem prop="alipayMerchantId" label="支付宝商户号">
				<MerchantSelect
					:merchantType="PaymentChannelEnum.Alipay"
					v-model="state.formData.alipayMerchantId"
					v-model:merchantNo="state.formData.alipayMerchantNo"
					maxlength="20"
					placeholder="请选择支付宝商户号"
				/>
			</FaFormItem>
		</FaForm>
	</FaDialog>
</template>

<script lang="ts" setup>
import { reactive, ref } from "vue";
import { ElMessage, type FormRules } from "element-plus";
import { withDefineType } from "@fast-china/utils";
import { AppEnvironmentEnum } from "@/api/enums/AppEnvironmentEnum";
import { EnvironmentTypeEnum } from "@/api/enums/EnvironmentTypeEnum";
import { PaymentChannelEnum } from "@/api/enums/PaymentChannelEnum";
import { applicationOpenIdApi } from "@/api/services/Admin/applicationOpenId";
import { AddApplicationOpenIdInput } from "@/api/services/Admin/applicationOpenId/models/AddApplicationOpenIdInput";
import { EditApplicationOpenIdInput } from "@/api/services/Admin/applicationOpenId/models/EditApplicationOpenIdInput";
import { useApp } from "@/stores";
import type { FaDialogInstance, FaFormInstance } from "fast-element-plus";

defineOptions({
	name: "SystemApplicationOpenIdEdit",
});

const emit = defineEmits(["ok"]);

const appStore = useApp();
const appEnvironmentEnum = appStore.getDictionary("AppEnvironmentEnum");

const faDialogRef = ref<FaDialogInstance>();
const faFormRef = ref<FaFormInstance>();

const state = reactive({
	formData: withDefineType<EditApplicationOpenIdInput & AddApplicationOpenIdInput & { appName?: string }>({}),
	formRules: withDefineType<FormRules>({
		openId: [{ required: true, message: "请输入应用标识", trigger: "blur" }],
		requestTimeout: [{ required: true, message: "请输入请求超时时间", trigger: "blur" }],
	}),
	formDisabled: false,
	dialogState: withDefineType<IPageStateType>("detail"),
	dialogTitle: "应用OpenId",
});

const handleConfirm = () => {
	faDialogRef.value.close(async () => {
		await faFormRef.value.validateScrollToField();
		switch (state.dialogState) {
			case "add":
				await applicationOpenIdApi.addApplicationOpenId(state.formData);
				ElMessage.success("新增成功！");
				break;
			case "edit":
				await applicationOpenIdApi.editApplicationOpenId(state.formData);
				ElMessage.success("保存成功！");
				break;
		}
		emit("ok");
	});
};

const detail = (recordId: number) => {
	faDialogRef.value.open(async () => {
		state.formDisabled = true;
		const apiRes = await applicationOpenIdApi.queryApplicationOpenIdDetail(recordId);
		state.formData = apiRes;
		state.dialogTitle = `应用OpenId详情 - ${apiRes.appName}`;
	});
};

const add = () => {
	faDialogRef.value.open(() => {
		state.dialogState = "add";
		state.dialogTitle = "添加应用OpenId";
		state.formDisabled = false;
		state.formData = {
			appType: AppEnvironmentEnum.Web,
			environmentType: EnvironmentTypeEnum.Production,
		};
	});
};

const edit = (recordId: number) => {
	faDialogRef.value.open(async () => {
		state.dialogState = "edit";
		state.formDisabled = false;
		const apiRes = await applicationOpenIdApi.queryApplicationOpenIdDetail(recordId);
		state.formData = apiRes;
		state.dialogTitle = `编辑应用OpenId - ${apiRes.openId}`;
	});
};

// 暴露给父组件的参数和方法(外部需要什么，都可以从这里暴露出去)
defineExpose({
	element: faDialogRef,
	detail,
	add,
	edit,
});
</script>
