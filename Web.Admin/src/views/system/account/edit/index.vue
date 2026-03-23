<template>
	<FaDialog ref="faDialogRef" width="1000" :title="state.dialogTitle" :showConfirmButton="false">
		<el-row>
			<el-col :span="4" style="text-align: center">
				<FaImage style="height: 100px; width: 100px" :src="state.formData.avatar" />
			</el-col>

			<el-col :span="20">
				<FaForm :model="state.formData" detailForm cols="2">
					<FaFormItem prop="mobile" label="手机">
						<el-text type="primary">{{ state.formData.mobile }}</el-text>
					</FaFormItem>
					<FaFormItem prop="nickName" label="昵称">
						{{ state.formData.nickName }}
					</FaFormItem>
					<FaFormItem prop="phone" label="电话">
						{{ state.formData.phone }}
					</FaFormItem>
					<FaFormItem prop="status" label="状态">
						<Text name="CommonStatusEnum" :value="state.formData.status" />
					</FaFormItem>
					<FaFormItem prop="sex" label="性别">
						<Text name="GenderEnum" :value="state.formData.sex" />
					</FaFormItem>
					<FaFormItem prop="birthday" label="生日">
						<el-text>{{ state.formData.birthday }}</el-text>
					</FaFormItem>
					<FaFormItem prop="createdTime" label="创建时间">
						{{ dayjs(state.formData.createdTime).format("YYYY-MM-DD HH:mm:ss") }}
					</FaFormItem>
					<FaFormItem prop="updatedTime" label="更新时间">
						<template v-if="state.formData.updatedTime">
							{{ dayjs(state.formData.updatedTime).format("YYYY-MM-DD HH:mm:ss") }}
						</template>
						<template v-else>-</template>
					</FaFormItem>
				</FaForm>
			</el-col>
		</el-row>

		<FaForm :model="state.formData" detailForm cols="2">
			<FaLayoutGridItem span="2">
				<el-divider contentPosition="left">初次登录信息</el-divider>
			</FaLayoutGridItem>
			<FaFormItem prop="firstLoginIp" label="Ip">
				<el-text type="success">{{ state.formData.firstLoginIp }}</el-text>
			</FaFormItem>
			<FaFormItem prop="firstLoginDevice" label="设备">
				{{ state.formData.firstLoginDevice }}
			</FaFormItem>
			<FaFormItem prop="firstLoginOS" label="操作系统">
				{{ state.formData.firstLoginOS }}
			</FaFormItem>
			<FaFormItem prop="firstLoginBrowser" label="浏览器">
				{{ state.formData.firstLoginBrowser }}
			</FaFormItem>
			<FaFormItem prop="firstLoginProvince" label="地区">
				{{ state.formData.firstLoginProvince }} - {{ state.formData.firstLoginCity }}
			</FaFormItem>
			<FaFormItem prop="firstLoginTime" label="时间">
				<template v-if="state.formData.firstLoginTime">
					{{ dayjs(state.formData.firstLoginTime).format("YYYY-MM-DD HH:mm:ss") }}
				</template>
				<template v-else>-</template>
			</FaFormItem>

			<FaLayoutGridItem span="2">
				<el-divider contentPosition="left">最后登录信息</el-divider>
			</FaLayoutGridItem>
			<FaFormItem prop="lastLoginIp" label="Ip">
				<el-text type="success">{{ state.formData.lastLoginIp }}</el-text>
			</FaFormItem>
			<FaFormItem prop="lastLoginDevice" label="设备">
				{{ state.formData.lastLoginDevice }}
			</FaFormItem>
			<FaFormItem prop="lastLoginOS" label="操作系统">
				{{ state.formData.lastLoginOS }}
			</FaFormItem>
			<FaFormItem prop="lastLoginBrowser" label="浏览器">
				{{ state.formData.lastLoginBrowser }}
			</FaFormItem>
			<FaFormItem prop="lastLoginProvince" label="地区">
				{{ state.formData.lastLoginProvince }} - {{ state.formData.lastLoginCity }}
			</FaFormItem>
			<FaFormItem prop="lastLoginTime" label="时间">
				<template v-if="state.formData.lastLoginTime">
					{{ dayjs(state.formData.lastLoginTime).format("YYYY-MM-DD HH:mm:ss") }}
				</template>
				<template v-else>-</template>
			</FaFormItem>

			<FaLayoutGridItem span="2">
				<el-divider contentPosition="left">验证信息</el-divider>
			</FaLayoutGridItem>
			<FaFormItem prop="passwordErrorTime" label="错误次数">
				<template v-if="state.formData.passwordErrorTime > 0">
					<el-text type="warning">{{ state.formData.passwordErrorTime }}次</el-text>
				</template>
				<template v-else>-</template>
			</FaFormItem>
			<FaFormItem prop="lockStartTime" label="锁定时间">
				<template v-if="state.formData.lockStartTime">
					<el-text type="warning">
						{{ dayjs(state.formData.lockStartTime).format("YYYY-MM-DD HH:mm:ss") }}
						~
						{{ dayjs(state.formData.lockEndTime).format("YYYY-MM-DD HH:mm:ss") }}
					</el-text>
				</template>
				<template v-else>-</template>
			</FaFormItem>
		</FaForm>
	</FaDialog>
</template>

<script lang="ts" setup>
import { reactive, ref } from "vue";
import { dayjs } from "element-plus";
import { withDefineType } from "@fast-china/utils";
import { accountApi } from "@/api/services/Admin/account";
import { QueryAccountDetailOutput } from "@/api/services/Admin/account/models/QueryAccountDetailOutput";
import type { FaDialogInstance } from "fast-element-plus";

defineOptions({
	name: "SystemAccountEdit",
});

const faDialogRef = ref<FaDialogInstance>();

const state = reactive({
	formData: withDefineType<QueryAccountDetailOutput>({}),
	dialogTitle: "账号",
});

const detail = (accountId: number) => {
	faDialogRef.value.open(async () => {
		const apiRes = await accountApi.queryAccountDetail(accountId);
		state.formData = apiRes;
		state.dialogTitle = `账号详情 - ${apiRes.mobile}`;
	});
};

// 暴露给父组件的参数和方法(外部需要什么，都可以从这里暴露出去)
defineExpose({
	element: faDialogRef,
	detail,
});
</script>
