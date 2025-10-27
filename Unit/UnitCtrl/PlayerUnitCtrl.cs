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
using static SaveJsonData;
using static GameConfig;

public class PlayerUnitCtrl : UnitCtrlBase
{
    public PlayerCtrlObj playerCtrlObj
    {
        get => (PlayerCtrlObj)unitCtrlObj;
        set => unitCtrlObj = value;
    }
    float playerCurrentSpeed;
    bool isShift;
    // ReplayKey replayKey;
    List<KeyCode> playKeyCodes;
    public uint zTime { get; set; }

    public void PlayerReset()
    {
        playerCurrentSpeed = 0;
        isShift = false;
        zTime = 0;
        // replayKey.Reset();
        playKeyCodes.Clear();
    }


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
        else
        {
            zTime = 0;
        }
        // unitCtrlObj.objPrintContent += $"Time:{GameReplay.keyPressTime}   POS:{unitCtrlObj.transform.position.x},{unitCtrlObj.transform.position.y}\n";

    }



    public PlayerUnitCtrl(UnitCtrlObj unitCtrlObj) : base(unitCtrlObj)
    {
        playKeyCodes = new List<KeyCode>();
        PlayerReset();
    }

    public void PlayerActionCtrl()
    {

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

        if (Input.GetKey(KeyCode.P))
        {
            unitCtrlObj.FileWriteContent();
        }
    }

    public void PlayerReplayCtrl()
    {
        if (Input.GetKey(KeyCode.P))
        {
            unitCtrlObj.FileWriteContent();
        }

        if (!GameReplay.CheckPlayKeyExist())
            return;

        playKeyCodes = GameReplay.GetNowPlayKeyCodes();
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
            // unitCtrlObj.objPrintContent += $"Time:{GameReplay.keyPressTime}   Shift\n";
            GameReplay.InputSaveData.AddReplayKey(GameReplay.keyPressTime, KeyCode.LeftShift);

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
        GameReplay.InputSaveData.AddReplayKey(GameReplay.keyPressTime, KeyCode.Z);

        if (GameMainCtrl.Instance.nowGameProgressState == GameProgressState.Dialog)
            return;

        zTime++;
        foreach (var playerShotCreateStageSetting in GameSelect.playerData.playerShotCreateStageSettings)
        {

            if (
                 zTime % playerShotCreateStageSetting.coreSetting.playerShotHzTime == playerShotCreateStageSetting.coreSetting.playerShotDelayTime &&
                isShift == playerShotCreateStageSetting.coreSetting.playerShift &&
                GamePlayer.power >= playerShotCreateStageSetting.coreSetting.playerPowerNeed &&
                GamePlayer.power < playerShotCreateStageSetting.coreSetting.playerPowerNeed + 1f

            )
            {
                // unitCtrlObj.objPrintContent += $"Time:{GameReplay.keyPressTime}   Shot\n";
                unitProp.propLateDebutByCreateSettings.Add(playerShotCreateStageSetting);
            }
        }
    }

    void MoveLeft()
    {
        // unitCtrlObj.objPrintContent += $"Time:{GameReplay.keyPressTime}   ←\n";

        GameReplay.InputSaveData.AddReplayKey(GameReplay.keyPressTime, KeyCode.LeftArrow);
        if (unitCtrlObj.transform.position.x > GameConfig.PLAYER_MOVE_BORDER_LEFT)
        {
            unitCtrlObj.transform.Translate(new Vector2(-playerCurrentSpeed, 0));
        }
    }

    void MoveRight()
    {
        // unitCtrlObj.objPrintContent += $"Time:{GameReplay.keyPressTime}   →\n";
        GameReplay.InputSaveData.AddReplayKey(GameReplay.keyPressTime, KeyCode.RightArrow);
        if (unitCtrlObj.transform.position.x < GameConfig.PLAYER_MOVE_BORDER_RIGHT)
        {
            unitCtrlObj.transform.Translate(new Vector2(playerCurrentSpeed, 0));
        }
    }
    void MoveUp()
    {
        // unitCtrlObj.objPrintContent += $"Time:{GameReplay.keyPressTime}   ↑\n";
        GameReplay.InputSaveData.AddReplayKey(GameReplay.keyPressTime, KeyCode.UpArrow);
        if (unitCtrlObj.transform.position.y < GameConfig.PLAYER_MOVE_BORDER_TOP)
        {
            unitCtrlObj.transform.Translate(new Vector2(0, playerCurrentSpeed));
        }
    }

    void MoveDown()
    {
        // unitCtrlObj.objPrintContent += $"Time:{GameReplay.keyPressTime}   ↓\n";
        GameReplay.InputSaveData.AddReplayKey(GameReplay.keyPressTime, KeyCode.DownArrow);
        if (unitCtrlObj.transform.position.y > GameConfig.PLAYER_MOVE_BORDER_BOTTOM)
        {
            unitCtrlObj.transform.Translate(new Vector2(0, -playerCurrentSpeed));
        }
    }


    public void HandlePlayerHpEmpty()
    {
        // unitCtrlObj.objPrintContent += $"Time:{GameReplay.keyPressTime}   HP0\n";
        GameMainCtrl.Instance.gameSceneUpdateFlag |= GameMainCtrl.UpdateFlag.LatePlayerHpEmptyPause;
        unitCtrlObj.FileWriteContent();
    }
}
