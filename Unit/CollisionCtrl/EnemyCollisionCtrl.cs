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
using static GameMainCtrl;

public class EnemyCollisionCtrl : CollisionCtrlBase
{
    protected void OnTriggerStay2D(Collider2D collision)
    {
        CollisionHandle(collision);
    }

    protected override void Attacked(UnitCtrlBase opponentUnitCtrl)
    {
        if (unitCtrl.unitProp.isInvincible)
            return;

        opponentUnitCtrl.unitProp.isTriggerDead = true;
        var enemyUnitCtrl = unitCtrl as EnemyUnitCtrl;
        if (enemyUnitCtrl != null)
        {
            EnemyHpCost(enemyUnitCtrl, (PlayerShotUnitCtrl)opponentUnitCtrl);
        }
        else
        {
            Debug.LogWarning("Attacked method called on a non-enemy unitCtrl.");
        }
    }

    public void EnemyHpCost(EnemyUnitCtrl enemyUnitCtrl, PlayerShotUnitCtrl playerShotUnitCtrl)
    {
        if (playerShotUnitCtrl == null)
        {
            Debug.LogWarning("ShotUnitCtrl is null in HandleAttacked.");
            return;
        }
        var playerShotUnitProp = playerShotUnitCtrl.unitProp as PlayerShotUnitProp;
        if (playerShotUnitProp == null)
        {
            Debug.LogWarning("UnitProp is not of type PlayerShotUnitProp in HandleAttacked.");
            return;
        }

        var enemyUnitProp = enemyUnitCtrl.unitProp as EnemyUnitProp;
        if (enemyUnitProp == null)
        {
            Debug.LogWarning("UnitProp is not of type EnemyUnitProp in HandleAttacked.");
            return;
        }

        CostHp(enemyUnitProp, playerShotUnitProp.dmg);

        if (enemyUnitCtrl == GameBoss.nowUnit)
        {
            // if (BossIsNotHp())
            //     return;
            GameObjCtrl.Instance.UpdateBossHpLine();
            if (enemyUnitProp.hp == 0)
            {
                Debug.Log("enemyUnitProp.hp == 0");
                GameMainCtrl.Instance.gameSceneUpdateFlag |= UpdateFlag.WaitSpellEnd ;
            }
        }
        else
        {
            if (enemyUnitProp.hp == 0)
            {
                enemyUnitProp.isTriggerDead = true;
            }
        }

    }

    // bool BossIsNotHp()
    // {
    //     return GameSceneValCtrl.Instance.GameSceneValCtrl.Instance.EnemyBossMaxHp == 0;
    // }


    public void CostHp(EnemyUnitProp enemyUnitProp, uint dmg)
    {
        var oldHp = enemyUnitProp.hp;
        var costHp = dmg;

        if (costHp > oldHp) enemyUnitProp.hp = 0;
        else enemyUnitProp.hp -= costHp;
    }


}
