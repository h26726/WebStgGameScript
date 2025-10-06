using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static GameConfig;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using System.IO;

public abstract partial class UnitCtrlBase
{
    public UnitCtrlObj unitCtrlObj { get; set; }
    public enum UpdateFlag
    {
        None = 0,
        OutToInBorder = 1 << 0,
        InToOutBorder = 1 << 1,
        WaitDead = 1 << 2,
    }
    UpdateFlag updateFlag = UpdateFlag.None;
    public UnitPropBase unitProp { get; set; }


    public string externalPoolName { get; set; }
    public uint debutNo { get; set; }
    public uint endDeadAniTime { get; set; }



    public UnitCtrlBase(UnitCtrlObj unitCtrlObj)
    {
        this.unitCtrlObj = unitCtrlObj;
        Reset();
    }

    // public bool isRestoreQueue { get; set; }
    // public bool isDeadQueue { get; set; }
    public void Reset()
    {
        updateFlag |= UpdateFlag.None;
        uTime = 0;
        parentUnitCtrl = null;
        createStageSetting = null;
        actCtrlDict.Clear();
        if (unitProp == null)
            unitProp = UnitPropFactory.Create(this);
        unitProp.Reset();
        unitCtrlObj.Reset();
    }

    public UnitCtrlBase parentUnitCtrl { get; set; }
    public CreateStageSetting createStageSetting { get; set; }
    public Dictionary<uint, ActCtrl> actCtrlDict = new Dictionary<uint, ActCtrl>();
    // public UnitPropBase unitProp { get; set; }
    public SettingBase coreSetting { get { return createStageSetting.coreSetting; } }

    public Transform childFrame { get; set; }

    public uint uTime { get; set; }
    public bool isRun { get; set; }




    public void DebutUpdateHandler()
    {
        uTime++;
        if (updateFlag.HasFlag(UpdateFlag.OutToInBorder) && uTime % 10 == 0)
        {
            TryOutToInBorderIntoNext();
        }
        else if (updateFlag.HasFlag(UpdateFlag.InToOutBorder) && uTime % 10 == 0)
        {
            TryInToOutBorderRestore();
        }

        if (actCtrlDict != null && actCtrlDict.Count > 0)
        {
            unitCtrlObj.SetBeforePos();
            unitCtrlObj.actionTimeText.text = "";
            foreach (var actCtrlPair in actCtrlDict)
            {
                var actCtrl = actCtrlPair.Value;
                if (actCtrl.isRun)
                {
                    actCtrl.UpdateHandler();
                }
            }
        }
    }



    public void TryDeadUpdateHandler()
    {
        if (unitProp.isTriggerDead)
        {
            Unit3_TriggerDead_OnWaitAni();
        }
        else if (updateFlag.HasFlag(UpdateFlag.WaitDead) && GameReplay.keyPressTime == endDeadAniTime)
        {
            unitProp.isTriggerRestore = true;
            // DeadAnimEndHandle();
        }
    }

    public bool TryRestoreUpdateHandler()
    {
        if (unitProp.isTriggerRestore)
        {
            unitProp.isTriggerRestore = false;
            unitProp.isTriggerDead = false;
            Unit4_ResetAndRestore();
            return true;
        }
        return false;
    }

    public void Unit1_SetVal_OnOutToIn(CreateStageSetting createStageSetting, UnitCtrlBase parentUnitCtrl = null)
    {
        this.createStageSetting = createStageSetting;
        unitCtrlObj.Set_zIndex_zCode(createStageSetting.Id);

        this.actCtrlDict = ActCtrlFactory.CreateActCtrlDict(this);
        SetParent(parentUnitCtrl);


        if (this != GameBoss.nowUnit && this != GamePlayer.nowUnit && unitProp.restoreDistance < GameConfig.RESTORE_DISTANCE_MAX)
        {
            updateFlag |= UpdateFlag.OutToInBorder;
        }
        unitCtrlObj.EnableUnit();
        Unit2_TriggerCoreAct();
    }

    void SetParent(UnitCtrlBase parentUnitCtrl)
    {
        if (parentUnitCtrl == null)
            return;
        this.parentUnitCtrl = parentUnitCtrl;
        this.unitCtrlObj.SetParent(parentUnitCtrl.unitCtrlObj);
        this.unitProp.SetParent(parentUnitCtrl);
    }

    public void Unit2_TriggerCoreAct()
    {
        if (actCtrlDict.ContainsKey(coreSetting.Id))
        {
            unitProp.propWaitCallActs.Add((coreSetting.Id, coreSetting.Id, null));
            // actCtrlDict[coreSetting.Id].Act1_RunAndReset();
        }
        else
            Debug.LogError($"ActCtrlDict does not contain CoreSetting.Id: {coreSetting.Id}" +
                $"  ActCtrlDict.Count = {actCtrlDict.Count} ");
    }


    private void TryInToOutBorderRestore()
    {
        if (unitCtrlObj.IsOutBorder(unitProp.restoreDistance))
        {
            updateFlag &= ~UpdateFlag.InToOutBorder;
            unitProp.isTriggerRestore = true;
        }
    }

    private void TryOutToInBorderIntoNext()
    {
        if (!unitCtrlObj.IsOutBorder(unitProp.restoreDistance))
        {
            updateFlag &= ~UpdateFlag.OutToInBorder;
            updateFlag |= UpdateFlag.InToOutBorder;
        }
    }



    public virtual void Unit3_TriggerDead_OnWaitAni()
    {
        unitProp.isTriggerDead = false;
        unitProp.isAllowCollision = false;
        unitCtrlObj.PlayDeadAni();
        endDeadAniTime = GameReplay.keyPressTime + DEFAULT_DEADANI_KEY_TIME;
        updateFlag |= UpdateFlag.WaitDead;
        if (this == GamePlayer.nowUnit)
        {
            endDeadAniTime = GameReplay.keyPressTime + DEFAULT_PLAYER_DEADANI_KEY_TIME;
            unitProp.isInvincible = true;
            GamePlayer.nowUnit.playerCtrlObj.core.SetActive(false);
            GamePlayer.DeadCost();
        }
        else
        {
            actCtrlDict.Clear();
            unitCtrlObj.LeaveRelatParent();
        }
    }

    public void Unit4_ResetAndRestore()
    {
        if (this == GamePlayer.nowUnit)
        {
            if (GamePlayer.life == 0)
            {
                GamePlayer.nowUnit.HandlePlayerHpEmpty();
            }
            GamePlayer.CoreActionRun();
        }
        else
        {
            TriggerRestore();
        }
    }


    public static UnitCtrlBase GetOutSideUnit(uint id)
    {
        if ((IdVal)id == IdVal.Player)
        {
            return GamePlayer.nowUnit;
        }
        else if ((IdVal)id == IdVal.Boss)
        {
            return GameBoss.nowUnit;
        }
        else if (GameDebut.coreDictById.ContainsKey(id))
        {
            return GameDebut.coreDictById[id];
        }
        Debug.LogError("GetOutSideUnit id not exist:" + id);
        return null;
    }

    public void TriggerRestore()
    {
        // Debug.Log(nameof(TriggerRestore));
        GameDebut.AddQueueRestore(this);
    }


}


