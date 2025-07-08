using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static CommonData;
using static CommonFunc;
using static PlayerKeyCtrl;
using static GameConfig;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using System.IO;
public partial class UnitCtrlBase
{
    public IUnitCtrlData GetParent()
    {
        return parentUnitCtrl;
    }

    public Vector2 GetTransformPos()
    {
        return transform.position;
    }
    public float GetRotateZ()
    {
        if (mainObjCtrl == null)
            return 0;
        return mainObjCtrl.transform.rotation.eulerAngles.z;
    }

    public float GetChildRotateZ()
    {
        if (childFrame == null)
            return 0;
        return childFrame.transform.rotation.eulerAngles.z;
    }
    
    public bool TryGetActionMoveAngle(uint key, out float angle)
    {
        angle = 0f;
        var ActCtrls = actCtrlDict.Values.ToArray();
        if (key >= ActCtrls.Length)
            return false;

        angle = actCtrlDict.Values.ToArray()[key].actionProp.moveAngle;
        return true;
    }

    public UnitPropBase GetUnitProp()
    {
        return unitProp;
    }

    public uint GetCoreSettingId()
    {
        return coreSetting.Id;
    }

    public void MovePos(Vector2 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, zIndex);
    }

    public void MoveTranslate(Vector2 moveVector)
    {
        transform.Translate(moveVector);
    }


    public void SetRotateZ(float RotateZ)
    {
        if (mainObjCtrl == null)
            return;
        mainObjCtrl.transform.rotation = Quaternion.Euler(0f, 0f, RotateZ);
    }
    public void SetChildRotateZ(float ChildRotateZ)
    {
        if (childFrame == null)
            return;
        childFrame.transform.rotation = Quaternion.Euler(0f, 0f, ChildRotateZ);
    }

    public virtual void TriggerRestore()
    {
        if (isDeadQueue || isRestoreQueue)
            return;
        foreach (UnitCtrlBase child in childFrameUnits)
        {
            if (child.transform.parent == transform)
            {
                child.TriggerRestore();
            }
        }
        isRestoreQueue = true;
        GameSystem.Instance.waitRestores += RestoreIntoPool;

    }

    public void TriggerDead()
    {
        if (isDeadQueue || isRestoreQueue)
            return;
        foreach (UnitCtrlBase child in childFrameUnits)
        {
            if (child.transform.parent == transform)
            {
                child.TriggerDead();
            }
        }
        isDeadQueue = true;
        GameSystem.Instance.waitDeads += HandleDead;
    }

    public void ClearAllAction(ActCtrl actCtrl)
    {
        actCtrl.coreAction = null;
        actCtrl.callTimeAction = null;
        actCtrl.callPosAction = null;
        actCtrl.stopAction = null;
    }

    public void ExtHandle(CallRule callRule, ActCtrl actCtrl)
    {
        if (callRule.createStageSetting != null)
        {
            //創造子單位
            GameSystem.Instance.waitCreates += () =>
            {
                GameSystem.Instance.CreateUnit(callRule.createStageSetting, this);
            };
        }
        else if (callRule.baseId != null && callRule.actId != null)
        {
            //啟動其他單位行動
            List<UnitCtrlBase> Units;
            if (callRule.baseId == coreSetting.Id)
            {
                Units = new List<UnitCtrlBase>() { this };
            }
            else
            {
                Units = GetUnitsById(callRule.baseId.Value);
            }

            GameSystem.Instance.waitCallActs += () =>
            {
                GameSystem.Instance.ExtAct(Units, callRule.actId.Value, actCtrl);
            };
        }
    }

}


