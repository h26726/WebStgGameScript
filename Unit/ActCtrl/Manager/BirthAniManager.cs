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
public class WaitBirthAniManager
{
    public UnitCtrlObj unitCtrlObj;
    public UnitPropBase unitProp;
    public bool isRun = false;
    public uint showAniTime { get; set; }
    public uint birthDurTime;
    public float birthAniSpeed;
    public float birthAniStart;
    public SettingBase setting;
    public WaitBirthAniManager(ActCtrl actCtrl)
    {
        this.isRun = true;
        this.setting = actCtrl.setting;
        this.unitCtrlObj = actCtrl.unitCtrlObj;
        this.unitProp = actCtrl.unitProp;
        birthAniSpeed = setting.birthAniSpeed == null ? DEFAULT_SHOW_ANI_SPEED : setting.birthAniSpeed.Value;
        birthAniStart = setting.birthAniStart == null ? DEFAULT_SHOW_ANI_START : setting.birthAniStart.Value;
        birthDurTime = setting.birthDurTime == null ? DEFAULT_SHOW_ANI_TIME : setting.birthDurTime.Value;
        unitCtrlObj.PlayBirthAniInit(this);

    }
    public void UpdateFadeIn()
    {
        showAniTime++;
        // AddPrintContent($"showAniTime:{showAniTime}");
        if (showAniTime < birthDurTime)
        {
            return;
        }
        isRun = false;
    }

}

