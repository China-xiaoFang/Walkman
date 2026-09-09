/**
 * Http请求方式枚举
 */
export enum HttpRequestMethodEnum {
	/**
	 * 未知请求
	 */
	Unknown = 0,
	/**
	 * Get请求
	 */
	Get = 1,
	/**
	 * Post请求
	 */
	Post = 2,
	/**
	 * Put请求
	 */
	Put = 4,
	/**
	 * Delete请求
	 */
	Delete = 8,
	/**
	 * Patch请求
	 */
	Patch = 16,
	/**
	 * Head请求
	 */
	Head = 32,
	/**
	 * Options请求
	 */
	Options = 64,
	/**
	 * Connect请求
	 */
	Connect = 128,
	/**
	 * Trace请求
	 */
	Trace = 256,
}
