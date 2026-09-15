import { wdHookState } from "../index";

export const useLoading = {
	/**
	 * 显示
	 * @param options 选项
	 */
	show(options?: { text?: string; fullscreen?: boolean } | string): void {
		if (typeof options === "string") {
			wdHookState.loading = {
				state: true,
				text: options,
				fullscreen: false,
			};
		} else {
			wdHookState.loading = {
				state: true,
				text: options?.text,
				fullscreen: options?.fullscreen,
			};
		}
	},
	/**
	 * 隐藏
	 */
	hide(): void {
		if (wdHookState.loading?.state) {
			setTimeout(() => {
				wdHookState.loading = undefined;
			}, 500);
		}
	},
};
