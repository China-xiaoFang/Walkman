/**
 * Fast.Admin.Service.WeChat.Dto.WeChatCode2SessionInput 换取微信用户身份信息输入
 */
export interface WeChatCode2SessionInput {
  /**
   * 微信Code
   */
  code?: string;
  /**
   * 加密算法的初始向量
   */
  iv?: string;
  /**
   * 包括敏感数据在内的完整用户信息的加密数据
   */
  encryptedData?: string;
}

