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

public class EnemyBossUnitCtrl : EnemyUnitCtrl
{
    public EnemyBossUnitProp enemyBossProp => unitProp as EnemyBossUnitProp;

    protected override void CustomizeReset()
    {

    }

    public override void OnActive(ActCtrl actCtrl, ActCtrl parentActCtrl = null)
    {
        if (coreSetting.type == TypeValue.符卡)
        {
            eventMoveVectorCal += SpellCardTimeUpdate;
        }
        base.OnActive(actCtrl, parentActCtrl); 
    }

    void SpellCardTimeUpdate()
    {
        if (uTime % 10 == 0)
        {
            if (enemyBossProp.SpellTime - uTime > 0)
            {
                GameSystem.Instance.spellTime.text = MathF.Floor((enemyBossProp.SpellTime - uTime) / 60).ToString();
            }
            else
            {
                if (coreSetting.hp == 1)
                {
                    GivePower();
                }
                SpellEnd();
                return;
            }
        }
    }

    public override void OnActTimeEndCustomize()
    {
        if (coreSetting.type == TypeValue.BOSS || coreSetting.type == TypeValue.復位)
        {
            SpellEnd();
            return;
        }
        else if (coreSetting.type == TypeValue.BOSSLEAVE)
        {
            TriggerRestore();
            GameSystem.Instance.nowGameProgressState = GameProgressState.Stage;
            GameSystem.Instance.playerUnitCtrl.unitProp.isInvincible = false;
        }
    }

    public override bool CheckOutBorder(Vector2? Pos = null)
    {
        return false;
    }

    public override void DeadAnimEndCustomize()
    {
        if (GameSystem.Instance.isPractice)
        {
            GameSystem.Instance.Pause();
            PracticeOverSelect.Instance.Show();
        }
    }

    protected override void RestoreInToPoolCustomize()
    {
        GameSystem.Instance.nowEnemyBoss = null;
    }

    public override void OnCostHp(EnemyUnitProp enemyUnitProp, uint dmg)
    {
        base.OnCostHp(enemyUnitProp, dmg);
        HandleHpRefresh();
    }

    public void HandleHpRefresh()
    {
        if (enemyBossProp.MaxHp == 0)
            return;

        //BOSS更新血條
        GameSystem.Instance.bossHpLine.localScale = new Vector2((float)enemyBossProp.hp / (float)enemyBossProp.MaxHp * 65f, GameSystem.Instance.bossHpLine.localScale.y);
    }

    public override void HandleHpEmpty()
    {
        GivePower();
        SpellEnd();
    }

    public void SpellEnd()
    {
        ClearEvent();
        if (GameSystem.Instance.isPractice && coreSetting.type == TypeValue.符卡)
        {
            TriggerDead();
        }
        else
        {
            GameSystem.Instance.nowGameProgressState = GameProgressState.SpellEndTrigger;
        }
        enemyBossProp.hp = 0;
        enemyBossProp.isInvincible = true;
        GameSystem.Instance.bossHpLine.localScale = new Vector2(0f, GameSystem.Instance.bossHpLine.localScale.y);
    }

    public override void CustomizeActive(SettingBase setting)
    {
        if (TryBossDeath(setting))
            return;
    }
    protected bool TryBossDeath(SettingBase setting)
    {
        if (setting.bossDead == true)
        {
            GameSystem.Instance.nowGameProgressState = GameProgressState.Stage;
            TriggerDead();
            return true;
        }
        return false;
    }
}
