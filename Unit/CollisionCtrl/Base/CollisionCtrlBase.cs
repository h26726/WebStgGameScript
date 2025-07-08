using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Text;
using System.Linq;
using static CommonData;
using static CommonFunc;
using static PlayerKeyCtrl;
using static PlayerSaveData;
using static GameConfig;

public abstract class CollisionCtrlBase : MonoBehaviour
{
    public UnitCtrlBase unitCtrlBase { get; set; }
    void Awake()
    {
        unitCtrlBase = transform.parent.GetComponent<UnitCtrlBase>();
    }

    protected virtual void TriggerHandle(Collider2D collision)
    {
        var attackerUnitCtrl = collision.GetComponent<CollisionCtrlBase>().unitCtrlBase;
        var shotUnitCtrl = attackerUnitCtrl as ShotUnitCtrl;

        Attacked(attackerUnitCtrl);

        //敵彈回收
        TargetDeadHandle(shotUnitCtrl, attackerUnitCtrl.TriggerDead);

    }

    protected void TargetDeadHandle(ShotUnitCtrl shotUnitCtrl, Action triggerDead)
    {
        if (shotUnitCtrl == null) //非子彈直接回收
        {
            triggerDead();
            return;
        }

        if (shotUnitCtrl.unitProp.isThrough == true || shotUnitCtrl.isThrough == true)
            return; //穿透子彈不回收

        triggerDead();
    }

    protected virtual void Attacked(UnitCtrlBase AttackerUnitCtrl)
    {
        
    }
}
