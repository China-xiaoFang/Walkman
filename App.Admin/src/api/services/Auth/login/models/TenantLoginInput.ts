/**
 * 租户登录输入
 */
export interface TenantLoginInput {
	/**
	 * 账号Key
	 */
	accountKey?: string;
	/**
	 * 用户Key
	 */
	userKey?: string;
	/**
	 * 密码
	 */
	password?: string;
	/**
	 * 登录凭据
	 */
	loginTicket?: string;
	/**
	 * 图片验证码Key
	 */
	captchaKey?: string;
	/**
	 * 图片验证码
	 */
	captchaCode?: string;
}

