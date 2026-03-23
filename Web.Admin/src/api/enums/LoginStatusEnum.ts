/**
 * Fast.Admin.Service.Login.Dto.LoginStatusEnum 登录状态枚举
 */
export enum LoginStatusEnum {
  /**
   * 登录成功
   */
  Success = 1,
  /**
   * 授权过期
   */
  AuthExpired = 2,
  /**
   * 无账号
   */
  NotAccount = 4,
}
