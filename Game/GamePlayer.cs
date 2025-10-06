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

public static class GamePlayer
{
    static float _power = 0f;
    static uint _boom = 0;
    static uint _life = 0;

    public static float power
    {
        get { return _power; }
        set
        {
            float clampedValue = Mathf.Min(value, GameConfig.PLAYER_MAX_POWER);
            _power = clampedValue;
            GameObjCtrl.Instance.UpdatePlayerPower();
        }
    }
    public static uint boom
    {
        get { return _boom; }
        set
        {
            _boom = value;
            GameObjCtrl.Instance.UpdatePlayerBoom();
        }
    }
    public static uint life
    {
        get { return _life; }
        set
        {
            _life = value;
            GameObjCtrl.Instance.UpdatePlayerLife();
        }
    }

    public static PlayerUnitCtrl nowUnit { get; set; }
    public static bool isContinue { get; set; } = false;

    public static void TryRegister(TypeValue type, UnitCtrlBase unitCtrlBase)
    {
        if (type == TypeValue.玩家)
        {
            nowUnit = (PlayerUnitCtrl)unitCtrlBase;
        }
    }

    public static void TryUnRegister(UnitCtrlBase unitCtrlBase)
    {
        if (GamePlayer.nowUnit == unitCtrlBase)
        {
            GamePlayer.nowUnit = null;
        }
    }

    public static void SetDef()
    {
        power = GameConfig.PLAYER_BIRTH_POWER;
        boom = GameSelect.isPracticeMode ? 0 : GameConfig.PLAYER_BIRTH_BOOM;
        life = GameSelect.isPracticeMode ? 1 : GameConfig.PLAYER_BIRTH_LIFE;
    }

    public static void DeadCost()
    {
        life--;
        power -= 1f;
        if (power < PLAYER_BIRTH_POWER)
            power = PLAYER_BIRTH_POWER;
    }

    public static void GetPower()
    {
        power += GameConfig.PLAYER_EVERY_POWER_GET;
        power = Mathf.Round(power * 100f) / 100f;
    }

    public static void CoreActionRun()
    {
        GamePlayer.nowUnit.actCtrlDict[(uint)PlayerAct.UnInvinciblePlayerCtrl].isRun = false;
        GamePlayer.nowUnit.actCtrlDict[(uint)PlayerAct.Base].Act1_RunAndReset();
    }













}
