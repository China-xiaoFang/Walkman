/**
 * 密码重置输入
 */
export interface PasswordResetInput {
	/**
	 * 验证Key
	 */
	verificationKey?: string;
	/**
	 * 验证码
	 */
	verificationCode?: string;
	/**
	 * 新密码
	 */
	newPassword?: string;
	/**
	 * 确认密码
	 */
	confirmPassword?: string;
}

