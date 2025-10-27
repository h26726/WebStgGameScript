using UnityEngine;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static GameConfig;
using static PlayerKeyHelper;
using static SaveJsonData;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public class CallRuleCollection
{
    public List<CallRuleScheme> callRuleSchemes;
    public CallRule[] callRules;
    public uint useCount;
    // List<CallRuleScheme> callRuleSchemes;
    public CallRuleCollection()
    {
        callRuleSchemes = new List<CallRuleScheme>();
        callRules = new CallRule[20];
        for (int i = 0; i < callRules.Length; i++)
        {
            callRules[i] = new CallRule();
        }
        useCount = 0;
    }
    public void Set(ActCtrl actCtrl)
    {
        //取得轉換後排序後的參數
        SetCallRuleScheme(actCtrl);
        useCount = (uint)callRuleSchemes.Count;
        if (useCount == 0)
            return;

        if (useCount > callRules.Length)
        {
            int oldLength = callRules.Length;
            Array.Resize(ref callRules, (int)useCount);

            for (int i = oldLength; i < useCount; i++)
            {
                callRules[i] = new CallRule();
            }
        }

        for (int i = 0; i < useCount; i++)
        {
            callRules[i].Set(actCtrl, callRuleSchemes[i]);
        }

        // 僅排序使用中的元素範圍
        Array.Sort(callRules, 0, (int)useCount, CallRuleComparer);
    }

    public void Reset()
    {
        for (int i = 0; i < useCount; i++)
        {
            callRules[i].Reset();
        }
        useCount = 0;
        callRuleSchemes.Clear();
    }


    public void SetCallRuleScheme(ActCtrl actCtrl)
    {
        SetCallRuleSchemeById(actCtrl);
        SetCallRuleSchemeByAddId(actCtrl);
    }
    public void SetCallRuleSchemeById(ActCtrl actCtrl)
    {
        SetCallRuleScheme(actCtrl.Id);
    }

    public void SetCallRuleSchemeByAddId(ActCtrl actCtrl)
    {

        var addIds = actCtrl.setting.addIds;
        if (addIds != null)
        {
            for (int i = 0; i < addIds.Count; i++)
            {
                var addId = addIds[i];
                SetCallRuleScheme(addId);
            }
        }

    }


    public void SetCallRuleScheme(uint Id)
    {
        // Debug.Log("SetCallRuleScheme:" + Id);
        if (GameSelect.callRuleSchemesById.TryGetValue(Id, out var insertCallRuleSchemes))
        {
            callRuleSchemes.AddRange(insertCallRuleSchemes);
            // if (Id == 171011)
            // {
            //     foreach (var item in insertCallRuleSchemes)
            //     {
            //         Debug.Log(item.Print());
            //     }
            // }
            // Debug.Log("insertCallRuleSchemes:" + insertCallRuleSchemes.Count);
        }

    }





    static readonly Comparer<CallRule> CallRuleComparer = Comparer<CallRule>.Create((x, y) =>
    {
        var a = x.callATime;
        var b = y.callATime;

        if (InvalidHelper.IsInvalid(a) && InvalidHelper.IsInvalid(b)) return 0;
        if (InvalidHelper.IsInvalid(a)) return 1;   // null 視為最大 → 排最後
        if (InvalidHelper.IsInvalid(b)) return -1;
        return a.CompareTo(b);
    });
}

