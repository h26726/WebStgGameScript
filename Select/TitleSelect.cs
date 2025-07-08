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
public class TitleSelect : SelectBase<TitleSelect, TextOption>
{
    protected override void ClickExtraHandle()
    {

        if (Input.GetKeyDown(GetSetKey(KeyCode.Z)))
        {
            Hide();
            switch (nowBtn.name)
            {
                case TextName.開始遊戲:
                    GameSystem.Instance.selectPracticeId = 0;
                    GameSystem.Instance.isReplay = false;
                    DifficultSelect.Instance.Show();
                    DifficultSelect.Instance.AddNext(() =>
                    {
                        LoadingCtrl.Instance.SwitchPage(LoadingCtrl.PageIndex.Game);
                    });
                    DifficultSelect.Instance.AddBack(() =>
                    {
                        Show();
                    });
                    break;
                case TextName.彈幕練習:
                    GameSystem.Instance.isReplay = false;
                    DifficultSelect.Instance.Show();
                    DifficultSelect.Instance.AddNext(() =>
                    {
                        PracticeSelect.Instance.Show();
                    });
                    DifficultSelect.Instance.AddBack(() =>
                    {
                        Show();
                    });
                    break;
                case TextName.播放錄像:
                    ReplaySelect.Instance.IsRead = true;
                    ReplaySelect.Instance.Show();
                    ReplaySelect.Instance.AddBack(() =>
                    {
                        Show();
                    });
                    break;
                case TextName.遊戲設置:
                    OptionSelect.Instance.Show();
                    OptionSelect.Instance.AddBack(() =>
                    {
                        Show();
                    });
                    break;
                case TextName.按鍵設置:
                    KeyBoardSelect.Instance.Show();
                    KeyBoardSelect.Instance.AddBack(() =>
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
