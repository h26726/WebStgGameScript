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

public class PlayerUnitCtrl : UnitCtrlBase
{

    public GameObject core;
    public Collider2D coreCollider2D { get; set; }
    public SpriteRenderer coreSpriteRenderer { get; set; }

    float playerCurrentSpeed = 0;

    public enum PlayerAct
    {
        Base = 101,
        Rebirth = 102,
        InvinciblePlayerCtrl = 103,
        UnInvinciblePlayerCtrl = 104,
    }

    public PlayerAct playerAct { get; set; } = PlayerAct.Base;





    public override void CustomizeAwake()
    {
        core.SetActive(false);
        coreCollider2D = core.GetComponent<Collider2D>();
        coreSpriteRenderer = core.GetComponent<SpriteRenderer>();
    }

    public override void OnActive(ActCtrl actCtrl, ActCtrl parentActCtrl = null)
    {
        playerAct = (PlayerAct)actCtrl.stageSetting.Id;
        base.OnActive(actCtrl, parentActCtrl);
        if (playerAct == PlayerAct.InvinciblePlayerCtrl || playerAct == PlayerAct.UnInvinciblePlayerCtrl)
        {
            if (!GameSystem.Instance.isReplay)
            {
                GameSystem.Instance.waitPlayerKey -= PlayerActionCtrl;
                GameSystem.Instance.waitPlayerKey += PlayerActionCtrl;
            }
            else
            {
                GameSystem.Instance.waitPlayerKey -= PlayerReplayCtrl;
                GameSystem.Instance.waitPlayerKey += PlayerReplayCtrl;
            }
        }
    }

    bool isShift = false;
    uint zTime { get; set; }
    ReplayKeyClass replayKey = new ReplayKeyClass();
    List<KeyCode> playKeyCodes = new List<KeyCode>();


    public void PlayerActionCtrl()
    {

        replayKey = new ReplayKeyClass();
        replayKey.keyTime = GameSystem.Instance.keyTime;
        if (Input.GetKey(GetSetKey(KeyCode.LeftShift)) || Input.GetKey(KeyCode.Joystick1Button0))
        {
            SlowMove(true);
        }
        else
        {
            SlowMove(false);
        }

        if (Input.GetKey(GetSetKey(KeyCode.Z)) || Input.GetKey(KeyCode.Joystick1Button1))
        {
            Shot();
        }


        var CheckLeftRight = CheckLeftRightKey();
        if (CheckLeftRight[0])
        {
            MoveLeft();
        }
        else if (CheckLeftRight[1])
        {
            MoveRight();
        }

        var CheckUpDown = CheckUpDownKey();
        if (CheckUpDown[1])
        {
            MoveDown();
        }
        else if (CheckUpDown[0])
        {
            MoveUp();
        }
        if (replayKey.keyCodes.Count > 0)
            GameSystem.Instance.replaySaveData.replayKeys.Add(replayKey);
    }

    public void PlayerReplayCtrl()
    {
        if (GameSystem.Instance.playReplayKeys.Where(r => r.keyTime == GameSystem.Instance.keyTime).Count() > 1)
        {
            Debug.LogError($"KeyTime:{GameSystem.Instance.keyTime}  Repeat");
        }


        playKeyCodes = GameSystem.Instance.playReplayKeys.FirstOrDefault(r => r.keyTime == GameSystem.Instance.keyTime).keyCodes;
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

        var CheckUpDown = CheckUpDownKey();
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
            replayKey.keyCodes.Add(KeyCode.LeftShift);
            playerCurrentSpeed = GameConfig.PLAYER_MOVE_SPEED * GameConfig.PLAYER_SLOW_SPEED_RATE;
            isShift = true;
            core.SetActive(true);
        }
        else
        {
            playerCurrentSpeed = GameConfig.PLAYER_MOVE_SPEED;
            isShift = false;
            core.SetActive(false);
        }
    }

    void Shot()
    {
        if (GameSystem.Instance.nowGameProgressState == GameProgressState.Dialog)
            return;
        replayKey.keyCodes.Add(KeyCode.Z);
        zTime++;
        foreach (var _playerShotCreateStageSetting in GameSystem.Instance.playerShotCreateStageSettings)
        {

            if (
                 zTime % _playerShotCreateStageSetting.coreSetting.playerShotHzTime == _playerShotCreateStageSetting.coreSetting.playerShotDelayTime &&
                isShift == _playerShotCreateStageSetting.coreSetting.playerShift.Value &&
                GameSystem.Instance.playePower >= _playerShotCreateStageSetting.coreSetting.playerPowerNeed &&
                GameSystem.Instance.playePower < _playerShotCreateStageSetting.coreSetting.playerPowerNeed + 1f
            )
            {
                GameSystem.Instance.waitCreates += () =>
                {
                    GameSystem.Instance.CreateUnit(_playerShotCreateStageSetting, this);
                };
            }
        }
    }

    void MoveLeft()
    {
        replayKey.keyCodes.Add(KeyCode.LeftArrow);
        if (transform.position.x > GameConfig.PLAYER_MOVE_BORDER_LEFT)
        {
            transform.Translate(new Vector2(-playerCurrentSpeed, 0));
        }
    }

    void MoveRight()
    {
        replayKey.keyCodes.Add(KeyCode.RightArrow);
        if (transform.position.x < GameConfig.PLAYER_MOVE_BORDER_RIGHT)
        {
            transform.Translate(new Vector2(playerCurrentSpeed, 0));
        }
    }
    void MoveUp()
    {

        replayKey.keyCodes.Add(KeyCode.UpArrow);
        if (transform.position.y < GameConfig.PLAYER_MOVE_BORDER_TOP)
        {
            transform.Translate(new Vector2(0, playerCurrentSpeed));
        }
    }

    void MoveDown()
    {
        replayKey.keyCodes.Add(KeyCode.DownArrow);
        if (transform.position.y > GameConfig.PLAYER_MOVE_BORDER_BOTTOM)
        {
            transform.Translate(new Vector2(0, -playerCurrentSpeed));
        }
    }


    public override bool CheckOutBorder(Vector2? Pos = null)
    {
        return false;
    }

    public override void HandleDead()
    {
        HandlePlayerDead();
        ClearEvent();
        PlayDeadAnim();
        AddPrintContent($"HandlePlayerDead {Environment.NewLine}");
    }

    public void HandlePlayerDead()
    {
        unitProp.isInvincible = true;
        core.SetActive(false);
        if (playerAct != PlayerAct.UnInvinciblePlayerCtrl)
        {
            Debug.LogError("_PlayerAct != PlayerAct.UnInvinciblePlayerCtrl");
        }

        if (!GameSystem.Instance.isReplay)
        {
            GameSystem.Instance.waitPlayerKey -= PlayerActionCtrl;
        }
        else
        {
            GameSystem.Instance.waitPlayerKey -= PlayerReplayCtrl;
        }

        foreach (var unit in GameSystem.Instance.takeDict.Values)
        {
            foreach (var actCtrlPair in unit.actCtrlDict)
            {
                var actCtrl = actCtrlPair.Value;
                foreach (var callRule in actCtrl.callRules)
                {
                    if (callRule.callPlayerDead == true)
                    {
                        ExtHandle(callRule, actCtrl);
                    }
                }
            }

        }
        GameSystem.Instance.playerLife--;
        GameSystem.Instance.playePower -= 1f;
        if (GameSystem.Instance.playePower < PLAYER_BIRTH_POWER)
            GameSystem.Instance.playePower = PLAYER_BIRTH_POWER;
    }

    public override void TriggerRestore()
    {

    }

    public override void DeadAnimEndHandle()
    {
        if (playerAct != PlayerAct.UnInvinciblePlayerCtrl)
        {
            Debug.LogError("_PlayerAct != PlayerAct.UnInvinciblePlayerCtrl");
        }
        if (GameSystem.Instance.playerLife == 0)
        {
            HandlePlayerHpEmpty();
        }
        core.SetActive(false);
        OnActive(actCtrlDict[(int)PlayerAct.Base]);
    }

    public void HandlePlayerHpEmpty()
    {
        if (GameSystem.Instance.isReplay)
        {
            GameSystem.Instance.SetPlayerItem(true);
        }
        else
        {
            GameSystem.Instance.Pause();
            if (GameSystem.Instance.isPractice)
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
