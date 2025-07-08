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
public abstract partial class UnitCtrlBase : MonoBehaviour, IUnitCtrlData
{
    public string paramPoolName { get; set; } 
    public uint takeDictKeyNo { get; set; }
    public float zIndex { get; set; } = 0;
    public bool isAllowCollision { get; set; }
    public event Action eventMoveVectorCal;
    public void RunEventMoveVectorCal()
    {
        if (eventMoveVectorCal != null)
            eventMoveVectorCal.Invoke();
    }

    public event Action eventBorderHandle;
    public void RunEventBorderHandle()
    {
        if (eventBorderHandle != null)
            eventBorderHandle.Invoke();
    }


    public bool isRestoreQueue { get; set; }
    public bool isDeadQueue { get; set; }



    public UnitCtrlBase parentUnitCtrl { get; set; }
    public CreateStageSetting createSetting { get; set; }
    public Dictionary<uint, ActCtrl> actCtrlDict = new Dictionary<uint, ActCtrl>();
    public UnitPropBase unitProp { get; set; }
    protected string zCode { get; set; } = "";
    public SettingBase coreSetting { get { return createSetting.coreSetting; } }



    protected Text log { get; set; }
    protected string logStr { get; set; }
    protected Collider2D colliderComponent { get; set; }
    public SpriteRenderer spriteRenderer { get; set; }
    public CollisionCtrlBase mainObjCtrl { get; set; }
    public Transform childFrame { get; set; }
    public List<UnitCtrlBase> childFrameUnits { get; set; } = new List<UnitCtrlBase>();
    public Animator animator { get; set; }



    void Awake()
    {
        if (transform.childCount < 2 || transform.GetChild(0).transform.childCount < 1)
        {
            Debug.LogError($"{GetType().Name} childCount Error");
        }
        gameObject.transform.position = new Vector3(GameConfig.POOL_RESET_POS.x, GameConfig.POOL_RESET_POS.y, zIndex);
        spriteRenderer = transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
        mainObjCtrl = transform.GetChild(0).GetComponent<CollisionCtrlBase>();
        colliderComponent = transform.GetChild(0).GetComponent<Collider2D>();
        childFrame = transform.GetChild(1);
        log = GetComponent<Text>();
        animator = GetComponent<Animator>();
        CustomizeAwake();
        ResetParam();
    }

    public virtual void CustomizeAwake()
    {

    }

    public void CreateUnitAndSet(CreateStageSetting createStageSetting, UnitCtrlBase parentUnitCtrl = null)
    {
        SetCreateSetting(createStageSetting, parentUnitCtrl);
        EnableUnit();
        BorderTriggerForEvent();
        if (actCtrlDict.ContainsKey(coreSetting.Id))
        {
            OnActive(actCtrlDict[coreSetting.Id]);
        }
        else
            Debug.LogError($"ActCtrlDict does not contain CoreSetting.Id: {coreSetting.Id}" +
                $"  ActCtrlDict.Count = {actCtrlDict.Count} ");
    }

    public void SetCreateSetting(CreateStageSetting createStageSetting, UnitCtrlBase parentUnitCtrl = null)
    {
        createSetting = createStageSetting;
        this.parentUnitCtrl = parentUnitCtrl;
        actCtrlDict = ActCtrlFactory.CreateActCtrlDict(this);
        unitProp = UnitPropFactory.Create(this);
        zCode = coreSetting.Id.ToString() + "_z" + zIndex * 1000f;

        //延續父紀錄
        if (parentUnitCtrl != null)
        {
            if (parentUnitCtrl.recordAngleDict.Count > 0)
                recordAngleDict = new Dictionary<uint, float>(parentUnitCtrl.recordAngleDict);
            if (parentUnitCtrl.recordPosDict.Count > 0)
                recordPosDict = new Dictionary<uint, Vector2>(parentUnitCtrl.recordPosDict);
        }
        PrintCreate();
    }
    void EnableUnit()
    {
        gameObject.SetActive(true);
        enabled = true;
        transform.SetAsLastSibling();
    }
    private void BorderTriggerForEvent()
    {
        eventBorderHandle = null;
        eventBorderHandle += BorderEnterAction;
    }

    private void BorderEnterAction()
    {
        uTime++;
        if (uTime % 10 != 0) return;

        if (!CheckOutBorder())
        {
            eventBorderHandle -= BorderEnterAction;
            eventBorderHandle += BorderOutAction;
        }
    }

    private void BorderOutAction()
    {
        uTime++;
        if (uTime % 10 != 0) return;

        if (CheckOutBorder())
        {
            TriggerRestore();
            eventBorderHandle -= BorderOutAction;
        }
    }

    public virtual void OnActive(ActCtrl actCtrl, ActCtrl parentActCtrl = null)
    {
        var setting = actCtrl.stageSetting;
        unitProp.Active(setting, this); ;
        Active(setting);
        OnActiveActCtrl(actCtrl, parentActCtrl);
    }

    public virtual void OnActiveActCtrl(ActCtrl actCtrl, ActCtrl parentActCtrl = null)
    {
        isAllowCollision = true;
        actCtrl.Active(parentActCtrl);
        eventMoveVectorCal -= ActiveMoveVectorCal;
        eventMoveVectorCal += ActiveMoveVectorCal;
    }


    void ActiveMoveVectorCal()
    {
        var moveVector = actCtrlDict.Values.Select(r => r.moveVector).Aggregate(Vector2.zero, (acc, v) => acc + v);
        unitProp.moveAngle = Mathf.Atan2(moveVector.y, moveVector.x) * Mathf.Rad2Deg;
        unitProp.speed = moveVector.magnitude * 60f;
    }

    protected virtual void ActiveActCtrlCustomize(SettingBase stageSetting)
    {
    }


    public virtual void OnActTimeEndCustomize()
    {
    }

    public virtual bool CheckOutBorder(Vector2? Pos = null)
    {
        if (Pos == null)
        {
            Pos = transform.position;
        }
        return CheckOutBorderDis(Pos, unitProp.restoreDistance);
    }

    public virtual void HandleDead()
    {
        isAllowCollision = false;
        ClearEvent();
        CustomizeDeadHandle();
        transform.SetParent(LoadingCtrl.Instance.pool.transform);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        PlayDeadAnim();
    }

    public virtual void CustomizeDeadHandle()
    {

    }

    public virtual void PlayDeadAnim()
    {
        var endDeadAniTime = GameSystem.Instance.keyTime + DEFAULT_DEADANI_KEY_TIME;
        if (animator != null)
            animator.Play("Dead");
        Action selfAction = null;
        selfAction += () =>
        {
            if (GameSystem.Instance.keyTime == endDeadAniTime)
            {
                isDeadQueue = false;
                TriggerRestore();
                GameSystem.Instance.waitDeadAnis -= selfAction;
                DeadAnimEndHandle();
            }
        };
        GameSystem.Instance.waitDeadAnis += selfAction;
    }

    public virtual void DeadAnimEndHandle()
    {
        DeadAnimEndCustomize();
        TriggerRestore();
    }

    public virtual void DeadAnimEndCustomize()
    {

    }


    public void RestoreIntoPool()
    {
        isRestoreQueue = false;
        ResetParam();
        ClearEvent();
        if (animator != null)
            animator.Play("Idle");
        gameObject.SetActive(false);

        childFrameUnits = new List<UnitCtrlBase>();
        transform.SetParent(LoadingCtrl.Instance.pool.transform);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        transform.SetAsFirstSibling();


        gameObject.transform.position = new Vector3(GameConfig.POOL_RESET_POS.x, GameConfig.POOL_RESET_POS.y, zIndex);

        GameSystem.Instance.takeDict.Remove(takeDictKeyNo);
        LoadingCtrl.Instance.pool.objectDict[paramPoolName].Push(this);
        RestoreInToPoolCustomize();
    }

    protected virtual void RestoreInToPoolCustomize()
    {

    }

    public void ClearEvent()
    {
        eventMoveVectorCal = null;
        eventBorderHandle = null;
        foreach (var actCtrl in actCtrlDict.Values)
        {
            ClearAllAction(actCtrl);
        }

    }

    public virtual void ResetParam()
    {
        logStr = "";
        log.text = "";

        parentUnitCtrl = null;
        createSetting = null;
        isAllowCollision = false;

        actCtrlDict = new Dictionary<uint, ActCtrl>();
        recordPosDict = new Dictionary<uint, Vector2>();
        recordAngleDict = new Dictionary<uint, float>();

        zCode = "";
        uTime = 0;
        SetRotateZ(0);
        SetChildRotateZ(0);
        CustomizeReset();
    }

    protected virtual void CustomizeReset()
    {

    }




}


