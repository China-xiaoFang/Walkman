/**
 * Fast.Admin.Service.File.Dto.QueryFilePagedOutput 获取文件分页列表输出
 */
export interface QueryFilePagedOutput {
  /**
   * 文件Id
   */
  fileId?: number;
  /**
   * 文件唯一标识
   */
  fileObjectName?: string;
  /**
   * 文件名称（上传时候的文件名）
   */
  fileOriginName?: string;
  /**
   * 文件后缀
   */
  fileSuffix?: string;
  /**
   * 文件Mime类型
   */
  fileMimeType?: string;
  /**
   * 文件大小kb
   */
  fileSizeKb?: number;
  /**
   * 存储路径
   */
  filePath?: string;
  /**
   * 访问地址
   */
  fileLocation?: string;
  /**
   * 文件哈希
   */
  fileHash?: string;
  /**
   * 上传设备
   */
  uploadDevice?: string;
  /**
   * 上传操作系统（版本）
   */
  uploadOS?: string;
  /**
   * 上传浏览器（版本）
   */
  uploadBrowser?: string;
  /**
   * 上传省份
   */
  uploadProvince?: string;
  /**
   * 上传城市
   */
  uploadCity?: string;
  /**
   * 上传Ip
   */
  uploadIp?: string;
  /**
   * 创建者用户名称
   */
  createdUserName?: string;
  /**
   * 创建时间
   */
  createdTime?: Date;
}

