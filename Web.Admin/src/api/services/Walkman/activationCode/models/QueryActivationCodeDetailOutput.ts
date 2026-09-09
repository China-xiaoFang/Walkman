/**
 * 获取激活码详情输出
 */
export interface QueryActivationCodeDetailOutput {
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
	 * 手机
	 */
	mobile?: string;
	/**
	 * 唯一用户标识
	 */
	openId?: string;
	/**
	 * 昵称
	 */
	nickName?: string;
	/**
	 * 头像
	 */
	avatar?: string;
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

