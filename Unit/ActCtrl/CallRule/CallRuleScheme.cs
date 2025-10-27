using System;
using System.Collections.Generic;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;

[Serializable]
public class CallRuleScheme
{
    public uint callGameTime = GameConfig.UINT_INVAILD;
    public bool callPlayerDead = false;
    public uint callStartAfTime = GameConfig.UINT_INVAILD;
    public uint callEndBfTime = GameConfig.UINT_INVAILD;
    public uint callExistId = GameConfig.UINT_INVAILD;
    public List<Pos> callPos = null;
    public float callPosDis = GameConfig.FLOAT_INVAILD;
    public bool callPosIsActive = false;
    public uint coreId = GameConfig.UINT_INVAILD;
    public uint actId = GameConfig.UINT_INVAILD;
    public CreateStageSetting createStageSetting = null;
    public CallTriggerFlag callTriggerFlag = CallTriggerFlag.None;
    public CallTargetFlag callTargetFlag = CallTargetFlag.None;

    public enum CallTriggerFlag
    {
        None = 0,
        Pos = 1 << 0,
        IdTime = 1 << 1,
    }

    public enum CallTargetFlag
    {
        None = 0,
        Create = 1 << 0,
        ActRun = 1 << 1,
    }


    public string Print()
    {
        string str = $"------------{Environment.NewLine}";
        str += $"[CRS]  callTriggerFlag= {callTriggerFlag} {Environment.NewLine}";
        str += $"[CRS]  callTargetFlag= {callTargetFlag} {Environment.NewLine}";

        if (!InvalidHelper.IsInvalid(coreId))
            str += $"[CRS]  baseId= {coreId} , actId= {actId}{Environment.NewLine}";

        if (createStageSetting != null)
        {
            str += $"[CRS]  createStageSetting {Environment.NewLine}";
            str += createStageSetting.Print();
        }

        if (!InvalidHelper.IsInvalid(callGameTime))
            str += $"[CRS]  CallGameTime= {callGameTime}{Environment.NewLine}";

        if (!InvalidHelper.IsInvalid(callExistId))
            str += $"[CRS]  CallExistId= {callExistId}{Environment.NewLine}";

        if (!InvalidHelper.IsInvalid(callStartAfTime))
            str += $"[CRS]  CallStartAfTime= {callStartAfTime}{Environment.NewLine}";

        if (!InvalidHelper.IsInvalid(callEndBfTime))
            str += $"[CRS]  CallEndBfTime= {callEndBfTime}{Environment.NewLine}";

        if (!InvalidHelper.IsInvalid(callPosDis))
            str += $"[CRS]  CallPosDis= {callPosDis}{Environment.NewLine}";

        if (callPos != null)
        {
            str += $"[CRS]  CallPos {Environment.NewLine}";
            foreach (var item in callPos)
            {
                str += $"{item.Print()}{Environment.NewLine}";
            }
        }

        return str;
    }


}
