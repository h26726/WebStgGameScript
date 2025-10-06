using UnityEngine;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static GameConfig;
using static PlayerKeyHelper;
using static PlayerSaveData;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public static class CallRuleFactory
{
    public static List<CallRule> CreateCallRules(ActCtrl actCtrl)
    {
        //取得轉換後排序後的參數
        var callRules = new List<CallRule>();
        var callParamList = GetCallRuleScheme(actCtrl);
        if (callParamList == null || callParamList.Count == 0)
            return callRules;
        foreach (var callParam in callParamList)
        {
            callRules.Add(new CallRule(actCtrl, callParam));
        }
        //依時間排序
        callRules.Sort((x, y) =>
        {
            if (x.callATime == null && y.callATime == null)
                return 0;
            if (x.callATime == null)
                return 1;
            if (y.callATime == null)
                return -1;
            return x.callATime.Value.CompareTo(y.callATime.Value);
        });
        return callRules;
    }

    public static List<CallRuleScheme> GetCallRuleScheme(ActCtrl actCtrl)
    {
        var Id = actCtrl.Id;
        var callParamList = new List<CallRuleScheme>();
        if (GameSelect.callRuleSchemeById.TryGetValue(Id, out var baseParams))
        {
            callParamList.AddRange(baseParams);
        }
        foreach (var addId in actCtrl.setting.addIds)
        {
            if (GameSelect.callRuleSchemeById.TryGetValue(addId, out var addParams))
            {
                callParamList.AddRange(addParams);
            }
        }
        return callParamList;
    }
}

