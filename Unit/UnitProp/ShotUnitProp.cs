using UnityEngine;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static GameConfig;
using static PlayerKeyHelper;
using static SaveJsonData;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class ShotUnitProp : UnitPropBase
{
    public ShotUnitProp(UnitCtrlBase unitCtrl) : base(unitCtrl)
    {
    }
}

