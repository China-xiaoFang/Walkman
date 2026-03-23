// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using System.Collections.Concurrent;
using Fast.Admin.Entity;
using Fast.Admin.Enum;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.Core;

/// <summary>
/// <see cref="SerialContext"/> 系统序号规则上下文
/// </summary>
[SuppressSniffer]
public class SerialContext
{
    /// <summary>
    /// 线程锁
    /// </summary>
    private static readonly object _lock = new();

    /// <summary>
    /// 请求作用域系统序号规则缓存
    /// </summary>
    private static readonly AsyncLocal<ConcurrentDictionary<SerialRuleTypeEnum, SerialRuleModel>> SerialRuleRequestAsyncLocal =
        new();

    /// <summary>
    /// 系统序号规则缓存
    /// </summary>
    private static ConcurrentDictionary<SerialRuleTypeEnum, SerialRuleModel> SerialRuleList
    {
        get
        {
            SerialRuleRequestAsyncLocal.Value ??= new ConcurrentDictionary<SerialRuleTypeEnum, SerialRuleModel>();
            return SerialRuleRequestAsyncLocal.Value;
        }
    }

    /// <summary>
    /// 请求作用域系统序号配置缓存
    /// </summary>
    private static readonly AsyncLocal<ConcurrentDictionary<SerialRuleTypeEnum, SerialSettingModel>>
        SerialSettingRequestAsyncLocal = new();

    /// <summary>
    /// 系统序号配置缓存
    /// </summary>
    private static ConcurrentDictionary<SerialRuleTypeEnum, SerialSettingModel> SerialSettingList
    {
        get
        {
            SerialSettingRequestAsyncLocal.Value ??= new ConcurrentDictionary<SerialRuleTypeEnum, SerialSettingModel>();
            return SerialSettingRequestAsyncLocal.Value;
        }
    }

    /// <summary>
    /// 生成工号
    /// </summary>
    /// <param name="db"><see cref="ISqlSugarClient"/> SqlSugar上下文</param>
    /// <returns></returns>
    public static string GenEmployeeNo(ISqlSugarClient db)
    {
        return GenerateSerialNo(db, SerialRuleTypeEnum.EmployeeNo);
    }

    /// <summary>
    /// 生成序号
    /// </summary>
    /// <param name="db"><see cref="ISqlSugarClient"/> SqlSugar上下文</param>
    /// <param name="ruleType"><see cref="SerialRuleTypeEnum"/> 系统序号规则类型</param>
    /// <returns></returns>
    private static string GenerateSerialNo(ISqlSugarClient db, SerialRuleTypeEnum ruleType)
    {
        if (!db.Ado.IsAnyTran())
        {
            throw new SqlSugarException("请保证当前上下文在事务中，才能正确的调用此方法！");
        }

        var dateTime = DateTime.Now;

        // 获取序号规则配置
        var SerialRuleModel = SerialRuleList.GetOrAdd(ruleType, key =>
        {
            var result = db.Queryable<SerialRuleModel>()
                .Where(wh => wh.RuleType == key)
                .Single();

            if (result == null)
            {
                throw new UserFriendlyException($"未能找到【{key.GetDescription()}】规则配置！");
            }

            return result;
        });

        // 获取序号配置
        var SerialSettingModel = SerialSettingList.GetOrAdd(ruleType, key =>
        {
            return db.Queryable<SerialSettingModel>()
                .Where(wh => wh.RuleType == key)
                .Single();
        });

        lock (_lock)
        {
            if (SerialSettingModel == null)
            {
                SerialSettingModel = new SerialSettingModel
                {
                    SerialSettingId = YitIdHelper.NextId(),
                    RuleType = ruleType,
                    LastSerial = null,
                    LastSerialNo = null,
                    LastTime = null
                };
                SerialSettingModel = db.Insertable(SerialSettingModel)
                    .ExecuteReturnEntity();
            }

            // 分隔符
            var spacer = SerialRuleModel.Spacer switch
            {
                SerialSpacerEnum.None => "",
                SerialSpacerEnum.Underscore => "_",
                SerialSpacerEnum.Hyphen => "-",
                SerialSpacerEnum.Dot => ".",
                _ => ""
            };

            var curSerialNo = $"{SerialRuleModel.Prefix ?? ""}{spacer}";

            var lastSerial = SerialSettingModel.LastSerial.GetValueOrDefault();

            switch (SerialRuleModel.DateType)
            {
                default:
                case SerialDateTypeEnum.Year:
                    curSerialNo += dateTime.ToString("yyyy");
                    if (SerialSettingModel.LastTime == null || SerialSettingModel.LastTime.Value.Year != dateTime.Year)
                    {
                        lastSerial = 0;
                    }

                    break;
                case SerialDateTypeEnum.Month:
                    curSerialNo += dateTime.ToString("yyyyMM");
                    if (SerialSettingModel.LastTime == null
                        || SerialSettingModel.LastTime.Value.Year != dateTime.Year
                        || SerialSettingModel.LastTime.Value.Month != dateTime.Month)
                    {
                        lastSerial = 0;
                    }

                    break;
                case SerialDateTypeEnum.Day:
                    curSerialNo += dateTime.ToString("yyyyMMdd");
                    if (SerialSettingModel.LastTime == null || SerialSettingModel.LastTime.Value.Date != dateTime.Date)
                    {
                        lastSerial = 0;
                    }

                    break;
                case SerialDateTypeEnum.Hour:
                    curSerialNo += dateTime.ToString("yyyyMMddHH");
                    if (SerialSettingModel.LastTime == null
                        || SerialSettingModel.LastTime.Value.Year != dateTime.Year
                        || SerialSettingModel.LastTime.Value.Month != dateTime.Month
                        || SerialSettingModel.LastTime.Value.Day != dateTime.Day
                        || SerialSettingModel.LastTime.Value.Hour != dateTime.Hour)
                    {
                        lastSerial = 0;
                    }

                    break;
            }

            // 拼接分隔符
            curSerialNo += spacer;

            var curSerial = lastSerial + 1;

            // 组装左侧的0
            var serialLength = $"D{SerialRuleModel.Length}";
            curSerialNo += curSerial.ToString(serialLength);

            SerialSettingModel.LastSerial = curSerial;
            SerialSettingModel.LastSerialNo = curSerialNo;
            SerialSettingModel.LastTime = dateTime;


            SerialSettingModel = db.Updateable(SerialSettingModel)
                .UpdateColumns(e => new {e.LastSerial, e.LastSerialNo, e.LastTime})
                .ExecuteReturnEntity();
            SerialSettingList.AddOrUpdate(ruleType, SerialSettingModel, (_, _) => SerialSettingModel);

            return curSerialNo;
        }
    }
}