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
using System.Collections.Generic;

public abstract class UnitPropBase
{
    public UnitCtrlObj unitCtrlObj;
    public UnitPropBase parent;
    public float restoreDistance { get; set; } = DEFAULT_RESTORE_DIS;

    public bool isAllowCollision { get; set; } = false;
    public bool isThrough { get; set; } = false;
    public bool isInvincible { get; set; } = false;
    public bool rotateIsMoveAngle { get; set; } = false;

    public float moveAngle { get; set; } = 0f;
    public float speed { get; set; } = 0f;
    public List<UnitPropBase> relatChildProps { get; set; } = new List<UnitPropBase>();
    public List<CreateStageSetting> propWaitDebutByCreateSettings = new List<CreateStageSetting>();
    public List<(uint coreId, uint actId, ActionProp actionProp)> propWaitCallActs = new List<(uint coreId, uint actId, ActionProp actionProp)>();


    private bool _isTriggerDead;
    public bool isTriggerDead
    {
        get => _isTriggerDead;
        set
        {
            _isTriggerDead = value;
            if (value) isDead = true;
        }
    }

    private bool _isTriggerRestore;
    public bool isTriggerRestore
    {
        get => _isTriggerRestore;
        set
        {
            _isTriggerRestore = value;
            isDead = value;
        }
    }

    public bool isDead { get; private set; }


    public UnitPropBase(UnitCtrlBase unitCtrl)
    {
        unitCtrlObj = unitCtrl.unitCtrlObj;
    }

    public void SetParent(UnitCtrlBase parentUnitCtrl)
    {
        parent = parentUnitCtrl.unitProp;
    }

    public virtual void Reset()
    {
        parent = null;
        restoreDistance = DEFAULT_RESTORE_DIS;
        isAllowCollision = false;
        isThrough = false;
        isInvincible = false;
        rotateIsMoveAngle = false;
        moveAngle = 0f;
        speed = 0f;
        isDead = false;
        isTriggerDead = false;
        isTriggerRestore = false;
        relatChildProps.Clear();
        propWaitDebutByCreateSettings.Clear();
        propWaitCallActs.Clear();
    }



    public virtual void RefreshVal(SettingBase setting)
    {
        if (setting.restoreDistance != null)
        {
            restoreDistance = setting.restoreDistance.Value;
        }
        if (setting.rotateIsMoveAngle != null)
        {
            rotateIsMoveAngle = setting.rotateIsMoveAngle.Value;
        }

        if (setting.isInvincible != null)
        {
            isInvincible = setting.isInvincible.Value;
        }

        if (setting.isThrough != null)
        {
            isThrough = setting.isThrough.Value;
        }
    }




    public UnitPropBase GetUnitProp(uint Id)
    {
        if ((IdVal)Id == IdVal.Self)
        {
            return this;
        }
        if ((IdVal)Id == IdVal.Parent)
        {
            return parent;
        }
        else
        {
            return GetOutSideUnitProp(Id);
        }
    }

    public UnitPropBase GetOutSideUnitProp(uint Id)
    {
        if ((IdVal)Id == IdVal.Player)
        {
            return GamePlayer.nowUnit.unitProp;
        }
        else if ((IdVal)Id == IdVal.Boss)
        {
            return GameBoss.nowUnit.unitProp;
        }
        else
        {
            var unitCtrl = UnitCtrlBase.GetOutSideUnit(Id);
            if (unitCtrl != null)
                return unitCtrl.unitProp;
        }
        return null;
    }

    string PrintSettingVal(SettingBase setting)
    {
        string printContent = $"[UP]UnitPropBase settingId: {setting.Id}  {Environment.NewLine}";
        printContent += $"--[UP]RestoreDistance: {restoreDistance}  {Environment.NewLine}";
        printContent += $"--[UP]isThrough: {isThrough}  {Environment.NewLine}";
        printContent += $"--[UP]isInvincible: {isInvincible}  {Environment.NewLine}";
        printContent += $"--[UP]rotateIsMoveAngle: {rotateIsMoveAngle}  {Environment.NewLine}";
        printContent += $"--{Environment.NewLine}";
        return printContent;
    }


}

