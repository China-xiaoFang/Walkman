/**
 * 登录图片验证码输出
 */
export interface LoginCaptchaOutput {
	/**
	 * 是否启用
	 */
	enabled?: boolean;
	/**
	 * 验证码Key
	 */
	captchaKey?: string;
	/**
	 * 验证码图片
	 */
	captchaImage?: string;
}

