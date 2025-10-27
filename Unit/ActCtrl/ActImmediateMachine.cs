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
public class ActImmediateMachine
{
    public SettingBase setting;
    public UnitCtrlObj unitCtrlObj;
    public UnitPropBase unitProp;
    public uint coreSettingId;
    public ActImmediateMachine()
    {
        Reset();
    }
    public void Set(ActCtrl actCtrl)
    {
        this.unitCtrlObj = actCtrl.unitCtrlObj;
        this.unitProp = actCtrl.unitProp;
        this.setting = actCtrl.setting;
        this.coreSettingId = actCtrl.coreSettingId;
    }

    public void Reset()
    {
        this.unitCtrlObj = null;
        this.unitProp = null;
        this.setting = null;
        this.coreSettingId = 0;
    }

    public void Run()
    {
        unitProp.RefreshVal(setting);
        TryObjRotation();
        TryObjMovePos();
        TryObjRelatPos();
        TryObjChildRotation();
        TryObjChangeSprite();
        unitCtrlObj.TryObjAddRecord(setting);
    }



    void TryObjMovePos()
    {
        if (setting.movePos != null)
        {
            var getPos = unitCtrlObj.GetPos(setting.movePos);
            if (setting.isIn == BoolState.True && unitCtrlObj.IsOutBorder(unitProp.restoreDistance, getPos))
            {
                unitProp.isTriggerRestore = true;
            }
            else
            {
                unitCtrlObj.MovePos(new Vector2(getPos.x, getPos.y));
            }
        }
    }



    void TryObjRotation()
    {
        if (setting.rotateZ != null)
        {
            unitCtrlObj.SetRotateZ(setting.rotateZ);
        }
        else if (unitProp.rotateIsMoveAngle == true && setting.moveAngle != null)
        {
            unitCtrlObj.SetRotateZ(setting.moveAngle);
        }
    }

    void TryObjRelatPos()
    {
        if (setting.relatPos != null && setting.relatPos.Count > 0 && !InvalidHelper.IsInvalid(setting.relatPos[0].Id))
        {
            var id = setting.relatPos[0].Id;
            var relatUnitProp = unitProp.GetUnitProp(id);
            relatUnitProp.unitCtrlObj.InsertRelatChild(unitCtrlObj);
            relatUnitProp.relatChildProps.Add(unitProp);
        }
    }

    void TryObjChildRotation()
    {
        if (setting.childRotateZ != null)
        {
            unitCtrlObj.SetChildRotateZ(setting.childRotateZ);
        }
    }



    void TryObjChangeSprite()
    {
        if (!InvalidHelper.IsInvalid(setting.sprite))
        {
            unitCtrlObj.ChangeSprite(setting.sprite);
        }
    }
}

