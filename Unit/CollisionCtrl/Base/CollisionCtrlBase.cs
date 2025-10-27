using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Text;
using System.Linq;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static SaveJsonData;
using static GameConfig;

public abstract class CollisionCtrlBase : MonoBehaviour
{
    // public UnitCtrlBase unitCtrl { get; set; }
    public UnitCtrlObj unitCtrlObj { get; set; }
    public UnitPropBase unitProp { get; set; }


    void Awake()
    {
    }
    public void Init(UnitCtrlBase unitCtrl)
    {
        this.unitCtrlObj = unitCtrl.unitCtrlObj;
        this.unitProp = unitCtrl.unitProp;
    }

    protected void CollisionHandle(Collider2D opponentCollision)
    {
        if (unitProp.isDead || !unitProp.isAllowCollision)
            return;
        var opponentCollisionCtrl = opponentCollision.GetComponent<CollisionCtrlBase>();
        var opponentUnitProp = opponentCollisionCtrl.unitProp;
        var opponentUnitCtrlObj = opponentCollisionCtrl.unitCtrlObj;

        if (opponentUnitProp.isDead || !opponentUnitProp.isAllowCollision)
            return;
        Attacked(opponentUnitProp);

        if (opponentUnitProp is ShotUnitProp && opponentUnitCtrlObj is ShotCtrlObj)
        {
            var opponentShotUnitProp = opponentUnitProp as ShotUnitProp;
            var opponentShotUnitCtrlObj = opponentUnitCtrlObj as ShotCtrlObj;
            TryCollisionDead(opponentShotUnitProp, opponentShotUnitCtrlObj);
        }
        else
        {
            opponentUnitProp.isTriggerDead = true;
        }
    }

    void TryCollisionDead(ShotUnitProp opponentShotUnitProp, ShotCtrlObj opponentShotUnitCtrlObj)
    {
        if (opponentShotUnitProp.isThrough || opponentShotUnitCtrlObj.isThrough)
            return;
        opponentShotUnitProp.isTriggerDead = true;
    }



    protected virtual void Attacked(UnitPropBase opponentUnitProp)
    {

    }

    public virtual void UpdateHandler()
    {

    }
}
