<template>
	<FaDialog ref="faDialogRef" width="800" :title="state.dialogTitle" :show-confirm-button="false">
		<FaForm :model="state.formData" detail-form cols="2">
			<FaFormItem prop="code" label="激活码">
				<el-text type="primary">{{ state.formData.code }}</el-text>
			</FaFormItem>
			<FaFormItem label="已使用">
				<Text name="BooleanEnum" :value="!!state.formData.userId" />
			</FaFormItem>
			<FaFormItem prop="expireTime" label="过期时间">
				<template v-if="state.formData.expireTime">
					{{ dayjs(state.formData.expireTime).format("YYYY-MM-DD HH:mm:ss") }}
				</template>
				<template v-else>永久有效</template>
			</FaFormItem>
			<FaFormItem prop="activationTime" label="激活时间">
				<template v-if="state.formData.activationTime">
					{{ dayjs(state.formData.activationTime).format("YYYY-MM-DD HH:mm:ss") }}
				</template>
				<template v-else>-</template>
			</FaFormItem>
			<FaFormItem prop="createdTime" label="创建时间">
				{{ dayjs(state.formData.createdTime).format("YYYY-MM-DD HH:mm:ss") }}
			</FaFormItem>
			<FaFormItem prop="createdUserName" label="创建人">
				{{ state.formData.createdUserName }}
			</FaFormItem>
		</FaForm>

		<el-divider content-position="left">用户信息</el-divider>
		<el-row>
			<el-col :span="4" style="text-align: center">
				<FaImage style="height: 100px; width: 100px" :src="state.formData.avatar" />
			</el-col>

			<el-col :span="20">
				<FaForm :model="state.formData" detail-form cols="2">
					<FaFormItem prop="nickName" label="昵称">
						{{ state.formData.nickName }}
					</FaFormItem>
					<FaFormItem prop="mobile" label="手机">
						<el-text type="primary">{{ state.formData.mobile }}</el-text>
					</FaFormItem>
					<FaFormItem prop="openId" label="唯一用户标识">
						{{ state.formData.openId }}
					</FaFormItem>
				</FaForm>
			</el-col>
		</el-row>
	</FaDialog>
</template>

<script lang="ts" setup>
import { reactive, useTemplateRef } from "vue";
import { dayjs } from "element-plus";
import { withDefineType } from "@fast-china/utils";
import { activationCodeApi } from "@/api/services/Walkman/activationCode";
import type { FaDialogInstance } from "fast-element-plus";
import type { QueryActivationCodeDetailOutput } from "@/api/services/Walkman/activationCode/models/QueryActivationCodeDetailOutput";

defineOptions({ name: "WalkmanActivationCodeDetail" });

const faDialogRef = useTemplateRef<FaDialogInstance>("faDialogRef");
const state = reactive({
	formData: withDefineType<QueryActivationCodeDetailOutput>({}),
	dialogTitle: "激活码详情",
});

const open = (activationCodeId: string) => {
	void faDialogRef.value.open(async () => {
		const apiRes = await activationCodeApi.queryActivationCodeDetail(activationCodeId);
		state.formData = apiRes;
		state.dialogTitle = `激活码详情 - ${apiRes.code}`;
	});
};

defineExpose({ element: faDialogRef, open });
</script>
