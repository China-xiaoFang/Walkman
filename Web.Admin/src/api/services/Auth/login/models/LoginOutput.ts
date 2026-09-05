import type { LoginTenantOutput } from "./LoginTenantOutput";
import type { LoginStatusEnum } from "@/api/enums/LoginStatusEnum";

/**
 * 登录输出
 */
export interface LoginOutput {
	/**
	 * 
	 */
	status?: LoginStatusEnum;
	/**
	 * 消息
	 */
	message?: string;
	/**
	 * 账号Key
	 */
	accountKey?: string;
	/**
	 * 昵称
	 */
	nickName?: string;
	/**
	 * 头像
	 */
	avatar?: string;
	/**
	 * 选择租户的一次性租户登录凭据
	 */
	loginTicket?: string;
	/**
	 * 租户集合
	 */
	tenantList?: LoginTenantOutput[];
}

