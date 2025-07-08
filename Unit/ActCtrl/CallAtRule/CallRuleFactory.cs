using UnityEngine;
using static CommonData;
using static CommonFunc;
using static GameConfig;
using static PlayerKeyCtrl;
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
        if (GameSystem.Instance.callRuleSchemeById.TryGetValue(Id, out var baseParams))
        {
            actCtrl.unitCtrlData.AddPrintContent($"baseParams {Environment.NewLine}");
            foreach (var item in baseParams)
            {
                actCtrl.unitCtrlData.AddPrintContent(item.Print());
            }
            callParamList.AddRange(baseParams);
        }
        foreach (var addId in actCtrl.stageSetting.addIds)
        {
            if (GameSystem.Instance.callRuleSchemeById.TryGetValue(addId, out var addParams))
            {
                actCtrl.unitCtrlData.AddPrintContent($"addParams {Environment.NewLine}");
                foreach (var item in addParams)
                {
                    actCtrl.unitCtrlData.AddPrintContent(item.Print());
                }
                callParamList.AddRange(addParams);
            }
        }
        return callParamList;
    }
}

