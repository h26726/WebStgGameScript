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
using static PlayerSaveData;
using static GameConfig;

public abstract class CollisionCtrlBase : MonoBehaviour
{
    public UnitCtrlBase unitCtrl { get; set; }


    void Awake()
    {
    }
    public void Init(UnitCtrlBase unitCtrl)
    {
        this.unitCtrl = unitCtrl;
    }

    protected void CollisionHandle(Collider2D opponentCollision)
    {
        if (unitCtrl.unitProp.isDead || !unitCtrl.unitProp.isAllowCollision)
            return;
        var opponentUnitCtrl = opponentCollision.GetComponent<CollisionCtrlBase>().unitCtrl;
        if (opponentUnitCtrl.unitProp.isDead || !opponentUnitCtrl.unitProp.isAllowCollision)
            return;
        Attacked(opponentUnitCtrl);

        if (opponentUnitCtrl is ShotUnitCtrl)
        {
            var opponentShotUnitCtrl = opponentUnitCtrl as ShotUnitCtrl;
            opponentShotUnitCtrl.TryCollisionDead();
        }
        else
        {
            opponentUnitCtrl.unitProp.isTriggerDead = true;
        }
    }



    protected virtual void Attacked(UnitCtrlBase opponentUnitCtrl)
    {

    }
}
