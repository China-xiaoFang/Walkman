import { wdHookState } from "../index";
import type { ToastOptions } from "@wot-ui/ui/components/wd-toast/types";

export const useToast = {
	/**
	 * 显示提示
	 * @param options 选项
	 */
	show(options: ToastOptions | string): void {
		wdHookState.wdToast = {
			type: "show",
			options,
		};
	},
	/**
	 * 成功提示
	 * @param options 选项
	 */
	success(options: ToastOptions | string): void {
		wdHookState.wdToast = {
			type: "success",
			options,
		};
	},
	/**
	 * 错误提示
	 * @param options 选项
	 */
	error(options: ToastOptions | string): void {
		wdHookState.wdToast = {
			type: "error",
			options,
		};
	},
	/**
	 * 常规提示
	 * @param options 选项
	 */
	info(options: ToastOptions | string): void {
		wdHookState.wdToast = {
			type: "info",
			options,
		};
	},
	/**
	 * 警告提示
	 * @param options 选项
	 */
	warning(options: ToastOptions | string): void {
		wdHookState.wdToast = {
			type: "warning",
			options,
		};
	},
	/**
	 * 加载提示
	 * @param options 选项
	 */
	loading(options: ToastOptions | string): void {
		wdHookState.wdToast = {
			type: "loading",
			options,
		};
	},
	/**
	 * 关闭提示
	 */
	close(): void {
		wdHookState.wdToast = {
			type: "close",
			options: undefined,
		};
	},
};
