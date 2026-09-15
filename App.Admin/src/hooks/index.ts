import { reactive } from "vue";
import { withDefineType } from "@fast-china/utils";
import type { Dialog, DialogOptions, DialogResult } from "@wot-ui/ui/components/wd-dialog/types";
import type { NotifyProps } from "@wot-ui/ui/components/wd-notify/types";
import type { Toast, ToastOptions } from "@wot-ui/ui/components/wd-toast/types";

export * from "./use-loading";
export * from "./use-message-box";
export * from "./use-notify";
export * from "./use-overlay";
export * from "./use-paging";
export * from "./use-toast";
export * from "./use-update";

/** Wot Ui Hooks 共享状态 */
export const wdHookState = reactive({
	/** 加载状态 */
	loading: {
		/** 状态 @default false */
		state: false,
		/** 加载文字 @default "加载中..." */
		text: "加载中...",
		/** 全屏Loading，和默认Loading不一样 @default false */
		fullscreen: false,
	},
	/** 遮罩层 */
	overlay: {
		/** 状态 @default false */
		state: false,
		/** 透明度（0 ~ 1） @default 0 */
		transparent: 0,
	},
	/** Notify 通知 */
	wdNotify: {
		/** 通知操作类型 */
		type: withDefineType<"showNotify" | "closeNotify">(),
		/** 通知配置或通知内容 */
		options: withDefineType<NotifyProps | string>(),
	},
	/** Toast 消息 */
	wdToast: {
		/** Toast 操作类型 */
		type: withDefineType<keyof Toast>(),
		/** Toast 配置或消息内容 */
		options: withDefineType<ToastOptions | string>(),
	},
	/** Dialog 消息弹窗 */
	wdMessageBox: {
		/** 弹窗操作类型 */
		type: withDefineType<keyof Dialog>(),
		/** 弹窗配置 */
		options: withDefineType<DialogOptions>(),
		/** 弹窗操作成功后的回调 */
		then: withDefineType<(res?: DialogResult) => void>(),
		/** 弹窗操作失败或取消后的回调 */
		catch: withDefineType<(error?: unknown) => void>(),
	},
});
