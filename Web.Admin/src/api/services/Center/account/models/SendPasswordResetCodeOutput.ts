/**
 * 发送密码重置验证码输出
 */
export interface SendPasswordResetCodeOutput {
	/**
	 * 验证Key
	 */
	verificationKey?: string;
	/**
	 * 消息
	 */
	message?: string;
}

