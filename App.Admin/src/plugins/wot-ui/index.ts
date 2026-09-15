import { inputProps } from "@wot-ui/ui/components/wd-input/types";
import { textareaProps } from "@wot-ui/ui/components/wd-textarea/types";

/** 加载 wot-ui */
export function loadWotUi(): void {
	/** 默认显示字数统计 */
	inputProps.showWordLimit.default = true;
	/** 默认显示字数统计 */
	textareaProps.showWordLimit.default = true;
}
