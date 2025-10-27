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
public class WaitBirthAniManager
{
    public bool isRun;
    public uint showAniTime;
    public uint birthDurTime;
    public float birthAniSpeed;
    public float birthAniStart;
    public UnitCtrlObj unitCtrlObj;
    public UnitPropBase unitProp;
    public SettingBase setting;

    public WaitBirthAniManager()
    {
        Reset();
    }
    public void SetAndPlay(ActCtrl actCtrl)
    {
        this.isRun = true;
        this.setting = actCtrl.setting;
        this.unitCtrlObj = actCtrl.unitCtrlObj;
        this.unitProp = actCtrl.unitProp;
        birthAniSpeed = !InvalidHelper.IsInvalid(setting.birthAniSpeed) ? setting.birthAniSpeed : DEFAULT_SHOW_ANI_SPEED;
        birthAniStart = !InvalidHelper.IsInvalid(setting.birthAniStart) ? setting.birthAniStart : DEFAULT_SHOW_ANI_START;
        birthDurTime = !InvalidHelper.IsInvalid(setting.birthDurTime) ? setting.birthDurTime : DEFAULT_SHOW_ANI_TIME;
        unitCtrlObj.PlayBirthAniInit(this);
    }

    public void Reset()
    {
        isRun = false;
        showAniTime = 0;
        birthDurTime = 0;
        birthAniSpeed = 0;
        birthAniStart = 0;
        unitCtrlObj = null;
        unitProp = null;
        setting = null;
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

