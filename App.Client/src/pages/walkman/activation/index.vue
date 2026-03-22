<template>
	<view class="page activation-page">
		<view class="activation-header">
			<wd-icon name="gift" size="120rpx" color="var(--wot-color-theme)" />
			<text class="activation-title">输入激活码</text>
			<text class="activation-desc">输入激活码解锁全部课程内容</text>
		</view>

		<view class="activation-form">
			<wd-input
				v-model="state.code"
				placeholder="请输入激活码"
				clearable
				:maxlength="16"
			/>
			<wd-button
				type="primary"
				block
				:disabled="!state.code"
				@click="handleActivate"
			>
				立即激活
			</wd-button>
		</view>

		<!-- 激活状态 -->
		<view class="activation-status" v-if="state.activationStatus">
			<view class="status-header">
				<wd-icon :name="state.activationStatus.isActivated ? 'check-circle-fill' : 'info-circle-fill'" :size="'36rpx'" :color="state.activationStatus.isActivated ? '#52c41a' : '#999'" />
				<text class="status-text">{{ state.activationStatus.isActivated ? '已激活' : '未激活' }}</text>
			</view>
		</view>
	</view>
</template>

<script setup lang="ts">
import { onLoad } from "@dcloudio/uni-app";
import { reactive } from "vue";
import { playApi } from "@/api/services/Play";
import type { ActivationStatusOutput } from "@/api/services/Play/models/ActivationStatusOutput";

definePage({
	name: "WalkmanActivation",
	layout: "layout",
	style: {
		navigationBarTitleText: "激活码",
	},
});

const state = reactive({
	code: "",
	activationStatus: null as ActivationStatusOutput | null,
});

const handleActivate = async () => {
	if (!state.code) return;
	try {
		await playApi.useActivationCode({ code: state.code });
		uni.showToast({ title: "激活成功！", icon: "success" });
		state.code = "";
		await loadActivationStatus();
	} catch {
		// 错误由框架处理
	}
};

const loadActivationStatus = async () => {
	try {
		state.activationStatus = await playApi.getActivationStatus();
	} catch {
		state.activationStatus = null;
	}
};

onLoad(async () => {
	await loadActivationStatus();
});
</script>

<style scoped lang="scss">
@import "./index.scss";
</style>
