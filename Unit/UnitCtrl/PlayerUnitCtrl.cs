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

public class PlayerUnitCtrl : UnitCtrlBase
{
    public PlayerCtrlObj playerCtrlObj
    {
        get => (PlayerCtrlObj)unitCtrlObj;
        set => unitCtrlObj = value;
    }
    float playerCurrentSpeed = 0;


    public void PlayerUpdateHandler()
    {
        if (!unitProp.isDead && (actCtrlDict[(uint)PlayerAct.InvinciblePlayerCtrl].isRun || actCtrlDict[(uint)PlayerAct.UnInvinciblePlayerCtrl].isRun))
        {
            if (!GameReplay.isReplayMode)
            {
                PlayerActionCtrl();
            }
            else
            {
                PlayerReplayCtrl();
            }
        }
    }

    bool isShift = false;
    uint zTime { get; set; }
    ReplayKey replayKey = new ReplayKey();
    List<KeyCode> playKeyCodes = new List<KeyCode>();

    public PlayerUnitCtrl(UnitCtrlObj unitCtrlObj) : base(unitCtrlObj)
    {
    }

    public void PlayerActionCtrl()
    {

        replayKey = new ReplayKey();
        replayKey.keyPressTime =GameReplay.keyPressTime;
        if (Input.GetKey(TransferToPlayerSetKey(KeyCode.LeftShift)) || Input.GetKey(KeyCode.Joystick1Button0))
        {
            SlowMove(true);
        }
        else
        {
            SlowMove(false);
        }

        if (Input.GetKey(TransferToPlayerSetKey(KeyCode.Z)) || Input.GetKey(KeyCode.Joystick1Button1))
        {
            Shot();
        }


        var CheckLeftRight = IsKey_LeftRight();
        if (CheckLeftRight[0])
        {
            MoveLeft();
        }
        else if (CheckLeftRight[1])
        {
            MoveRight();
        }

        var CheckUpDown = IsKey_UpDown();
        if (CheckUpDown[1])
        {
            MoveDown();
        }
        else if (CheckUpDown[0])
        {
            MoveUp();
        }
        if (replayKey.pressKeyCodes.Count > 0)
            GameReplay.InputSaveData.replayKeys.Add(replayKey);
    }

    public void PlayerReplayCtrl()
    {
        if (GameReplay.playKeys.Where(r => r.keyPressTime ==GameReplay.keyPressTime).Count() > 1)
        {
            Debug.LogError($"KeyPressTime:{GameReplay.keyPressTime}  Repeat");
        }


        playKeyCodes = GameReplay.playKeys.FirstOrDefault(r => r.keyPressTime ==GameReplay.keyPressTime).pressKeyCodes;
        if (playKeyCodes.Contains(KeyCode.LeftShift))
        {
            SlowMove(true);
        }
        else
        {
            SlowMove(false);
        }

        if (playKeyCodes.Contains(KeyCode.Z))
        {
            Shot();
        }


        if (playKeyCodes.Contains(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        else if (playKeyCodes.Contains(KeyCode.RightArrow))
        {
            MoveRight();
        }

        var CheckUpDown = IsKey_UpDown();
        if (playKeyCodes.Contains(KeyCode.DownArrow))
        {
            MoveDown();
        }
        else if (playKeyCodes.Contains(KeyCode.UpArrow))
        {
            MoveUp();
        }
    }

    void SlowMove(bool slow)
    {
        if (slow)
        {
            replayKey.pressKeyCodes.Add(KeyCode.LeftShift);
            playerCurrentSpeed = GameConfig.PLAYER_MOVE_SPEED * GameConfig.PLAYER_SLOW_SPEED_RATE;
            isShift = true;
            playerCtrlObj.core.SetActive(true);
        }
        else
        {
            playerCurrentSpeed = GameConfig.PLAYER_MOVE_SPEED;
            isShift = false;
            playerCtrlObj.core.SetActive(false);
        }
    }

    void Shot()
    {
        if (GameMainCtrl.Instance.nowGameProgressState == GameProgressState.Dialog)
            return;
        replayKey.pressKeyCodes.Add(KeyCode.Z);
        zTime++;
        foreach (var playerShotCreateStageSetting in GameSelect.playerData.playerShotCreateStageSettings)
        {

            if (
                 zTime % playerShotCreateStageSetting.coreSetting.playerShotHzTime == playerShotCreateStageSetting.coreSetting.playerShotDelayTime &&
                isShift == playerShotCreateStageSetting.coreSetting.playerShift.Value &&
                GamePlayer.power >= playerShotCreateStageSetting.coreSetting.playerPowerNeed &&
                GamePlayer.power < playerShotCreateStageSetting.coreSetting.playerPowerNeed + 1f
            
            )
            {
                unitProp.propWaitDebutByCreateSettings.Add(playerShotCreateStageSetting);
            }
        }
    }

    void MoveLeft()
    {
        replayKey.pressKeyCodes.Add(KeyCode.LeftArrow);
        if (unitCtrlObj.transform.position.x > GameConfig.PLAYER_MOVE_BORDER_LEFT)
        {
            unitCtrlObj.transform.Translate(new Vector2(-playerCurrentSpeed, 0));
        }
    }

    void MoveRight()
    {
        replayKey.pressKeyCodes.Add(KeyCode.RightArrow);
        if (unitCtrlObj.transform.position.x < GameConfig.PLAYER_MOVE_BORDER_RIGHT)
        {
            unitCtrlObj.transform.Translate(new Vector2(playerCurrentSpeed, 0));
        }
    }
    void MoveUp()
    {
        replayKey.pressKeyCodes.Add(KeyCode.UpArrow);
        if (unitCtrlObj.transform.position.y < GameConfig.PLAYER_MOVE_BORDER_TOP)
        {
            unitCtrlObj.transform.Translate(new Vector2(0, playerCurrentSpeed));
        }
    }

    void MoveDown()
    {
        replayKey.pressKeyCodes.Add(KeyCode.DownArrow);
        if (unitCtrlObj.transform.position.y > GameConfig.PLAYER_MOVE_BORDER_BOTTOM)
        {
            unitCtrlObj.transform.Translate(new Vector2(0, -playerCurrentSpeed));
        }
    }


    public void HandlePlayerHpEmpty()
    {
        if (GameReplay.isReplayMode)
        {
            GamePlayer.SetDef();
        }
        else
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
    }
}
