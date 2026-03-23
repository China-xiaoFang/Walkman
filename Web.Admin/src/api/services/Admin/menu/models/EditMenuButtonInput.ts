import { RoleTypeEnum } from "@/api/enums/RoleTypeEnum";
import { CommonStatusEnum } from "@/api/enums/CommonStatusEnum";

/**
 * Fast.Admin.Service.Menu.Dto.EditMenuButtonInput 编辑菜单按钮输入
 */
export interface EditMenuButtonInput {
  /**
   * 按钮Id
   */
  buttonId?: number;
  /**
   * 按钮编码
   */
  buttonCode?: string;
  /**
   * 按钮名称
   */
  buttonName?: string;
  /**
   * 
   */
  roleType?: RoleTypeEnum;
  /**
   * 是否桌面端
   */
  hasDesktop?: boolean;
  /**
   * 是否Web端
   */
  hasWeb?: boolean;
  /**
   * 是否移动端
   */
  hasMobile?: boolean;
  /**
   * 排序
   */
  sort?: number;
  /**
   * 
   */
  status?: CommonStatusEnum;
}

