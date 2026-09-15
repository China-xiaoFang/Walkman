<template>
	<view class="page">
		<view class="page__brand">
			<view class="page__logo-wrap"><image class="page__logo" :src="appStore.logoUrl || defaultLogo" mode="aspectFit" /></view>
			<view class="page__title">{{ appStore.appName }}</view>
			<view class="page__subtitle"> 统一、安全、便捷的移动管理平台。随时掌握业务动态，高效处理日常工作，让组织协作与数据管理触手可及。 </view>
		</view>

		<wd-form ref="formRef" :model="state.formData" :schema="state.formSchema" error-type="toast">
			<view v-if="userInfoStore.userKey" class="page__user-info">
				<FaIcon name="tenant" size="40rpx" />
				<view class="page__user-info-content">
					<text class="page__tenant-name">{{ userInfoStore.tenantName }}</text>
					<text class="page__employee-name">{{ userInfoStore.employeeName || userInfoStore.nickName }}</text>
				</view>
			</view>
			<wd-form-item v-else custom-class="page__field" prop="account">
				<wd-input
					v-model="state.formData.account"
					clearable
					:compact="false"
					prefix-icon="user"
					:show-word-limit="false"
					:maxlength="50"
					placeholder="请输入登录账号"
				/>
			</wd-form-item>
			<wd-form-item custom-class="page__field" prop="password">
				<wd-input
					v-model="state.formData.password"
					clearable
					:compact="false"
					prefix-icon="lock"
					:show-word-limit="false"
					show-password
					:maxlength="20"
					placeholder="请输入登录密码"
				/>
			</wd-form-item>

			<FaImageCaptcha ref="captchaRef" v-model="state.formData.captchaCode" v-model:captcha-key="state.formData.captchaKey" />

			<view class="page__assist" @click="router.push(CommonRoute.PasswordReset)">
				<wd-icon name="question-circle" size="28rpx" />
				<text>忘记密码</text>
			</view>
			<view class="page__agreement">
				<wd-checkbox v-model="state.formData.agreed" type="square" />
				<view class="page__agreement-text" @click="state.formData.agreed = !state.formData.agreed">
					我已阅读并同意
					<text @click.stop="router.push(CommonRoute.ServiceAgreement)">《服务协议》</text>
					和
					<text @click.stop="router.push(CommonRoute.PrivacyAgreement)">《隐私政策》</text>
				</view>
			</view>
			<wd-button custom-class="page__submit" type="primary" block @click="handleLogin">账号登录</wd-button>
		</wd-form>

		<!-- #ifdef MP-WEIXIN -->
		<view class="page__other">
			<wd-divider>其他登录方式</wd-divider>
			<wd-button open-type="getUserInfo" custom-class="page__wechat" @getuserinfo="handleWeChatLogin">
				<image class="page__wechat-icon" :src="weChatLogo" mode="aspectFit" />
			</wd-button>
		</view>
		<!-- #endif -->
	</view>

	<wd-dialog selector="confirm_agreement_dialog">
		<view class="page__agreement-text">
			我已阅读并同意
			<text @click.stop="router.push(CommonRoute.ServiceAgreement)">《服务协议》</text>
			和
			<text @click.stop="router.push(CommonRoute.PrivacyAgreement)">《隐私政策》</text>
		</view>
	</wd-dialog>

	<!-- #ifdef MP-WEIXIN -->
	<FaPopup ref="authLoginPopupRef" width="80%" :close-on-click-modal="false">
		<view class="page__auth-popup">
			<image class="page__auth-logo" :src="appStore.logoUrl || defaultLogo" mode="aspectFit" />
			<view class="page__auth-title">绑定手机号</view>
			<view class="page__auth-description">该微信尚未绑定手机号。授权手机号后，将自动匹配对应管理账号并继续登录。</view>
			<view class="page__auth-agreement">授权即表示您已阅读并同意《服务协议》和《隐私政策》</view>
			<view class="page__auth-actions">
				<wd-button type="info" block variant="plain" @click="authLoginPopupRef?.close()">暂不登录</wd-button>
				<wd-button open-type="getPhoneNumber" type="primary" block @getphonenumber="handlePhoneLogin"> 授权手机号 </wd-button>
			</view>
		</view>
	</FaPopup>
	<!-- #endif -->
</template>

<script setup lang="ts">
import { reactive, ref } from "vue";
import { logger, throttle, withDefineType } from "@fast-china/utils";
import { useDialog } from "@wot-ui/ui";
import { useRouter } from "uni-mini-router";
import { LoginStatusEnum } from "@/api/enums/LoginStatusEnum";
import { loginApi } from "@/api/services/Auth/login";
import { CommonRoute } from "@/common";
import FaPopup from "@/components/popup/index.vue";
import { useMessageBox, useToast } from "@/hooks";
import weChatLogo from "@/static/images/wechat.png";
import defaultLogo from "@/static/logo.png";
import { useApp, useUserInfo } from "@/stores";
import type { FormInstance, FormSchema } from "@wot-ui/ui/components/wd-form/types";
import type { LoginInput } from "@/api/services/Auth/login/models/LoginInput";
import type { LoginOutput } from "@/api/services/Auth/login/models/LoginOutput";
import type { FaImageCaptchaInstance, FaPopupInstance } from "@/components";

definePage({
	name: "Login",
	layout: "layout",
	noLogin: true,
	authForbidView: true,
	watermark: false,
	style: { navigationBarTitleText: "登录" },
});

const appStore = useApp();
const userInfoStore = useUserInfo();
const router = useRouter();

const formRef = ref<FormInstance>();
const captchaRef = ref<FaImageCaptchaInstance>();
const authLoginPopupRef = ref<FaPopupInstance>();

const confirmAgreementDialog = useDialog("confirm_agreement_dialog");

const state = reactive({
	formData: withDefineType<LoginInput & { agreed?: boolean }>({}),
	formSchema: withDefineType<FormSchema>({
		validate(formModel: LoginInput) {
			return [
				...(!formModel.account ? [{ path: ["account"], message: "请输入账号" }] : []),
				...(formModel.account?.length > 50 ? [{ path: ["account"], message: "账号长度必须为 1~50 个字符" }] : []),
				...(!formModel.password ? [{ path: ["password"], message: "请输入密码" }] : []),
				...(formModel.password?.length < 6 || formModel.password?.length > 20
					? [{ path: ["password"], message: "密码长度必须为 6~20 个字符" }]
					: []),
				...(!/^[A-Za-z0-9]{4}$/.test(formModel.captchaCode) ? [{ path: ["captchaCode"], message: "图形验证码必须为4位字母或数字" }] : []),
			];
		},
	}),
});

/** 协议检查 */
const agreementCheck = async () => {
	if (state.formData.agreed) return true;
	try {
		await confirmAgreementDialog.confirm({
			confirmButtonText: "同意",
			cancelButtonText: "取消",
			closeOnClickModal: false,
		});
		state.formData.agreed = true;
	} catch {
		useToast.warning(`登录前需确认您已阅读并同意《服务协议》和《隐私协议》，以便为您提供更优质的服务。`);
	}
	return state.formData.agreed;
};

/** 登录成功 */
const loginSuccess = (loginRes: LoginOutput) => {
	switch (loginRes.status) {
		case LoginStatusEnum.Success:
			userInfoStore.login(loginRes);
			break;
		case LoginStatusEnum.SelectTenant:
			uni.navigateTo({
				url: CommonRoute.SelectTenant,
				success({ eventChannel }) {
					eventChannel.emit("selectTenantLogin", {
						nickName: loginRes.nickName,
						loginTicket: loginRes.loginTicket,
						tenantList: loginRes.tenantList,
					});
				},
			});
			break;
		case LoginStatusEnum.NotAccount:
			// #ifdef MP-WEIXIN
			authLoginPopupRef.value?.open();
			// #endif
			// #ifndef MP-WEIXIN
			useMessageBox.alert(loginRes.message || "未找到可登录的账号");
			// #endif
			break;
		default:
			useMessageBox.alert(loginRes.message || "登录失败");
	}
};

/** 登录 */
const handleLogin = throttle(async () => {
	const { valid } = await formRef.value.validate();
	if (!valid) return;
	if (!(await agreementCheck())) return;
	const { userKey } = userInfoStore;
	const { account, password, captchaKey, captchaCode } = state.formData;
	try {
		let apiRes: LoginOutput;
		// 判断是否存在用户Key，如果存在直接租户登录
		if (userKey) {
			apiRes = await loginApi.tenantLogin({ userKey, password, captchaKey, captchaCode });
		} else {
			apiRes = await loginApi.login({ account, password, captchaKey, captchaCode });
		}
		loginSuccess(apiRes);
	} catch {
		captchaRef.value?.refresh();
	}
});

/** 微信登录 */
const handleWeChatLogin = throttle(async ({ detail }: { detail: UniNamespace.GetUserInfoRes }) => {
	if (!(await agreementCheck())) return;
	const { iv, encryptedData, userInfo } = detail;
	if (userInfo) {
		logger.log("Login", "GetUserInfo", userInfo);
		const weChatCode = await userInfoStore.getWeChatCode();
		if (weChatCode) {
			const loginRes = await loginApi.weChatLogin({ weChatCode, iv, encryptedData });
			loginSuccess(loginRes);
			return;
		}
	}
	useToast.warning("授权失败，无法获取您的信息。请重新授权以继续使用我们的服务。");
});

/** 手机登录 */
const handlePhoneLogin = throttle(({ detail }: { detail: UniHelper.ButtonOnGetphonenumberDetail }) => {
	authLoginPopupRef.value.close(async () => {
		logger.log("Login", "GetPhoneNumber", detail);
		const { code } = detail;
		const weChatCode = await userInfoStore.getWeChatCode();
		if (code && weChatCode) {
			const loginRes = await loginApi.weChatAuthLogin({ weChatCode, code });
			loginSuccess(loginRes);
			return;
		}
		useToast.warning("授权失败，无法获取您的信息。请重新授权以继续使用我们的服务。");
	});
});
</script>

<style scoped lang="scss">
@use "./index.scss";
</style>
