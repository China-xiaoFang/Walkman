/**
 * 编辑账号输入
 */
export interface EditAccountInput {
	/**
	 * 手机
	 */
	mobile?: string;
	/**
	 * 短信验证码
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
	/**
	 * 昵称
	 */
	nickName?: string;
	/**
	 * 头像
	 */
	avatar?: string;
	/**
	 * 
	 */
	rowVersion?: string;
}

