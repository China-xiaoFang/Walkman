import { axiosUtil } from "@fast-china/axios";
import { QuerySchedulerDetailOutput } from "./models/QuerySchedulerDetailOutput";
import { SchedulerJobKeyInput } from "./models/SchedulerJobKeyInput";
import { QueryAllSchedulerJobOutput } from "./models/QueryAllSchedulerJobOutput";
import { SchedulerJobGroupEnum } from "@/api/enums/Scheduler/SchedulerJobGroupEnum";
import { SchedulerJobInfo } from "./models/SchedulerJobInfo";
import { AddSchedulerJobInput } from "./models/AddSchedulerJobInput";
import { EditSchedulerJobInput } from "./models/EditSchedulerJobInput";

/**
 * Fast.Scheduler.Applications.SchedulerApplication 调度作业Api
 */
export const schedulerApi = {
  /**
   * 运行并验证Cron表达式
   */
  runVerifyCron(cron: string) {
    return axiosUtil.request<string[]>({
      url: "/scheduler/runVerifyCron",
      method: "get",
      params: {
        cron,
      },
      requestType: "other",
    });
  },
  /**
   * 获取调度器详情
   */
  querySchedulerDetail() {
    return axiosUtil.request<QuerySchedulerDetailOutput>({
      url: "/scheduler/querySchedulerDetail",
      method: "get",
      requestType: "query",
    });
  },
  /**
   * 启动调度器
   */
  startScheduler() {
    return axiosUtil.request({
      url: "/scheduler/startScheduler",
      method: "post",
      requestType: "other",
    });
  },
  /**
   * 停止调度器
   */
  stopScheduler() {
    return axiosUtil.request({
      url: "/scheduler/stopScheduler",
      method: "post",
      requestType: "other",
    });
  },
  /**
   * 暂停调度作业
   */
  stopSchedulerJob(data: SchedulerJobKeyInput) {
    return axiosUtil.request({
      url: "/scheduler/stopSchedulerJob",
      method: "post",
      data,
      requestType: "other",
    });
  },
  /**
   * 恢复调度作业
   */
  resumeSchedulerJob(data: SchedulerJobKeyInput) {
    return axiosUtil.request({
      url: "/scheduler/resumeSchedulerJob",
      method: "post",
      data,
      requestType: "other",
    });
  },
  /**
   * 立即执行调度作业
   */
  triggerSchedulerJob(data: SchedulerJobKeyInput) {
    return axiosUtil.request({
      url: "/scheduler/triggerSchedulerJob",
      method: "post",
      data,
      requestType: "other",
    });
  },
  /**
   * 获取调度作业日志
   */
  querySchedulerJobLogs(data: SchedulerJobKeyInput) {
    return axiosUtil.request<string[]>({
      url: "/scheduler/querySchedulerJobLogs",
      method: "post",
      data,
      requestType: "query",
    });
  },
  /**
   * 获取调度作业运行次数
   */
  querySchedulerJobRunNumber(data: SchedulerJobKeyInput) {
    return axiosUtil.request<number>({
      url: "/scheduler/querySchedulerJobRunNumber",
      method: "post",
      data,
      requestType: "query",
    });
  },
  /**
   * 获取全部调度作业
   */
  queryAllSchedulerJob(jobGroup: SchedulerJobGroupEnum) {
    return axiosUtil.request<QueryAllSchedulerJobOutput[]>({
      url: "/scheduler/queryAllSchedulerJob",
      method: "get",
      params: {
        jobGroup,
      },
      requestType: "query",
    });
  },
  /**
   * 获取调度作业
   */
  querySchedulerJob(data: SchedulerJobKeyInput) {
    return axiosUtil.request<SchedulerJobInfo>({
      url: "/scheduler/querySchedulerJob",
      method: "post",
      data,
      requestType: "query",
    });
  },
  /**
   * 添加调度作业
   */
  addSchedulerJob(data: AddSchedulerJobInput) {
    return axiosUtil.request({
      url: "/scheduler/addSchedulerJob",
      method: "post",
      data,
      requestType: "add",
    });
  },
  /**
   * 编辑调度作业
   */
  editSchedulerJob(data: EditSchedulerJobInput) {
    return axiosUtil.request({
      url: "/scheduler/editSchedulerJob",
      method: "post",
      data,
      requestType: "edit",
    });
  },
  /**
   * 删除调度作业
   */
  deleteSchedulerJob(data: SchedulerJobKeyInput) {
    return axiosUtil.request({
      url: "/scheduler/deleteSchedulerJob",
      method: "post",
      data,
      requestType: "delete",
    });
  },
  /**
   * 移除调度作业异常信息
   */
  deleteSchedulerJobException(data: SchedulerJobKeyInput) {
    return axiosUtil.request({
      url: "/scheduler/deleteSchedulerJobException",
      method: "post",
      data,
      requestType: "delete",
    });
  },
};
