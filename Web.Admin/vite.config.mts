import { resolve } from "path";
import vue from "@vitejs/plugin-vue";
import vueJsx from "@vitejs/plugin-vue-jsx";
import { buildRouterPath, buildSvgIcon, cdnImport, componentAutoImport, versionUpdatePlugin } from "fast-vite-plugins";
// import { visualizer } from "rollup-plugin-visualizer";
import { loadEnv } from "vite";
import viteCompression from "vite-plugin-compression";
import { getCdnModules } from "./vite.cdn";
import { rollupManualChunks } from "./vite.rollup";
import type { ConfigEnv, ProxyOptions, UserConfig } from "vite";

const pathResolve = (dir: string): any => {
	return resolve(__dirname, ".", dir);
};

/** 配置项文档：https://cn.vitejs.dev/config */
const ViteConfig = ({ mode }: ConfigEnv): UserConfig => {
	const viteEnv = loadEnv(mode, process.cwd()) as ImportMetaEnv;
	const viteDev = viteEnv.DEV || viteEnv.VITE_ENV === "development";

	// 配置别名
	const alias: Record<string, string> = {
		"@": pathResolve("./src"),
		assets: pathResolve("./src/assets"),
	};

	// 配置代理
	let proxy: Record<string, string | ProxyOptions> = {};
	if (viteEnv.VITE_AXIOS_PROXY_URL) {
		proxy = {
			"/api": {
				target: viteEnv.VITE_AXIOS_PROXY_URL,
				ws: true,
				/** 是否允许跨域 */
				changeOrigin: true,
				// rewrite: (path: string) => path.replace(/^\/api/, ""),
				rewrite: (path: string) => path,
			},
		};
	}

	return {
		root: process.cwd(),
		resolve: { alias },
		/** 打包时根据实际情况修改 base */
		base: viteEnv.VITE_BASE_PATH,
		server: {
			/** 设置 host: true 才可以使用 Network 的形式，以 IP 访问项目 */
			host: true,
			// host: "0.0.0.0"
			/** 端口号 */
			port: viteEnv.VITE_PORT,
			/** 是否自动打开浏览器 */
			open: false,
			/** 跨域设置允许 */
			cors: true,
			/** 端口被占用时，是否直接退出 */
			strictPort: false,
			/** 接口代理 */
			proxy,
		},
		build: {
			/** 消除打包大小超过 500kb 警告，不建议使用 */
			chunkSizeWarningLimit: 2000,
			/** Vite 2.6.x 以上需要配置 minify: "terser", terserOptions 才能生效 */
			minify: "terser",
			// esbuild 打包更快，但是不能去除 console.log，terser打包慢，但能去除 console.log
			// minify: "terser",
			/** 在打包代码时移除 console.log、debugger 和 注释 */
			terserOptions: {
				compress: {
					/** 移除所有 console.* 调用 */
					drop_console: !viteDev,
					/** 移除 debugger 语句 */
					drop_debugger: !viteDev,
					/** 仅移除 console.log */
					pure_funcs: viteDev ? [] : ["console.log"],
				},
				format: {
					/** 删除注释 */
					comments: !viteDev,
					/** 格式化代码 */
					beautify: !viteDev,
				},
				/** 混淆变量名 */
				mangle: !viteDev,
			},
			/** 关闭文件计算，禁用 gzip 压缩大小报告，可略微减少打包时间 */
			reportCompressedSize: false,
			/** 启用/禁用 CSS 代码拆分 */
			cssCodeSplit: false,
			/** 关闭生成 map 文件，可以达到缩小打包体积 */
			sourcemap: false,
			/** 打包输出路径 */
			outDir: viteEnv.VITE_OUT_DIR,
			/** 打包的时候清空目录 */
			emptyOutDir: true,
			/** 0kb，小于此阈值的导入或引用资源将内联为 base64 编码 */
			assetsInlineLimit: 0,
			/** 静态资源打包处理 */
			rollupOptions: {
				output: {
					// 在 UMD 构建模式下为这些外部化的依赖提供一个全局变量
					globals: {},
					chunkFileNames: "js/[name]-[hash].js",
					entryFileNames: "js/[name]-[hash].js",
					assetFileNames(chunkInfo) {
						if (chunkInfo.name.endsWith(".css")) {
							return "[ext]/[name]-[hash].[ext]";
						} else {
							return "[ext]/[name].[ext]";
						}
					},
					manualChunks(id) {
						return rollupManualChunks(id);
					},
				},
			},
		},
		/** Vite 插件 */
		plugins: [
			vue(),
			// vue 可以使用 jsx/tsx 语法
			vueJsx(),
			/** 本地 SVG 图标 */
			buildSvgIcon("src/assets/icons/", "src/icons"),
			/** 组件自动导入 */
			componentAutoImport(),
			/** 路由路径 */
			buildRouterPath(),
			/** 版本号 */
			versionUpdatePlugin(viteEnv.VITE_APP_VERSION),
			/** gzip静态资源压缩 */
			viteCompression({
				// 记录压缩文件及其压缩率。默认true
				verbose: true,
				// 是否禁用压缩，默认false
				disable: false,
				// 删除源文件
				deleteOriginFile: false,
				// 需要使用压缩前的最小文件大小，单位字节（byte） b，1b(字节)=8bit(比特), 1KB=1024B
				threshold: 1024 * 10,
				// 压缩算法 可选 'gzip' | 'brotliCompress' | 'deflate' | 'deflateRaw'
				algorithm: "gzip",
				// 压缩后的文件格式
				ext: ".gz",
				// 排除 index.html 文件的压缩
				filter: (file) => !file.endsWith("index.html"),
			}),
			cdnImport({
				// 开发环境使用 CDN
				enableInDevMode: false,
				prodUrl: viteEnv.VITE_CDN_URL,
				modules: getCdnModules(viteDev),
			}),
			// visualizer({
			// 	filename: "analysis.html",
			// 	open: true,
			// }),
		],
	};
};

export default ViteConfig;
