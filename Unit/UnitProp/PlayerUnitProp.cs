using UnityEngine;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static GameConfig;
using static PlayerKeyHelper;
using static PlayerSaveData;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerUnitProp : UnitPropBase
{
    public PlayerUnitProp(UnitCtrlBase unitCtrl) : base(unitCtrl)
    {
    }
}

