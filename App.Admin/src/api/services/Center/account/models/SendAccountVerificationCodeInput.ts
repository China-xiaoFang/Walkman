/**
 * 发送账号校验验证码输入
 */
export interface SendAccountVerificationCodeInput {
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

