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

public class EnemyUnitCtrl : UnitCtrlBase
{
    public virtual void HandleHpEmpty()
    {
        HandleDead();
    }

    public void Attacked(PlayerShotUnitCtrl playerShotUnitCtrl)
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

        var enemyUnitProp = unitProp as EnemyUnitProp;
        if (enemyUnitProp == null)
        {
            Debug.LogWarning("UnitProp is not of type EnemyUnitProp in HandleAttacked.");
            return;
        }

        OnCostHp(enemyUnitProp, playerShotUnitProp.dmg);
        if (enemyUnitProp.hp == 0)
            HandleHpEmpty();
    }

    public override void CustomizeDeadHandle()
    {
        GivePower();
    }

    public void GivePower()
    {
        if (GameSystem.Instance.isPractice || createSetting == null || coreSetting.powerGive == 0)
            return;

        var PowerGiveList = coreSetting.powerGive;
        for (int i = 0; i < PowerGiveList; i++)
        {
            var unit = GameSystem.Instance.powerCreateStageSettings[i];
            GameSystem.Instance.waitCreates += () =>
            {
                GameSystem.Instance.CreateUnit(unit, this);
            };
        }
    }


    public virtual void OnCostHp(EnemyUnitProp enemyUnitProp, uint dmg)
    {
        var oldHp = enemyUnitProp.hp;
        var costHp = dmg;

        if (costHp > oldHp) enemyUnitProp.hp = 0;
        else enemyUnitProp.hp -= costHp;
    }
}
