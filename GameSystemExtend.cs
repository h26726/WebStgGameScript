using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;
using System.Xml;
using System.IO;
using static CommonData;
using static CommonFunc;
using static PlayerKeyCtrl;
using static PlayerSaveData;
using static GameConfig;
using System.Linq;
using static LoadingCtrl;

public partial class GameSystem
{
    public void GameSystemUpdate()
    {
        if (isPause || LoadingCtrl.Instance.gameState != GameSceneState.Run)
            return;

        if (isReplay && playReplayMaxKeyTime == keyTime)
        {
            Pause();
            ReplayOverSelect.Instance.Show();
            return;
        }

        keyTime++;


        if (nowGameProgressState == GameProgressState.Stage)
        {
            gTime++;
            if (isPractice)
            {
                while (gTime > practiceSetting.bossEnterTime && gTime < practiceSetting.bossSpellTime)
                {
                    gTime++;
                }

            }
            if (gTime % 100 == 0)
            {
                while (GameConfig.CONFIG_PARAMS.Any(r => r.key == "SkipTime" && gTime == r.intVal))
                {
                    //測試用
                    gTime += 100;
                }


                while (gTimeCreateCallRuleSchemes[(int)nowGTimeCallRuleSchemeKey].callGameTime < gTime)
                {
                    Debug.Log($"skip CallGameTime:{gTimeCreateCallRuleSchemes[(int)nowGTimeCallRuleSchemeKey].callGameTime}");
                    nowGTimeCallRuleSchemeKey++;
                }
                Debug.Log($"gTime:{gTime}");
            }
            while (nowGTimeCallRuleSchemeKey < gTimeCreateCallRuleSchemes.Count && gTime == gTimeCreateCallRuleSchemes[(int)nowGTimeCallRuleSchemeKey].callGameTime)
            {
                var createStageSetting = gTimeCreateCallRuleSchemes[(int)nowGTimeCallRuleSchemeKey].createStageSetting;
                var baseId = gTimeCreateCallRuleSchemes[(int)nowGTimeCallRuleSchemeKey].baseId;
                var actId = gTimeCreateCallRuleSchemes[(int)nowGTimeCallRuleSchemeKey].actId;
                if (createStageSetting != null)
                {
                    var newBaseStageSetting = createStageSetting.coreSetting;
                    if (createStageSetting.type == TypeValue.對話)
                    {
                        CreateDialog(newBaseStageSetting);
                    }
                    else if (createStageSetting.type == TypeValue.標題)
                    {
                        CreateTitle(newBaseStageSetting, PlayGameAni);
                    }
                    else if (createStageSetting.type == TypeValue.播放音樂)
                    {
                        LoadingCtrl.Instance.pool.PlayBgm(createStageSetting.coreSetting.obj);
                    }
                    else if (createStageSetting.type == TypeValue.下一關)
                    {
                        stageKey++;
                        LoadingCtrl.Instance.SwitchPage(PageIndex.Game);
                    }
                    else if (createStageSetting.type == TypeValue.BOSSDEAD)
                    {
                        nowGameProgressState = GameProgressState.Stage;
                        nowEnemyBoss.HandleDead();

                    }
                    else if (createStageSetting.type == TypeValue.關卡結束)
                    {
                        // GameEndSelect.Show();
                    }
                    else
                    {
                        if (createStageSetting.type == TypeValue.符卡)
                        {
                            nowGameProgressState = GameProgressState.BossTime;
                            CreateSpell(newBaseStageSetting, PlayGameAni);
                        }
                        else if (createStageSetting.type == TypeValue.復位 || createStageSetting.type == TypeValue.BOSSLEAVE)
                        {
                            nowGameProgressState = GameProgressState.BossTime;
                        }
                        waitCreates += () =>
                        {
                            CreateUnit(createStageSetting);
                        };

                    }
                }
                else if (baseId != null && actId != null)
                {
                    var Units = GetUnitsById(baseId.Value);
                    waitCallActs += () =>
                    {
                        ExtAct(Units, actId.Value);
                    };
                }
                nowGTimeCallRuleSchemeKey++;
            }
        }

        foreach (var unit in takeDict)
        {
            unit.Value.RunEventMoveVectorCal();
            unit.Value.RunEventBorderHandle();
        }

        if (DialogCtrl.nowInstance != null)
            DialogCtrl.nowInstance.RunEventUpdate();



        waitPlayerKey?.Invoke();

        waitDeadAnis?.Invoke();

        waitActStops?.Invoke();
        waitActStops = null;

        waitCallActs?.Invoke();
        waitCallActs = null;


        foreach (var unit in takeDict)
        {
            foreach (var actCtrl in unit.Value.actCtrlDict.Values)
            {
                actCtrl.coreAction?.Invoke();
                actCtrl.callTimeAction?.Invoke();
                actCtrl.callPosAction?.Invoke();
                actCtrl.stopAction?.Invoke();
            }
        }

        waitCreates?.Invoke();
        waitCreates = null;

        if (nowGameProgressState == GameProgressState.SpellEndTrigger)
        {
            if (nowSpellBg != null)
                nowSpellBg.SetActive(false);
            spellTime.text = "";
            spellCardNameAnimator.gameObject.SetActive(false);
            ClearAllEnemyShot();
            nowEnemyBoss.ClearEvent();
            nowEnemyBoss.ResetParam();
            nowGameProgressState = GameProgressState.Stage;
        }

        waitDeads?.Invoke();
        waitDeads = null;

        waitRestores?.Invoke();
        waitRestores = null;

        if (Input.GetKeyDown(KeyCode.P))
        {
            foreach (var unitCtrlBase in takeDict.Values)
            {
                unitCtrlBase.FileWriteContent();
            }
        }


        if (Input.GetKeyDown(GetSetKey(KeyCode.Escape)) || Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            Pause();
            if (isReplay)
                ReplayPauseSelect.Instance.Show();
            else
                PauseSelect.Instance.Show();
        }
    }




    public void PlayGameAni(Animator animator)
    {
        this.StartCoroutine(AniPlayCor(animator));
    }

    IEnumerator AniPlayCor(Animator animator)
    {
        animator.transform.gameObject.SetActive(true);
        yield return new WaitUntil(() => { return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1; });
        animator.transform.gameObject.SetActive(false);

    }

    public UnitCtrlBase takeFormPool(string ParamPoolName)//物件池取出單位並附takeDictKeyNo
    {
        if (!LoadingCtrl.Instance.pool.objectDict.ContainsKey(ParamPoolName))
        {
            Debug.LogError($"takeFormPool ParamPoolName not found:{ParamPoolName}");
            return null;
        }
        var stack = LoadingCtrl.Instance.pool.objectDict[ParamPoolName];
        if (stack.Count == 0)
        {
            Debug.LogError($"takeFormPool ParamPoolName stack is empty:{ParamPoolName}");
            return null;
        }
        UnitCtrlBase UnitCtrlBase = LoadingCtrl.Instance.pool.objectDict[ParamPoolName].Pop();
        takeDict.Add(takeDictKeyNo, UnitCtrlBase);
        UnitCtrlBase.takeDictKeyNo = takeDictKeyNo;
        takeDictKeyNo++;
        return UnitCtrlBase;
    }



    public UnitCtrlBase CreateUnit(CreateStageSetting createStageSetting, UnitCtrlBase parent = null)
    {
        var newBaseStageSetting = createStageSetting.coreSetting;
        UnitCtrlBase obj = null;
        if (newBaseStageSetting.type == TypeValue.符卡 || newBaseStageSetting.type == TypeValue.復位 || newBaseStageSetting.type == TypeValue.BOSSLEAVE)
        {
            obj = nowEnemyBoss;
        }
        else
        {
            var objStr = newBaseStageSetting.obj;
            obj = takeFormPool(objStr);
        }
        if (newBaseStageSetting.type == TypeValue.玩家)
        {
            playerUnitCtrl = (PlayerUnitCtrl)obj;
        }
        else if (newBaseStageSetting.type == TypeValue.BOSS)
        {
            nowEnemyBoss = (EnemyBossUnitCtrl)obj;
        }
        obj.zIndex = zIndex;
        zIndex -= Z_INDEX_REDUCE;
        obj.CreateUnitAndSet(createStageSetting, parent);
        return obj;
    }

    public void ExtAct(List<UnitCtrlBase> unitCtrls, uint actId, ActCtrl parentActCtrl = null)
    {
        foreach (var UnitCtrlBase in unitCtrls)
        {
            UnitCtrlBase.AddPrintContent("UnitCtrlBase.ActCtrlDict:" + string.Join(",", UnitCtrlBase.actCtrlDict.Keys));
            if (UnitCtrlBase.actCtrlDict.ContainsKey(actId))
            {
                UnitCtrlBase.AddPrintContent("ExtAct:" + actId);

                UnitCtrlBase.OnActive(UnitCtrlBase.actCtrlDict[actId], parentActCtrl);
            }
        }
    }

    public void CreateSpell(SettingBase stageSetting, Action<Animator> PlayAniAct)
    {
        if (string.IsNullOrEmpty(stageSetting.name))
            return;

        Animator animator = LoadingCtrl.Instance.pool.spellDict[stageSetting.spellAni][0];
        nowSpellBg = LoadingCtrl.Instance.pool.spellDict[stageSetting.spellAni][1].transform.gameObject;
        nowSpellBg.SetActive(true);
        if (IS_OPEN_SPELLIMG)
        {
            Instance.spellCardNameAnimator.gameObject.SetActive(true);
            Instance.spellCardNameText.text = stageSetting.name;
            PlayAniAct?.Invoke(animator);
        }
    }

    public void CreateTitle(SettingBase stageSetting, Action<Animator> PlayAniAct)
    {
        Debug.Log("CreateTitle.Id:" + stageSetting.Id);
        var ParamPoolName = stageSetting.obj;
        var ani = stageSetting.ani;
        Animator animator = LoadingCtrl.Instance.pool.titleDict[stageSetting.ani];
        PlayAniAct?.Invoke(animator);
    }
    public void CreateDialog(SettingBase stageSetting)
    {
        var ParamPoolName = stageSetting.obj;
        var Id = stageSetting.Id;
        if (LoadingCtrl.Instance.pool.dialogDict.ContainsKey(ParamPoolName))
        {
            DialogCtrl dialogCtrl = LoadingCtrl.Instance.pool.dialogDict[ParamPoolName];
            dialogCtrl.DialogStart(Id);
        }
    }

    public void UpdatePlayerLife()
    {
        for (int i = 0; i < Instance.playerLifeRect.childCount; i++)
        {
            var HpObj = Instance.playerLifeRect.GetChild(i);
            HpObj.gameObject.SetActive(i < _playerLife);
        }
    }

    public void UpdatePlayerBoom()
    {
        for (int i = 0; i < Instance.playerBoomRect.childCount; i++)
        {
            var BoomObj = Instance.playerBoomRect.GetChild(i);
            BoomObj.gameObject.SetActive(i < _playerBoom);
        }
    }

    public void GameLogtext(string str)
    {
        Instance.gameLog.text += "[ " + DateTime.Now.ToString("HH:mm:ss") + " ]" + str + "\r\n";
    }



    public void Pause()
    {
        if (isPause) return;
        Time.timeScale = 0;
        LoadingCtrl.Instance.audioSource.Pause();
        isPause = true;
        StartCoroutine(PauseHandler());
    }

    public void UnPause()
    {
        if (!isPause) return;
        Time.timeScale = 1;
        LoadingCtrl.Instance.audioSource.UnPause();
        isPause = false;
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
            while (time < 10)
            {
                time++;
                yield return null;
            }
            yield return null;
        }
        UnPause();
    }



    public void ClearAllEnemyShot()
    {
        var Takes = takeDict.Where(r => r.Value is EnemyShotUnitCtrl).ToList();
        foreach (var TakePair in Takes)
        {
            TakePair.Value.TriggerDead();
        }
    }

    public void ClearAll()
    {
        var Takes = takeDict.ToList();
        foreach (var TakePair in Takes)
        {
            TakePair.Value.RestoreIntoPool();
        }
    }

    public void CloseAll()
    {
        var Takes = takeDict;
        foreach (var TakePair in Takes)
        {
            TakePair.Value.enabled = false;
        }
    }


    public IEnumerator GetXmlStageSettings(List<XmlStageSetting> xmlStageSettings)
    {
        uint k = 0;
        List<Pos> poses = new List<Pos>();
        List<AngleSet> angleSets = new List<AngleSet>();
        Pos pos = new Pos();
        AngleSet angleSet = new AngleSet();
        CallRuleScheme callParam = new CallRuleScheme();
        CreateStageSetting createStageSetting = null;
        SettingBase stageSetting = null;

        void SetStartTypeVal(XmlStageSetting xmlStageSetting)
        {
            SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.Id);
            stageSetting.type = xmlStageSetting.Type;

            if (xmlStageSetting.Type == TypeValue.行動ID)
            {
                createStageSetting.actionSettingsDict.Add(stageSetting.Id, stageSetting);
            }
            else
            {
                createStageSetting = new CreateStageSetting
                {
                    coreSetting = stageSetting
                };
            }
            if (xmlStageSetting.Type == TypeValue.玩家)
            {
                playerCreateStageSetting = createStageSetting;
            }
            else if (xmlStageSetting.Type == TypeValue.玩家子彈)
            {
                createStageSetting.coreSetting.playerGameTime = 0;
                createStageSetting.coreSetting.playerShift = false;
                createStageSetting.coreSetting.playerShotDelayTime = 0;
                playerShotCreateStageSettings.Add(createStageSetting);
            }
            else if (xmlStageSetting.Type == TypeValue.Power)
            {
                powerCreateStageSettings.Add(createStageSetting);
            }

        }

        bool checkAnglePos1Reference = false;
        bool checkAnglePos2Reference = false;
        bool checkPosReference = false;
        bool checkAngleReference = false;
        while (k < xmlStageSettings.Count)
        {
#if !UNITY_EDITOR
                if (k > 0 && k % 100 == 0) // 每 100 筆等一幀
                yield return null;
#endif
            var xmlStageSetting = xmlStageSettings[(int)k];

            Debug.Log($"table:{xmlStageSetting.table} , xmlID:{xmlStageSetting.Order}, xmlStageSetting.Type:{xmlStageSetting.Type}");
            if ((checkPosReference || checkAnglePos1Reference || checkAnglePos2Reference) && xmlStageSetting.Type != TypeValue.位置參數)
            {
                TestEnd($"table:{xmlStageSetting.table}, order:{xmlStageSetting.Order}");
            }
            else if (checkAngleReference && xmlStageSetting.Type != TypeValue.角度參數)
            {
                TestEnd($"table:{xmlStageSetting.table}, order:{xmlStageSetting.Order}");
            }

            checkPosReference = false;
            checkAngleReference = false;

            if (StartType.Contains(xmlStageSetting.Type))
            {
                poses = new List<Pos>();
                angleSets = new List<AngleSet>();
                stageSetting = new SettingBase();

                SetStartTypeVal(xmlStageSetting);

                k++;
                continue;
            }

            if (xmlStageSetting.ParamVals.Length == 0)
            {
                if (!NoValPosType.Contains(xmlStageSetting.PosParamTypeVal) && !NoValAngleType.Contains(xmlStageSetting.AngleParamTypeVal) && !NoValType.Contains(xmlStageSetting.Type))
                {
                    TestEnd($"table: {xmlStageSetting.table}  order: {xmlStageSetting.Order} type: {xmlStageSetting.Type}");
                }
            }
            if (PosType.Contains(xmlStageSetting.Type) || AngleType.Contains(xmlStageSetting.Type))
            {

                if (xmlStageSetting.Type == TypeValue.位置參數 || xmlStageSetting.Type == TypeValue.加位置)
                {
                    if (xmlStageSetting.Type == TypeValue.位置參數)
                    {
                        if (checkAnglePos1Reference)
                        {
                            poses = new List<Pos>();
                            pos = new Pos();
                            poses.Add(pos);
                            angleSet.pos1 = pos;
                            checkAnglePos1Reference = false;
                            checkAnglePos2Reference = true;
                        }
                        else if (checkAnglePos2Reference)
                        {
                            poses = new List<Pos>();
                            pos = new Pos();
                            poses.Add(pos);
                            angleSet.pos2 = pos;
                            checkAnglePos2Reference = false;
                        }
                    }
                    else if (xmlStageSetting.Type == TypeValue.加位置)
                    {
                        pos = new Pos();
                        poses.Add(pos);
                    }
                }
                else if (xmlStageSetting.Type == TypeValue.角度參數 || xmlStageSetting.Type == TypeValue.加角度)
                {
                    if (xmlStageSetting.Type == TypeValue.加角度)
                    {
                        angleSet = new AngleSet();
                        angleSets.Add(angleSet);
                    }
                }
                else
                {
                    poses = new List<Pos>();
                    angleSets = new List<AngleSet>();
                }




                if (PosType.Contains(xmlStageSetting.Type))
                {
                    if (xmlStageSetting.PosParamTypeVal == PosParamType.None)
                    {
                        TestEnd("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order);
                    }
                    if (poses == null || poses.Count == 0)
                    {
                        pos = new Pos();
                        poses = new List<Pos>();
                        poses.Add(pos);
                    }

                    if (xmlStageSetting.Type == TypeValue.移位置)
                    {
                        stageSetting.movePos = poses;
                    }
                    else if (xmlStageSetting.Type == TypeValue.相對位置)
                    {
                        stageSetting.relatPos = poses;
                    }
                    else if (xmlStageSetting.Type == TypeValue.時位位置)
                    {
                        stageSetting.timPosPos = poses;
                    }
                    else if (xmlStageSetting.Type == TypeValue.啟動依位置)
                    {
                        callParam.callPos = poses;
                    }
                    else if (xmlStageSetting.Type == TypeValue.記錄位置)
                    {
                        stageSetting.recordPos = poses;
                    }
                    else if (xmlStageSetting.Type == TypeValue.位置參數)
                    {
                    }

                    if (xmlStageSetting.PosParamTypeVal == PosParamType.位置參數角距角度)
                    {
                        angleSets = new List<AngleSet>();
                        angleSet = new AngleSet();
                        angleSets.Add(angleSet);
                        pos.ADangle = angleSets;
                    }
                    else if (xmlStageSetting.PosParamTypeVal == PosParamType.位置參數角距距離)
                    {
                        SetFormString(xmlStageSetting.ParamVals[0], ref pos.ADdistance);
                    }

                }
                else if (AngleType.Contains(xmlStageSetting.Type))
                {
                    if (xmlStageSetting.AngleParamTypeVal == AngleParamType.None)
                    {
                        TestEnd("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order);
                    }
                    if (angleSets == null || angleSets.Count == 0)
                    {
                        angleSet = new AngleSet();
                        angleSets = new List<AngleSet>();
                        angleSets.Add(angleSet);
                    }


                    if (xmlStageSetting.Type == TypeValue.移動角度)
                    {
                        stageSetting.moveAngle = angleSets;
                    }
                    else if (xmlStageSetting.Type == TypeValue.加移動角度)
                    {
                        stageSetting.addMoveAngle = angleSets;
                    }
                    else if (xmlStageSetting.Type == TypeValue.旋轉角度)
                    {
                        stageSetting.rotateZ = angleSets;
                    }
                    else if (xmlStageSetting.Type == TypeValue.加旋轉角度)
                    {
                        stageSetting.addRotateZ = angleSets;
                    }
                    else if (xmlStageSetting.Type == TypeValue.子旋轉角度)
                    {
                        stageSetting.childRotateZ = angleSets;
                    }
                    else if (xmlStageSetting.Type == TypeValue.子加旋轉角度)
                    {
                        stageSetting.childAddRotateZ = angleSets;
                    }
                    else if (xmlStageSetting.Type == TypeValue.記錄角度)
                    {
                        stageSetting.recordAngle = angleSets;
                    }
                    else if (xmlStageSetting.Type == TypeValue.角度參數)
                    {
                    }
                }

                if (xmlStageSetting.AngleParamTypeVal == AngleParamType.角度值)
                {
                    SetFormString(xmlStageSetting.ParamVals[0], ref angleSet.angle);
                }
                else if (xmlStageSetting.AngleParamTypeVal == AngleParamType.角度ID記錄)
                {
                    SetFormString(xmlStageSetting.ParamVals[0], ref angleSet.recordId);
                }
                else if (xmlStageSetting.AngleParamTypeVal == AngleParamType.角度IDs)//1=自身 2=父
                {
                    SetFormString(xmlStageSetting.ParamVals[0], ref angleSet.Ids);
                    if (angleSet.Ids.Length != 2)
                    {
                        TestEnd("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order);
                    }
                }
                else if (xmlStageSetting.AngleParamTypeVal == AngleParamType.角度ID旋轉)//1=自身 2=父
                {
                    SetFormString(xmlStageSetting.ParamVals[0], ref angleSet.IdRotateZ);
                }
                else if (xmlStageSetting.AngleParamTypeVal == AngleParamType.角度ID移動角度)//1=自身 2=父
                {
                    SetFormString(xmlStageSetting.ParamVals[0], ref angleSet.IdMoveAngle);
                    if (angleSet.IdMoveAngle.Length != 2)
                    {
                        TestEnd("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order);
                    }
                }
                else if (xmlStageSetting.AngleParamTypeVal == AngleParamType.新參照角度)
                {
                    checkAngleReference = true;
                }
                else if (xmlStageSetting.AngleParamTypeVal == AngleParamType.新參照位置12)
                {
                    checkAnglePos1Reference = true;
                }

                if (xmlStageSetting.PosParamTypeVal == PosParamType.位置xy)
                {
                    SetFormString(xmlStageSetting.ParamVals[0], ref pos.point);
                }
                else if (xmlStageSetting.PosParamTypeVal == PosParamType.位置ID)//1=自身 2=父
                {
                    SetFormString(xmlStageSetting.ParamVals[0], ref pos.Id);
                }
                else if (xmlStageSetting.PosParamTypeVal == PosParamType.新參照位置)
                {
                    checkPosReference = true;
                }

            }

            if (xmlStageSetting.Type == TypeValue.物件)
            {
                if (
                    (
                        stageSetting.type == TypeValue.ID ||
                        stageSetting.type == TypeValue.BOSS ||
                        stageSetting.type == TypeValue.玩家子彈 ||
                        stageSetting.type == TypeValue.Power ||
                        stageSetting.type == TypeValue.玩家
                    )
                &&
                !LoadingCtrl.Instance.pool.objectPoolList.Any(r => r.name == xmlStageSetting.ParamVals[0])
                )
                {
                    TestEnd("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order);
                }
                else if (stageSetting.type == TypeValue.對話 && !LoadingCtrl.Instance.pool.dialogPoolList.Any(r => r.name == xmlStageSetting.ParamVals[0]))
                {
                    TestEnd("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order);
                }
                else if (stageSetting.type == TypeValue.播放音樂 && !LoadingCtrl.Instance.pool.musicPoolList.Any(r => r.name == xmlStageSetting.ParamVals[0]))
                {
                    TestEnd("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order);
                }
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.obj);
            }
            else if (xmlStageSetting.Type == TypeValue.動畫)
            {
                if (stageSetting.type == TypeValue.標題 && !LoadingCtrl.Instance.pool.titlePoolList.Any(r => r.name == xmlStageSetting.ParamVals[0]))
                {
                    TestEnd("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order);
                }
                else if (stageSetting.type == TypeValue.符卡 && !LoadingCtrl.Instance.pool.spellPoolList.Any(r => r.name == xmlStageSetting.ParamVals[0]))
                {
                    TestEnd("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order);
                }

                if (stageSetting.type == TypeValue.符卡)
                {
                    SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.spellAni);
                }
                else
                {
                    SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.ani);
                }
            }

            else if (xmlStageSetting.Type == TypeValue.Sprite)
            {
                if (!LoadingCtrl.Instance.pool.spritePoolList.Any(r => r.name == xmlStageSetting.ParamVals[0]))
                {
                    TestEnd("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order);
                }
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.sprite);
            }
            else if (xmlStageSetting.Type == TypeValue.Name)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.name);
            }
            else if (xmlStageSetting.Type == TypeValue.啟動依ID)
            {
                callParam = new CallRuleScheme();
                if (createStageSetting.coreSetting.Id == stageSetting.Id)
                {
                    callParam.createStageSetting = createStageSetting;
                }
                else
                {
                    callParam.baseId = createStageSetting.coreSetting.Id;
                    callParam.actId = stageSetting.Id;
                }

                SetFormString(xmlStageSetting.ParamVals[0], ref callParam.callExistId);

                if (!callRuleSchemeById.ContainsKey(callParam.callExistId.Value))
                {
                    callRuleSchemeById.Add(callParam.callExistId.Value, new List<CallRuleScheme>());
                }
                if (callParam.actId == null || !callRuleSchemeById[callParam.callExistId.Value].Any(r => r.actId == callParam.actId))
                {
                    callRuleSchemeById[callParam.callExistId.Value].Add(callParam);
                }

            }
            else if (xmlStageSetting.Type == TypeValue.啟動依玩家死亡)
            {
                callParam.callPlayerDead = true;
            }
            else if (xmlStageSetting.Type == TypeValue.啟動位置是活動)
            {
                callParam.callPosIsActive = true;
            }
            else if (xmlStageSetting.Type == TypeValue.加ID)
            {
                stageSetting.addIds.Add(uint.Parse(xmlStageSetting.ParamVals[0]));
            }
            else if (xmlStageSetting.Type == TypeValue.啟動依遊戲時間)
            {
                callParam = new CallRuleScheme();
                if (createStageSetting.coreSetting.Id == stageSetting.Id)
                {
                    callParam.createStageSetting = createStageSetting;
                }
                else
                {
                    callParam.baseId = createStageSetting.coreSetting.Id;
                    callParam.actId = stageSetting.Id;
                }
                SetFormString(xmlStageSetting.ParamVals[0], ref callParam.callGameTime);
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.callGameTime);
                gTimeCreateCallRuleSchemes.Add(callParam);

            }
            else if (xmlStageSetting.Type == TypeValue.啟動依開始時間後)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref callParam.callStartAfTime);
            }
            else if (xmlStageSetting.Type == TypeValue.啟動依結束時間前)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref callParam.callEndBfTime);
            }
            else if (xmlStageSetting.Type == TypeValue.行動時間)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.actTime);
            }
            else if (xmlStageSetting.Type == TypeValue.回收時間)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.restoreTime);
            }
            else if (xmlStageSetting.Type == TypeValue.死亡時間)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.deadTime);
            }
            else if (xmlStageSetting.Type == TypeValue.時位時間)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.timPosTime);
            }

            else if (xmlStageSetting.Type == TypeValue.玩家遊戲時間)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.playerGameTime);
            }
            else if (xmlStageSetting.Type == TypeValue.玩家子彈間隔)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.playerShotHzTime);
            }
            else if (xmlStageSetting.Type == TypeValue.出生動畫時間)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.birthDurTime);
            }
            else if (xmlStageSetting.Type == TypeValue.加速度)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.addSpeed);
            }
            else if (xmlStageSetting.Type == TypeValue.最大速度)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.maxSpeed);
            }
            else if (xmlStageSetting.Type == TypeValue.最小速度)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.minSpeed);
            }
            else if (xmlStageSetting.Type == TypeValue.速度)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.speed);
            }
            else if (xmlStageSetting.Type == TypeValue.時位變速點)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.timPosSpeedPoint);
            }
            else if (xmlStageSetting.Type == TypeValue.時位開始速度)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.timPosStartSpeed);
            }
            else if (xmlStageSetting.Type == TypeValue.時位結束速度)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.timPosEndSpeed);
            }
            else if (xmlStageSetting.Type == TypeValue.回收距離)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.restoreDistance);
            }
            else if (xmlStageSetting.Type == TypeValue.啟動依位置距離)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref callParam.callPosDis);
            }
            else if (xmlStageSetting.Type == TypeValue.出生動畫速率)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.birthAniSpeed);
            }
            else if (xmlStageSetting.Type == TypeValue.出生動畫開始)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.birthAniStart);
            }
            else if (xmlStageSetting.Type == TypeValue.傷害)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.dmg);
            }
            else if (xmlStageSetting.Type == TypeValue.HP)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.hp);
            }
            else if (xmlStageSetting.Type == TypeValue.無敵)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.isInvincible);
            }
            else if (xmlStageSetting.Type == TypeValue.貫穿)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.isThrough);
            }
            else if (xmlStageSetting.Type == TypeValue.是界內)
            {
                stageSetting.isIn = true;
            }
            else if (xmlStageSetting.Type == TypeValue.旋轉是移動角度)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.rotateIsMoveAngle);
            }
            else if (xmlStageSetting.Type == TypeValue.玩家Power需求)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.playerPowerNeed);
            }
            else if (xmlStageSetting.Type == TypeValue.玩家子彈延遲)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.playerShotDelayTime);
            }
            else if (xmlStageSetting.Type == TypeValue.玩家Shift)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.playerShift);
            }
            else if (xmlStageSetting.Type == TypeValue.Power掉落)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.powerGive);
            }


            else if (xmlStageSetting.Type == TypeValue.符卡時間)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.spellTime);
            }
            else if (xmlStageSetting.Type == TypeValue.記錄位置ID)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.recordPosId);
            }
            else if (xmlStageSetting.Type == TypeValue.記錄角度ID)
            {
                SetFormString(xmlStageSetting.ParamVals[0], ref stageSetting.recordAngleId);
            }
            k++;
        }

        gTimeCreateCallRuleSchemes.Sort((x, y) =>
        {
            if (x.callGameTime == null && y.callGameTime == null) return 0;
            if (x.callGameTime == null) return 1;
            if (y.callGameTime == null) return -1;
            return x.callGameTime.Value.CompareTo(y.callGameTime.Value);
        });
        yield break;
    }

    void DeleteFile(string fullPath)
    {
        DirectoryInfo direction = new DirectoryInfo(fullPath);
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            string FilePath = fullPath + "/" + files[i].Name;
            File.Delete(FilePath);
        }
    }
}
