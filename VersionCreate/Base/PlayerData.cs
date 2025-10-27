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
public class PlayerData
{
    public void Set(string version, string playerFileName)
    {
        var playerXmlStageSettings = XmlStageSettingBuilder.BuildByReadFileName(version, playerFileName);
        CreateDataBuilder.Build(
            playerXmlStageSettings, out var playerAndShotCreateStageSettings, out _, out playerCallRuleSchemeById);
        playerCreateStageSetting = playerAndShotCreateStageSettings.Where(r => r.coreSetting.type == TypeValue.玩家).FirstOrDefault();
        if (playerCreateStageSetting == null)
        {
            Debug.LogError($"Player Create Stage Setting not found for {playerFileName}");
        }
        Id = playerCreateStageSetting.Id;
        playerShotCreateStageSettings = playerAndShotCreateStageSettings.Where(r => r.coreSetting.type == TypeValue.玩家子彈).ToList();
        if (playerShotCreateStageSettings.Count == 0)
        {
            Debug.LogError($"Player Shot Create Stage Setting not found for {playerFileName}");
        }

        if (playerCallRuleSchemeById.Count == 0)
        {
            Debug.LogError($"Player Call Rule Scheme not found for {playerFileName}");
        }

    }
    public uint Id;
    public CreateStageSetting playerCreateStageSetting = new CreateStageSetting();
    public List<CreateStageSetting> playerShotCreateStageSettings = new List<CreateStageSetting>();
    public List<CallRuleScheme> playerCallRuleSchemeById = new List<CallRuleScheme>();
}