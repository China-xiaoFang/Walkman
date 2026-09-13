/**
 * 账号校验输入
 */
export interface AccountVerificationInput {
	/**
	 * 手机
	 */
	mobile?: string;
	/**
	 * 手机号验证码
	 */
	mobileVerificationCode?: string;
	/**
	 * 邮箱
	 */
	email?: string;
	/**
	 * 邮箱验证码
	 */
	emailVerificationCode?: string;
}

