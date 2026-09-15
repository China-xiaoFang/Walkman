<template>
	<wd-popup
		v-bind="wdPopupProps"
		:custom-class="`fa-popup ${props.customClass}`"
		:custom-style="`${state.style} ${props.customStyle}`"
		v-model="state.visible"
		:transition="state.transition"
		safe-area-inset-bottom
		@before-enter="() => emit('beforeEnter')"
		@enter="() => emit('enter')"
		@after-enter="() => emit('afterEnter')"
		@before-leave="() => emit('beforeLeave')"
		@leave="() => emit('leave')"
		@after-leave="() => emit('afterLeave')"
		@click-modal="() => emit('clickModal')"
		@close="handleClose"
	>
		<slot />
		<FaLoading mask :loading="state.loading" />
	</wd-popup>
</template>

<script setup lang="ts">
import { computed, nextTick, reactive } from "vue";
import { addCssUnit, callOptionalFunction, definePropType, logger, useProps } from "@fast-china/utils";
import { popupProps } from "@wot-ui/ui/components/wd-popup/types";
import FaLoading from "../loading/index.vue";

defineOptions({
	name: "Popup",
	options: {
		virtualHost: true,
		addGlobalClass: true,
		styleIsolation: "shared",
	},
});

const props = defineProps({
	...popupProps,
	/** @description 宽度 */
	width: [String, Number],
	/** @description 高度 */
	height: [String, Number],
	/** @description 显示关闭回调 */
	showBeforeClose: Boolean,
	/** @description 打开之后 */
	afterOpen: {
		type: definePropType<() => void>(Function),
	},
});

const wdPopupProps = useProps(props, popupProps);

const emit = defineEmits<{
	/** @description 进入前触发 */
	beforeEnter: [];
	/** @description 进入时触发 */
	enter: [];
	/** @description 进入后触发 */
	afterEnter: [];
	/** @description 离开前触发 */
	beforeLeave: [];
	/** @description 离开时触发 */
	leave: [];
	/** @description 离开后触发 */
	afterLeave: [];
	/** @description 点击遮罩时触发 */
	clickModal: [];
	/** @description 弹出层打开时触发 */
	open: [];
	/** @description 弹出层关闭时触发 */
	close: [];
}>();

const state = reactive({
	loading: false,
	visible: false,
	style: computed(() => {
		let result = "";
		if (props.width) {
			result += `width: ${addCssUnit(props.width)};`;
		}
		if (props.height) {
			result += `height: ${addCssUnit(props.height)};`;
		}

		return result;
	}),
	transition: computed(() => {
		switch (props.position) {
			case "center":
				return "zoom-in";
			case "top":
				return "fade-down";
			case "bottom":
				return "fade-up";
			case "left":
				return "fade-left";
			case "right":
				return "fade-right";
			default:
				return "fade";
		}
	}),
});

const handleOpen = (openFunction?: () => void | Promise<void>) => {
	state.visible = true;
	nextTick(() => {
		state.loading = true;
		callOptionalFunction(props.afterOpen ?? openFunction)
			.then(() => {
				emit("open");
			})
			.catch((error: unknown) => {
				logger.error("FaPopup", error);
				// 自动关闭
				state.visible = false;
			})
			.finally(() => {
				state.loading = false;
			});
	});
};

const handleClose = (closeFunction?: () => void | Promise<void>) => {
	state.loading = true;
	callOptionalFunction(closeFunction)
		.then(() => {
			emit("close");
			state.visible = false;
		})
		.catch((error: unknown) => {
			logger.error("FaPopup", error);
		})
		.finally(() => {
			state.loading = false;
		});
};

const handleLoading = (loadingFunction: () => void | Promise<void>) => {
	state.loading = true;
	callOptionalFunction(loadingFunction)
		.then()
		.catch((error: unknown) => {
			logger.error("FaPopup", error);
		})
		.finally(() => {
			state.loading = false;
		});
};

defineExpose({
	/** @description 加载状态 */
	loading: computed(() => state.loading),
	/** @description 是否显示 */
	visible: computed(() => state.visible),
	/** @description 打开弹窗 */
	open: handleOpen,
	/** @description 关闭弹窗 */
	close: handleClose,
	/** @description 弹窗加载 */
	doLoading: handleLoading,
});
</script>
