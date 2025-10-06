using UnityEngine;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static GameConfig;
using static PlayerKeyHelper;
using static PlayerSaveData;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyUnitProp : UnitPropBase
{
    public uint hp = DEFAULT_ENEMY_HP;
    public bool isBoss = false;

    public EnemyUnitProp(UnitCtrlBase unitCtrl) : base(unitCtrl)
    {
        
    }

    public override void Reset()
    {
        base.Reset();
        if (isBoss)
        {
            hp = 0;
            isInvincible = true;
            isAllowCollision = true;
        }
    }

    public override void RefreshVal(SettingBase setting)
    {
        base.RefreshVal(setting);
        if (setting.hp != null)
        {
            hp = setting.hp.Value;
        }

        if (isBoss)
        {
            if (setting.hp != null)
            {
                GameBoss.MaxHp = setting.hp.Value;
            }
            if (setting.spellTime != null)
            {
                GameBoss.SpellTime = setting.spellTime.Value;
            }
            UpdateBossHp(setting);
        }
    }

    void UpdateBossHp(SettingBase setting)
    {
        //Boss血條重設
        if (setting.hp > 0)
        {
            if (setting.hp != 1)
                GameObjCtrl.Instance.FillBossHpLine();
            else
                GameObjCtrl.Instance.ClearBossHpLine();

        }
    }
}

