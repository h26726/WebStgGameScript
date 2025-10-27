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
using static SaveJsonData;
using static GameConfig;
using System.Linq;
using static LoadCtrl;

public static class GameBoss
{
    public static EnemyBossUnitCtrl nowUnit;
    public static uint MaxHp;
    public static uint SpellTime;
    public static void TryRegister(TypeValue type, UnitCtrlBase unitCtrlBase)
    {
        if (type == TypeValue.BOSS)
        {
            nowUnit = (EnemyBossUnitCtrl)unitCtrlBase;
        }
    }

    public static void TryUnRegister(UnitCtrlBase unitCtrlBase)
    {
        if (nowUnit == unitCtrlBase)
        {
            nowUnit = null;
        }
    }
    public static void Reset()
    {
        nowUnit = null;
        MaxHp = 0;
        SpellTime = 0;
    }











}
