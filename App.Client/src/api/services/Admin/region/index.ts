import { axiosUtil } from "@fast-china/axios";

/**
 * Fast.Admin.Service.Region.RegionService 地区服务Api
 */
export const regionApi = {
  /**
   * 地区选择器
   */
  regionSelector() {
    return axiosUtil.request<ElSelectorOutput<number>[]>({
      url: "/region/regionSelector",
      method: "get",
      requestType: "query",
      cache: true,
    });
  },
  /**
   * 省份选择器
   */
  provinceSelector() {
    return axiosUtil.request<ElSelectorOutput<number>[]>({
      url: "/region/provinceSelector",
      method: "get",
      requestType: "query",
      cache: true,
    });
  },
  /**
   * 城市选择器
   */
  citySelector() {
    return axiosUtil.request<ElSelectorOutput<number>[]>({
      url: "/region/citySelector",
      method: "get",
      requestType: "query",
      cache: true,
    });
  },
};
