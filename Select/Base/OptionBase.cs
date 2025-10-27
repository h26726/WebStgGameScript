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

[Serializable]
public class OptionBase
{
    public Animator animator;
    public Text text;
    public Image image;

    public bool isHide;
    public bool isDisable;

}
[Serializable]
public class TextOption : OptionBase
{
    public TextName name = TextName.None;
}

[Serializable]
public class DifficultOption : OptionBase
{
    public Difficult difficult = Difficult.Easy;
}

[Serializable]
public class PracticeOption : OptionBase
{
    public uint practiceId = 0;

}

[Serializable]
public class KeyOption : TextOption
{
    public KeyCode keyCode = KeyCode.None;
}

[Serializable]
public class ReplayOption : OptionBase
{
    public Text[] line { get; set; }
    public uint no { get; set; }
    public InputField inputField { get; set; }
    public SaveJsonData.ReplaySaveData replayData { get; set; }
}

