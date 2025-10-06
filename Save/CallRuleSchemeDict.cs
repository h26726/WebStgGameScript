using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static GameReplay;

public static class CallRuleSchemeDict
{
    [Serializable]
    public class CallRuleSchemeDictEntry
    {
        public uint key;
        public List<CallRuleScheme> value;
    }


    [Serializable]
    public class CallRuleSchemeDictWrapper
    {
        public List<CallRuleSchemeDictEntry> entries = new List<CallRuleSchemeDictEntry>();
    }

    public static CallRuleSchemeDictWrapper ConvertDictToWrapper(Dictionary<uint, List<CallRuleScheme>> dict)
    {
        var wrapper = new CallRuleSchemeDictWrapper();
        foreach (var kv in dict)
        {
            wrapper.entries.Add(new CallRuleSchemeDictEntry
            {
                key = kv.Key,
                value = kv.Value
            });
        }
        return wrapper;
    }

    public static Dictionary<uint, List<CallRuleScheme>> ConvertWrapperToDict(CallRuleSchemeDictWrapper wrapper)
    {
        var dict = new Dictionary<uint, List<CallRuleScheme>>();
        foreach (var entry in wrapper.entries)
        {
            dict[entry.key] = entry.value;
        }
        return dict;
    }
}
