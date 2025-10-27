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
    public string externalPoolName { get; set; }
    public enum UpdateFlag
    {
        None = 0,
        OutToInBorder = 1 << 0,
        InToOutBorder = 1 << 1,
        WaitDeadAni = 1 << 2,
    }
    public uint Id { get; set; }
    public uint uTime { get; set; }
    public uint debutNo { get; set; }
    public uint endDeadAniTime { get; set; }
    protected UpdateFlag updateFlag;

    public ActCtrl[] actCtrls;
    public uint actCtrlsUseCount;
    public Dictionary<uint, ActCtrl> actCtrlDict;
    public CollisionCtrlBase collisionCtrl { get; set; }
    public UnitCtrlObj unitCtrlObj { get; set; }
    public UnitPropBase unitProp { get; set; }
    public UnitCtrlBase parentUnitCtrl { get; set; }
    public CreateStageSetting createStageSetting { get; set; }
    // public UnitPropBase unitProp { get; set; }
    public SettingBase coreSetting { get { return createStageSetting.coreSetting; } }








    public UnitCtrlBase(UnitCtrlObj unitCtrlObj)
    {
        this.unitCtrlObj = unitCtrlObj;
        actCtrls = new ActCtrl[10];
        for (int i = 0; i < actCtrls.Length; i++)
        {
            actCtrls[i] = new ActCtrl();
        }
        unitProp = UnitPropFactory.Create(this);
        collisionCtrl = unitCtrlObj.collisionCtrl;
        collisionCtrl.Init(this);
        actCtrlDict = new Dictionary<uint, ActCtrl>();
        Reset();
    }

    // public bool isRestoreQueue { get; set; }
    // public bool isDeadQueue { get; set; }
    public void Reset()
    {
        Id = 0;
        debutNo = uint.MaxValue;
        uTime = 0;
        endDeadAniTime = 0;
        updateFlag = UpdateFlag.None;
        parentUnitCtrl = null;
        createStageSetting = null;
        actCtrlDict.Clear();
        unitProp.Reset();
        unitCtrlObj.Reset();
        for (int i = 0; i < actCtrlsUseCount; i++)
        {
            actCtrls[i].Reset();
        }
        actCtrlsUseCount = 0;

        if (this == GamePlayer.nowUnit)
        {
            GamePlayer.nowUnit.PlayerReset();
        }
    }

    public (uint debutNo, uint powerGiveNum) KeepData()
    {
        return (debutNo, !InvalidHelper.IsInvalid(coreSetting.powerGive) ? coreSetting.powerGive : 0);
    }






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
        collisionCtrl.UpdateHandler();
    }



    public void TryDeadUpdateHandler()
    {
        if (unitProp.isTriggerDead)
        {
            Unit3_TriggerDead_OnWaitAni();
        }
        else if (updateFlag.HasFlag(UpdateFlag.WaitDeadAni) && GameReplay.keyPressTime == endDeadAniTime)
        {
            endDeadAniTime = 0;
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
        SetActCtrlsAndDict();
        SetParent(parentUnitCtrl);


        if (CheckEnableBorderRestore())
        {
            Debug.Log("CheckEnableBorderRestore");
            updateFlag |= UpdateFlag.OutToInBorder;
        }
        unitCtrlObj.EnableUnit();
        Unit2_TriggerCoreAct();
    }

    bool CheckEnableBorderRestore()
    {
        if (this == GameBoss.nowUnit)
            return false;
        if (this == GamePlayer.nowUnit)
            return false;
        if (!InvalidHelper.IsInvalid(unitProp.restoreDistance) && unitProp.restoreDistance > GameConfig.RESTORE_DISTANCE_MAX)
            return false;
        return true;
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
            unitProp.propLateCallActs.Add((coreSetting.Id, coreSetting.Id, null));
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
        updateFlag |= UpdateFlag.WaitDeadAni;
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

    void SetActCtrlsAndDict()
    {
        var coreActCtrl = actCtrls[0];
        coreActCtrl.Set(this, coreSetting);
        actCtrlDict[coreActCtrl.Id] = coreActCtrl;
        actCtrlsUseCount++;
        for (int i = 0; i < createStageSetting.actionSettingList.Count; i++)
        {
            var setting = createStageSetting.actionSettingList[i];
            var actCtrl = actCtrls[actCtrlsUseCount];
            actCtrl.Set(this, setting);
            actCtrlDict[actCtrl.Id] = actCtrl;
            actCtrlsUseCount++;
        }
    }


}


