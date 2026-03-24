<template>
	<!-- 'background-image': configStore.layout.isDark ? '' : `url(${appStore.statusBarImageUrl})`, -->

	<view
		class="page"
		:style="{
			'background-image': configStore.layout.isDark ? '' : `url(${topImage})`,
		}"
	>
		<view
			class="status-bar"
			:style="{
				'background-color': state.isScrolled ? 'var(--wot-navbar-bg-color)' : '',
			}"
		/>

		<wd-navbar class="fa-navbar" :customClass="state.isScrolled ? 'is-scrolled' : ''" :bordered="false" title="个人中心" />

		<view class="user__card">
			<template v-if="!configStore.layout.isDark">
				<image class="top_bg" src="@/static/images/card_top_bg.png" />
				<image class="bottom_bg" src="@/static/images/card_bottom_bg.png" />
			</template>

			<view class="user__warp">
				<view class="left">
					<template v-if="userInfoStore.hasUserInfo">
						<FaImage width="120rpx" height="120rpx" round original :hideImage="false" :src="userInfoStore.avatar" />
						<view class="user__info" @click="router.push('/pages/setting/userInfo/index')">
							<view class="nickName">{{ userInfoStore.nickName }}</view>
							<view class="mobile">{{ userInfoStore.mobile || "暂未授权手机号" }}</view>
						</view>
					</template>

					<template v-else>
						<FaImage width="120rpx" height="120rpx" round :hideImage="false" :src="defaultLogo" />
						<view class="user__info">
							<view class="nickName">Fast随身听 用户</view>
							<view class="mobile">登录后体验更多功能</view>
						</view>
					</template>
				</view>

				<!-- #ifdef MP-WEIXIN -->
				<template v-if="!userInfoStore.hasUserInfo">
					<wd-button size="small" openType="getUserInfo" type="primary" @getuserinfo="handleWeChatLogin">登录</wd-button>
				</template>
				<template v-else>
					<template v-if="!userInfoStore.mobile">
						<wd-button
							v-if="appStore.isClient"
							size="small"
							openType="getPhoneNumber"
							type="primary"
							block
							@getphonenumber="handlePhoneLogin"
						>
							授权手机号
						</wd-button>
						<wd-button
							v-else
							size="small"
							openType="getRealtimePhoneNumber"
							type="primary"
							block
							@getrealtimephonenumber="handlePhoneLogin"
						>
							授权手机号
						</wd-button>
					</template>
				</template>
				<!-- #endif -->
			</view>
		</view>

		<view class="mb30 data-card">
			<wd-cell customClass="card__cell" title="我的功能" />
			<view class="card__content">
				<view class="card__item" @click="makePhoneCall">
					<FaIcon name="call" />
					<text>联系我们</text>
				</view>
				<view class="card__item" @click="router.push(CommonRoute.ComplaintSubmit)">
					<wd-icon name="evaluation" />
					<text>投诉建议</text>
				</view>
			</view>
		</view>

		<wd-cell-group border>
			<wd-cell title="系统版本" :value="appVersion" clickable @click="checkMiniAppVersion" />
			<!-- #ifdef APP-PLUS -->
			<wd-cell title="热更新版本" :value="`v${appStore.appVersion}`" />
			<!-- #endif -->
			<wd-cell title="服务有效期" value="2029-12-31 23:59:59" />
		</wd-cell-group>

		<wd-cell-group border>
			<wd-cell customClass="mt20" title="消息通知" clickable isLink @click="useToast.info('敬请期待')" />
			<wd-cell title="通用" clickable isLink to="/pages/setting/general/index" />
			<wd-cell customClass="mb20" title="清除缓存" :value="`${currentSize}/${limitSize}`" clickable isLink @click="handleClearCache" />
		</wd-cell-group>

		<view class="agreement mb20">
			<text @click="router.push(CommonRoute.UserAgreement)">《用户协议》</text>
			<text @click="router.push(CommonRoute.PrivacyAgreement)">《隐私协议》</text>
			<text @click="router.push(CommonRoute.ServiceAgreement)">《服务协议》</text>
		</view>

		<wd-button
			v-if="userInfoStore.hasUserInfo"
			customClass="btn__exit-login"
			type="primary"
			block
			:round="false"
			icon="exit"
			@click="handleLogout"
		>
			退出登录
		</wd-button>
		<wd-message-box selector="confirm-agreement-box">
			<view class="agreement__warp">
				我已阅读并同意
				<text @click="router.push(CommonRoute.UserAgreement)">《用户协议》</text>
				<text @click="router.push(CommonRoute.PrivacyAgreement)">《隐私协议》</text>
				<text @click="router.push(CommonRoute.ServiceAgreement)">《服务协议》</text>
			</view>
		</wd-message-box>
	</view>
</template>

<script setup lang="ts">
import { onPageScroll, onPullDownRefresh, onShow } from "@dcloudio/uni-app";
import { computed, reactive, ref } from "vue";
import { clickUtil, consoleError, consoleLog } from "@fast-china/utils";
import { useRouter } from "uni-mini-router";
import { useMessage } from "wot-design-uni";
import { CommonRoute } from "@/common";
import { useMessageBox, useToast } from "@/hooks";
import topImage from "@/static/images/topImage.png";
import defaultLogo from "@/static/logo.png";
import { useApp, useConfig, useUserInfo } from "@/stores";

definePage({
	name: "My",
	layout: "layout",
	isTabBar: true,
	style: {
		navigationStyle: "custom",
		navigationBarTitleText: "个人中心",
		enablePullDownRefresh: true,
	},
});

const appStore = useApp();
const configStore = useConfig();
const userInfoStore = useUserInfo();
const router = useRouter();

const confirmAgreementBox = useMessage("confirm-agreement-box");

const state = reactive({
	/** 是否滚动 */
	isScrolled: false,
});

const storageInfo = ref<Partial<UniNamespace.GetStorageInfoSuccess>>({});

const appVersion = computed(() => {
	let envName = "";
	switch (appStore.env) {
		case "production":
			envName = "生产版";
			break;
		case "development":
			envName = "开发版";
			break;
		case "test":
			envName = "测试版";
			break;
		case "staging":
			envName = "预览版";
			break;
	}
	return `${envName} v${appStore.appBaseInfo.appVersion}`;
});

const currentSize = computed(() => {
	if (storageInfo.value.currentSize >= 1024) {
		const mb = storageInfo.value.currentSize / 1024;
		return `${Number.isInteger(mb) ? mb : mb.toFixed(2)}MB`;
	} else {
		return `${storageInfo.value.currentSize}KB`;
	}
});

const limitSize = computed(() => {
	if (storageInfo.value.limitSize >= 1024) {
		const mb = storageInfo.value.limitSize / 1024;
		return `${Number.isInteger(mb) ? mb : mb.toFixed(2)}MB`;
	} else {
		return `${storageInfo.value.limitSize}KB`;
	}
});

/** 检查小程序版本 */
const checkMiniAppVersion = () => {
	const updateManager = uni.getUpdateManager();
	// 检查小程序是否有新版本
	updateManager.onCheckForUpdate((res) => {
		if (res.hasUpdate) {
			updateManager.onUpdateReady(() => {
				consoleLog("Launcher", "小程序存在新版本");
				useMessageBox
					.alert({
						title: "更新提示",
						msg: "新版本已经准备好，需要重启后才能正常使用应用。",
					})
					.then(() => {
						updateManager.applyUpdate(); //调用 applyUpdate 应用新版本并重启
					});
			});
			updateManager.onUpdateFailed(() => {
				consoleError("Launcher", "小程序更新检测异常");
				useMessageBox
					.alert({
						msg: "系统异常，无法更新新版本，是否重启应用？",
						confirmButtonText: "重启应用",
					})
					.then(() => {
						updateManager.applyUpdate(); //调用 applyUpdate 应用新版本并重启
					});
			});
		} else {
			useToast.success("小程序已是最新版本");
		}
	});
};

/** 拨打电话 */
const makePhoneCall = (): void => {
	uni.makePhoneCall({
		phoneNumber: "",
	});
};

/** 微信登录 */
const handleWeChatLogin = async (detail: UniNamespace.GetUserInfoRes) => {
	await clickUtil.throttleAsync(async () => {
		try {
			await confirmAgreementBox.confirm({
				confirmButtonText: "同意",
				cancelButtonText: "取消",
				closeOnClickModal: false,
			});
		} catch {
			useToast.warning(`登录前需确认您已阅读并同意《用户协议》、《隐私协议》、《服务协议》，以便为您提供更优质的服务。`);
			return;
		}
		const { iv, encryptedData, userInfo } = detail;
		if (userInfo) {
			consoleLog("Login", "GetUserInfo", detail);
			await userInfoStore.login({
				iv,
				encryptedData,
			});
		} else {
			useToast.warning("授权失败，无法获取您的信息。请重新授权以继续使用我们的服务。");
		}
	});
};

/** 手机登录 */
const handlePhoneLogin = async (detail: UniHelper.ButtonOnGetrealtimephonenumberEvent | UniHelper.ButtonOnGetphonenumberEvent) => {
	await clickUtil.throttleAsync(async () => {
		consoleLog("Login", "PhoneNumber", detail);
		const { code } = detail;
		if (!code) {
			useToast.warning("暂不授权可能会影响部分功能的正常使用，您可在后续使用过程中再次授权。");
			return;
		}
		await userInfoStore.login({
			code,
		});
	});
};

/** 清除缓存 */
const handleClearCache = async () => {
	await clickUtil.throttleAsync(async () => {
		await useMessageBox.confirm("清除缓存后需要重新登录，确定要清除？");
		appStore.clearAppCache();
		await userInfoStore.logout();
		useToast.success("退出登录成功");
	});
};

/** 退出登录 */
const handleLogout = async () => {
	await clickUtil.throttleAsync(async () => {
		await useMessageBox.confirm("确定要退出登录？");
		await userInfoStore.logout();
		useToast.success("退出登录成功");
	});
};

/** 页面刷新 */
const loadRefresh = async () => {};

onShow(async () => {
	storageInfo.value = await uni.getStorageInfo();
	await loadRefresh();
});

onPullDownRefresh(async () => {
	// 刷新App信息
	await userInfoStore.refreshApp();
	await loadRefresh();
	uni.stopPullDownRefresh();
});

onPageScroll(({ scrollTop }) => {
	state.isScrolled = scrollTop > 0;
});
</script>

<style scoped lang="scss">
@import "./index.scss";
</style>
