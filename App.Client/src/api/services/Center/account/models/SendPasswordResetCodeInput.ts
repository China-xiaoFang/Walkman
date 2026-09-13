/**
 * 发送密码重置验证码输入
 */
export interface SendPasswordResetCodeInput {
	/**
	 * 账号
	 */
	account?: string;
	/**
	 * 图片验证码Key
	 */
	captchaKey?: string;
	/**
	 * 图片验证码
	 */
	captchaCode?: string;
}

