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

public partial class GameMainCtrl : SingletonBase<GameMainCtrl>
{
    public enum UpdateFlag
    {
        None = 0,
        SpellTimeRefresh = 1 << 0,
        WaitPracticeBossDead = 1 << 1,
        LateSpellEnd = 1 << 2,
        Pause = 1 << 3,
        LatePlayerHpEmptyPause = 1 << 4,
        LateResetPlayerDef = 1 << 5,
    }
    public uint PracticeBossDeadCalTime;
    public float objLast_zIndex { get; set; }
    public UpdateFlag gameSceneUpdateFlag;

    public GameProgressState nowGameProgressState;




    public void GameStartSet()
    {
        GameDebut.GameStartSet();
        GameProgressStageCtrl.GameStartSet();
        GameReplay.GameStartSet();
    }

    public void Reset()
    {
        PracticeBossDeadCalTime = 0;
        objLast_zIndex = GameConfig.Z_INDEX_TOP;
        gameSceneUpdateFlag = UpdateFlag.None;
        nowGameProgressState = GameProgressState.Stage;
    }

    public float Get_zIndex()
    {
        objLast_zIndex -= GameConfig.Z_INDEX_REDUCE;
        return objLast_zIndex;
    }

    public void UpdateHandler()
    {
        if (gameSceneUpdateFlag.HasFlag(UpdateFlag.Pause) || LoadCtrl.Instance.gameState != GameSceneState.Run)
            return;

        GameReplay.UpdateHandler(out bool isSkipRemain);
        if (isSkipRemain)
            return;

        if (nowGameProgressState == GameProgressState.Stage)
        {
            GameProgressStageCtrl.UpdateHandler();
        }

        GameDebut.UpdateHandler();
        if (nowGameProgressState == GameProgressState.Dialog)
        {
            DialogCtrl.nowInstance?.UpdateHandler();
        }

        if (nowGameProgressState == GameProgressState.BossTime)
        {
            if (GameBoss.nowUnit == null)
            {
                nowGameProgressState = GameProgressState.Stage;
            }

            if (gameSceneUpdateFlag.HasFlag(UpdateFlag.SpellTimeRefresh))
            {
                GameObjCtrl.Instance.UpdateSpellTimeText();
                SpellEndCheck();
            }

            if (gameSceneUpdateFlag.HasFlag(UpdateFlag.LateSpellEnd))
            {
                gameSceneUpdateFlag &= ~UpdateFlag.LateSpellEnd;
                SpellEnd();
            }



            if (GameSelect.isPracticeMode && gameSceneUpdateFlag.HasFlag(UpdateFlag.WaitPracticeBossDead))
            {
                TryPracticeBossDeadEnd();
            }
        }


        if (IsPressESC())
        {
            Pause();
            ShowPauseSelect();
        }

        if (gameSceneUpdateFlag.HasFlag(UpdateFlag.LateResetPlayerDef))
        {
            gameSceneUpdateFlag &= ~UpdateFlag.LateResetPlayerDef;
            GamePlayer.SetDef();
        }

        if (gameSceneUpdateFlag.HasFlag(UpdateFlag.LatePlayerHpEmptyPause))
        {
            gameSceneUpdateFlag &= ~UpdateFlag.LatePlayerHpEmptyPause;
            gameSceneUpdateFlag |= UpdateFlag.LateResetPlayerDef;
            if (!GameReplay.isReplayMode)
            {
                GameMainCtrl.Instance.Pause();
                if (GameSelect.isPracticeMode)
                {
                    PracticeOverSelect.Instance.Show();
                }
                else
                {
                    GameOverSelect.Instance.Show();
                }
            }
            Debug.Log("GameReplay Time:" + GameReplay.keyPressTime);
        }
    }

    void TryPracticeBossDeadEnd()
    {
        PracticeBossDeadCalTime++;
        if (PracticeBossDeadCalTime == GameConfig.PRACTICE_DEAD_DELAY_KEY_TIME)
        {
            PracticeBossDeadCalTime = 0;
            gameSceneUpdateFlag &= ~UpdateFlag.WaitPracticeBossDead;
            GameMainCtrl.Instance.Pause();
            PracticeOverSelect.Instance.Show();
        }
    }


    public void SpellEnd()
    {
        // 快取 nowUnit 避免多次屬性存取
        var GmaeBossUnitCtrl = GameBoss.nowUnit;
        if (GmaeBossUnitCtrl == null)
        {
            Debug.LogError("GameBoss.nowUnit == null");
            return;
        }

        // 關閉符卡刷新標誌
        gameSceneUpdateFlag &= ~UpdateFlag.SpellTimeRefresh;

        var coreSetting = GmaeBossUnitCtrl.coreSetting;
        if (GameSelect.isPracticeMode && coreSetting.type == TypeValue.符卡)
        {
            GmaeBossUnitCtrl.enemyProp.isTriggerDead = true;
            gameSceneUpdateFlag |= UpdateFlag.WaitPracticeBossDead;
        }
        else
        {
            nowGameProgressState = GameProgressState.Stage;
        }
        // 快取 coreSetting 與 powerGive
        uint originGivePowerNum = !InvalidHelper.IsInvalid(coreSetting.powerGive) ? coreSetting.powerGive : 0;
        uint originDebutNo = GmaeBossUnitCtrl.debutNo;

        // 保留狀態
        var keepData = GmaeBossUnitCtrl.KeepData();

        // 重置單位
        GmaeBossUnitCtrl.Reset();
        GmaeBossUnitCtrl.SetDef();
        GmaeBossUnitCtrl.SetKeepData(keepData);

        // 清除所有敵方彈幕
        GameDebut.ClearAllEnemyShot();
        GameObjCtrl.Instance.StopSpell();

        // 處理練習模式符卡邏輯

    }




    void SpellEndCheck()
    {
        if (IsSpellCardEndTime())
        {
            Debug.Log("SpellEndCheck");
            GameMainCtrl.Instance.gameSceneUpdateFlag |= UpdateFlag.LateSpellEnd;
        }
    }

    bool IsPressESC()
    {
        return Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.Escape)) || Input.GetKeyDown(KeyCode.JoystickButton3);
    }
    void ShowPauseSelect()
    {
        if (GameReplay.isReplayMode)
            ReplayPauseSelect.Instance.Show();
        else
            PauseSelect.Instance.Show();
    }

    bool IsSpellCardEndTime()
    {
        return GameBoss.SpellTime - GameBoss.nowUnit.uTime <= 0;
    }

    public void Pause()
    {
        if (gameSceneUpdateFlag.HasFlag(UpdateFlag.Pause)) return;
        gameSceneUpdateFlag |= UpdateFlag.Pause;
        Time.timeScale = 0;
        LoadCtrl.Instance.audioSource.Pause();
        StartCoroutine(PauseHandler());
    }

    public void UnPause()
    {
        gameSceneUpdateFlag &= ~UpdateFlag.Pause;
        Time.timeScale = 1;
        LoadCtrl.Instance.audioSource.UnPause();
    }

    public IEnumerator PauseHandler()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        while (
            PauseSelect.Instance.canvasGroup.alpha > 0 ||
            GameOverSelect.Instance.canvasGroup.alpha > 0 ||
            PracticeOverSelect.Instance.canvasGroup.alpha > 0 ||
            ReplaySelect.Instance.canvasGroup.alpha > 0 ||
            YesNoSelect.Instance.canvasGroup.alpha > 0 ||
            ReplayPauseSelect.Instance.canvasGroup.alpha > 0 ||
            ReplayOverSelect.Instance.canvasGroup.alpha > 0
        )
        {
            var time = 0;
            while (time < GameConfig.PAUSE_DELAY_TIME)
            {
                time++;
                yield return null;
            }
            yield return null;
        }
        UnPause();
    }




    public void EnterBossTime(CreateStageSetting createStageSetting, SettingBase coreSetting)
    {
        GameMainCtrl.Instance.nowGameProgressState = GameProgressState.BossTime;
        GameDebut.lateDebutByCreateSettings.Add(createStageSetting);
        if (createStageSetting.type == TypeValue.符卡)
        {
            GameMainCtrl.Instance.gameSceneUpdateFlag |= GameMainCtrl.UpdateFlag.SpellTimeRefresh;
            GameObjCtrl.Instance.OpenSpell(coreSetting);
        }
    }



}
