import { createApp } from "vue";
import App from "./App.vue";
import { loadDirectives } from "./directives";
import { loadPlugins } from "./plugins";
import router from "./router";
import { loadPinia, useApp } from "./stores";
import { checkVersionUpdate } from "./updateVersion";
import "./styles/index.scss";
import "vue-json-pretty/lib/styles.css";
import "element-plus/dist/index.css";
import "element-plus/theme-chalk/dark/css-vars.css";
import "fast-element-plus/styles/index.scss";

const app = createApp(App);

// 注册持久化存储
loadPinia(app);

/** 加载插件 */
loadPlugins(app);

/** Launch */
await useApp().launch();

/** 加载路由 */
app.use(router);

/** 加载自定义指令 */
loadDirectives(app);

app.mount("#app");

checkVersionUpdate(`v${import.meta.env.VITE_APP_VERSION}`);
