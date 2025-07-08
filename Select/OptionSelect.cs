using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using static CommonData;
using static CommonFunc;
using static PlayerKeyCtrl;
using static PlayerSaveData;
using static GameConfig;
using System.Collections.Generic;
public class OptionSelect : SelectBase<OptionSelect, TextOption>
{
    public Sprite[] screenModeSprites;
    uint BGMVolume;
    uint BGSVolume;
    ScreenMode screenModeType;
    uint longDownTime = 0;
    protected override void ClickExtraHandle()
    {
        
        if (Input.GetKeyDown(GetSetKey(KeyCode.Z)))
        {
            if (nowBtn.name == TextName.儲存並返回)
            {
                configSaveDatas.BGMVolume = BGMVolume;
                configSaveDatas.BGSVolume = BGSVolume;
                configSaveDatas.screenModeType = screenModeType;
                SaveConfigSaveData();
                Hide();
                TitleSelect.Instance.Show();
            }
            else if (nowBtn.name == TextName.取消)
            {
                Hide();
                TitleSelect.Instance.Show();
                LoadingCtrl.Instance.ConfigSaveHandle();
            }
        }
        else if (Input.GetKeyDown(GetSetKey(KeyCode.X)))
        {
            Hide();
            TitleSelect.Instance.Show();
            LoadingCtrl.Instance.ConfigSaveHandle();

        }
        else if (Input.GetKeyDown(GetSetKey(KeyCode.LeftArrow)))
        {
            longDownTime = 0;
            OptionChange(-1);
        }
        else if (Input.GetKeyDown(GetSetKey(KeyCode.RightArrow)))
        {
            longDownTime = 0;
            OptionChange(1);
        }
        else if (Input.GetKey(GetSetKey(KeyCode.LeftArrow)))
        {
            longDownTime++;
            if (longDownTime > 30 && longDownTime % 5 == 0)
                OptionChange(-1);
        }
        else if (Input.GetKey(GetSetKey(KeyCode.RightArrow)))
        {
            longDownTime++;
            if (longDownTime > 30 && longDownTime % 5 == 0)
                OptionChange(1);
        }
    }

    public void LoadData()
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
                BGMVolume = configSaveDatas.BGMVolume;
                btn.text.text = configSaveDatas.BGMVolume.ToString();
            }
            else if (btn.name == TextName.音效音量)
            {
                BGSVolume = configSaveDatas.BGSVolume;
                btn.text.text = configSaveDatas.BGSVolume.ToString();
            }
            else if (btn.name == TextName.畫面模式)
            {
                screenModeType = configSaveDatas.screenModeType;
                btn.image.sprite = screenModeSprites[(int)configSaveDatas.screenModeType];
            }
        }
    }
    void OptionChange(int num)
    {
        if (nowBtn.name == TextName.音樂音量)
        {
            if (BGMVolume == 0) return;
            BGMVolume = (uint)(BGMVolume + num);
            nowBtn.text.text = BGMVolume.ToString();
            LoadingCtrl.Instance.audioSource.volume = BGMVolume / 100f;
        }
        else if (nowBtn.name == TextName.音效音量)
        {
            if (BGSVolume == 0) return;
            BGSVolume = (uint)(BGSVolume + num);
            nowBtn.text.text = BGSVolume.ToString();
        }
        else if (nowBtn.name == TextName.畫面模式 && longDownTime == 0)
        {
            if (screenModeType == ScreenMode.FullScreen)
            {
                screenModeType = ScreenMode.Windowed;
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
            }
            else if (screenModeType == ScreenMode.Windowed)
            {
                screenModeType = ScreenMode.FullScreen;
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }
            nowBtn.image.sprite = screenModeSprites[(int)screenModeType];
        }
    }
}
