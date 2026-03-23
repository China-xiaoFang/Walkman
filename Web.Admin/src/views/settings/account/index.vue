<template>
	<div class="el-card" v-loading="state.loading" element-loading-text="加载中...">
		<el-scrollbar>
			<el-divider contentPosition="left">账号信息</el-divider>
			<FaForm ref="accountFaFormRef" :model="state.accountFormData" :rules="state.formRules" cols="3">
				<FaFormItem prop="nickName" label="昵称">
					<el-input v-model="state.accountFormData.nickName" maxlength="20" placeholder="请输入昵称" />
				</FaFormItem>
				<FaFormItem prop="mobile" label="手机">
					<el-input v-model="state.accountFormData.mobile" maxlength="11" placeholder="请输入手机" />
				</FaFormItem>
				<FaFormItem prop="phone" label="电话">
					<el-input v-model="state.accountFormData.phone" maxlength="11" placeholder="请输入电话" />
				</FaFormItem>
				<FaFormItem prop="sex" label="性别">
					<RadioGroup name="GenderEnum" v-model="state.accountFormData.sex" />
				</FaFormItem>
				<FaFormItem prop="birthday" label="生日">
					<el-date-picker
						type="date"
						v-model="state.accountFormData.birthday"
						:disabledDate="dateUtil.getDisabledDate"
						valueFormat="YYYY-MM-DD"
						placeholder="请选择生日"
					/>
				</FaFormItem>
				<FaFormItem prop="lastLoginIp" label="Ip">
					<el-text type="success">{{ state.accountFormData.lastLoginIp }}</el-text>
				</FaFormItem>
				<FaFormItem prop="lastLoginTime" label="时间">
					<template v-if="state.accountFormData.lastLoginTime">
						{{ dayjs(state.accountFormData.lastLoginTime).format("YYYY-MM-DD HH:mm:ss") }}
					</template>
					<template v-else>-</template>
				</FaFormItem>
				<FaFormItem prop="avatar" label="头像">
					<FaUploadImage v-model="state.accountFormData.avatar" :uploadApi="fileApi.uploadAvatar" />
				</FaFormItem>
			</FaForm>

			<template v-if="!userInfoStore.isSuperAdmin && !userInfoStore.isAdmin">
				<el-divider contentPosition="left">职员信息</el-divider>
				<FaForm ref="employeeFaFormRef" :model="state.employeeFormData" :rules="state.formRules" cols="3">
					<FaFormItem prop="employeeName" label="职员名称">
						<el-input v-model="state.employeeFormData.employeeName" maxlength="20" placeholder="请输入职员名称" />
					</FaFormItem>
					<FaFormItem prop="mobile" label="手机">
						<el-input v-model="state.employeeFormData.mobile" maxlength="11" placeholder="请输入手机" />
					</FaFormItem>
					<FaFormItem prop="email" label="邮箱">
						<el-input v-model="state.employeeFormData.email" maxlength="50" placeholder="请输入邮箱" />
					</FaFormItem>
					<FaFormItem prop="sex" label="性别">
						<RadioGroup name="GenderEnum" v-model="state.employeeFormData.sex" />
					</FaFormItem>
					<FaFormItem prop="birthday" label="生日">
						<el-date-picker
							type="date"
							v-model="state.employeeFormData.birthday"
							:disabledDate="dateUtil.getDisabledDate"
							valueFormat="YYYY-MM-DD"
							placeholder="请选择生日"
						/>
					</FaFormItem>
					<FaFormItem prop="idType" label="证件类型">
						<FaSelect
							:data="appStore.getDictionary('IdTypeEnum')"
							v-model="state.employeeFormData.idType"
							placeholder="请选择证件类型"
							clearable
						/>
					</FaFormItem>
					<FaFormItem prop="idNumber" label="证件号码">
						<el-input v-model="state.employeeFormData.idNumber" maxlength="50" placeholder="请输入证件号码" />
					</FaFormItem>
					<FaFormItem prop="idPhoto" label="证件照">
						<FaUploadImage v-model="state.employeeFormData.idPhoto" :uploadApi="fileApi.uploadIdPhoto" />
					</FaFormItem>

					<FaLayoutGridItem span="3">
						<el-divider contentPosition="left">人事档案</el-divider>
					</FaLayoutGridItem>

					<FaFormItem prop="nation" label="民族">
						<FaSelect
							:data="appStore.getDictionary('NationEnum')"
							v-model="state.employeeFormData.nation"
							placeholder="请选择民族"
							clearable
						/>
					</FaFormItem>
					<FaFormItem prop="nativePlace" label="籍贯">
						<el-input v-model="state.employeeFormData.nativePlace" maxlength="50" placeholder="请输入籍贯" />
					</FaFormItem>
					<FaFormItem prop="familyPhone" label="家庭电话">
						<el-input v-model="state.employeeFormData.familyPhone" maxlength="20" placeholder="请输入家庭电话" />
					</FaFormItem>
					<FaFormItem prop="officePhone" label="办公电话">
						<el-input v-model="state.employeeFormData.officePhone" maxlength="20" placeholder="请输入办公电话" />
					</FaFormItem>
					<FaFormItem prop="familyAddress" label="家庭地址">
						<el-input
							type="textarea"
							v-model="state.employeeFormData.familyAddress"
							:rows="2"
							maxlength="200"
							placeholder="请输入家庭地址"
						/>
					</FaFormItem>
					<FaFormItem prop="mailingAddress" label="通信地址">
						<el-input
							type="textarea"
							v-model="state.employeeFormData.mailingAddress"
							:rows="2"
							maxlength="200"
							placeholder="请输入通信地址"
						/>
					</FaFormItem>
					<FaFormItem prop="emergencyContact" label="紧急联系人">
						<el-input v-model="state.employeeFormData.emergencyContact" maxlength="20" placeholder="请输入紧急联系人" />
					</FaFormItem>
					<FaFormItem prop="emergencyPhone" label="紧急联系电话">
						<el-input v-model="state.employeeFormData.emergencyPhone" maxlength="20" placeholder="请输入紧急联系电话" />
					</FaFormItem>
					<FaFormItem prop="emergencyAddress" label="紧急联系地址">
						<el-input
							type="textarea"
							v-model="state.employeeFormData.emergencyAddress"
							:rows="2"
							maxlength="200"
							placeholder="请输入紧急联系地址"
						/>
					</FaFormItem>

					<FaLayoutGridItem span="3">
						<el-divider contentPosition="left">机构信息</el-divider>
					</FaLayoutGridItem>

					<FaLayoutGridItem span="3" style="min-height: 300px; max-height: 500px">
						<FaTable :data="state.employeeFormData.orgList" :pagination="false" :headerCard="false">
							<FaTableColumn prop="orgName" label="机构" width="280" />
							<FaTableColumn prop="departmentName" label="部门" width="280" />
							<FaTableColumn prop="isPrimary" label="主部门" width="80" tag :enum="appStore.getDictionary('BooleanEnum')" />
							<FaTableColumn prop="positionName" label="职位" width="280" />
							<FaTableColumn prop="jobLevelName" label="职级" width="280" />
							<FaTableColumn prop="isPrincipal" label="负责人" width="80" tag :enum="appStore.getDictionary('BooleanEnum')" />
						</FaTable>
					</FaLayoutGridItem>
				</FaForm>
			</template>
		</el-scrollbar>
		<div style="margin-top: 20px; padding: 20px; display: flex; align-items: center; justify-content: center; border-top: var(--el-border)">
			<el-button type="primary" @click="changePasswordRef.open()">修改密码</el-button>
			<FaButton type="primary" @click="handleConfirm">保存</FaButton>
		</div>
	</div>
</template>

<script lang="ts" setup>
import { inject, onMounted, reactive, ref } from "vue";
import { ElMessage, type FormRules, dayjs } from "element-plus";
import { dateUtil, withDefineType } from "@fast-china/utils";
import { accountApi } from "@/api/services/Admin/account";
import { EditAccountInput } from "@/api/services/Admin/account/models/EditAccountInput";
import { QueryAccountDetailOutput } from "@/api/services/Admin/account/models/QueryAccountDetailOutput";
import { employeeApi } from "@/api/services/Admin/employee";
import { EditEmployeeInput } from "@/api/services/Admin/employee/models/EditEmployeeInput";
import { fileApi } from "@/api/services/File";
import { changePasswordKey } from "@/layouts";
import { useApp, useUserInfo } from "@/stores";
import type { FaFormInstance } from "fast-element-plus";

defineOptions({
	name: "SettingsAccount",
});

const appStore = useApp();
const userInfoStore = useUserInfo();

const accountFaFormRef = ref<FaFormInstance>();
const employeeFaFormRef = ref<FaFormInstance>();
const changePasswordRef = inject(changePasswordKey);

const state = reactive({
	loading: false,
	accountFormData: withDefineType<EditAccountInput & QueryAccountDetailOutput>({}),
	employeeFormData: withDefineType<EditEmployeeInput>({}),
	formRules: withDefineType<FormRules>({
		orgId: [{ required: true, message: "请选择机构", trigger: "change" }],
		departmentId: [{ required: true, message: "请选择部门", trigger: "change" }],
		positionId: [{ required: true, message: "请选择职位", trigger: "change" }],
		jobLevelId: [{ required: true, message: "请选择职级", trigger: "change" }],
		employeeName: [{ required: true, message: "请输入职员名称", trigger: "blur" }],
		mobile: [{ required: true, message: "请输入手机", trigger: "blur" }],
		email: [{ required: true, message: "请输入邮箱", trigger: "blur" }],
		idPhoto: [{ required: true, message: "请上传证件照", trigger: "change" }],
		firstWorkDate: [{ required: true, message: "请选择初次工作日期", trigger: "change" }],
		entryDate: [{ required: true, message: "请选择入职日期", trigger: "change" }],
	}),
});

const handleConfirm = async (event: MouseEvent, done: () => void) => {
	state.loading = true;
	try {
		await accountFaFormRef.value.validateScrollToField();
		await accountApi.editAccount(state.accountFormData);
		if (!userInfoStore.isSuperAdmin && !userInfoStore.isAdmin) {
			await employeeFaFormRef.value.validateScrollToField();
			await employeeApi.editSelfEmployee(state.employeeFormData);
		}
		ElMessage.success("保存成功！");
		window.location.reload();
	} finally {
		state.loading = false;
		done();
	}
};

onMounted(async () => {
	state.loading = true;
	try {
		state.accountFormData = await accountApi.queryEditAccountDetail();
		if (!userInfoStore.isSuperAdmin && !userInfoStore.isAdmin) {
			state.employeeFormData = await employeeApi.queryEmployeeDetail(userInfoStore.employeeId);
		}
	} finally {
		state.loading = false;
	}
});
</script>
