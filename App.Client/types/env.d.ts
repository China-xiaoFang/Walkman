/// <reference types="vite/client" />

/** 声明 vite 环境变量的类型（如果未声明则默认是 any） */
declare interface ImportMetaEnv {
	/**
	 * 环境
	 */
	readonly VITE_ENV: "production" | "development" | "test" | "staging";
	/**
	 * 接口请求地址
	 */
	readonly VITE_AXIOS_REQUEST_URL: string;
	/**
	 * 接口请求时间
	 */
	readonly VITE_AXIOS_REQUEST_TIMEOUT: number;
	/**
	 * 接口请求加密
	 */
	readonly VITE_AXIOS_REQUEST_ENCIPHER: "true" | "false";
	/**
	 * 接口版本
	 */
	readonly VITE_AXIOS_API_VERSION: string;
	/**
	 * 本地缓存加密
	 */
	readonly VITE_STORAGE_CRYPTO: "true" | "false";
	/**
	 * APP 来源
	 */
	readonly VITE_APP_ORIGIN: string;
}
