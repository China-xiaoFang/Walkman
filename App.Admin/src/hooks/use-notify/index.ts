import { wdHookState } from "../index";
import type { NotifyProps } from "@wot-ui/ui/components/wd-notify/types";

export const useNotify = {
	/**
	 * 显示消息通知
	 * @param options 选项
	 */
	showNotify(options: NotifyProps | string): void {
		wdHookState.wdNotify = {
			type: "showNotify",
			options,
		};
	},
	/**
	 * 关闭消息通知
	 */
	closeNotify(): void {
		wdHookState.wdNotify = {
			type: "closeNotify",
			options: undefined,
		};
	},
};
