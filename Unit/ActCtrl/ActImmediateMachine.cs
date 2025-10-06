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
public class ActImmediateMachine
{
    public SettingBase setting;
    public UnitCtrlObj unitCtrlObj;
    public UnitPropBase unitProp;
    public uint coreSettingId;

    public bool isRestore = false;


    public ActImmediateMachine(ActCtrl actCtrl)
    {
        this.unitCtrlObj = actCtrl.unitCtrlObj;
        this.unitProp = actCtrl.unitProp;
        this.setting = actCtrl.setting;
        this.coreSettingId = actCtrl.coreSettingId;
        this.isRestore = false;
    }

    public void Run()
    {
        unitProp.RefreshVal(setting);
        TryObjMovePos();
        TryObjRotation();
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
            if (setting.isIn == true && unitCtrlObj.IsOutBorder(unitProp.restoreDistance, getPos))
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
    }

    void TryObjRelatPos()
    {
        if (setting.relatPos != null && setting.relatPos.Count > 0 && setting.relatPos[0].Id != null)
        {
            var id = setting.relatPos[0].Id.Value;
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
        if (setting.sprite != null)
        {
            unitCtrlObj.ChangeSprite(setting.sprite);
        }
    }
}

