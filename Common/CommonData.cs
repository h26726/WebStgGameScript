using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static GameSystem;

public static class CommonData
{
    public enum GameProgressState
    {
        Stage = 0,
        Dialog = 1,
        BossTime = 2,
        SpellEndTrigger = 3,
    }
    public class GameDataClass
    {
        public Difficult selectDifficult;
        public List<CallRuleScheme> callParGameTimes = new List<CallRuleScheme>();
        public Dictionary<uint, List<CallRuleScheme>> callParamsById = new Dictionary<uint, List<CallRuleScheme>>();
    }

    public enum Difficult
    {
        Easy = 0,
        Normal = 1,
        Hard = 2,
        Lunatic = 3,
    }
    public enum TextName
    {
        None,
        開始遊戲 = 1,
        彈幕練習,
        播放錄像,
        遊戲設置,
        按鍵設置,
        離開遊戲,
        解除暫停 = 11,
        保存錄像,
        回到標題,
        重新開始,
        接續生命,

        音樂音量 = 21,
        音效音量,
        畫面模式,
        取消,
        儲存並返回,

        是 = 101,
        否 = 102,
    }




    public enum TypeValue
    {

        None,
        ID,

        行動ID,

        玩家,
        玩家子彈,
        BOSS,
        復位,
        BOSSDEAD,
        BOSSLEAVE,
        Power,
        符卡,
        標題,
        對話,
        播放音樂,
        關卡結束,
        下一關,


        啟動依ID = 101, 
        啟動依遊戲時間, 
        啟動依開始時間後, 
        啟動依結束時間前, 
        啟動依位置,
        啟動依位置距離,
        啟動依玩家死亡,
        啟動位置是活動,


        物件 = 201,
        Sprite,
        動畫,
        Name,
        傷害,
        HP,
        Power掉落,
        加ID,
        符卡動畫,


        行動時間 = 301,
        回收時間,
        死亡時間,
        符卡時間,

        移位置 = 401,
        相對位置,
        位置參數,
        加位置,
        記錄位置,
        記錄位置ID,

        移動角度 = 501,
        加移動角度,
        旋轉角度,
        加旋轉角度,
        角度參數,
        加角度,
        旋轉是移動角度,
        子旋轉角度,
        子加旋轉角度,
        記錄角度,
        記錄角度ID,


        速度 = 601,
        加速度,
        最小速度,
        最大速度,


        是界內 = 701,
        無敵,
        貫穿,
        回收距離,
        出生動畫速率,
        出生動畫時間,
        出生動畫開始,
        行動ID結束,

        時位時間 = 801,
        時位位置,
        時位變速點,
        時位開始速度,
        時位結束速度,
        時位移動角度,

        玩家遊戲時間 = 901,
        玩家子彈間隔,
        玩家Power需求,
        玩家Shift,
        玩家子彈延遲,
    }

    public enum AngleParamType
    {
        None = 0,
        角度值,
        角度IDs,
        新參照位置12,
        新參照角度,
        角度ID旋轉,
        角度ID記錄,
        角度ID移動角度,
    }

    public enum PosParamType
    {
        None = 0,
        位置xy,
        位置ID,
        位置Gene,
        位置參數角距距離,
        位置參數角距角度,
        新參照位置,
    }

    public static TypeValue[] StartType = new TypeValue[]{
        TypeValue.BOSS ,
        TypeValue.玩家,
        TypeValue.玩家子彈,
        TypeValue.ID,
        TypeValue.行動ID,
        TypeValue.Power,
		
        TypeValue.符卡,
        TypeValue.標題,
        TypeValue.對話,
        TypeValue.關卡結束,
        TypeValue.播放音樂,
        TypeValue.BOSSDEAD,
        TypeValue.BOSSLEAVE,
		TypeValue.下一關,
        TypeValue.復位,




    };




    public static TypeValue[] AngleType = new TypeValue[]{
        TypeValue.移動角度,
        TypeValue.加旋轉角度 ,
        TypeValue.加移動角度 ,
        TypeValue.旋轉角度 ,
        TypeValue.角度參數,
        TypeValue.加角度,
        TypeValue.子加旋轉角度 ,
        TypeValue.子旋轉角度 ,
        TypeValue.記錄角度,
    };

    public static TypeValue[] PosType = new TypeValue[]{
        TypeValue.移位置,
        TypeValue.相對位置,
        TypeValue.時位位置,
        TypeValue.啟動依位置,
        TypeValue.位置參數,
        TypeValue.加位置,
        TypeValue.記錄位置,

    };
    public static TypeValue[] IntType = new TypeValue[]{
        TypeValue.行動時間 ,
        TypeValue.回收時間,
        TypeValue.死亡時間,
        TypeValue.時位時間,
        TypeValue.啟動依遊戲時間, 
        TypeValue.啟動依開始時間後, 
        TypeValue.啟動依結束時間前, 
        TypeValue.玩家遊戲時間,
        TypeValue.玩家子彈間隔,
        TypeValue.出生動畫時間,
    };

    public static TypeValue[] FloatType = new TypeValue[]{
        TypeValue.啟動依位置距離 ,
        TypeValue.加速度 ,
        TypeValue.最大速度,
        TypeValue.最小速度,
        TypeValue.速度,
        TypeValue.時位變速點,
        TypeValue.時位開始速度,
        TypeValue.時位結束速度,
        TypeValue.回收距離,
        TypeValue.出生動畫速率,
        TypeValue.出生動畫開始,
    };
    public static TypeValue[] NoValType = new TypeValue[]{
        TypeValue.是界內,
        TypeValue.啟動依玩家死亡,
        TypeValue.啟動位置是活動,
    };

    public static AngleParamType[] NoValAngleType = new AngleParamType[]{
        AngleParamType.新參照位置12,
        AngleParamType.新參照角度,
    };

    public static PosParamType[] NoValPosType = new PosParamType[]{
        PosParamType.新參照位置,
        PosParamType.位置參數角距角度,
    };

    public enum IdVal
    {
        None = 0,
        Self = 1,
        Parent = 2,
        Player = 3,
        Boss = 4,
        Center = 5,
        BossHome = 6,
        XCenter = 7,
        YCenter = 8,
    }

    public class XmlStageSetting
    {
        public string table = "";
        public uint Order = 0;
        public TypeValue Type = TypeValue.None;
        public PosParamType PosParamTypeVal = PosParamType.None;
        public AngleParamType AngleParamTypeVal = AngleParamType.None;


        public string[] ParamVals = new string[0];
    }
    public class ConfigParam
    {
        public string key = null;
        public string PreLayerNo = null;
        public Vector2? pos = null;
        public float? floatVal = null;
        public int? intVal = null;
        public string text = null;
    }

    public class CreateStageSetting
    {
        public uint Id
        {
            get
            {
                return coreSetting.Id;
            }
        }
        public TypeValue type
        {
            get
            {
                return coreSetting.type;
            }
        }

        public SettingBase coreSetting = new SettingBase();
        public Dictionary<uint, SettingBase> actionSettingsDict = new Dictionary<uint, SettingBase>();

        public string Print()
        {
            string str = $" [c] [CreateStageSetting Id= {Id}]{Environment.NewLine}";

            if (type != TypeValue.None)
                str += $" [c] Type= {type}{Environment.NewLine}";

            str += $" [c] _coreStageSetting{Environment.NewLine}";
            str += coreSetting.Print();
            str += $" [c]{Environment.NewLine}";

            foreach (var item in actionSettingsDict)
            {
                str += $" [c] _newActionSetting{Environment.NewLine}";
                str += item.Value.Print();
            }

            return str;
        }

    }






    public class Pos
    {
        public Vector2? point = null;
        public uint? Id = null;
        public List<AngleSet> ADangle = null;
        public float? ADdistance = null;
        public string Print()
        {
            string str = "";

            if (Id != null)
                str += $"   [p] Id= {Id}{Environment.NewLine}";

            if (point != null)
                str += $"   [p] point= {point}{Environment.NewLine}";

            if (ADangle != null)
            {
                foreach (var item in ADangle)
                {
                    str += $"   [p] ADangle=({item.Print()}){Environment.NewLine}";
                }
            }

            if (ADdistance != null)
                str += $"   [p] ADdistance= {ADdistance}{Environment.NewLine}";

            return str;
        }
    }

    public class AngleSet
    {
        public float? angle = null;
        public uint[] Ids = null;
        public uint? recordId = null;
        public uint? IdRotateZ = null;
        public uint[] IdMoveAngle = null;
        public Pos pos1 = null;
        public Pos pos2 = null;
        public string Print()
        {
            string str = "";

            if (Ids != null)
            {
                if (Ids.Length < 2)
                    str += $"   [a] Ids[0]= {Ids[0]}{Environment.NewLine}";
                else if (Ids.Length == 2)
                    str += $"   [a] Ids[0]= {Ids[0]} , Ids[1]= {Ids[1]}{Environment.NewLine}";
                else
                    str += $"   [a] Ids = error{Environment.NewLine}";
            }

            if (angle != null)
                str += $"   [a] angle= {angle}{Environment.NewLine}";

            if (pos1 != null)
                str += $"   [a] Pos1=({pos1.Print()}){Environment.NewLine}";

            if (pos2 != null)
                str += $"   [a] Pos2=({pos2.Print()}){Environment.NewLine}";

            return str;
        }
    }

    public class DialogSetting
    {
        public uint? Id = null;
        public uint? mainId = null;
        public string text = null;
        public string leftAni = null;
        public string rightAni = null;
        public string bgm = null;
    }

    
    public class PracticeSetting
    {
        public uint Id = 0;
        public string name = "";
        public uint bossEnterTime = 0;
        public uint bossSpellTime = 0;
        public string music = "";
    }
}
