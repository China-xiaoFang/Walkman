<template>
	<el-container :style="{ background: props.background }">
		<slot name="help" />
		<div class="animated-bg">
			<div class="bg-orb bg-orb--1" />
			<div class="bg-orb bg-orb--2" />
			<div class="bg-orb bg-orb--3" />
			<div class="bg-grid" />
			<div class="bg-glow bg-glow--top" />
			<div class="bg-glow bg-glow--bottom" />
		</div>
		<el-main>
			<div class="simple-login">
				<!-- 顶部装饰 -->
				<div class="simple-login__decor">
					<div class="decor-line decor-line--1" />
					<div class="decor-line decor-line--2" />
					<div class="decor-dot decor-dot--1" />
					<div class="decor-dot decor-dot--2" />
					<div class="decor-dot decor-dot--3" />
					<div class="decor__logo">
						<div class="decor__ring" />
						<img :src="logoImage" />
					</div>
					<h2>Fast随身听</h2>
					<p>欢迎使用管理系统</p>
					<div class="decor__line" />
				</div>
				<!-- 底部登录表单 -->
				<div class="simple-login__form">
					<div class="form-panel">
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
									placeholder="请输入账号"
									type="text"
									tabindex="1"
									:prefixIcon="User"
									@change="handleAccountChange"
								/>
							</el-form-item>
							<el-form-item prop="password">
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
import { Lock, User } from "@element-plus/icons-vue";
import { FaButtonInstance } from "fast-element-plus";
import { addUnit, definePropType } from "@fast-china/utils";
import logoImage from "@/assets/logo.png";
import { useLogin } from "../useLogin";

defineOptions({
	name: "SimpleLogin",
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
