<template>
	<FaDialog
		ref="faDialogRef"
		title="修改密码"
		:showFullscreen="false"
		showBeforeClose
		width="450"
		@confirm-click="handleConfirm"
		@close="faFormRef.resetFields()"
	>
		<FaForm ref="faFormRef" :model="state.formData" :rules="state.formRules">
			<FaFormItem label="旧密码" prop="oldPassword">
				<el-input type="password" v-model.trim="state.formData.oldPassword" placeholder="请输入旧密码" />
			</FaFormItem>
			<FaFormItem label="新密码" prop="newPassword">
				<el-input type="password" v-model.trim="state.formData.newPassword" placeholder="请输入新密码" autocomplete="off" />
			</FaFormItem>
			<FaFormItem label="确认密码" prop="confirmPassword">
				<el-input v-model.trim="state.formData.confirmPassword" placeholder="请输入确认新密码" type="password" />
			</FaFormItem>
		</FaForm>
	</FaDialog>
</template>
<script lang="ts" setup>
import { reactive, ref } from "vue";
import { ElMessage, ElMessageBox, type FormRules } from "element-plus";
import { FaDialogInstance, FaFormInstance } from "fast-element-plus";
import { cryptoUtil, withDefineType } from "@fast-china/utils";
import { accountApi } from "@/api/services/Admin/account";
import { ChangePasswordInput } from "@/api/services/Admin/account/models/ChangePasswordInput";
import { useUserInfo } from "@/stores";

defineOptions({
	name: "ChangePassword",
});

const userInfoStore = useUserInfo();

const faDialogRef = ref<FaDialogInstance>();
const faFormRef = ref<FaFormInstance>();

const state = reactive({
	formData: withDefineType<ChangePasswordInput>({}),
	formRules: withDefineType<FormRules>({
		oldPassword: [{ required: true, message: "请输入旧密码", trigger: "blur" }],
		newPassword: [{ required: true, message: "请输入新密码", trigger: "blur" }],
		confirmPassword: [{ required: true, message: "请输入确认新密码", trigger: "blur" }],
	}),
	formDisabled: false,
	dialogState: withDefineType<IPageStateType>("detail"),
	dialogTitle: "配置",
});

const handleConfirm = async () => {
	await faFormRef.value.validateScrollToField();
	let { newPassword, confirmPassword } = state.formData;
	newPassword = cryptoUtil.sha1.encrypt(newPassword);
	confirmPassword = cryptoUtil.sha1.encrypt(confirmPassword);
	if (newPassword !== confirmPassword) {
		ElMessage.warning("两次密码输入不一致");
		return;
	}
	faDialogRef.value.close(async () => {
		await ElMessageBox.confirm("确认修改密码", {
			type: "warning",
			async beforeClose() {
				await accountApi.changePassword({
					...state.formData,
					oldPassword: cryptoUtil.sha1.encrypt(state.formData.oldPassword),
					newPassword,
					confirmPassword,
				});
				ElMessageBox.alert("修改成功，请重新登录！", {
					type: "success",
					confirmButtonText: "重新登录",
					callback: () => {
						userInfoStore.logout();
					},
				});
			},
		});
	});
};

const open = () => {
	faDialogRef.value.open(async () => {
		const apiRes = await accountApi.queryEditAccountDetail();
		state.formData = {
			rowVersion: apiRes.rowVersion,
		};
	});
};

// 暴露给父组件的参数和方法(外部需要什么，都可以从这里暴露出去)
defineExpose({
	open,
});
</script>
