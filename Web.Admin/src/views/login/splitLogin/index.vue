<template>
	<div class="modern-login" :style="{ '--login-bg': props.background }">
		<slot name="help" />
		<!-- 左侧装饰面板 -->
		<div class="modern-login__panel">
			<div class="panel__decoration">
				<div class="deco-circle deco-circle--1"></div>
				<div class="deco-circle deco-circle--2"></div>
				<div class="deco-circle deco-circle--3"></div>
			</div>
			<div class="panel__content">
				<div class="panel__logo">
					<img :src="logoImage" class="logo-img" />
					<span class="logo-text">Fast随身听</span>
				</div>
				<div class="panel__icon">
					<svg viewBox="0 0 200 200" fill="none" xmlns="http://www.w3.org/2000/svg">
						<circle cx="100" cy="100" r="80" fill="rgba(255,255,255,0.08)" />
						<circle cx="100" cy="100" r="60" fill="rgba(255,255,255,0.06)" />
						<!-- 盾牌外框 -->
						<path
							d="M100 35 L145 55 V110 C145 140 120 162 100 170 C80 162 55 140 55 110 V55 Z"
							fill="rgba(255,255,255,0.15)"
							stroke="rgba(255,255,255,0.6)"
							stroke-width="2.5"
							stroke-linejoin="round"
						/>
						<!-- 对勾 -->
						<polyline
							points="78,105 93,120 122,85"
							stroke="rgba(255,255,255,0.9)"
							stroke-width="4"
							stroke-linecap="round"
							stroke-linejoin="round"
							fill="none"
						/>
					</svg>
				</div>
				<div class="panel__tagline">
					<h2>
						<span class="tagline-title--1">Fast随身听</span>
						<span class="tagline-title--2">管理平台</span>
					</h2>
					<p>高效、安全、便捷的一体化企业管理解决方案</p>
				</div>
			</div>
		</div>
		<!-- 右侧登录表单 -->
		<div class="modern-login__form-wrap">
			<div key="Account" class="form-container">
				<div class="form-header">
					<h1 class="form-title">欢迎登录</h1>
					<p class="form-subtitle">Fast随身听平台管理系统</p>
				</div>
				<el-form ref="elFormRef" :model="formData" :rules="props.formRules" size="large" labelPosition="top" @keyup.enter="handleKeyupEnter">
					<el-form-item prop="account" label="账号">
						<el-input
							v-model.trim="formData.account"
							placeholder="请输入账号"
							type="text"
							tabindex="1"
							:prefixIcon="User"
							@change="handleAccountChange"
						/>
					</el-form-item>
					<el-form-item prop="password" label="密码">
						<el-input
							v-model.trim="formData.password"
							placeholder="请输入密码"
							type="password"
							tabindex="2"
							:showPassword="!formData.encryptPassword"
							:prefixIcon="Lock"
							@input="handlePasswordInput"
						/>
					</el-form-item>
					<div class="form-options">
						<el-checkbox v-model.checked="formData.rememberMe">记住密码</el-checkbox>
					</div>
					<FaButton ref="faButtonRef" class="login-btn" type="primary" size="large" @click="handleFormLogin">
						<span>登 录</span>
					</FaButton>
				</el-form>
			</div>
			<div class="form-footer">
				<Footer />
			</div>
		</div>
	</div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { FormInstance, type FormRules } from "element-plus";
import { Lock, User } from "@element-plus/icons-vue";
import { FaButtonInstance } from "fast-element-plus";
import { definePropType } from "@fast-china/utils";
import logoImage from "@/assets/logo.png";
import { useLogin } from "../useLogin";

defineOptions({
	name: "SplitLogin",
});

const props = defineProps({
	/** 背景 */
	background: String,
	/** 页脚高度 */
	footerHeight: Number,
	/** 表单规则 */
	formRules: definePropType<FormRules>(Object),
});

const elFormRef = ref<FormInstance>();
const faButtonRef = ref<FaButtonInstance>();

const { formData, handleAccountChange, handlePasswordInput, handleFormLogin, handleKeyupEnter } = useLogin(elFormRef, faButtonRef);
</script>

<style scoped lang="scss">
@use "./index.scss";
</style>
