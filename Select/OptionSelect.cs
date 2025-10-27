using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static SaveJsonData;
using static GameConfig;
using System.Collections.Generic;
public class OptionSelect : SelectBase<OptionSelect, TextOption>
{
    public Sprite[] screenModeSprites;
    uint tmpBGMVolume;
    uint tmpBGSVolume;
    ScreenMode tmpScreenModeType;
    uint longDownTime = 0;
    protected override void ClickHandle()
    {

        if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.Z)))
        {
            if (nowBtn.name == TextName.儲存並返回)
            {
                configSaveDatas.BGMVolume = tmpBGMVolume;
                configSaveDatas.BGSVolume = tmpBGSVolume;
                configSaveDatas.screenModeType = tmpScreenModeType;
                SaveConfigSaveData();
                Hide();
                TitleSelect.Instance.Show();
            }
            else if (nowBtn.name == TextName.取消)
            {
                Hide();
                UseConfigSaveDatas();
                TitleSelect.Instance.Show();
                LoadCtrl.Instance.ChangeVolumeByConfigSave();
            }
        }
        else if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.X)))
        {
            Hide();
            UseConfigSaveDatas();
            TitleSelect.Instance.Show();
            LoadCtrl.Instance.ChangeVolumeByConfigSave();

        }
        else if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.LeftArrow)))
        {
            longDownTime = 0;
            OptionValChange(-1);
        }
        else if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.RightArrow)))
        {
            longDownTime = 0;
            OptionValChange(1);
        }
        else if (Input.GetKey(TransferToPlayerSetKey(KeyCode.LeftArrow)))
        {
            longDownTime++;
            if (longDownTime > 30 && longDownTime % 5 == 0)
                OptionValChange(-1);
        }
        else if (Input.GetKey(TransferToPlayerSetKey(KeyCode.RightArrow)))
        {
            longDownTime++;
            if (longDownTime > 30 && longDownTime % 5 == 0)
                OptionValChange(1);
        }
    }

    public void UseConfigSaveDatas()
    {
        foreach (var btn in btns)
        {
            if (btn.text == null)
            {
                btn.text = btn.animator.gameObject.GetComponent<Text>();
                btn.image = btn.animator.gameObject.GetComponent<Image>();
            }

            if (btn.name == TextName.音樂音量)
            {
                tmpBGMVolume = configSaveDatas.BGMVolume;
                btn.text.text = configSaveDatas.BGMVolume.ToString();
            }
            else if (btn.name == TextName.音效音量)
            {
                tmpBGSVolume = configSaveDatas.BGSVolume;
                btn.text.text = configSaveDatas.BGSVolume.ToString();
            }
            else if (btn.name == TextName.畫面模式)
            {
                tmpScreenModeType = configSaveDatas.screenModeType;
                btn.image.sprite = screenModeSprites[(int)configSaveDatas.screenModeType];
            }
        }
    }
    void OptionValChange(int num)
    {
        if (nowBtn.name == TextName.音樂音量)
        {
            if (tmpBGMVolume == 0) return;
            tmpBGMVolume = (uint)(tmpBGMVolume + num);
            nowBtn.text.text = tmpBGMVolume.ToString();
            LoadCtrl.Instance.audioSource.volume = tmpBGMVolume / 100f;
        }
        else if (nowBtn.name == TextName.音效音量)
        {
            if (tmpBGSVolume == 0) return;
            tmpBGSVolume = (uint)(tmpBGSVolume + num);
            nowBtn.text.text = tmpBGSVolume.ToString();
        }
        else if (nowBtn.name == TextName.畫面模式 && longDownTime == 0)
        {
            if (tmpScreenModeType == ScreenMode.FullScreen)
            {
                tmpScreenModeType = ScreenMode.Windowed;
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
            }
            else if (tmpScreenModeType == ScreenMode.Windowed)
            {
                tmpScreenModeType = ScreenMode.FullScreen;
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }
            nowBtn.image.sprite = screenModeSprites[(int)tmpScreenModeType];
        }
    }
}
