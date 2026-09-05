<template>
	<div class="el-card" v-loading="state.loading" element-loading-text="加载中...">
		<el-scrollbar>
			<FaForm ref="accountFaFormRef" :model="state.accountFormData" :rules="state.formRules" cols="3">
				<FaLayoutGridItem span="3">
					<el-divider content-position="left">手机号</el-divider>
				</FaLayoutGridItem>
				<FaFormItem prop="mobile" label="手机" row style="max-width: 450px">
					<template #label>
						<FaFormItemTip label="手机号" tips="没买短信服务，默认验证码为 123456" />
					</template>
					<el-input
						v-model.trim="state.accountFormData.mobile"
						maxlength="11"
						placeholder="请输入手机"
						autocapitalize="off"
						autocomplete="tel"
						inputmode="tel"
						spellcheck="false"
						disabled
					>
						<template #append>
							<FaButton
								ref="mobileButtonRef"
								:disabled="!mobileChanged || mobileCountdown > 0"
								@click="(_, done) => handleSendVerificationCode('mobile', done)"
							>
								{{ mobileCountdown > 0 ? mobileCountdown + " 秒后重发" : "发送验证码" }}
							</FaButton>
						</template>
					</el-input>
				</FaFormItem>
				<ImageCaptcha
					ref="mobileCaptchaRef"
					style="max-width: 450px"
					prop="mobileCaptchaCode"
					v-model="state.accountFormData.mobileCaptchaCode"
					v-model:captcha-key="state.accountFormData.mobileCaptchaKey"
					is-force
					disabled
					@keyup.enter="mobileButtonRef.doLoading(() => handleSendVerificationCode('mobile'))"
				/>
				<FaFormItem v-if="mobileChanged" prop="mobileVerificationCode" label="验证码" row style="max-width: 450px">
					<el-input
						v-model.trim="state.accountFormData.mobileVerificationCode"
						maxlength="6"
						placeholder="请输入 6 位短信验证码"
						:show-word-limit="false"
						autocomplete="one-time-code"
						inputmode="numeric"
					/>
				</FaFormItem>

				<FaLayoutGridItem span="3">
					<el-divider content-position="left">邮箱</el-divider>
				</FaLayoutGridItem>
				<FaFormItem prop="email" label="邮箱" row style="max-width: 450px">
					<el-input v-model.trim="state.accountFormData.email" maxlength="50" placeholder="请输入邮箱">
						<template #append>
							<FaButton
								ref="emailButtonRef"
								:disabled="!emailChanged || emailCountdown > 0"
								@click="(_, done) => handleSendVerificationCode('email', done)"
							>
								{{ emailCountdown > 0 ? emailCountdown + " 秒后重发" : "发送验证码" }}
							</FaButton>
						</template>
					</el-input>
				</FaFormItem>
				<ImageCaptcha
					ref="emailCaptchaRef"
					style="max-width: 450px"
					prop="emailCaptchaCode"
					v-model="state.accountFormData.emailCaptchaCode"
					v-model:captcha-key="state.accountFormData.emailCaptchaKey"
					is-force
					@keyup.enter="emailButtonRef.doLoading(() => handleSendVerificationCode('email'))"
				/>
				<FaFormItem v-if="emailChanged" prop="emailVerificationCode" label="验证码" row style="max-width: 450px">
					<el-input
						v-model.trim="state.accountFormData.emailVerificationCode"
						maxlength="6"
						placeholder="请输入 6 位邮箱验证码"
						:show-word-limit="false"
						autocomplete="one-time-code"
						inputmode="numeric"
					/>
				</FaFormItem>

				<FaLayoutGridItem span="3">
					<el-divider content-position="left">账号信息</el-divider>
				</FaLayoutGridItem>
				<FaFormItem prop="nickName" label="昵称">
					<el-input v-model="state.accountFormData.nickName" maxlength="20" placeholder="请输入昵称" />
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
					<FaUploadImage v-model="state.accountFormData.avatar" :upload-api="fileApi.uploadAvatar" />
				</FaFormItem>
			</FaForm>

			<template v-if="!userInfoStore.isSuperAdmin && !userInfoStore.isAdmin">
				<el-divider content-position="left">职员信息</el-divider>
				<FaForm ref="employeeFaFormRef" :model="state.employeeFormData" :rules="state.formRules" cols="3">
					<FaFormItem prop="employeeName" label="职员名称">
						<el-input v-model="state.employeeFormData.employeeName" maxlength="20" placeholder="请输入职员名称" />
					</FaFormItem>
					<FaFormItem prop="mobile" label="手机">
						<el-input
							v-model="state.employeeFormData.mobile"
							maxlength="11"
							placeholder="请输入手机"
							autocapitalize="off"
							autocomplete="tel"
							inputmode="tel"
							spellcheck="false"
						/>
					</FaFormItem>
					<FaFormItem prop="email" label="邮箱">
						<el-input v-model="state.employeeFormData.email" maxlength="50" placeholder="请输入邮箱" />
					</FaFormItem>
					<FaFormItem prop="sex" label="性别">
						<RadioGroup name="GenderEnum" v-model="state.employeeFormData.sex" />
					</FaFormItem>
					<FaFormItem prop="idPhoto" label="证件照">
						<FaUploadImage v-model="state.employeeFormData.idPhoto" :upload-api="fileApi.uploadIdPhoto" />
					</FaFormItem>

					<FaLayoutGridItem span="3">
						<el-divider content-position="left">机构信息</el-divider>
					</FaLayoutGridItem>

					<FaLayoutGridItem span="3" style="min-height: 300px; max-height: 500px">
						<FaTable :data="state.employeeFormData.orgList" :pagination="false" :header-card="false">
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
import { useNow } from "@vueuse/core";
import { computed, inject, onMounted, reactive, useTemplateRef, watch } from "vue";
import { ElMessage, dayjs } from "element-plus";
import { type FaButtonInstance, type FaFormInstance, RegExps } from "fast-element-plus";
import { withDefineType } from "@fast-china/utils";
import { employeeApi } from "@/api/services/Admin/employee";
import { accountApi } from "@/api/services/Center/account";
import { fileApi } from "@/api/services/File";
import { changePasswordKey } from "@/layouts";
import { useApp, useUserInfo } from "@/stores";
import type { FormRules } from "element-plus";
import type { EditEmployeeInput } from "@/api/services/Admin/employee/models/EditEmployeeInput";
import type { EditAccountInput } from "@/api/services/Center/account/models/EditAccountInput";
import type { QueryAccountDetailOutput } from "@/api/services/Center/account/models/QueryAccountDetailOutput";
import type { ImageCaptcha } from "@/components";

defineOptions({
	name: "SettingsAccount",
});

const appStore = useApp();
const userInfoStore = useUserInfo();

const mobileButtonRef = useTemplateRef<FaButtonInstance>("mobileButtonRef");
const emailButtonRef = useTemplateRef<FaButtonInstance>("emailButtonRef");
const accountFaFormRef = useTemplateRef<FaFormInstance>("accountFaFormRef");
const employeeFaFormRef = useTemplateRef<FaFormInstance>("employeeFaFormRef");
const mobileCaptchaRef = useTemplateRef<InstanceType<typeof ImageCaptcha>>("mobileCaptchaRef");
const emailCaptchaRef = useTemplateRef<InstanceType<typeof ImageCaptcha>>("emailCaptchaRef");
const changePasswordRef = inject(changePasswordKey);

type IAccountFormData = EditAccountInput &
	QueryAccountDetailOutput & {
		mobileCaptchaKey?: string;
		mobileCaptchaCode?: string;
		emailCaptchaKey?: string;
		emailCaptchaCode?: string;
	};

const state = reactive({
	loading: false,
	mobileNextSendAt: 0,
	emailNextSendAt: 0,
	initialMobile: "",
	initialEmail: "",
	accountFormData: withDefineType<IAccountFormData>({}),
	employeeFormData: withDefineType<EditEmployeeInput>({}),
	formRules: withDefineType<FormRules<IAccountFormData & EditEmployeeInput>>({
		nickName: [{ required: true, message: "请输入昵称", trigger: "blur" }],
		employeeName: [{ required: true, message: "请输入职员名称", trigger: "blur" }],
		mobile: [
			{ required: true, message: "请输入手机", trigger: "blur" },
			{ pattern: RegExps.Mobile, message: "请输入正确的手机号", trigger: "blur" },
		],
		mobileVerificationCode: [
			{ required: true, message: "请输入短信验证码", trigger: "blur" },
			{ pattern: RegExps.VerificationCode, message: "请输入正确的短信验证码", trigger: "blur" },
		],
		email: [
			{ required: true, message: "请输入邮箱", trigger: "blur" },
			{ pattern: RegExps.Email, message: "请输入正确的邮箱", trigger: "blur" },
			{ max: 50, message: "邮箱不能超过50位字符", trigger: "blur" },
		],
		emailVerificationCode: [
			{ required: true, message: "请输入邮箱验证码", trigger: "blur" },
			{ pattern: RegExps.VerificationCode, message: "请输入正确的邮箱验证码", trigger: "blur" },
		],
		idPhoto: [{ required: true, message: "请上传证件照", trigger: "change" }],
		entryDate: [{ required: true, message: "请选择入职日期", trigger: "change" }],
	}),
});

const now = useNow({ interval: 1000 });
const mobileChanged = computed(() => state.accountFormData.mobile?.trim() !== state.initialMobile);
const emailChanged = computed(() => state.accountFormData.email?.trim().toLowerCase() !== state.initialEmail);
/** 手机验证码重新发送倒计时 */
const mobileCountdown = computed(() => Math.min(60, Math.max(0, Math.ceil((state.mobileNextSendAt - now.value.getTime()) / 1000))));
/** 邮箱验证码重新发送倒计时 */
const emailCountdown = computed(() => Math.min(60, Math.max(0, Math.ceil((state.emailNextSendAt - now.value.getTime()) / 1000))));

/** 发送验证码 */
const handleSendVerificationCode = (channel: "mobile" | "email", done?: () => void) => {
	if (channel === "mobile" && mobileCountdown.value <= 0) {
		const { mobile, mobileCaptchaKey, mobileCaptchaCode } = state.accountFormData;
		void accountFaFormRef.value.validateField(["mobile", "mobileCaptchaKey", "mobileCaptchaCode"], async (isValid) => {
			if (!isValid) {
				done?.();
				return;
			}
			await accountApi
				.sendEditAccountVerificationCode({
					account: mobile,
					captchaKey: mobileCaptchaKey,
					captchaCode: mobileCaptchaCode,
				})
				.finally(() => {
					void mobileCaptchaRef.value?.refresh();
					done?.();
				});
			state.mobileNextSendAt = Date.now() + 60_000;
			state.accountFormData.mobileVerificationCode = "";
			ElMessage.success("短信验证码已发送，5分钟内有效");
		});
	} else if (channel === "email" && emailCountdown.value <= 0) {
		const { email, emailCaptchaKey, emailCaptchaCode } = state.accountFormData;
		void accountFaFormRef.value.validateField(["email", "emailCaptchaKey", "emailCaptchaCode"], async (isValid) => {
			if (!isValid) {
				done?.();
				return;
			}
			await accountApi
				.sendEditAccountVerificationCode({
					account: email,
					captchaKey: emailCaptchaKey,
					captchaCode: emailCaptchaCode,
				})
				.finally(() => {
					void emailCaptchaRef.value?.refresh();
					done?.();
				});
			state.emailNextSendAt = Date.now() + 60_000;
			state.accountFormData.emailVerificationCode = "";
			ElMessage.success("邮箱验证码已发送，5分钟内有效");
		});
	}
};

const handleConfirm = async (_event: MouseEvent, done: () => void) => {
	state.loading = true;
	try {
		try {
			await accountFaFormRef.value.validateField(["mobile", "mobileVerificationCode", "email", "emailVerificationCode", "nickName"]);
		} catch (invalidFields) {
			accountFaFormRef.value.scrollToField(Object.keys(invalidFields ?? {})[0]);
			return;
		}
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
		state.initialMobile = state.accountFormData.mobile;
		state.initialEmail = state.accountFormData.email.toLowerCase();
		if (!userInfoStore.isSuperAdmin && !userInfoStore.isAdmin) {
			state.employeeFormData = await employeeApi.queryEmployeeDetail(userInfoStore.employeeId);
		}
	} finally {
		state.loading = false;
	}
});

watch(
	() => state.accountFormData.mobile,
	() => {
		state.accountFormData.mobileVerificationCode = "";
	}
);

watch(
	() => state.accountFormData.email,
	() => {
		state.accountFormData.emailVerificationCode = "";
	}
);
</script>
