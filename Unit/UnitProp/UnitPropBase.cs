using UnityEngine;
using static CommonData;
using static CommonFunc;
using static GameConfig;
using static PlayerKeyCtrl;
using static PlayerSaveData;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class UnitPropBase
{
    public float restoreDistance { get; set; } = DEFAULT_RESTORE_DIS;

    public bool isThrough { get; set; } = false;
    public bool isInvincible { get; set; } = false;
    public bool rotateIsMoveAngle { get; set; } = false;

    public float moveAngle { get; set; } = 0f;
    public float speed { get; set; } = 0f;


    public UnitPropBase Set()
    {
        SetCustomize();
        return this;
    }

    protected virtual void SetCustomize()
    {

    }


    public void Active(SettingBase setting, IUnitCtrlData unitCtrlData)
    {
        UseSettingVal(setting, unitCtrlData);
        ActiveCustomize(setting, unitCtrlData);
    }

    protected virtual void ActiveCustomize(SettingBase setting, IUnitCtrlData unitCtrlData)
    {

    }


    void UseSettingVal(SettingBase setting, IUnitCtrlData unitCtrlData)
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

        UseSettingValCustomize(setting);
        unitCtrlData.AddPrintContent(PrintSettingVal(setting));
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
    protected virtual void UseSettingValCustomize(SettingBase setting)
    {

    }


}

