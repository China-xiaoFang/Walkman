<template>
	<el-container :style="{ background: props.background }">
		<slot name="help" />
		<el-main>
			<div class="modern-login">
				<!-- 左侧品牌展示区 -->
				<div class="split-login__brand">
					<div class="brand-bg">
						<div class="brand-particle brand-particle--1" />
						<div class="brand-particle brand-particle--2" />
						<div class="brand-particle brand-particle--3" />
						<div class="brand-particle brand-particle--4" />
						<div class="brand-ring brand-ring--1" />
						<div class="brand-ring brand-ring--2" />
					</div>
					<div class="brand-content">
						<div class="brand-logo">
							<img :src="logoImage" alt="Logo" />
						</div>
						<h1 class="brand-title">
							<span class="brand-title--1">Fast</span>
							<span class="brand-title--2">DotNet</span>
						</h1>
						<p class="brand-subtitle">
							<span class="brand-subtitle--1">Fast随身听</span>
							<span class="brand-subtitle--2">管理平台</span>
						</p>
						<div class="brand-divider" />
						<ul class="brand-features">
							<li>
								<el-icon><Monitor /></el-icon>
								<span>现代化企业级管理系统</span>
							</li>
							<li>
								<el-icon><Lock /></el-icon>
								<span>安全可靠的数据保护</span>
							</li>
							<li>
								<el-icon><TrendCharts /></el-icon>
								<span>高效便捷的业务流程</span>
							</li>
							<li>
								<el-icon><Iphone /></el-icon>
								<span>多终端适配体验</span>
							</li>
						</ul>
					</div>
				</div>
				<!-- 右侧登录表单 -->
				<div class="split-login__form">
					<div class="form-panel">
						<div class="form-header" style="margin-bottom: 28px">
							<h2>欢迎登录</h2>
							<p>
								<span class="bold">Fast随身听</span>
								管理平台
							</p>
						</div>
						<el-form
							ref="elFormRef"
							labelPosition="top"
							:model="formData"
							:rules="props.formRules"
							size="large"
							labelSuffix=""
							@keyup.enter="handleKeyupEnter"
						>
							<el-form-item prop="account" label="账号">
								<el-input
									v-model.trim="formData.account"
									placeholder="账号"
									type="text"
									tabindex="1"
									:prefixIcon="User"
									@change="handleAccountChange"
								/>
							</el-form-item>
							<el-form-item prop="password" label="密码">
								<el-input
									v-model.trim="formData.password"
									placeholder="密码"
									type="password"
									tabindex="2"
									:showPassword="!formData.encryptPassword"
									:prefixIcon="Lock"
									@input="handlePasswordInput"
								/>
							</el-form-item>
							<el-form-item prop="rememberMe">
								<el-checkbox v-model.checked="formData.rememberMe" size="default">记住密码</el-checkbox>
							</el-form-item>
							<FaButton ref="faButtonRef" class="w100 login-btn" type="primary" size="large" @click="handleFormLogin"> 登 录 </FaButton>
						</el-form>
					</div>
					<div class="glass-text">Powered by FastDotNet</div>
				</div>
			</div>
		</el-main>
		<el-footer :style="{ '--el-footer-height': addUnit(props.footerHeight) }">
			<Footer />
		</el-footer>
	</el-container>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { FormInstance, type FormRules } from "element-plus";
import { Iphone, Lock, Monitor, TrendCharts, User } from "@element-plus/icons-vue";
import { FaButtonInstance } from "fast-element-plus";
import { addUnit, definePropType } from "@fast-china/utils";
import logoImage from "@/assets/logo.png";
import { useLogin } from "../useLogin";

defineOptions({
	name: "ModernLogin",
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
