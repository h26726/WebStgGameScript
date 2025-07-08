using System;
using System.Collections.Generic;
using static CommonData;
using static CommonFunc;

public class CallRuleScheme
{
    public uint? callGameTime = null;
    public bool? callPlayerDead = null;
    public uint? callStartAfTime = null;
    public uint? callEndBfTime = null;
    public uint? callExistId = null;
    public List<Pos> callPos = null;
    public float? callPosDis = null;
    public bool? callPosIsActive = false;
    public uint? baseId = null;
    public uint? actId = null;
    public CreateStageSetting createStageSetting = null;
    public string Print()
    {
        string str = $"------------{Environment.NewLine}";

        if (baseId != null)
            str += $"[CRS]  baseId= {baseId} , actId= {actId}{Environment.NewLine}";

        if (createStageSetting != null)
        {
            str += $"[CRS]  createStageSetting {Environment.NewLine}";
            str += createStageSetting.Print();
        }

        if (callGameTime != null)
            str += $"[CRS]  CallGameTime= {callGameTime}{Environment.NewLine}";

        if (callExistId != null)
            str += $"[CRS]  CallExistId= {callExistId}{Environment.NewLine}";

        if (callStartAfTime != null)
            str += $"[CRS]  CallStartAfTime= {callStartAfTime}{Environment.NewLine}";

        if (callEndBfTime != null)
            str += $"[CRS]  CallEndBfTime= {callEndBfTime}{Environment.NewLine}";

        if (callPosDis != null)
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
