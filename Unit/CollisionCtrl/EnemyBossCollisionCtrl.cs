using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Text;
using System.Linq;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static SaveJsonData;
using static GameConfig;
using static GameMainCtrl;

public class EnemyBossCollisionCtrl : EnemyCollisionCtrl
{

    public override void EnemyHpChangeHandler(EnemyUnitProp enemyUnitProp, PlayerShotUnitProp playerShotUnitProp)
    {
        GameObjCtrl.Instance.UpdateBossHpLine();
        if (enemyUnitProp.hp == 0)
        {
            GameMainCtrl.Instance.gameSceneUpdateFlag |= UpdateFlag.LateSpellEnd;
        }

    }






}
