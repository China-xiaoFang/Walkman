import { SchedulerJobGroupEnum } from "@/api/enums/Scheduler/SchedulerJobGroupEnum";
import { SchedulerJobTypeEnum } from "@/api/enums/Scheduler/SchedulerJobTypeEnum";
import { TriggerTypeEnum } from "@/api/enums/Scheduler/TriggerTypeEnum";
import { WeekEnum } from "@/api/enums/Scheduler/WeekEnum";
import { MailMessageEnum } from "@/api/enums/Scheduler/MailMessageEnum";
import { HttpRequestMethodEnum } from "@/api/enums/Scheduler/HttpRequestMethodEnum";

/**
 * Fast.Scheduler.EditSchedulerJobInput 编辑调度作业输入
 */
export interface EditSchedulerJobInput {
  /**
   * 旧的作业名称
   */
  oldJobName?: string;
  /**
   * 
   */
  oldJobGroup?: SchedulerJobGroupEnum;
  /**
   * 作业名称
   */
  jobName?: string;
  /**
   * 
   */
  jobGroup?: SchedulerJobGroupEnum;
  /**
   * 
   */
  jobType?: SchedulerJobTypeEnum;
  /**
   * 开始时间
   */
  beginTime?: Date;
  /**
   * 结束时间
   */
  endTime?: Date;
  /**
   * 
   */
  triggerType?: TriggerTypeEnum;
  /**
   * Cron表达式
   */
  cron?: string;
  /**
   * 
   */
  week?: WeekEnum;
  /**
   * 每天开始时间
   */
  dailyStartTime?: any;
  /**
   * 每天结束时间
   */
  dailyEndTime?: any;
  /**
   * 执行间隔时间，单位秒
   */
  intervalSecond?: number;
  /**
   * 执行次数（默认无限循环）
   */
  runTimes?: number;
  /**
   * 警告秒数
   */
  warnTime?: number;
  /**
   * 重试次数
   */
  retryTimes?: number;
  /**
   * 重试间隔，单位毫秒
   */
  retryMillisecond?: number;
  /**
   * 
   */
  mailMessage?: MailMessageEnum;
  /**
   * 描述
   */
  description?: string;
  /**
   * 请求Url
   */
  requestUrl?: string;
  /**
   * 
   */
  requestMethod?: HttpRequestMethodEnum;
  /**
   * 请求超时时间，单位秒（默认不超时）
   */
  requestTimeout?: number;
  /**
   * 请求参数
   */
  requestParams?: any;
  /**
   * 请求头部
   */
  requestHeader?: any;
}

