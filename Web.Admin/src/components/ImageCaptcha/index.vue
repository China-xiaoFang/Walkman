<template>
	<el-form-item
		v-if="state.enabled"
		label="验证码"
		:prop="props.prop"
		:rules="[
			{ required: true, message: '请输入图形验证码', trigger: 'blur' },
			{ pattern: RegExps.ImageCaptchaCode, message: '图形验证码必须为4位字母或数字', trigger: 'blur' },
		]"
	>
		<div class="image-captcha">
			<el-input
				v-model.trim="modelValue"
				autocapitalize="off"
				autocomplete="off"
				:disabled="props.disabled || state.loading || state.loadFailed"
				maxlength="4"
				placeholder="请输入验证码"
				:prefix-icon="PictureRounded"
				:show-word-limit="false"
				spellcheck="false"
			/>
			<button
				class="image-captcha__image"
				type="button"
				aria-label="刷新验证码"
				:disabled="props.disabled || state.loading"
				title="点击刷新验证码"
				@click="refresh"
			>
				<img v-if="state.captchaImage" :src="state.captchaImage" alt="图形验证码" />
				<span v-else>{{ state.loading ? "加载中…" : state.loadFailed ? "加载失败" : "点击重试" }}</span>
			</button>
		</div>
	</el-form-item>
</template>

<script lang="ts" setup>
import { useVModel } from "@vueuse/core";
import { onMounted, reactive } from "vue";
import { PictureRounded } from "@element-plus/icons-vue";
import { RegExps } from "fast-element-plus";
import { loginApi } from "@/api/services/Auth/login";

defineOptions({
	name: "ImageCaptcha",
});

const props = withDefaults(
	defineProps<{
		modelValue?: string;
		/** 图形验证码Key */
		captchaKey?: string;
		/** 绑定的属性名称 @default 'captchaCode' */
		prop?: string;
		/** 是否强制启用；false 时由后端登录验证码开关决定。 */
		isForce?: boolean;
		/** 业务请求期间禁用输入与手动刷新。 */
		disabled?: boolean;
	}>(),
	{
		modelValue: undefined,
		captchaKey: undefined,
		prop: "captchaCode",
		isForce: false,
		disabled: false,
	}
);

const emit = defineEmits({
	"update:modelValue": (_value: string) => true,
	"update:captchaKey": (_value: string) => true,
});

const modelValue = useVModel(props, "modelValue", emit, { passive: false });
const captchaKey = useVModel(props, "captchaKey", emit, { passive: false });

const state = reactive({
	enabled: true,
	loading: false,
	loadFailed: false,
	captchaImage: undefined,
});

/** 重新获取图形验证码并清空旧答案。 */
const refresh = async () => {
	state.loading = true;
	state.loadFailed = false;
	captchaKey.value = undefined;
	modelValue.value = undefined;
	state.captchaImage = undefined;
	try {
		const apiRes = await loginApi.getLoginCaptcha(props.isForce).finally(() => {
			state.loading = false;
		});
		state.enabled = props.isForce || apiRes.enabled;
		captchaKey.value = apiRes.captchaKey;
		state.captchaImage = apiRes.captchaImage;
	} catch {
		state.enabled = true;
		state.loadFailed = true;
	}
};

onMounted(() => {
	void refresh();
});

defineExpose({
	refresh,
});
</script>

<style scoped lang="scss">
.image-captcha {
	display: grid;
	width: 100%;
	min-width: 0;
	grid-template-columns: minmax(0, 1fr) 132px;
	align-items: center;
	gap: 8px;
	&__image {
		height: 46px;
		min-width: 0;
		padding: 0;
		cursor: pointer;
		color: var(--el-text-color-secondary);
		border: 1px solid var(--el-border-color-lighter);
		border-radius: 8px;
		background: var(--el-fill-color-light);
		overflow: hidden;
		&:disabled {
			cursor: not-allowed;
		}
		img {
			display: block;
			width: 100%;
			height: 100%;
		}
	}
}
:deep(.el-form-item__label-wrap) {
	align-items: center;
}
</style>
