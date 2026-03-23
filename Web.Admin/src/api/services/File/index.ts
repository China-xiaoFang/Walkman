import { axiosUtil } from "@fast-china/axios";
import { PagedInput, PagedResult } from "fast-element-plus";
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
      url: "/fileStorage/download",
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
  uploadLogo(data: FormData) {
    return axiosUtil.request<string>({
      url: "/fileStorage/uploadLogo",
      method: "post",
      data,
      cancelDuplicateRequest: false,
      requestType: "upload",
    });
  },
  /**
   * 上传头像
   */
  uploadAvatar(data: FormData) {
    return axiosUtil.request<string>({
      url: "/fileStorage/uploadAvatar",
      method: "post",
      data,
      cancelDuplicateRequest: false,
      requestType: "upload",
    });
  },
  /**
   * 上传证件照
   */
  uploadIdPhoto(data: FormData) {
    return axiosUtil.request<string>({
      url: "/fileStorage/uploadIdPhoto",
      method: "post",
      data,
      cancelDuplicateRequest: false,
      requestType: "upload",
    });
  },
  /**
   * 上传富文本
   */
  uploadEditor(data: FormData) {
    return axiosUtil.request<string>({
      url: "/fileStorage/uploadEditor",
      method: "post",
      data,
      cancelDuplicateRequest: false,
      requestType: "upload",
    });
  },
  /**
   * 上传文件
   */
  uploadFile(data: FormData) {
    return axiosUtil.request<string>({
      url: "/fileStorage/uploadFile",
      method: "post",
      data,
      cancelDuplicateRequest: false,
      requestType: "upload",
    });
  },
};
