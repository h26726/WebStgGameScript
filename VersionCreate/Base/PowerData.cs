using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static SaveJsonData;
using System.Linq;
using System.IO;

[Serializable]
public class PowerData
{
    public void Set(string version, string fileName)
    {
        var powerXmlStageSettings = XmlStageSettingBuilder.BuildByReadFileName(version, fileName);
        CreateDataBuilder.Build(
            powerXmlStageSettings, out powerCreateStageSettings, out _, out powerCallRuleSchemeById);
    }
    public List<CreateStageSetting> powerCreateStageSettings = new List<CreateStageSetting>();
    public List<CallRuleScheme> powerCallRuleSchemeById = new List<CallRuleScheme>();
}