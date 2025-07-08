using UnityEngine;
using static CommonData;
using static CommonFunc;
using static GameConfig;
using static PlayerKeyCtrl;
using static PlayerSaveData;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyBossUnitProp : EnemyUnitProp
{
    public uint SpellTime = DEFAULT_SPELL_TIME;
    public uint MaxHp = DEFAULT_ENEMY_HP;

    protected override void SetCustomize()
    {
        isInvincible = true; //Boss初始為無敵
    }

    protected override void ActiveCustomize(SettingBase setting, IUnitCtrlData unitCtrlData)
    {
        UpdateBossHp(setting);
    }
    void UpdateBossHp(SettingBase setting)
    {
        //Boss血條重設
        if (setting.hp > 0)
        {
            if (setting.hp != 1)
                GameSystem.Instance.bossHpLine.localScale = new Vector2(65f, GameSystem.Instance.bossHpLine.localScale.y);
            else
                GameSystem.Instance.bossHpLine.localScale = new Vector2(0f, GameSystem.Instance.bossHpLine.localScale.y);
        }
    }
    protected override void UseSettingValCustomize(SettingBase setting)
    {
        base.UseSettingValCustomize(setting);
        if (setting.hp != null)
        {
            MaxHp = setting.hp.Value;
        }
        if (setting.spellTime != null)
        {
            SpellTime = setting.spellTime.Value;
        }
    }
}

