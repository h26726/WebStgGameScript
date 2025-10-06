using UnityEngine;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static GameConfig;
using static PlayerKeyHelper;
using static PlayerSaveData;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public abstract class UnitCtrlObj : MonoBehaviour
{
    public static float objLast_zIndex { get; set; }
    public Dictionary<uint, Vector2> objRecordPosDict = new Dictionary<uint, Vector2>();
    public Dictionary<uint, float> objRecordAngleDict = new Dictionary<uint, float>();
    public Dictionary<uint, float> objActionMoveAngleDict = new Dictionary<uint, float>();
    float obj_zIndex { get; set; }
    string obj_zCode { get; set; }
    public Vector2 beforeTransformPos { get; set; }
    public uint coreSettingId { get; set; }


    public UnitCtrlObj parent { get; set; }
    public Animator animator { get; set; }
    public Transform mainObjTransform { get; set; }
    public Transform childFrameTransform { get; set; }
    public SpriteRenderer spriteRenderer { get; set; }

    public Text actionTimeText;
    string objPrintContent = "";


    void Awake()
    {
        if (transform.childCount < 2 || transform.GetChild(0).transform.childCount < 1)
        {
            Debug.LogError($"{GetType().Name} childCount Error");
        }
        gameObject.transform.position = new Vector3(GameConfig.POOL_RESET_POS.x, GameConfig.POOL_RESET_POS.y, obj_zIndex);

        spriteRenderer = transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
        mainObjTransform = transform.GetChild(0).transform;
        childFrameTransform = transform.GetChild(1).transform;
        animator = GetComponent<Animator>();
        actionTimeText = GetComponent<Text>();
        if (this is PlayerCtrlObj)
        {
            var playerCtrlObj = this as PlayerCtrlObj;
            playerCtrlObj.PlayerAwake();
        }
    }


    public void SetBeforePos()
    {
        beforeTransformPos = transform.position;
    }

    public void AddActionMoveAngleDict(uint Id, float moveAngle)
    {
        objActionMoveAngleDict[Id] = moveAngle;
    }


    public bool IsParent(UnitCtrlObj target)
    {
        return object.ReferenceEquals(target, parent);
    }

    public void SetParent(UnitCtrlObj parent)
    {
        this.parent = parent;
        ReceiveParentRecord(parent);
    }


    public void Reset()
    {
        if (animator != null)
            animator.Play("Idle");

        objRecordPosDict.Clear();
        objRecordAngleDict.Clear();
        objActionMoveAngleDict.Clear();
        obj_zIndex = 0;
        obj_zCode = null;

    }

    public void EnableUnit()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    public void CloseUnit()
    {
        LeaveRelatParent();
        SetRotateZ(0);
        SetChildRotateZ(0);
        transform.position = new Vector3(GameConfig.POOL_RESET_POS.x, GameConfig.POOL_RESET_POS.y, obj_zIndex);
        gameObject.SetActive(false);
        transform.SetAsFirstSibling();
    }


    public void Set_zIndex_zCode(uint coreSettingId)
    {
        this.obj_zIndex = objLast_zIndex;
        objLast_zIndex -= GameConfig.Z_INDEX_REDUCE;

        this.obj_zCode = coreSettingId.ToString() + "_z" + obj_zIndex * 1000f;
        this.coreSettingId = coreSettingId;
    }



    public void PlayBirthAniInit(WaitBirthAniManager birthAniManager)
    {
        var stageSetting = birthAniManager.setting;
        var birthDurTime = birthAniManager.birthDurTime;
        var birthAniStart = birthAniManager.birthAniStart;
        var birthAniSpeed = birthAniManager.birthAniSpeed;
        if (!Mathf.Approximately(birthDurTime, 0f))
            PlayBirthAni(birthAniStart);

        if (stageSetting.rotateIsMoveAngle == true && stageSetting.moveAngle != null)
        {
            SetRotateZ(stageSetting.moveAngle);
        }
        else if (stageSetting.rotateZ != null)
        {
            SetRotateZ(stageSetting.rotateZ);
        }
        animator.speed = birthAniSpeed;
    }

    public void PlayBirthAni(float birthAniStart)
    {
        animator.Play("Idle", 0, birthAniStart);
    }

    public void PlayBirthFinishAni()
    {
        animator.Play("Show", 0, 0);
    }

    public void PlayAniName(string ani)
    {
        if (animator != null)
        {
            animator.Play(ani, 0, 0);
        }
        else
        {
            Debug.LogError($"Animator state '{ani}' not found in layer {0}.");
        }
    }

    public void PlayDeadAni()
    {
        if (animator != null)
            animator.Play("Dead");
    }

    public void ChangeSprite(string spriteName)
    {
        var spritePoolObj = LoadCtrl.Instance.pool.spritePoolList.FirstOrDefault(r => r.name == spriteName);
        if (spritePoolObj != null && spritePoolObj.sprite != null)
        {
            spriteRenderer.sprite = spritePoolObj.sprite;
        }
        else
        {
            Debug.LogError($"spriteName '{spriteName}' not found .");
        }



    }

    public void LeaveRelatParent()
    {
        transform.SetParent(LoadCtrl.Instance.pool.transform);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
    public void BackPoolPos()
    {
        LeaveRelatParent();
        if (animator != null)
            animator.Play("Idle");
        gameObject.SetActive(false);
        transform.SetAsFirstSibling();
        transform.position = new Vector3(GameConfig.POOL_RESET_POS.x, GameConfig.POOL_RESET_POS.y, obj_zIndex);
    }


    public float GetRotateZ()
    {
        if (mainObjTransform == null)
            return 0;
        return mainObjTransform.rotation.eulerAngles.z;
    }
    public float GetChildRotateZ()
    {
        if (childFrameTransform == null)
            return 0;
        return childFrameTransform.rotation.eulerAngles.z;
    }

    public void SetRotateZ(float RotateZ)
    {
        if (mainObjTransform == null)
            return;
        mainObjTransform.rotation = Quaternion.Euler(0f, 0f, RotateZ);
    }

    public void SetRotateZ(List<AngleSet> angleSets)
    {
        var angle = GetAngle(angleSets, out var isNewAngle);
        if (isNewAngle) SetRotateZ(angle);
        else SetRotateZ(angle);
    }

    public void SetAddRotateZ(List<AngleSet> angleSets)
    {
        var angle = GetAngle(angleSets, out var isNewAngle);
        if (isNewAngle) SetRotateZ(angle);
        else SetRotateZ(GetRotateZ() + angle / 60);
    }


    public void SetChildRotateZ(float ChildRotateZ)
    {
        if (childFrameTransform == null)
            return;
        childFrameTransform.rotation = Quaternion.Euler(0f, 0f, ChildRotateZ);
    }

    public void SetChildRotateZ(List<AngleSet> angleSets)
    {
        var angle = GetAngle(angleSets, out var isNewAngle);
        if (isNewAngle) SetChildRotateZ(angle);
        else SetChildRotateZ(angle);
    }

    public void SetChildAddRotateZ(List<AngleSet> angleSets)
    {
        var angle = GetAngle(angleSets, out var isNewAngle);
        if (isNewAngle) SetChildRotateZ(angle);
        else SetChildRotateZ(GetChildRotateZ() + angle / 60);
    }

    public bool IsOutBorder(float restoreDistance, Vector2? Pos = null)
    {
        if (Pos == null)
        {
            Pos = transform.position;
        }
        return IsOutBorderDis(Pos, restoreDistance);
    }

    public static bool IsOutBorderDis(Vector2? Pos, float dis)
    {
        if (Pos.Value.x < GameConfig.BORDER_LEFT - dis) return true;
        else if (Pos.Value.x > GameConfig.BORDER_RIGHT + dis) return true;
        else if (Pos.Value.y < GameConfig.BD_BOTTOM - dis) return true;
        else if (Pos.Value.y > GameConfig.BD_TOP + dis) return true;
        return false;
    }




    public void MovePos(Vector2 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, obj_zIndex);
    }

    public void MoveTranslate(Vector2 moveVector)
    {
        transform.Translate(moveVector);
    }






    public void TryObjAddRecord(SettingBase setting)
    {
        if (setting.recordAngle != null && setting.recordAngleId != null)
        {
            objRecordAngleDict.Add(setting.recordAngleId.Value, GetAngle(setting.recordAngle, out _));
        }
        if (setting.recordPos != null && setting.recordPosId != null)
        {
            objRecordPosDict.Add(setting.recordPosId.Value, GetPos(setting.recordPos));
        }
    }

    public void InsertRelatChild(UnitCtrlObj insertUnitCtrlObj)
    {
        if (childFrameTransform != null)
        {
            insertUnitCtrlObj.transform.SetParent(childFrameTransform);
            insertUnitCtrlObj.transform.SetAsLastSibling();
            insertUnitCtrlObj.SetRotateZ(0);
        }
        else
        {
            Debug.LogError($"id:{insertUnitCtrlObj.coreSettingId}  not childFrame");
        }
    }


    public Vector2 GetPos(List<Pos> pos)
    {
        Vector2 Pos = GameConfig.CENTER;
        foreach (var posData in pos)
        {
            if (posData.Id != null)
            {
                var id = posData.Id.Value;
                var idVal = (IdVal)id;
                if (idVal == IdVal.XCenter)
                {
                    Pos = new Vector2(GameConfig.CENTER.x, Pos.y);
                }
                else if (idVal == IdVal.YCenter)
                {
                    Pos = new Vector2(Pos.x, GameConfig.CENTER.y);
                }
                else
                {
                    Pos = GetPos(id);
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

    public Vector2 GetPos(uint id)
    {
        var idVal = (IdVal)id;
        if (idVal == IdVal.BossHome)
        {
            return GameConfig.CENTER + GameConfig.BOSS_HOMING_POS;
        }
        else if (idVal == IdVal.Center)
        {
            return GameConfig.CENTER;
        }
        var unitObj = GetUnitObj(id);
        if (unitObj == null)
            return GameConfig.CENTER;
        return unitObj.transform.position;
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
                var unitCtrlObj = GetUnitObj(angleData.IdRotateZ.Value);
                if (unitCtrlObj != null)
                {
                    Angle += unitCtrlObj.GetRotateZ();
                }
            }
            else if (angleData.IdMoveAngle != null && angleData.IdMoveAngle.Length == 2)
            {
                var targetUnitCtrlObj = GetUnitObj(angleData.IdMoveAngle[0]);
                if (targetUnitCtrlObj != null && targetUnitCtrlObj.TryGetActionMoveAngle(angleData.IdMoveAngle[1], out var actionMoveAngle))
                {
                    Angle += actionMoveAngle;
                }
                else
                {
                    Debug.LogError($"angleData.IdMoveAngle:{angleData.IdMoveAngle[1]} Not Get");
                }
            }
            else if (angleData.Ids != null && angleData.Ids.Length == 2)
            {
                Angle += CalAngle(
                    GetPos(angleData.Ids[0]),
                    GetPos(angleData.Ids[1])
                );
            }
            else if (angleData.recordId != null && angleData.recordId > RECORD_TMP_ID_MIN)
            {
                Angle += objRecordAngleDict[angleData.recordId.Value];
            }
        }
        return Angle;
    }


    public UnitCtrlObj GetUnitObj(uint Id)
    {
        if ((IdVal)Id == IdVal.Self)
        {
            return this;
        }
        if ((IdVal)Id == IdVal.Parent)
        {
            return parent;
        }
        else
        {
            return GetOutsideUnitObj(Id);
        }
    }



    public static UnitCtrlObj GetOutsideUnitObj(uint Id)
    {
        if ((IdVal)Id == IdVal.Player)
        {
            return GamePlayer.nowUnit.unitCtrlObj;
        }
        else if ((IdVal)Id == IdVal.Boss)
        {
            return GameBoss.nowUnit.unitCtrlObj;
        }
        else
        {
            var unitCtrl = UnitCtrlBase.GetOutSideUnit(Id);
            if (unitCtrl != null)
                return unitCtrl.unitCtrlObj;
        }
        return null;
    }

    public bool TryGetActionMoveAngle(uint coreId, out float angle)
    {
        angle = 0f;
        if (!objActionMoveAngleDict.ContainsKey(coreId))
            return false;

        angle = objActionMoveAngleDict[coreId];
        return true;
    }

    public void PrintCreate()
    {
        // printContent += $"zCode: {zCode}  {Environment.NewLine}";
        // if (parentUnitCtrl != null)
        //     printContent += $"parentName: {parentUnitCtrl.zCode}  {Environment.NewLine}";
        // else
        //     printContent += $"parentName: null  {Environment.NewLine}";
        // printContent += createSetting.Print();
    }


    public void AddPrintContent(string content)
    {
        // printContent += content;
    }
    public string FileWriteContent()
    {
        File.WriteAllText(LoadCtrl.Instance.unitLogPath + obj_zCode + ".txt", objPrintContent);
        return objPrintContent;
    }

    void ReceiveParentRecord(UnitCtrlObj parentUnitObj)
    {
        if (parentUnitObj != null)
        {
            if (parentUnitObj.objRecordAngleDict.Count > 0)
                objRecordAngleDict = new Dictionary<uint, float>(parentUnitObj.objRecordAngleDict);
            if (parentUnitObj.objRecordPosDict.Count > 0)
                objRecordPosDict = new Dictionary<uint, Vector2>(parentUnitObj.objRecordPosDict);
        }
    }





}

