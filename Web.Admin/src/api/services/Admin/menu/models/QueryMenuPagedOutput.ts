import { MenuTypeEnum } from "@/api/enums/MenuTypeEnum";
import { RoleTypeEnum } from "@/api/enums/RoleTypeEnum";
import { CommonStatusEnum } from "@/api/enums/CommonStatusEnum";

/**
 * Fast.Admin.Service.Menu.Dto.QueryMenuPagedOutput 获取菜单列表输出
 */
export interface QueryMenuPagedOutput {
  /**
   * 菜单Id
   */
  menuId?: number;
  /**
   * 菜单编码
   */
  menuCode?: string;
  /**
   * 菜单名称
   */
  menuName?: string;
  /**
   * 菜单标题
   */
  menuTitle?: string;
  /**
   * 父级Id
   */
  parentId?: number;
  /**
   * 
   */
  menuType?: MenuTypeEnum;
  /**
   * 
   */
  roleType?: RoleTypeEnum;
  /**
   * 是否桌面端
   */
  hasDesktop?: boolean;
  /**
   * 桌面端图标
   */
  desktopIcon?: string;
  /**
   * 桌面端路由地址
   */
  desktopRouter?: string;
  /**
   * 是否Web端
   */
  hasWeb?: boolean;
  /**
   * Web端图标
   */
  webIcon?: string;
  /**
   * Web端路由地址
   */
  webRouter?: string;
  /**
   * Web端组件地址
   */
  webComponent?: string;
  /**
   * Web端页面是否在导航栏显示
   */
  webTab?: boolean;
  /**
   * Web端页面是否缓存
   */
  webKeepAlive?: boolean;
  /**
   * 是否移动端
   */
  hasMobile?: boolean;
  /**
   * 移动端图标
   */
  mobileIcon?: string;
  /**
   * 移动端路由地址
   */
  mobileRouter?: string;
  /**
   * 内链/外链地址
   */
  link?: string;
  /**
   * 是否显示
   */
  visible?: boolean;
  /**
   * 排序
   */
  sort?: number;
  /**
   * 
   */
  status?: CommonStatusEnum;
  /**
   * 子节点
   */
  children?: Array<QueryMenuPagedOutput>;
  /**
   * 
   */
  departmentName?: string;
  /**
   * 
   */
  createdUserName?: string;
  /**
   * 
   */
  createdTime?: Date;
  /**
   * 
   */
  updatedUserName?: string;
  /**
   * 
   */
  updatedTime?: Date;
  /**
   * 
   */
  rowVersion?: number;
}

