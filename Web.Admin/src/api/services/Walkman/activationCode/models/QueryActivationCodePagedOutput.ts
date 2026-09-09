/**
 * 获取激活码分页列表输出
 */
export interface QueryActivationCodePagedOutput {
	/**
	 * 激活码Id
	 */
	activationCodeId?: string;
	/**
	 * 激活码
	 */
	code?: string;
	/**
	 * 过期时间
	 */
	expireTime?: string;
	/**
	 * 客户端用户Id
	 */
	userId?: string;
	/**
	 * 激活时间
	 */
	activationTime?: string;
	/**
	 * 创建者用户名称
	 */
	createdUserName?: string;
	/**
	 * 创建时间
	 */
	createdTime?: string;
	/**
	 * 更新版本控制字段
	 */
	rowVersion?: string;
}

