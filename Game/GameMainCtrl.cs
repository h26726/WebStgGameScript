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

public partial class GameMainCtrl : SingletonBase<GameMainCtrl>
{
    public enum UpdateFlag
    {
        None = 0,
        SpellTimeRefresh = 1 << 0,
        WaitPracticeBossDead = 1 << 1,
        WaitSpellEnd = 1 << 2,
        Pause = 1 << 3,
    }
    public UpdateFlag gameSceneUpdateFlag = UpdateFlag.None;

    public GameProgressState nowGameProgressState;
    public uint PracticeBossDeadCalTime;


    public void Init()
    {
        nowGameProgressState = GameProgressState.Stage;
        PlayerSaveData.score = 0;
        UnitCtrlObj.objLast_zIndex = GameConfig.Z_INDEX_TOP;
        GameDebut.Init();
        GamePlayer.SetDef();
        GameBoss.SetDef();
        GameProgressStageCtrl.Init();
        GameReplay.Init();
        PracticeBossDeadCalTime = 0;
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
        if (nowGameProgressState == GameProgressState.Dialog)
        {
            DialogCtrl.nowInstance?.UpdateHandler();
        }
        GameDebut.UpdateHandler();

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

            if (gameSceneUpdateFlag.HasFlag(UpdateFlag.WaitSpellEnd))
            {
                gameSceneUpdateFlag &= ~UpdateFlag.WaitSpellEnd;
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
        Debug.Log("SpellEnd");

        if (GameBoss.nowUnit == null)
        {
            Debug.LogError("GameBoss.nowUnit == null");
            return;
        }
        gameSceneUpdateFlag &= ~UpdateFlag.SpellTimeRefresh;

        var givePowerNum = GameBoss.nowUnit.coreSetting.powerGive == null ? 0 : GameBoss.nowUnit.coreSetting.powerGive.Value;
        GameBoss.nowUnit.Reset();//注意會連givePower內容也清除
        GameBoss.nowUnit.GivePower(givePowerNum);
        GameDebut.ClearAllEnemyShot();
        GameObjCtrl.Instance.StopSpell();

        if (GameSelect.isPracticeMode && GameBoss.nowUnit.coreSetting.type == TypeValue.符卡)
        {
            GameBoss.nowUnit.enemyProp.isTriggerDead = true;
            gameSceneUpdateFlag |= UpdateFlag.WaitPracticeBossDead;
        }
        else
        {
            nowGameProgressState = GameProgressState.Stage;
        }


    }





    void SpellEndCheck()
    {
        if (IsSpellCardEndTime())
        {
            Debug.Log("SpellEndCheck");
            GameMainCtrl.Instance.gameSceneUpdateFlag |= UpdateFlag.WaitSpellEnd;
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
        if (!gameSceneUpdateFlag.HasFlag(UpdateFlag.Pause)) return;
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
        GameDebut.waitDebutByCreateSettings.Add(createStageSetting);
        if (createStageSetting.type == TypeValue.符卡)
        {
            GameMainCtrl.Instance.gameSceneUpdateFlag |= GameMainCtrl.UpdateFlag.SpellTimeRefresh;
            GameObjCtrl.Instance.OpenSpell(coreSetting);
        }
    }



}
