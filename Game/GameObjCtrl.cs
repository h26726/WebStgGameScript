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
using static SaveJsonData;
using static GameConfig;
using System.Linq;
using static LoadCtrl;
using System.Text;

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

    public void Reset()
    {
        scoreText.text = "";
        powerText.text = "";
        gameLog.text = "";
        if (nowSpellBg != null)
            nowSpellBg.SetActive(false);
        nowSpellBg = null;
        spellCardNameAnimator.gameObject.SetActive(false);
        spellCardNameText.text = "";
        spellTime.text = "";
        ClearBossHpLine();
        dialogBox.SetActive(false);
        DialogChangeText("");
        DialogCloseText();
        if (!dialogBoxAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hide")) dialogBoxAnimator.Play("Hide");
    }

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
        if (InvalidHelper.IsInvalid(spellSetting.name))
            return;
        var pool = LoadCtrl.Instance.pool;
        var spell = pool.spellDict[spellSetting.spellAni];
        Animator animator = spell[0];
        nowSpellBg = spell[1].transform.gameObject;
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
    private readonly StringBuilder powerSb = new(8); // 共用緩衝區，避免重複配置

    public void UpdatePlayerPower()
    {
        var text = GameObjCtrl.Instance.powerText;

        if (GamePlayer.power == GameConfig.PLAYER_MAX_POWER)
        {
            text.text = "MAX";
        }
        else
        {
            powerSb.Clear();
            // 將格式化結果寫入 StringBuilder
            powerSb.AppendFormat("{0:F1}", GamePlayer.power);
            text.text = powerSb.ToString();
        }
    }

    private readonly StringBuilder spellTimeSb = new(8);
    public void UpdateSpellTimeText()
    {
        var nowEnemyBoss = GameBoss.nowUnit;
        float remain = (GameBoss.SpellTime - nowEnemyBoss.uTime) / 60f;
        int floorTime = (int)MathF.Floor(remain);

        spellTimeSb.Clear();
        spellTimeSb.Append(floorTime);
        spellTime.text = spellTimeSb.ToString();
    }

    public void UpdateBossHpLine()
    {
        var nowEnemyBoss = GameBoss.nowUnit;
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
        scoreText.text = SaveJsonData.score.ToString().PadLeft(8, '0');
    }

    public void ShowDialogBox()
    {
        dialogBox.SetActive(true);
        dialogBoxAnimator.Play("Show");
    }

    public void DialogOpenText()
    {
        dialogBoxText.enabled = true;
    }

    public void DialogCloseText()
    {
        dialogBoxText.enabled = false;
    }

    public void DialogChangeText(string dialogText)
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
