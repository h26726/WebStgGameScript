using UnityEngine;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static GameConfig;
using static PlayerKeyHelper;
using static PlayerSaveData;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

public class PlayerCtrlObj : UnitCtrlObj
{
    public GameObject core;
    Collider2D coreCollider2D { get; set; }
    SpriteRenderer coreSpriteRenderer { get; set; }

    public void PlayerAwake()
    {
        core.SetActive(false);
        coreCollider2D = core.GetComponent<Collider2D>();
        coreSpriteRenderer = core.GetComponent<SpriteRenderer>();
    }
}

