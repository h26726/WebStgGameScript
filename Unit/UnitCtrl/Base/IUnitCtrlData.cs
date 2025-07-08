using UnityEngine;
using static CommonData;
using static CommonFunc;
using static GameConfig;
using static PlayerKeyCtrl;
using static PlayerSaveData;
using System;
using System.Linq;
using System.Collections.Generic;

public interface IUnitCtrlData
{
    public Vector2 GetTransformPos();
    public float GetRotateZ();
    public float GetChildRotateZ();
    public void SetRotateZ(float RotateZ);
    public void SetChildRotateZ(float ChildRotateZ);
    public bool TryGetActionMoveAngle(uint key, out float angle);
    public IUnitCtrlData GetParent();
    public UnitPropBase GetUnitProp();

    public Vector2 GetPos(List<Pos> pos);
    public Vector2 GetPosById(uint Id);
    public uint GetCoreSettingId();
    public void OnActTimeEndCustomize();
    public void MovePos(Vector2 pos);
    public void MoveTranslate(Vector2 moveVector);
    public float GetAngle(List<AngleSet> angle, out bool isNewAngle);
    public void HandleDead();
    public void RestoreIntoPool();

    public void TriggerRestore();
    public void TriggerDead();

    public void ClearAllAction(ActCtrl actCtrl);
    public void AddPrintContent(string content);
    public void ExtHandle(CallRule callRule, ActCtrl actCtrl);

}

