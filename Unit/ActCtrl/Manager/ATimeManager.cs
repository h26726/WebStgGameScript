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
using static GameMainCtrl;

public class ATimeManager
{
    public SettingBase setting;
    public UnitCtrlObj unitCtrlObj;
    public bool isRun = false;
    public bool isMoveStop = false;
    public bool isBoss = false;
    public uint aTime = 0;
    public uint coreSettingId;


    bool isNotSetActTime
    {
        get
        {
            return setting.actTime == null;
        }
    }

    bool isArriveSetActTime
    {
        get
        {
            return aTime == setting.actTime;
        }
    }

    bool isOverSetActTime
    {
        get
        {
            return aTime == setting.actTime + 1;
        }
    }
    public ATimeManager(ActCtrl actCtrl, bool isBoss)
    {
        this.isRun = true;
        this.isMoveStop = false;
        this.isBoss = isBoss;
        this.unitCtrlObj = actCtrl.unitCtrlObj;
        this.setting = actCtrl.setting;
        this.coreSettingId = actCtrl.coreSettingId;
    }
    public void UpdateAddCount()
    {
        if (setting == null)
            return;

        aTime++;

        unitCtrlObj.AddPrintContent($"aTime:{aTime}   {Environment.NewLine}");
        if (isNotSetActTime || isMoveStop)
            return;
        if (isArriveSetActTime)
        {
            if (setting.Id != coreSettingId)
            {
                isRun = false;
            }
            if (isBoss)
            {
                TryBossActEnd(setting);
            }
        }
        else if (isOverSetActTime)
        {
            isMoveStop = true;
        }
    }


    void TryBossActEnd(SettingBase setting)
    {
        if (setting.type == TypeValue.BOSS || setting.type == TypeValue.復位)
        {
            GameMainCtrl.Instance.gameSceneUpdateFlag |= UpdateFlag.WaitSpellEnd;
        }
        else if (setting.type == TypeValue.BOSSLEAVE)
        {
            GameBoss.nowUnit.enemyProp.isTriggerRestore = true;
            GamePlayer.nowUnit.unitProp.isInvincible = false;
        }
    }



}

