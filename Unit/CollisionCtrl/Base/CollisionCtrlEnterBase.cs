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

public abstract class CollisionCtrlEnterBase : CollisionCtrlBase
{
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        TriggerHandle(collision);
    }

    protected override void Attacked(UnitCtrlBase shotUnitCtrl)
    {
        if (!shotUnitCtrl.isAllowCollision || !unitCtrlBase.isAllowCollision || unitCtrlBase.unitProp.isInvincible)
            return;
       
        var enemyUnitCtrl = unitCtrlBase as EnemyUnitCtrl;
        if (enemyUnitCtrl != null)
        {
            enemyUnitCtrl.Attacked((PlayerShotUnitCtrl)shotUnitCtrl);
        }
        else
        {
            Debug.LogWarning("Attacked method called on a non-enemy unit.");
        }
    }
}
