using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static CommonData;
using static CommonFunc;
using static PlayerKeyCtrl;
using static GameConfig;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using System.IO;
public partial class UnitCtrlBase
{
    protected uint uTime { get; set; }
    public Dictionary<uint, Vector2> recordPosDict = new Dictionary<uint, Vector2>();
    public Dictionary<uint, float> recordAngleDict = new Dictionary<uint, float>();
    public void Active(SettingBase setting)
    {
        CustomizeActive(setting);

        HandleMovePos(setting);
        HandleRotation(setting);
        HandleRelatPos(setting);
        HandleChildRotation(setting);
        HandlePlayAni(setting);
        HandleSprite(setting);
        HandleRecord(setting);
    }

    public virtual void CustomizeActive(SettingBase setting)
    {

    }



    void HandleMovePos(SettingBase setting)
    {
        if (setting.movePos != null)
        {
            var getPos = GetPos(setting.movePos);
            if (setting.isIn == true && CheckOutBorder(getPos))
            {
                TriggerRestore();
            }
            else
            {
                transform.position = new Vector3(getPos.x, getPos.y, zIndex);
            }
        }
    }



    void HandleRotation(SettingBase setting)
    {
        if (setting.rotateZ != null)
        {
            SetRotateZ(GetAngle(setting.rotateZ, out _));
        }
    }

    void HandleRelatPos(SettingBase setting)
    {
        if (setting.relatPos != null && setting.relatPos.Count > 0 && setting.relatPos[0].Id != null)
        {
            var id = setting.relatPos[0].Id.Value;
            var relatUnit = GetUnitCtrlById(this, id);
            if (relatUnit.childFrame != null)
            {
                transform.SetParent(relatUnit.childFrame);
                transform.SetAsLastSibling();
                relatUnit.childFrameUnits.Add(this);
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                Debug.LogError($"id:{id}  not childFrame");
            }

        }
    }

    void HandleChildRotation(SettingBase setting)
    {
        if (setting.childRotateZ != null)
        {
            SetChildRotateZ(GetAngle(setting.childRotateZ, out _));
        }
    }

    void HandlePlayAni(SettingBase setting)
    {
        if (setting.ani != null && animator != null)
        {
            if (animator.HasState(0, Animator.StringToHash(setting.ani)))
            {
                animator.Play(setting.ani, 0, 0);
            }
            else
            {
                Debug.LogError($"Animator state '{setting.ani}' not found in layer {0}.");
            }
        }
    }

    void HandleSprite(SettingBase setting)
    {
        if (setting.sprite != null)
        {
            spriteRenderer.sprite = LoadingCtrl.Instance.pool.spritePoolList.Where(r => r.name == setting.sprite).FirstOrDefault().sprite;
        }
    }

    void HandleRecord(SettingBase setting)
    {
        if (setting.recordAngle != null && setting.recordAngleId != null)
        {
            recordAngleDict.Add(setting.recordAngleId.Value, GetAngle(setting.recordAngle, out _));
        }
        if (setting.recordPos != null && setting.recordPosId != null)
        {
            recordPosDict.Add(setting.recordPosId.Value, GetPos(setting.recordPos));
        }
    }

    public Vector2 GetPos(List<Pos> pos)
    {
        Vector2 Pos = GameConfig.CENTER;
        foreach (var posData in pos)
        {
            if (posData.Id != null)
            {
                if ((IdVal)posData.Id.Value == IdVal.XCenter)
                {
                    Pos = new Vector2(GameConfig.CENTER.x, Pos.y);
                }
                else if ((IdVal)posData.Id.Value == IdVal.YCenter)
                {
                    Pos = new Vector2(Pos.x, GameConfig.CENTER.y);
                }
                else
                {
                    Pos = GetPosById(posData.Id.Value);
                }
            }
            else if (posData.point != null) Pos += posData.point.Value;
            else if (posData.ADangle != null && posData.ADdistance != null)
            {
                var adAngle = GetAngle(posData.ADangle, out _);
                var adDis = posData.ADdistance.Value;
                Pos += CalPos(adAngle, adDis);
            }
        }
        return Pos;
    }

    public Vector2 GetPosById(uint Id)
    {
        Vector2 Pos = Vector2.zero;
        if (Id > RECORD_TMP_ID_MIN)
        {
            Pos = recordPosDict[Id];
        }
        else if ((IdVal)Id == IdVal.Player)
        {
            if (GameSystem.Instance.playerUnitCtrl == null || CheckOutBorderDis((Vector2)GameSystem.Instance.playerUnitCtrl.GetTransformPos(), 0f))
                return GameConfig.CENTER + GameConfig.PLAYER_DEAD_TRACE_POS;
            Pos = GameSystem.Instance.playerUnitCtrl.GetTransformPos();
        }
        else if ((IdVal)Id == IdVal.Boss)
        {
            if (GameSystem.Instance.nowEnemyBoss == null)
                return GameConfig.CENTER + GameConfig.BOSS_HOMING_POS;
            Pos = GameSystem.Instance.nowEnemyBoss.GetTransformPos();
        }
        else if ((IdVal)Id == IdVal.Center)
        {
            Pos = GameConfig.CENTER;
        }
        else if ((IdVal)Id == IdVal.BossHome)
        {
            Pos = GameConfig.CENTER + GameConfig.BOSS_HOMING_POS;
        }
        else
        {
            var unitCtrlData = GetUnitCtrlDataById(this, Id);
            Pos = unitCtrlData == null ? GameConfig.CENTER : unitCtrlData.GetTransformPos();
        }

        return Pos;
    }

    public float GetAngle(List<AngleSet> angle, out bool isNewAngle)
    {
        float Angle = 0;
        isNewAngle = true;
        foreach (var angleData in angle)
        {

            if (angleData.angle != null)
            {
                Angle += angleData.angle.Value;
                isNewAngle = false;
            }
            else if (angleData.pos1 != null && angleData.pos2 != null)
            {
                Angle += CalAngle(
                    GetPos(new List<Pos>() { angleData.pos1 }),
                    GetPos(new List<Pos>() { angleData.pos2 })
                );
            }
            else if (angleData.IdRotateZ != null)
            {
                var unitCtrlData = GetUnitCtrlDataById(this, angleData.IdRotateZ.Value);
                if (unitCtrlData != null)
                {
                    Angle += unitCtrlData.GetRotateZ();
                }
            }
            else if (angleData.IdMoveAngle != null && angleData.IdMoveAngle.Length == 2)
            {
                var unitCtrlData = GetUnitCtrlDataById(this, angleData.IdMoveAngle[0]);
                if (unitCtrlData != null && unitCtrlData.TryGetActionMoveAngle(angleData.IdMoveAngle[1], out var actionMoveAngle))
                {
                    Angle += actionMoveAngle;
                }
                else
                {
                    Debug.LogError($"angleData.IdMoveAngle:{angleData.IdMoveAngle[0]} Not Get");
                }
            }
            else if (angleData.Ids != null && angleData.Ids.Length == 2)
            {
                Angle += CalAngle(
                    GetPosById(angleData.Ids[0]),
                    GetPosById(angleData.Ids[1])
                );
            }
            else if (angleData.recordId != null && angleData.recordId > RECORD_TMP_ID_MIN)
            {
                Angle += recordAngleDict[angleData.recordId.Value];
            }
        }
        return Angle;
    }

}


