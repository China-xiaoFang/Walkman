/**
 * Fast.Admin.Service.Login.Dto.WeChatClientLoginInput 微信客户端登录输入
 */
export interface WeChatClientLoginInput {
  /**
   * 微信Code
   */
  weChatCode?: string;
  /**
   * 加密算法的初始向量
   */
  iv?: string;
  /**
   * 包括敏感数据在内的完整用户信息的加密数据
   */
  encryptedData?: string;
  /**
   * 动态令牌
   */
  code?: string;
}

