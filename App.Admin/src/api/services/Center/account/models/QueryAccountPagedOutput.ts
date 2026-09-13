import type { CommonStatusEnum } from "@/api/enums/CommonStatusEnum";

/**
 * 获取账号分页列表输出
 */
export interface QueryAccountPagedOutput {
	/**
	 * 账号Id
	 */
	accountId?: string;
	/**
	 * 手机
	 */
	mobile?: string;
	/**
	 * 邮箱
	 */
	email?: string;
	/**
	 * 身份验证
	 */
	identityVerification?: boolean;
	/**
	 * 
	 */
	status?: CommonStatusEnum;
	/**
	 * 昵称
	 */
	nickName?: string;
	/**
	 * 头像
	 */
	avatar?: string;
	/**
	 * 初次登录租户
	 */
	firstLoginTenantName?: string;
	/**
	 * 初次登录设备
	 */
	firstLoginDevice?: string;
	/**
	 * 初次登录操作系统（版本）
	 */
	firstLoginOS?: string;
	/**
	 * 初次登录浏览器（版本）
	 */
	firstLoginBrowser?: string;
	/**
	 * 初次登录省份
	 */
	firstLoginProvince?: string;
	/**
	 * 初次登录城市
	 */
	firstLoginCity?: string;
	/**
	 * 初次登录Ip
	 */
	firstLoginIp?: string;
	/**
	 * 初次登录时间
	 */
	firstLoginTime?: string;
	/**
	 * 最后登录租户
	 */
	lastLoginTenantName?: string;
	/**
	 * 最后登录设备
	 */
	lastLoginDevice?: string;
	/**
	 * 最后登录操作系统（版本）
	 */
	lastLoginOS?: string;
	/**
	 * 最后登录浏览器（版本）
	 */
	lastLoginBrowser?: string;
	/**
	 * 最后登录省份
	 */
	lastLoginProvince?: string;
	/**
	 * 最后登录城市
	 */
	lastLoginCity?: string;
	/**
	 * 最后登录Ip
	 */
	lastLoginIp?: string;
	/**
	 * 最后登录时间
	 */
	lastLoginTime?: string;
	/**
	 * 密码错误次数
	 */
	passwordErrorTime?: number;
	/**
	 * 锁定开始时间
	 */
	lockStartTime?: string;
	/**
	 * 锁定结束时间
	 */
	lockEndTime?: string;
	/**
	 * 是否锁定
	 */
	isLock?: boolean;
	/**
	 * 创建时间
	 */
	createdTime?: string;
	/**
	 * 更新时间
	 */
	updatedTime?: string;
	/**
	 * 更新版本控制字段
	 */
	rowVersion?: string;
}

