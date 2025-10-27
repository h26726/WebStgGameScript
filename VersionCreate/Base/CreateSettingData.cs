using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static EnumData;
using static CommonHelper;
using static PlayerKeyHelper;

public static class CreateSettingData
{


    [Serializable]
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
        public List<SettingBase> actionSettingList = new List<SettingBase>();

        public bool IsOperateExistBossByType()
        {
            return type == TypeValue.符卡 || type == TypeValue.復位 || type == TypeValue.BOSSLEAVE;
        }

        public string Print()
        {
            string str = $" [c] [CreateStageSetting Id= {Id}]{Environment.NewLine}";

            if (type != TypeValue.None)
                str += $" [c] Type= {type}{Environment.NewLine}";

            str += $" [c] _coreStageSetting{Environment.NewLine}";
            str += coreSetting.Print();
            str += $" [c]{Environment.NewLine}";

            foreach (var item in actionSettingList)
            {
                str += $" [c] _newActionSetting{Environment.NewLine}";
                str += item.Print();
            }

            return str;
        }

        public void TriggerDebut(UnitCtrlBase parentUnitCtrl = null)
        {
            // Debug.Log(nameof(TriggerDebut));
            GameDebut.AddQueueDebut(this, parentUnitCtrl);
        }
    }

    [Serializable]
    public class Pos : IPrintable
    {
        public Vector2 point = GameConfig.VECTOR2_INVAILD;
        public uint Id = GameConfig.UINT_INVAILD;
        public List<AngleSet> ADangle = null;
        public float ADdistance = GameConfig.FLOAT_INVAILD;
        public string Print()
        {
            string str = "";

            if (!InvalidHelper.IsInvalid(Id))
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

            if (!InvalidHelper.IsInvalid(ADdistance))
                str += $"   [p] ADdistance= {ADdistance}{Environment.NewLine}";

            return str;
        }
    }


    [Serializable]
    public class AngleSet : IPrintable
    {
        public float angle = GameConfig.FLOAT_INVAILD;
        public uint[] Ids = null;
        public uint recordId = GameConfig.UINT_INVAILD;
        public uint IdRotateZ = GameConfig.UINT_INVAILD;
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

            if (!InvalidHelper.IsInvalid(angle))
                str += $"   [a] angle= {angle}{Environment.NewLine}";

            if (pos1 != null)
                str += $"   [a] Pos1=({pos1.Print()}){Environment.NewLine}";

            if (pos2 != null)
                str += $"   [a] Pos2=({pos2.Print()}){Environment.NewLine}";

            return str;
        }
    }

    [Serializable]
    public class DialogSetting
    {
        public uint Id = GameConfig.UINT_INVAILD;
        public uint mainId = GameConfig.UINT_INVAILD;
        public string text = GameConfig.STRING_INVAILD;
        public string leftAni = GameConfig.STRING_INVAILD;
        public string rightAni = GameConfig.STRING_INVAILD;
        public string bgm = GameConfig.STRING_INVAILD;
    }

    [Serializable]
    public class PracticeSetting
    {
        public uint Id = 0;
        public string name = "";
        public uint bossEnterTime = 0;
        public uint bossSpellTime = 0;
        public string music = "";
    }
}
