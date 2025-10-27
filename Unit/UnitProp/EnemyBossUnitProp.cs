using UnityEngine;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static GameConfig;
using static PlayerKeyHelper;
using static SaveJsonData;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyBossUnitProp : EnemyUnitProp
{
    public EnemyBossUnitProp(UnitCtrlBase unitCtrl) : base(unitCtrl)
    {
    }

    public void SetDef()
    {
        hp = 0;
        isInvincible = true;
        isAllowCollision = true;
    }

    public override void RefreshVal(SettingBase setting)
    {
        base.RefreshVal(setting);
        if (!InvalidHelper.IsInvalid(setting.hp))
        {
            GameBoss.MaxHp = setting.hp;
        }
        if (!InvalidHelper.IsInvalid(setting.spellTime))
        {
            GameBoss.SpellTime = setting.spellTime;
        }
        UpdateBossHp(setting);
    }

    void UpdateBossHp(SettingBase setting)
    {
        //Boss血條重設
        if (!InvalidHelper.IsInvalid(setting.hp) && setting.hp > 0)
        {
            if (setting.hp != 1)
                GameObjCtrl.Instance.FillBossHpLine();
            else
                GameObjCtrl.Instance.ClearBossHpLine();

        }
    }

}

