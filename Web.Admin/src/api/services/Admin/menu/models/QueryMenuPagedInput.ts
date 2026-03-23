import { PagedInput } from "fast-element-plus";
import { MenuTypeEnum } from "@/api/enums/MenuTypeEnum";
import { CommonStatusEnum } from "@/api/enums/CommonStatusEnum";

/**
 * Fast.Admin.Service.Menu.Dto.QueryMenuPagedInput 获取菜单列表输入
 */
export interface QueryMenuPagedInput extends PagedInput  {
  /**
   * 
   */
  menuType?: MenuTypeEnum;
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
   * 是否显示
   */
  visible?: boolean;
  /**
   * 
   */
  status?: CommonStatusEnum;
  /**
   * 
   */
  readonly isOrderBy?: boolean;
}

