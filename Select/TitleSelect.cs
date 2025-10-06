using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static PlayerSaveData;
using static GameConfig;
using System.Collections.Generic;
public class TitleSelect : SelectBase<TitleSelect, TextOption>
{
    protected override void ClickHandle()
    {

        if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.Z)))
        {
            Hide();
            switch (nowBtn.name)
            {
                case TextName.開始遊戲:
                    GameSelect.practiceId = 0;
                    GameReplay.playKeys = null;
                    DifficultSelect.Instance.Show();
                    DifficultSelect.Instance.SetNextAction(() =>
                    {
                        LoadCtrl.Instance.SwitchPage(PageIndex.Game);
                    });
                    DifficultSelect.Instance.SetBackAction(() =>
                    {
                        Show();
                    });
                    break;
                case TextName.彈幕練習:
                    GameReplay.playKeys = null;
                    DifficultSelect.Instance.Show();
                    DifficultSelect.Instance.SetNextAction(() =>
                    {
                        PracticeSelect.Instance.Show();
                    });
                    DifficultSelect.Instance.SetBackAction(() =>
                    {
                        Show();
                    });
                    break;
                case TextName.播放錄像:
                    ReplaySelect.Instance.IsRead = true;
                    ReplaySelect.Instance.Show();
                    ReplaySelect.Instance.SetBackAction(() =>
                    {
                        Show();
                    });
                    break;
                case TextName.遊戲設置:
                    OptionSelect.Instance.Show();
                    OptionSelect.Instance.SetBackAction(() =>
                    {
                        Show();
                    });
                    break;
                case TextName.按鍵設置:
                    KeyBoardSelect.Instance.Show();
                    KeyBoardSelect.Instance.SetBackAction(() =>
                    {
                        Show();
                    });
                    break;
                case TextName.離開遊戲:
#if UNITY_STANDALONE                 
                    Application.Quit();
#endif
                    TestEnd();
                    break;

                default:
                    break;
            }

        }
    }
}
