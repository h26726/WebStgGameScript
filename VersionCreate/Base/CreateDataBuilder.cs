using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static PlayerSaveData;
using static XmlStageSettingBuilder;

using System.Linq;
using System.IO;

public static class CreateDataBuilder
{
    public static void Build(List<XmlStageSetting> xmlStageSettings, out List<CreateStageSetting> createStageSettings, out List<CallRuleScheme> callRuleSchemesByGTime, out List<CallRuleScheme> callRuleSchemeById)
    {

        //依單位ID行動時創造或激活行動資料 
        createStageSettings = new List<CreateStageSetting>();
        callRuleSchemesByGTime = new List<CallRuleScheme>();
        callRuleSchemeById = new List<CallRuleScheme>();

        if (xmlStageSettings.Count == 0)
            return;

        uint k = 0;
        List<Pos> poses = new List<Pos>();
        List<AngleSet> angleSets = new List<AngleSet>();
        Pos pos = new Pos();
        AngleSet angleSet = new AngleSet();
        CallRuleScheme callRuleScheme = new CallRuleScheme();
        CreateStageSetting createStageSetting = null;
        SettingBase stageSetting = null;

        void StartTypeHandle(XmlStageSetting xmlStageSetting, ref List<CreateStageSetting> createStageSettings)
        {
            StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.Id);
            stageSetting.type = xmlStageSetting.Type;

            if (xmlStageSetting.Type == TypeValue.行動ID)
            {
                createStageSetting.actionSettingList.Add(stageSetting);
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
                createStageSettings.Add(createStageSetting);
            }
            else if (xmlStageSetting.Type == TypeValue.玩家子彈)
            {
                createStageSetting.coreSetting.playerGameTime = 0;
                createStageSetting.coreSetting.playerShift = false;
                createStageSetting.coreSetting.playerShotDelayTime = 0;
                createStageSettings.Add(createStageSetting);
            }
            else if (xmlStageSetting.Type == TypeValue.Power)
            {
                createStageSettings.Add(createStageSetting);
            }

        }

        bool checkAnglePos1Reference = false;
        bool checkAnglePos2Reference = false;
        bool checkPosReference = false;
        bool checkAngleReference = false;
        while (k < xmlStageSettings.Count)
        {
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

                StartTypeHandle(xmlStageSetting, ref createStageSettings);

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
                switch (xmlStageSetting.Type)
                {
                    case TypeValue.位置參數:
                        if (!checkAnglePos1Reference && !checkAnglePos2Reference)
                            break;

                        poses = new List<Pos>();
                        pos = new Pos();
                        poses.Add(pos);
                        if (checkAnglePos1Reference)
                        {
                            angleSet.pos1 = pos;
                            checkAnglePos1Reference = false;
                            checkAnglePos2Reference = true;
                        }
                        else if (checkAnglePos2Reference)
                        {
                            angleSet.pos2 = pos;
                            checkAnglePos2Reference = false;
                        }
                        break;

                    case TypeValue.加位置:
                        pos = new Pos();
                        poses.Add(pos);
                        break;

                    case TypeValue.角度參數:
                        // Do nothing
                        break;

                    case TypeValue.加角度:
                        angleSet = new AngleSet();
                        angleSets.Add(angleSet);
                        break;

                    default:
                        poses = new List<Pos>();
                        angleSets = new List<AngleSet>();
                        break;
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
                        callRuleScheme.callPos = poses;
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
                        StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref pos.ADdistance);
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
                    StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref angleSet.angle);
                }
                else if (xmlStageSetting.AngleParamTypeVal == AngleParamType.角度ID記錄)
                {
                    StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref angleSet.recordId);
                }
                else if (xmlStageSetting.AngleParamTypeVal == AngleParamType.角度IDs)//1=自身 2=父
                {
                    StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref angleSet.Ids);
                    if (angleSet.Ids.Length != 2)
                    {
                        TestEnd("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order);
                    }
                }
                else if (xmlStageSetting.AngleParamTypeVal == AngleParamType.角度ID旋轉)//1=自身 2=父
                {
                    StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref angleSet.IdRotateZ);
                }
                else if (xmlStageSetting.AngleParamTypeVal == AngleParamType.角度ID移動角度)//1=自身 2=父
                {
                    StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref angleSet.IdMoveAngle);
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
                    StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref pos.point);
                }
                else if (xmlStageSetting.PosParamTypeVal == PosParamType.位置ID)//1=自身 2=父
                {
                    StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref pos.Id);
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
                !LoadCtrl.Instance.pool.objectPoolList.Any(r => r.name == xmlStageSetting.ParamVals[0])
                )
                {
                    TestEnd("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order);
                }
                else if (stageSetting.type == TypeValue.對話 && !LoadCtrl.Instance.pool.dialogPoolList.Any(r => r.name == xmlStageSetting.ParamVals[0]))
                {
                    TestEnd("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order);
                }
                else if (stageSetting.type == TypeValue.播放音樂 && !LoadCtrl.Instance.pool.musicPoolList.Any(r => r.name == xmlStageSetting.ParamVals[0]))
                {
                    TestEnd("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order);
                }
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.obj);
            }
            else if (xmlStageSetting.Type == TypeValue.動畫)
            {
                if (stageSetting.type == TypeValue.標題 && !LoadCtrl.Instance.pool.titlePoolList.Any(r => r.name == xmlStageSetting.ParamVals[0]))
                {
                    TestEnd("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order);
                }
                else if (stageSetting.type == TypeValue.符卡 && !LoadCtrl.Instance.pool.spellPoolList.Any(r => r.name == xmlStageSetting.ParamVals[0]))
                {
                    TestEnd("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order);
                }

                if (stageSetting.type == TypeValue.符卡)
                {
                    StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.spellAni);
                }
                else
                {
                    StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.ani);
                }
            }

            else if (xmlStageSetting.Type == TypeValue.Sprite)
            {
                if (!LoadCtrl.Instance.pool.spritePoolList.Any(r => r.name == xmlStageSetting.ParamVals[0]))
                {
                    TestEnd("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order);
                }
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.sprite);
            }
            else if (xmlStageSetting.Type == TypeValue.Name)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.name);
            }
            else if (xmlStageSetting.Type == TypeValue.啟動依ID)
            {
                callRuleScheme = new CallRuleScheme();
                if (createStageSetting.coreSetting.Id == stageSetting.Id)
                {
                    callRuleScheme.createStageSetting = createStageSetting;
                }
                else
                {
                    callRuleScheme.coreId = createStageSetting.coreSetting.Id;
                    callRuleScheme.actId = stageSetting.Id;
                }

                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref callRuleScheme.callExistId);
                callRuleSchemeById.Add(callRuleScheme);
            }
            else if (xmlStageSetting.Type == TypeValue.啟動依玩家死亡)
            {
                callRuleScheme.callPlayerDead = true;
            }
            else if (xmlStageSetting.Type == TypeValue.啟動位置是活動)
            {
                callRuleScheme.callPosIsActive = true;
            }
            else if (xmlStageSetting.Type == TypeValue.加ID)
            {
                stageSetting.addIds.Add(uint.Parse(xmlStageSetting.ParamVals[0]));
            }
            else if (xmlStageSetting.Type == TypeValue.啟動依遊戲時間)
            {
                callRuleScheme = new CallRuleScheme();
                if (createStageSetting.coreSetting.Id == stageSetting.Id)
                {
                    callRuleScheme.createStageSetting = createStageSetting;
                }
                else
                {
                    callRuleScheme.coreId = createStageSetting.coreSetting.Id;
                    callRuleScheme.actId = stageSetting.Id;
                }
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref callRuleScheme.callGameTime);
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.callGameTime);
                callRuleSchemesByGTime.Add(callRuleScheme);

            }
            else if (xmlStageSetting.Type == TypeValue.啟動依開始時間後)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref callRuleScheme.callStartAfTime);
                if (callRuleScheme.callStartAfTime == 0)
                {
                    callRuleScheme.callStartAfTime = 1;
                    // Debug.LogError("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order + ", callStartAfTime0");
                }
            }
            else if (xmlStageSetting.Type == TypeValue.啟動依結束時間前)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref callRuleScheme.callEndBfTime);
            }
            else if (xmlStageSetting.Type == TypeValue.行動時間)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.actTime);
                if (stageSetting.actTime == 0)
                {
                    // stageSetting.actTime = 1;
                    // Debug.LogError("table:" + xmlStageSetting.table + "     order:" + xmlStageSetting.Order + ", actTime0");
                }
            }
            else if (xmlStageSetting.Type == TypeValue.回收時間)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.restoreTime);
            }
            else if (xmlStageSetting.Type == TypeValue.死亡時間)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.deadTime);
            }
            else if (xmlStageSetting.Type == TypeValue.時位時間)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.timPosTime);
            }

            else if (xmlStageSetting.Type == TypeValue.玩家遊戲時間)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.playerGameTime);
            }
            else if (xmlStageSetting.Type == TypeValue.玩家子彈間隔)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.playerShotHzTime);
            }
            else if (xmlStageSetting.Type == TypeValue.出生動畫時間)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.birthDurTime);
            }
            else if (xmlStageSetting.Type == TypeValue.加速度)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.addSpeed);
            }
            else if (xmlStageSetting.Type == TypeValue.最大速度)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.maxSpeed);
            }
            else if (xmlStageSetting.Type == TypeValue.最小速度)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.minSpeed);
            }
            else if (xmlStageSetting.Type == TypeValue.速度)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.speed);
            }
            else if (xmlStageSetting.Type == TypeValue.時位變速點)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.timPosSpeedPoint);
            }
            else if (xmlStageSetting.Type == TypeValue.時位開始速度)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.timPosStartSpeed);
            }
            else if (xmlStageSetting.Type == TypeValue.時位結束速度)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.timPosEndSpeed);
            }
            else if (xmlStageSetting.Type == TypeValue.回收距離)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.restoreDistance);
            }
            else if (xmlStageSetting.Type == TypeValue.啟動依位置距離)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref callRuleScheme.callPosDis);
            }
            else if (xmlStageSetting.Type == TypeValue.出生動畫速率)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.birthAniSpeed);
            }
            else if (xmlStageSetting.Type == TypeValue.出生動畫開始)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.birthAniStart);
            }
            else if (xmlStageSetting.Type == TypeValue.傷害)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.dmg);
            }
            else if (xmlStageSetting.Type == TypeValue.HP)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.hp);
            }
            else if (xmlStageSetting.Type == TypeValue.無敵)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.isInvincible);
            }
            else if (xmlStageSetting.Type == TypeValue.貫穿)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.isThrough);
            }
            else if (xmlStageSetting.Type == TypeValue.是界內)
            {
                stageSetting.isIn = true;
            }
            else if (xmlStageSetting.Type == TypeValue.旋轉是移動角度)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.rotateIsMoveAngle);
            }
            else if (xmlStageSetting.Type == TypeValue.玩家Power需求)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.playerPowerNeed);
            }
            else if (xmlStageSetting.Type == TypeValue.玩家子彈延遲)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.playerShotDelayTime);
            }
            else if (xmlStageSetting.Type == TypeValue.玩家Shift)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.playerShift);
            }
            else if (xmlStageSetting.Type == TypeValue.Power掉落)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.powerGive);
            }


            else if (xmlStageSetting.Type == TypeValue.符卡時間)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.spellTime);
            }
            else if (xmlStageSetting.Type == TypeValue.記錄位置ID)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.recordPosId);
            }
            else if (xmlStageSetting.Type == TypeValue.記錄角度ID)
            {
                StrToValFunc.Set(xmlStageSetting.ParamVals[0], ref stageSetting.recordAngleId);
            }
            k++;
        }

        callRuleSchemesByGTime.Sort((x, y) =>
        {
            if (x.callGameTime == null && y.callGameTime == null) return 0;
            if (x.callGameTime == null) return 1;
            if (y.callGameTime == null) return -1;
            return x.callGameTime.Value.CompareTo(y.callGameTime.Value);
        });
    }



}