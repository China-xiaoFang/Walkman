import { dayjs } from "element-plus";
import FastElementPlus from "fast-element-plus";
import type { App } from "vue";
import "dayjs/locale/zh-cn";

/** 加载 Element Plus */
export async function loadElementPlus(app: App): Promise<void> {
	dayjs.locale("zh-cn");

	/** Fast Element Plus 组件完整引入 */
	app.use(FastElementPlus);
}
