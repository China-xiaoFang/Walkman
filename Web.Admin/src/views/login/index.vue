<template>
	<component
		:is="loginComponents['ModernLogin']"
		:background="getThemeGradient(configStore.layout.themeColor, configStore.layout.isDark ? 'dark' : 'light')"
		:footerHeight="configStore.layout.footerHeight"
		:formRules="state.formRules"
	>
		<template #help>
			<el-dropdown ref="helpDropdownRef" class="help_dropdown" size="default" trigger="click" @command="handleDropdownClick">
				<div>
					<el-icon :size="20" title="主题">
						<ChromeFilled />
					</el-icon>
				</div>
				<template #dropdown>
					<el-dropdown-menu>
						<el-dropdown-item disabled>主题</el-dropdown-item>
						<el-dropdown-item divided :icon="Light" command="浅色模式" :disabled="!configStore.layout.isDark">浅色模式</el-dropdown-item>
						<el-dropdown-item :icon="Dark" command="深色模式" :disabled="configStore.layout.isDark">深色模式</el-dropdown-item>
						<el-dropdown-item :icon="Operation" command="系统设置" :disabled="configStore.layout.autoThemMode">
							系统设置
						</el-dropdown-item>
						<el-dropdown-item disabled>谨慎使用</el-dropdown-item>
						<el-dropdown-item divided :icon="Refresh" command="重置系统">重置系统</el-dropdown-item>
					</el-dropdown-menu>
				</template>
			</el-dropdown>
			<el-tour v-model="state.helpTourValue" :showClose="false" :closeOnPressEscape="false">
				<el-tour-step :target="helpDropdownRef?.$el" title="重新加载系统" placement="left-end">
					<span>如果存在异常显示。</span>
					<br />
					<span>点击右侧图标可进行重置系统操作。</span>
				</el-tour-step>
			</el-tour>
		</template>
	</component>
</template>

<script lang="ts" setup>
import { defineAsyncComponent, onMounted, provide, reactive, ref, toRef } from "vue";
import { ElMessageBox } from "element-plus";
import { ChromeFilled, Operation, Refresh } from "@element-plus/icons-vue";
import { Dark, Light } from "@fast-element-plus/icons-vue";
import { Local, Session, consoleError, useIdentity, withDefineType } from "@fast-china/utils";
import { useConfig } from "@/stores";
import type { LoginInput } from "@/api/services/Auth/login/models/LoginInput";
import type { ILoginComponent } from "@/stores";
import type { DropdownInstance, FormRules } from "element-plus";
import type { Component } from "vue";

defineOptions({
	name: "Login",
});

export type IFormData = {
	/** 记住密码 */
	rememberMe?: boolean;
	/** 加密密码 */
	encryptPassword?: boolean;
} & LoginInput;

export type IFormStep = "Account" | "TenantAccount" | "SelectTenant" | "NewAccount";

/** 登录组件 */
const loginComponents = withDefineType<Record<ILoginComponent, Component>>({
	ClassicLogin: defineAsyncComponent(() => import("./classicLogin/index.vue")),
	ModernLogin: defineAsyncComponent(() => import("./modernLogin/index.vue")),
	SimpleLogin: defineAsyncComponent(() => import("./simpleLogin/index.vue")),
	SplitLogin: defineAsyncComponent(() => import("./splitLogin/index.vue")),
});

const configStore = useConfig();

const helpDropdownRef = ref<DropdownInstance>();

const state = reactive({
	/** 帮助漫游式引导值 */
	helpTourValue: false,
	/** 表单数据 */
	formData: withDefineType<IFormData>({}),
	/** 表单规则 */
	formRules: withDefineType<FormRules>({
		account: [{ required: true, message: "请输入账号", trigger: "blur" }],
		password: [{ required: true, message: "请输入密码", trigger: "blur" }],
		userKey: [{ required: true, message: "请选择租户", trigger: "change" }],
	}),
	/** 缓存Key */
	cFormKey: "LOGIN_FORM",
});

provide("formData", toRef(state, "formData"));
provide("cFormKey", state.cFormKey);

onMounted(() => {
	try {
		const formData = Local.get<IFormData>(state.cFormKey);
		if (formData) {
			state.formData = formData;
			state.formData.encryptPassword = formData.rememberMe;
		}
	} catch (error) {
		state.helpTourValue = true;
		consoleError("Login", error);
	}
});

const handleDropdownClick = (command: string) => {
	switch (command) {
		case "浅色模式":
			configStore.layout.autoThemMode = false;
			configStore.layout.isDark = false;
			configStore.switchDark();
			break;
		case "深色模式":
			configStore.layout.autoThemMode = false;
			configStore.layout.isDark = true;
			configStore.switchDark();
			break;
		case "系统设置":
			configStore.layout.autoThemMode = true;
			configStore.switchAutoThemMode();
			break;
		case "重置系统":
			ElMessageBox.confirm(
				`确定重置系统？<br/><span class="el-text el-text--danger">重置系统将清除所有缓存信息，系统将进行初始化处理，确定要继续执行吗？</span>`,
				{
					dangerouslyUseHTMLString: true,
					type: "warning",
					async beforeClose(_, instance) {
						instance.confirmButtonText = "重置中...";
						await new Promise((resolve) => {
							setTimeout(() => {
								// 获取设备Id
								const uIdentity = useIdentity();
								// 清空 Local 缓存
								Local.clear();
								// 清空 Session 缓存
								Session.clear();
								// 重新设置设备Id
								uIdentity.makeIdentity(uIdentity.deviceId);
								// 刷新App
								window.location.reload();
								resolve(true);
							}, 2000);
						});
					},
				}
			);
			break;
	}
};

/**
 * 根据主题色生成线性渐变（支持浅色 / 深色模式）
 * --------------------------------------------------------
 * 特点：
 *  - 主色为中心，向两侧偏蓝与偏紫延伸
 *  - 提供 light / dark 模式自适应亮度与饱和度
 *
 * @param baseColor HEX 主题色，如 "#0487d0"
 * @param mode 'light' | 'dark'，默认 'light'
 * @param angleDeg 渐变角度，默认 165°
 * @returns CSS linear-gradient 字符串
 */
const getThemeGradient = (baseColor: string, mode: "light" | "dark" = "light", angleDeg = 165): string => {
	/**
	 * HEX 转 HSL
	 */
	const hexToHsl = (hex: string) => {
		hex = hex.replace("#", "");
		const r = parseInt(hex.substring(0, 2), 16) / 255;
		const g = parseInt(hex.substring(2, 4), 16) / 255;
		const b = parseInt(hex.substring(4, 6), 16) / 255;

		const max = Math.max(r, g, b);
		const min = Math.min(r, g, b);
		const l = (max + min) / 2;
		const d = max - min;
		let h = 0,
			s = 0;

		if (d !== 0) {
			s = d / (1 - Math.abs(2 * l - 1));
			switch (max) {
				case r:
					h = ((g - b) / d) % 6;
					break;
				case g:
					h = (b - r) / d + 2;
					break;
				case b:
					h = (r - g) / d + 4;
					break;
			}
			h = Math.round(h * 60);
			if (h < 0) h += 360;
		}

		return { h, s: s * 100, l: l * 100 };
	};

	/**
	 * HSL 转 HEX
	 */
	const hslToHex = (h: number, s: number, l: number) => {
		s /= 100;
		l /= 100;
		const k = (n: number) => (n + h / 30) % 12;
		const a = s * Math.min(l, 1 - l);
		const f = (n: number) => l - a * Math.max(-1, Math.min(k(n) - 3, Math.min(9 - k(n), 1)));
		const toHex = (x: number) =>
			Math.round(x * 255)
				.toString(16)
				.padStart(2, "0");
		return `#${toHex(f(0))}${toHex(f(8))}${toHex(f(4))}`;
	};

	const base = hexToHsl(baseColor);

	// 调整系数（light / dark 模式下不同）
	const tone =
		mode === "light"
			? { l: 1.0, s: 1.0 } // 浅色：保持原亮度
			: { l: 0.7, s: 0.9 }; // 深色：整体降低亮度饱和度

	/**
	 * 渐变层定义（蓝紫系层次感）
	 */
	const variations = [
		// 深紫蓝
		{ h: base.h + 30, s: base.s * 0.6 * tone.s, l: base.l * 0.85 * tone.l },
		// 主色
		{ h: base.h, s: base.s * 1.0 * tone.s, l: base.l * 1.0 * tone.l },
		// 冷蓝
		{ h: base.h - 10, s: base.s * 0.8 * tone.s, l: base.l * 0.9 * tone.l },
		// 亮蓝紫
		{ h: base.h + 45, s: base.s * 0.6 * tone.s, l: base.l * 1.4 * tone.l },
	];

	const colors = variations.map((v) => hslToHex(v.h % 360, Math.min(v.s, 100), Math.min(v.l, 100)));

	return `linear-gradient(${angleDeg}deg, ${colors.join(", ")})`;
};
</script>

<style scoped lang="scss">
.help_dropdown {
	position: fixed;
	top: 5%;
	right: 5%;
	cursor: pointer;
	color: var(--el-color-white);
	z-index: 2001;
}
</style>
