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
using System.Collections.Generic;

public abstract class UnitPropBase
{

    public float restoreDistance { get; set; }

    public bool isAllowCollision { get; set; }
    public bool isThrough { get; set; }
    public bool isInvincible { get; set; }
    public bool rotateIsMoveAngle { get; set; }

    public float moveAngle { get; set; }
    public float speed { get; set; }
    public UnitPropBase parent;
    public List<UnitPropBase> relatChildProps { get; set; }
    public List<CreateStageSetting> propLateDebutByCreateSettings { get; set; }
    public List<(uint coreId, uint actId, ActionProp actionProp)> propLateCallActs { get; set; }

    public UnitCtrlObj unitCtrlObj;
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
        relatChildProps = new List<UnitPropBase>();
        propLateDebutByCreateSettings = new List<CreateStageSetting>();
        propLateCallActs = new List<(uint coreId, uint actId, ActionProp actionProp)>();
        Reset();
    }
    public virtual void Reset()
    {
        restoreDistance = DEFAULT_RESTORE_DIS;
        isAllowCollision = false;
        isThrough = false;
        isInvincible = false;
        rotateIsMoveAngle = false;
        moveAngle = 0f;
        speed = 0f;
        parent = null;

        isDead = false;
        _isTriggerDead = false;
        _isTriggerRestore = false;
        relatChildProps.Clear();
        propLateDebutByCreateSettings.Clear();
        propLateCallActs.Clear();
    }

    public void SetParent(UnitCtrlBase parentUnitCtrl)
    {
        parent = parentUnitCtrl.unitProp;
    }



    public virtual void RefreshVal(SettingBase setting)
    {
        if (!InvalidHelper.IsInvalid(setting.restoreDistance))
        {
            restoreDistance = setting.restoreDistance;
        }
        if (!InvalidHelper.IsInvalid(setting.rotateIsMoveAngle))
        {
            rotateIsMoveAngle = setting.rotateIsMoveAngle == BoolState.True ? true : false;
        }

        if (!InvalidHelper.IsInvalid(setting.isInvincible))
        {
            isInvincible = setting.isInvincible == BoolState.True ? true : false;
        }

        if (!InvalidHelper.IsInvalid(setting.isThrough))
        {
            isThrough = setting.isThrough == BoolState.True ? true : false;
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

