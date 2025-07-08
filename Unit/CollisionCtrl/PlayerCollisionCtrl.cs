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

public class PlayerCollisionCtrl : CollisionCtrlBase
{
    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        TriggerHandle(collision);
    }
    protected override void Attacked(UnitCtrlBase attackerUnitCtrl)
    {
        if (unitCtrlBase.unitProp.isInvincible == true || !attackerUnitCtrl.isAllowCollision)
            return;
        if (attackerUnitCtrl.isDeadQueue || attackerUnitCtrl.isRestoreQueue)
            return;
        if (attackerUnitCtrl is EnemyShotUnitCtrl)
        {
            unitCtrlBase.HandleDead();
        }
        else if (attackerUnitCtrl is PowerUnitCtrl)
        {
            GameSystem.Instance.playePower += GameConfig.PLAYER_EVERY_POWER_GET;
            GameSystem.Instance.playePower = Mathf.Round(GameSystem.Instance.playePower * 100f) / 100f;
        }
    }
}
