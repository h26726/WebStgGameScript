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
using static SaveJsonData;
using static GameConfig;
using System.Linq;
using static LoadCtrl;

public static class GameDebut
{
    public static uint No = 0;
    public static bool isNotDebutEnemyShot;
    public static Dictionary<uint, UnitCtrlBase> poolDictByNo;
    public static Dictionary<uint, UnitCtrlBase> coreDictById;
    //取位置用，出現與消失時註冊解註冊
    public static List<uint> poolNos;

    //TriggerDebut、TriggerRestore在後續統一註冊與解註冊(出池→出現→註冊→消失→解註冊→回池)
    public static List<(CreateStageSetting createStageSetting, UnitCtrlBase parentUnitCtrl)> lateDebuts;
    public static List<UnitCtrlBase> lateRestores;
    public static List<CreateStageSetting> lateDebutByCreateSettings;
    public static List<(uint coreId, uint actId)> lateCallActs;

    public static void GameStartSet()
    {
        lateDebutByCreateSettings.Add(GameSelect.playerData.playerCreateStageSetting);
    }

    public static void Init()
    {
        poolDictByNo = new Dictionary<uint, UnitCtrlBase>();
        coreDictById = new Dictionary<uint, UnitCtrlBase>();
        //取位置用，出現與消失時註冊解註冊
        poolNos = new List<uint>();

        //TriggerDebut、TriggerRestore在後續統一註冊與解註冊(出池→出現→註冊→消失→解註冊→回池)
        lateDebuts = new List<(CreateStageSetting createStageSetting, UnitCtrlBase parentUnitCtrl)>();
        lateRestores = new List<UnitCtrlBase>();
        lateDebutByCreateSettings = new List<CreateStageSetting>();
        lateCallActs = new List<(uint coreId, uint actId)>();
        Reset();
    }

    public static void Reset()
    {
        No = 0;
        isNotDebutEnemyShot = false;
        poolDictByNo.Clear();
        coreDictById.Clear();
        poolNos.Clear();
        lateDebuts.Clear();
        lateRestores.Clear();
        lateDebutByCreateSettings.Clear();
        lateCallActs.Clear();
    }

    public static void AddQueueDebut(CreateStageSetting createStageSetting, UnitCtrlBase parentUnitCtrl)
    {
        lateDebuts.Add((createStageSetting, parentUnitCtrl));
    }

    public static void SetNo(UnitCtrlBase unitCtrlBase)
    {
        unitCtrlBase.debutNo = No;
        No++;
    }

    public static void AddQueueRestore(UnitCtrlBase unitCtrlBase)
    {
        lateRestores.Add(unitCtrlBase);
    }

    public static void UpdateHandler()
    {
        TriggerDebuts(lateDebutByCreateSettings);
        TriggerCallActs(lateCallActs);
        for (int i = 0; i < poolNos.Count; i++)
        {
            var unitCtrl = poolDictByNo[poolNos[i]];
            var unitProp = unitCtrl.unitProp;
            unitCtrl.DebutUpdateHandler();
            if (unitCtrl == GamePlayer.nowUnit)
            {
                GamePlayer.nowUnit.PlayerUpdateHandler();
            }
            if (isAllowDebut(unitCtrl))
            {
                TriggerDebuts(unitProp.propLateDebutByCreateSettings, unitCtrl);
            }
            TriggerCallActs(unitProp.propLateCallActs, unitCtrl);

            if (!unitCtrl.TryRestoreUpdateHandler())
                unitCtrl.TryDeadUpdateHandler();
        }

        foreach (var (createStageSetting, parentUnitCtrl) in lateDebuts)
        {
            Debut(createStageSetting, parentUnitCtrl);
        }
        lateDebuts.Clear();

        foreach (var unitCtrl in lateRestores)
        {
            Restore(unitCtrl);
        }
        lateRestores.Clear();
    }

    static void Debut(CreateStageSetting createStageSetting, UnitCtrlBase parentUnitCtrl)
    {
        UnitCtrlBase unitCtrl = null;
        if (createStageSetting.IsOperateExistBossByType())
        {
            unitCtrl = GameBoss.nowUnit;
        }
        else
        {
            var Id = createStageSetting.Id;
            unitCtrl = ObjectPoolCtrl.Instance.GetOne(createStageSetting.coreSetting.obj);
            unitCtrl.Id = Id;
            GameDebut.CoreDictAdd(Id, unitCtrl);
            GameDebut.SetNo(unitCtrl);
            poolDictByNo.Add(unitCtrl.debutNo, unitCtrl);
            poolNos.Add(unitCtrl.debutNo);
            GamePlayer.TryRegister(createStageSetting.type, unitCtrl);
            GameBoss.TryRegister(createStageSetting.type, unitCtrl);
        }
        unitCtrl.Unit1_SetVal_OnOutToIn(createStageSetting, parentUnitCtrl);
    }

    static void Restore(UnitCtrlBase unitCtrl)
    {
        GameDebut.CoreDictRemove(unitCtrl);
        ObjectPoolCtrl.Instance.RestoreOne(unitCtrl);
        if (!poolDictByNo.ContainsKey(unitCtrl.debutNo))
        {
            Debug.LogError("unit.debutDictNo not Exist:" + unitCtrl.debutNo);
        }
        poolDictByNo.Remove(unitCtrl.debutNo);
        int index = poolNos.IndexOf(unitCtrl.debutNo); // O(n)，可額外維護 key→index map 優化
        poolNos.RemoveAt(index);
        GamePlayer.TryUnRegister(unitCtrl);
        GameBoss.TryUnRegister(unitCtrl);
        unitCtrl.Reset();
        unitCtrl.unitCtrlObj.CloseUnit();
    }

    static bool isAllowDebut(UnitCtrlBase unitCtrl)
    {
        return !isNotDebutEnemyShot || unitCtrl == GamePlayer.nowUnit || unitCtrl == GameBoss.nowUnit;
    }

    static void TriggerDebuts(List<CreateStageSetting> debutList, UnitCtrlBase parentUnitCtrl = null)
    {
        foreach (var createStageSetting in debutList)
        {
            createStageSetting.TriggerDebut(parentUnitCtrl);
        }
        debutList.Clear();
    }
    public static void TriggerCallActs(List<(uint coreId, uint actId, ActionProp parentActionProp)> callActList, UnitCtrlBase parent)
    {
        foreach ((uint coreId, uint actId, ActionProp parentActionProp) in callActList)
        {
            TriggerCallAct(coreId, actId, parentActionProp, parent);
        }
        callActList.Clear();
    }

    public static void TriggerCallActs(List<(uint coreId, uint actId)> callActList)
    {
        foreach ((uint coreId, uint actId) in callActList)
        {
            TriggerCallAct(coreId, actId, null, null);
        }
        callActList.Clear();
    }

    static void TriggerCallAct(uint coreId, uint actId, ActionProp parentActionProp, UnitCtrlBase parentUnitCtrl)
    {
        var unitCtrl = (parentUnitCtrl != null && parentUnitCtrl.coreSetting.Id == coreId)
                ? parentUnitCtrl
                : UnitCtrlBase.GetOutSideUnit(coreId);

        if (unitCtrl == null)
        {
            Debug.LogError($"TriggerCallAct Id not exist:{coreId}");
            return;
        }

        if (!unitCtrl.actCtrlDict.TryGetValue(actId, out var actCtrl))
        {
            Debug.LogError($"TriggerCallAct actCtrlDict actId not exist:{coreId},{actId}");
            return;
        }
        actCtrl.Act1_RunAndReset(parentActionProp);
    }

    public static void ClearAllEnemyShot()
    {
        isNotDebutEnemyShot = true;
        var Debuts = poolDictByNo.Where(r => r.Value is EnemyShotUnitCtrl).ToList();
        foreach (var DebutPair in Debuts)
        {
            DebutPair.Value.unitProp.isTriggerDead = true;
        }
    }
    public static void ClearAll()
    {
        var Debuts = poolDictByNo.ToList();
        foreach (var DebutPair in Debuts)
        {
            var unit = DebutPair.Value;
            unit.Reset();
            ObjectPoolCtrl.Instance.RestoreOne(unit);
            unit.unitCtrlObj.CloseUnit();
        }
    }

    public static void CoreDictAdd(uint Id, UnitCtrlBase unitCtrl)
    {
        if (!coreDictById.ContainsKey(Id))
            coreDictById.Add(Id, unitCtrl);
        else
            coreDictById[Id] = unitCtrl;
    }

    public static void CoreDictRemove(UnitCtrlBase unitCtrl)
    {
        coreDictById.Remove(unitCtrl.Id);
    }


}
