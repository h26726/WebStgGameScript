using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;
using System.Xml;
using System.IO;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static PlayerSaveData;
using static GameConfig;
using System.Linq;
using static LoadCtrl;

public static class GameBoss
{
    public static EnemyUnitCtrl nowUnit;
    public static uint MaxHp = DEFAULT_ENEMY_HP;
    public static uint SpellTime = DEFAULT_SPELL_TIME;
    public static void TryRegister(TypeValue type, UnitCtrlBase unitCtrlBase)
    {
        if (type == TypeValue.BOSS)
        {
            GameBoss.nowUnit = (EnemyUnitCtrl)unitCtrlBase;
        }
    }

    public static void TryUnRegister(UnitCtrlBase unitCtrlBase)
    {
        if (GameBoss.nowUnit == unitCtrlBase)
        {
            GameBoss.nowUnit = null;
        }
    }
    public static void SetDef()
    {
        nowUnit = null;
    }











}
