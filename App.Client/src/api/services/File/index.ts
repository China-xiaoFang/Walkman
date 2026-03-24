import { axiosUtil } from "@fast-china/axios";
import { QueryFilePagedOutput } from "./models/QueryFilePagedOutput";
import { DownloadFileInput } from "./models/DownloadFileInput";

/**
 * Fast.Admin.Service.File.FileService 文件服务Api
 */
export const fileApi = {
  /**
   * 获取文件分页列表
   */
  queryFilePaged(data: PagedInput) {
    return axiosUtil.request<PagedResult<QueryFilePagedOutput>>({
      url: "/file/queryFilePaged",
      method: "post",
      data,
      requestType: "query",
    });
  },
  /**
   * 下载文件
   */
  download(data: DownloadFileInput) {
    return axiosUtil.request({
      url: "/file/download",
      method: "post",
      data,
      responseType: "blob",
      autoDownloadFile: true,
      requestType: "download",
    });
  },
  /**
   * 上传Logo
   */
  uploadLogo(filePath: string) {
    return axiosUtil.request<string>({
      url: "/file/uploadLogo",
      method: "upload",
      name: "file",
      filePath,
      cancelDuplicateRequest: false,
      requestType: "upload",
    });
  },
  /**
   * 上传头像
   */
  uploadAvatar(filePath: string) {
    return axiosUtil.request<string>({
      url: "/file/uploadAvatar",
      method: "upload",
      name: "file",
      filePath,
      cancelDuplicateRequest: false,
      requestType: "upload",
    });
  },
  /**
   * 上传证件照
   */
  uploadIdPhoto(filePath: string) {
    return axiosUtil.request<string>({
      url: "/file/uploadIdPhoto",
      method: "upload",
      name: "file",
      filePath,
      cancelDuplicateRequest: false,
      requestType: "upload",
    });
  },
  /**
   * 上传富文本
   */
  uploadEditor(filePath: string) {
    return axiosUtil.request<string>({
      url: "/file/uploadEditor",
      method: "upload",
      name: "file",
      filePath,
      cancelDuplicateRequest: false,
      requestType: "upload",
    });
  },
  /**
   * 上传文件
   */
  uploadFile(filePath: string) {
    return axiosUtil.request<string>({
      url: "/file/uploadFile",
      method: "upload",
      name: "file",
      filePath,
      cancelDuplicateRequest: false,
      requestType: "upload",
    });
  },
};
