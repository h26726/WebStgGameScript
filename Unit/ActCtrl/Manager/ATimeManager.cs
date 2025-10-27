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
using static GameMainCtrl;

public class ATimeManager
{
    public SettingBase setting;
    public UnitCtrlObj unitCtrlObj;
    public bool isRun;
    public bool isMoveStop;
    public bool isBoss;
    public uint aTime;
    public uint coreSettingId;


    bool isNotSetActTime
    {
        get
        {
            return InvalidHelper.IsInvalid(setting.actTime);
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
    public ATimeManager()
    {
        Reset();
    }
    public void Set(ActCtrl actCtrl, bool isBoss)
    {
        this.isRun = true;
        this.isMoveStop = false;
        this.isBoss = isBoss;
        this.unitCtrlObj = actCtrl.unitCtrlObj;
        this.setting = actCtrl.setting;
        this.coreSettingId = actCtrl.coreSettingId;
        this.aTime = 0;
    }

    public void Reset()
    {
        this.isRun = false;
        this.isMoveStop = false;
        this.isBoss = false;
        this.unitCtrlObj = null;
        this.setting = null;
        this.coreSettingId = 0;
        this.aTime = 0;

    }
    public void UpdateAddCount()
    {
        if (setting == null)
            return;

        aTime++;

        // unitCtrlObj.AddPrintContent($"aTime:{aTime}   {Environment.NewLine}");
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
            GameMainCtrl.Instance.gameSceneUpdateFlag |= UpdateFlag.LateSpellEnd;
        }
        else if (setting.type == TypeValue.BOSSLEAVE)
        {
            GameBoss.nowUnit.enemyProp.isTriggerRestore = true;
            // GamePlayer.nowUnit.unitProp.isInvincible = false;
        }
    }



}

