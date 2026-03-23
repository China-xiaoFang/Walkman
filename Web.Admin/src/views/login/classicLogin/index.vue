<template>
	<el-container :style="{ background: props.background }">
		<slot name="help" />
		<el-main>
			<div class="classic-login">
				<!-- 左侧插图 -->
				<div class="classic-login__brand">
					<div class="brand__title">
						<img :src="logoImage" />
						Fast随身听
					</div>
					<div class="brand__ikon">
						<img :src="loginIkonImage" />
					</div>
				</div>
				<!-- 右侧登录表单 -->
				<div class="classic-login__form">
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
import { Lock, User } from "@element-plus/icons-vue";
import { FaButtonInstance } from "fast-element-plus";
import { addUnit, definePropType } from "@fast-china/utils";
import loginIkonImage from "@/assets/images/login_ikon.png";
import logoImage from "@/assets/logo.png";
import { useLogin } from "../useLogin";

defineOptions({
	name: "ClassicLogin",
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
