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

public class PlayerCollisionCtrl : CollisionCtrlBase
{
    protected void OnTriggerStay2D(Collider2D opponentCollision)
    {
        CollisionHandle(opponentCollision);
    }
    protected override void Attacked(UnitCtrlBase opponentUnitCtrl)
    {
        if (unitCtrl.unitProp.isInvincible != true && opponentUnitCtrl is EnemyShotUnitCtrl)
        {
            unitCtrl.unitProp.isTriggerDead = true;
        }
        else if (opponentUnitCtrl is PowerUnitCtrl)
        {
            GamePlayer.GetPower();
        }
    }
}
