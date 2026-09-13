/**
 * 登录输入
 */
export interface LoginInput {
	/**
	 * 账号
	 */
	account?: string;
	/**
	 * 密码
	 */
	password?: string;
	/**
	 * 图片验证码Key
	 */
	captchaKey?: string;
	/**
	 * 图片验证码
	 */
	captchaCode?: string;
}

