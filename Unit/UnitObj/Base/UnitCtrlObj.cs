using UnityEngine;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static GameConfig;
using static PlayerKeyHelper;
using static SaveJsonData;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public abstract class UnitCtrlObj : MonoBehaviour
{
    public Dictionary<uint, Vector2> objRecordPosDict;
    public Dictionary<uint, float> objRecordAngleDict;
    public Dictionary<uint, float> objActionMoveAngleDict;
    float obj_zIndex { get; set; }
    int obj_zCodeHash { get; set; }
    public uint coreSettingId { get; set; }
    public Vector2 beforeTransformPos { get; set; }
    public string objPrintContent;


    public UnitCtrlObj parent { get; set; }
    public Animator animator { get; set; }
    public Transform mainObjTransform { get; set; }
    public CollisionCtrlBase collisionCtrl { get; set; }
    public Transform childFrameTransform { get; set; }
    public SpriteRenderer spriteRenderer { get; set; }

    public Text actionTimeText { get; set; }

    List<Pos> tmpPoses1;
    List<Pos> tmpPoses2;
    List<AngleSet> tmpAngles;
    private Vector3 tmpPos; // class 層級共用

    void Awake()
    {
        if (transform.childCount < 2 || transform.GetChild(0).transform.childCount < 1)
        {
            Debug.LogError($"{GetType().Name} childCount Error");
        }
        gameObject.transform.position = new Vector3(GameConfig.POOL_RESET_POS.x, GameConfig.POOL_RESET_POS.y, obj_zIndex);

        spriteRenderer = transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
        mainObjTransform = transform.GetChild(0).transform;
        collisionCtrl = transform.GetChild(0).GetComponent<CollisionCtrlBase>();
        childFrameTransform = transform.GetChild(1).transform;
        animator = GetComponent<Animator>();
        actionTimeText = GetComponent<Text>();

        objRecordPosDict = new Dictionary<uint, Vector2>();
        objRecordAngleDict = new Dictionary<uint, float>();
        objActionMoveAngleDict = new Dictionary<uint, float>();
        tmpPoses1 = new List<Pos>() { null };
        tmpPoses2 = new List<Pos>() { null }; ;
        tmpAngles = new List<AngleSet>();
        if (this is PlayerCtrlObj)
        {
            var playerCtrlObj = this as PlayerCtrlObj;
            playerCtrlObj.PlayerAwake();

        }
    }

    public void Reset()
    {
        if (animator != null)
            animator.Play("Idle");

        obj_zIndex = 0;
        obj_zCodeHash = 0;
        coreSettingId = 0;
        beforeTransformPos = Vector2.zero;
        objPrintContent = "";
        parent = null;
        objRecordPosDict.Clear();
        objRecordAngleDict.Clear();
        objActionMoveAngleDict.Clear();
        tmpPoses1[0] = null;
        tmpPoses2[0] = null;
        tmpAngles.Clear();
        if (this is PlayerCtrlObj)
        {
            var playerCtrlObj = this as PlayerCtrlObj;
            playerCtrlObj.PlayerCtrlObjReset();
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




    public void EnableUnit()
    {
        gameObject.SetActive(true);
        // transform.SetAsLastSibling();
    }

    public void CloseUnit()
    {
        LeaveRelatParent();
        SetRotateZ(0);
        SetChildRotateZ(0);
        transform.position = new Vector3(GameConfig.POOL_RESET_POS.x, GameConfig.POOL_RESET_POS.y, obj_zIndex);
        gameObject.SetActive(false);
        // transform.SetAsFirstSibling();
    }


    public void Set_zIndex_zCode(uint coreSettingId)
    {
        this.obj_zIndex = GameMainCtrl.Instance.Get_zIndex();
        this.obj_zCodeHash = (int)(coreSettingId * 1000000 + (int)(obj_zIndex * 1000));
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

        if (stageSetting.rotateIsMoveAngle == BoolState.True && stageSetting.moveAngle != null)
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
        spriteRenderer.sprite = ObjectPoolCtrl.Instance.spriteDict[spriteName];
    }

    public void LeaveRelatParent()
    {
        transform.SetParent(LoadCtrl.Instance.pool.transform);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
    //原版本 保留
    // public float GetRotateZ()
    // {
    //     if (mainObjTransform == null)
    //         return 0;
    //     return mainObjTransform.rotation.eulerAngles.z;
    // }
    // public float GetChildRotateZ()
    // {
    //     if (childFrameTransform == null)
    //         return 0;
    //     return childFrameTransform.rotation.eulerAngles.z;
    // }

    // public void SetRotateZ(float RotateZ)
    // {
    //     if (mainObjTransform == null)
    //         return;
    //     mainObjTransform.rotation = Quaternion.Euler(0f, 0f, RotateZ);
    // }

    // public void SetRotateZ(List<AngleSet> angleSets)
    // {
    //     var angle = GetAngle(angleSets, out var isNewAngle);
    //     if (isNewAngle) SetRotateZ(angle);
    //     else SetRotateZ(angle);
    // }

    // public void SetAddRotateZ(List<AngleSet> angleSets)
    // {
    //     var angle = GetAngle(angleSets, out var isNewAngle);
    //     if (isNewAngle) SetRotateZ(angle);
    //     else SetRotateZ(GetRotateZ() + angle / 60);
    // }


    // public void SetChildRotateZ(float ChildRotateZ)
    // {
    //     if (childFrameTransform == null)
    //         return;
    //     childFrameTransform.rotation = Quaternion.Euler(0f, 0f, ChildRotateZ);
    // }

    // public void SetChildRotateZ(List<AngleSet> angleSets)
    // {
    //     var angle = GetAngle(angleSets, out var isNewAngle);
    //     if (isNewAngle) SetChildRotateZ(angle);
    //     else SetChildRotateZ(angle);
    // }

    // public void SetChildAddRotateZ(List<AngleSet> angleSets)
    // {
    //     var angle = GetAngle(angleSets, out var isNewAngle);
    //     if (isNewAngle) SetChildRotateZ(angle);
    //     else SetChildRotateZ(GetChildRotateZ() + angle / 60);
    // }



    private const float INV_60 = 1f / 60f;
    private static readonly Vector3 ForwardAxis = Vector3.forward;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float GetRotateZ()
    {
        return mainObjTransform != null ? mainObjTransform.rotation.eulerAngles.z : 0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float GetChildRotateZ()
    {
        return childFrameTransform != null ? childFrameTransform.rotation.eulerAngles.z : 0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetRotateZ(float rotateZ)
    {
        if (mainObjTransform == null) return;
        mainObjTransform.rotation = Quaternion.AngleAxis(rotateZ, ForwardAxis);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetRotateZ(List<AngleSet> angleSets)
    {
        float angle = GetAngle(angleSets, out bool _);
        SetRotateZ(angle);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetAddRotateZ(List<AngleSet> angleSets)
    {
        float angle = GetAngle(angleSets, out bool isNewAngle);
        if (mainObjTransform == null) return;

        if (isNewAngle)
        {
            SetRotateZ(angle);
        }
        else
        {
            // 快取 currentZ，避免重取 rotation.eulerAngles
            float currentZ = mainObjTransform.rotation.eulerAngles.z;
            SetRotateZ(currentZ + angle * INV_60);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetChildRotateZ(float childRotateZ)
    {
        if (childFrameTransform == null) return;
        childFrameTransform.rotation = Quaternion.AngleAxis(childRotateZ, ForwardAxis);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetChildRotateZ(List<AngleSet> angleSets)
    {
        float angle = GetAngle(angleSets, out bool _);
        SetChildRotateZ(angle);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetChildAddRotateZ(List<AngleSet> angleSets)
    {
        float angle = GetAngle(angleSets, out bool isNewAngle);
        if (childFrameTransform == null) return;

        if (isNewAngle)
        {
            SetChildRotateZ(angle);
        }
        else
        {
            // 快取 currentZ，避免重取 rotation.eulerAngles
            float currentZ = childFrameTransform.rotation.eulerAngles.z;
            SetChildRotateZ(currentZ + angle * INV_60);
        }
    }

    // 這是外部既有函式，不動它



    public bool IsOutBorder(float restoreDistance, Vector2 Pos)
    {
        return IsOutBorderDis(Pos, restoreDistance);
    }

    public bool IsOutBorder(float restoreDistance)
    {
        return IsOutBorderDis(transform.position, restoreDistance);
    }

    public static bool IsOutBorderDis(Vector2 pos, float dis)
    {
        if (InvalidHelper.IsInvalid(pos))
            return true; // 或 false，看邏輯需求
        if (pos.x < GameConfig.BORDER_LEFT - dis) return true;
        if (pos.x > GameConfig.BORDER_RIGHT + dis) return true;
        if (pos.y < GameConfig.BD_BOTTOM - dis) return true;
        if (pos.y > GameConfig.BD_TOP + dis) return true;
        return false;
    }




    public void MovePos(Vector2 pos)
    {
        tmpPos.x = pos.x;
        tmpPos.y = pos.y;
        tmpPos.z = obj_zIndex;
        transform.position = tmpPos;
    }

    public void MoveTranslate(Vector2 moveVector)
    {
        tmpPos.x = moveVector.x;
        tmpPos.y = moveVector.y;
        tmpPos.z = 0f;
        transform.Translate(tmpPos);
    }






    public void TryObjAddRecord(SettingBase setting)
    {
        var recordAngleId = setting.recordAngleId;
        var recordPosId = setting.recordPosId;

        if (!InvalidHelper.IsInvalid(recordAngleId) && setting.recordAngle != null)
            objRecordAngleDict[recordAngleId] = GetAngle(setting.recordAngle, out _);

        if (!InvalidHelper.IsInvalid(recordPosId) && setting.recordPos != null)
            objRecordPosDict[recordPosId] = GetPos(setting.recordPos);
    }
    public void InsertRelatChild(UnitCtrlObj insertUnitCtrlObj)
    {
        if (childFrameTransform != null)
        {
            var insertTransform = insertUnitCtrlObj.transform;
            insertTransform.SetParent(childFrameTransform);
            insertTransform.SetAsLastSibling();
            insertUnitCtrlObj.SetRotateZ(0);
        }
        else
        {
            Debug.LogError($"id:{insertUnitCtrlObj.coreSettingId}  not childFrame");
        }
    }

    public Vector2 GetPos(List<Pos> posList)
    {
        Vector2 pos = GameConfig.CENTER;

        for (int i = 0; i < posList.Count; i++)
        {
            var posData = posList[i];

            if (!InvalidHelper.IsInvalid(posData.Id))
            {
                var idVal = (IdVal)posData.Id;
                if (idVal == IdVal.XCenter) pos.x = GameConfig.CENTER.x;
                else if (idVal == IdVal.YCenter) pos.y = GameConfig.CENTER.y;
                else pos = GetPos(posData.Id);
            }
            else if (!InvalidHelper.IsInvalid(posData.point))
            {
                pos += posData.point;
            }
            else if (posData.ADangle != null && !InvalidHelper.IsInvalid(posData.ADdistance))
            {
                float angle = GetAngle(posData.ADangle, out _);
                float dis = posData.ADdistance;
                pos += CalPos(angle, dis);
            }
        }

        return pos;
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
        for (int i = 0, c = angle.Count; i < c; i++)
        {
            var angleData = angle[i];
            if (!InvalidHelper.IsInvalid(angleData.angle))
            {
                Angle += angleData.angle;
                isNewAngle = false;
            }
            else if (angleData.pos1 != null && angleData.pos2 != null)
            {
                tmpPoses1[0] = angleData.pos1;
                tmpPoses2[0] = angleData.pos2;
                Angle += CalAngle(
                    GetPos(tmpPoses1),
                    GetPos(tmpPoses2)
                );
            }
            else if (!InvalidHelper.IsInvalid(angleData.IdRotateZ))
            {
                var unitCtrlObj = GetUnitObj(angleData.IdRotateZ);
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
            else if (!InvalidHelper.IsInvalid(angleData.recordId) && angleData.recordId > RECORD_TMP_ID_MIN)
            {
                Angle += objRecordAngleDict[angleData.recordId];
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
        return objActionMoveAngleDict.TryGetValue(coreId, out angle);
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
        File.WriteAllText(LoadCtrl.Instance.unitLogPath + obj_zCodeHash + ".txt", objPrintContent);
        return objPrintContent;
    }


    void ReceiveParentRecord(UnitCtrlObj parentUnitObj)
    {
        if (parentUnitObj == null)
            return;

        if (parentUnitObj.objRecordAngleDict.Count > 0)
        {
            objRecordAngleDict.Clear();

            foreach (var kv in parentUnitObj.objRecordAngleDict)
                objRecordAngleDict[kv.Key] = kv.Value;
        }

        if (parentUnitObj.objRecordPosDict.Count > 0)
        {
            objRecordPosDict.Clear();
            foreach (var kv in parentUnitObj.objRecordPosDict)
                objRecordPosDict[kv.Key] = kv.Value;
        }
    }





}

