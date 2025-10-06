using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;
using System.Xml;
using System.IO;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static PlayerSaveData;
using static GameConfig;
using System.Linq;
using static LoadCtrl;

public partial class GameObjCtrl : SingletonBase<GameObjCtrl>
{
    public RectTransform playerLifeRect;
    public RectTransform playerBoomRect;
    public Text scoreText;
    public Text powerText;
    public Text gameLog;

    public GameObject nowSpellBg { get; set; }
    public Animator spellCardNameAnimator;
    public Text spellCardNameText;

    public Text spellTime;
    public RectTransform bossHpLine;

    public GameObject dialogBox;
    public Text dialogBoxText;
    public Animator dialogBoxAnimator;

    public void StopSpell()
    {
        if (nowSpellBg != null)
            nowSpellBg.SetActive(false);
        spellTime.text = "";
        spellCardNameAnimator.gameObject.SetActive(false);
        ClearBossHpLine();
    }

    public void OpenSpell(SettingBase spellSetting)
    {
        if(string.IsNullOrEmpty(spellSetting.name))
            return;
        Animator animator = LoadCtrl.Instance.pool.spellDict[spellSetting.spellAni][0];
        nowSpellBg = LoadCtrl.Instance.pool.spellDict[spellSetting.spellAni][1].transform.gameObject;
        nowSpellBg.SetActive(true);
        if (IS_OPEN_SPELLIMG)
        {
            Instance.spellCardNameAnimator.gameObject.SetActive(true);
            Instance.spellCardNameText.text = spellSetting.name;
            this.StartCoroutine(AnimationHelper.PlayAniCoroutine(animator));
        }
    }

    public void UpdatePlayerLife()
    {
        for (int i = 0; i < Instance.playerLifeRect.childCount; i++)
        {
            var HpObj = Instance.playerLifeRect.GetChild(i);
            HpObj.gameObject.SetActive(i < GamePlayer.life);
        }
    }

    public void UpdatePlayerBoom()
    {
        for (int i = 0; i < Instance.playerBoomRect.childCount; i++)
        {
            var BoomObj = Instance.playerBoomRect.GetChild(i);
            BoomObj.gameObject.SetActive(i < GamePlayer.boom);
        }
    }

    public void UpdatePlayerPower()
    {
        if (GamePlayer.power == GameConfig.PLAYER_MAX_POWER)
        {
            GameObjCtrl.Instance.powerText.text = "MAX";
        }
        else
        {
            GameObjCtrl.Instance.powerText.text = GamePlayer.power.ToString("F1");
        }
    }

    public void UpdateSpellTimeText()
    {
        var nowEnemyBoss = GameBoss.nowUnit;
        float totalSpellTime = GameBoss.SpellTime;
        float usedSpellTime = nowEnemyBoss.uTime;
        spellTime.text = MathF.Floor((totalSpellTime - usedSpellTime) / 60).ToString();
    }

    public void UpdateBossHpLine()
    {
        var  nowEnemyBoss = GameBoss.nowUnit;
        uint maxHp = GameBoss.MaxHp;
        uint nowHp = nowEnemyBoss.enemyProp.hp;
        bossHpLine.localScale = new Vector2((float)nowHp / (float)maxHp * 65f, bossHpLine.localScale.y);
    }

    public void ClearBossHpLine()
    {
        bossHpLine.localScale = new Vector2(0, bossHpLine.localScale.y);
    }

    public void FillBossHpLine()
    {
        bossHpLine.localScale = new Vector2(65f, bossHpLine.localScale.y);
    }

    public void UpdateScoreText()
    {
        scoreText.text = PlayerSaveData.score.ToString().PadLeft(8, '0');
    }

    public void ShowDialogBox()
    {
        dialogBox.SetActive(true);
        dialogBoxAnimator.Play("Show");
    }

    public void DialogBoxChangeText(string dialogText)
    {
        dialogBoxText.text = dialogText;
    }

    public void HideDialogBoxStep1()
    {
        dialogBoxText.text = "";
        dialogBoxAnimator.Play("Hide");
    }

    public void HideDialogBoxStep2()
    {
        dialogBox.SetActive(false);
    }
}
