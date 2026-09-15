import { wdHookState } from "../index";
import type { DialogOptions, DialogResult } from "@wot-ui/ui/components/wd-dialog/types";

const defaultTitle = "温馨提示";

export const useMessageBox = {
	/**
	 * 显示弹框
	 * @param options 选项
	 */
	show(options: DialogOptions | string): Promise<DialogResult> {
		return new Promise<DialogResult>((resolve, reject) => {
			wdHookState.wdMessageBox = {
				type: "show",
				options: typeof options === "string" ? { title: defaultTitle, msg: options } : { ...options, title: options.title ?? defaultTitle },
				then: (res) => resolve(res),
				catch: (error) => reject(error),
			};
		});
	},
	/**
	 * Alert 弹框
	 * @param options 选项
	 */
	alert(options: DialogOptions | string): Promise<DialogResult> {
		return new Promise<DialogResult>((resolve, reject) => {
			wdHookState.wdMessageBox = {
				type: "alert",
				options: typeof options === "string" ? { title: defaultTitle, msg: options } : { ...options, title: options.title ?? defaultTitle },
				then: (res) => resolve(res),
				catch: (error) => reject(error),
			};
		});
	},
	/**
	 * Confirm 弹框
	 * @param options 选项
	 */
	confirm(options: DialogOptions | string): Promise<DialogResult> {
		return new Promise<DialogResult>((resolve, reject) => {
			wdHookState.wdMessageBox = {
				type: "confirm",
				options: typeof options === "string" ? { title: defaultTitle, msg: options } : { ...options, title: options.title ?? defaultTitle },
				then: (res) => resolve(res),
				catch: (error) => reject(error),
			};
		});
	},
	/**
	 * Prompt 弹框
	 * @param options 选项
	 */
	prompt(options: DialogOptions | string): Promise<DialogResult> {
		return new Promise<DialogResult>((resolve, reject) => {
			wdHookState.wdMessageBox = {
				type: "prompt",
				options: typeof options === "string" ? { title: defaultTitle, msg: options } : { ...options, title: options.title ?? defaultTitle },
				then: (res) => resolve(res),
				catch: (error) => reject(error),
			};
		});
	},
	/**
	 * 关闭弹框
	 */
	close(): void {
		wdHookState.wdMessageBox = {
			type: "close",
			options: undefined,
			then: undefined,
			catch: undefined,
		};
	},
};
