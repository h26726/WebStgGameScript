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
using System.Collections.Generic;

[Serializable]
public class SettingBase : IPrintable
{
    public uint Id = GameConfig.UINT_INVAILD;
    public float birthAniSpeed = GameConfig.FLOAT_INVAILD;
    public float birthAniStart = GameConfig.FLOAT_INVAILD;
    public uint birthDurTime = GameConfig.UINT_INVAILD;

    public List<uint> addIds = null;
    public string name = GameConfig.STRING_INVAILD;
    public BoolState isIn = GameConfig.BOOL_INVAILD;
    public TypeValue type = TypeValue.None;
    public string obj = GameConfig.STRING_INVAILD;
    public string spellAni = GameConfig.STRING_INVAILD;
    public string ani = GameConfig.STRING_INVAILD;
    public uint dmg = GameConfig.UINT_INVAILD;
    public uint spellTime = GameConfig.UINT_INVAILD;
    public uint hp = GameConfig.UINT_INVAILD;
    public uint actTime = GameConfig.UINT_INVAILD;
    public float restoreDistance = GameConfig.FLOAT_INVAILD;

    public float speed = GameConfig.FLOAT_INVAILD;
    public float addSpeed = GameConfig.FLOAT_INVAILD;
    public float maxSpeed = GameConfig.FLOAT_INVAILD;
    public float minSpeed = GameConfig.FLOAT_INVAILD;
    public float timPosSpeedPoint = GameConfig.FLOAT_INVAILD;
    public float timPosStartSpeed = GameConfig.FLOAT_INVAILD;
    public float timPosEndSpeed = GameConfig.FLOAT_INVAILD;
    public uint timPosTime = GameConfig.UINT_INVAILD;
    public List<Pos> timPosPos = null;
    public BoolState rotateIsMoveAngle = GameConfig.BOOL_INVAILD;
    public List<Pos> movePos = null;
    public List<AngleSet> moveAngle = null;
    public List<AngleSet> addMoveAngle = null;
    public List<AngleSet> rotateZ = null;
    public List<AngleSet> addRotateZ = null;

    public List<AngleSet> childRotateZ = null;
    public List<AngleSet> childAddRotateZ = null;

    public BoolState isThrough = GameConfig.BOOL_INVAILD;
    public string sprite = GameConfig.STRING_INVAILD;
    public uint restoreTime = GameConfig.UINT_INVAILD;
    public uint deadTime = GameConfig.UINT_INVAILD;

    public BoolState isInvincible = GameConfig.BOOL_INVAILD;
    public uint callGameTime = GameConfig.UINT_INVAILD;
    public uint playerGameTime = GameConfig.UINT_INVAILD;
    public uint playerShotHzTime = GameConfig.UINT_INVAILD;

    public uint playerPowerNeed = GameConfig.UINT_INVAILD;
    public bool playerShift = false;
    public uint playerShotDelayTime = GameConfig.UINT_INVAILD;
    public uint powerGive = GameConfig.UINT_INVAILD;

    public List<Pos> relatPos = null;
    public List<AngleSet> recordAngle = null;
    public List<Pos> recordPos = null;
    public uint recordPosId = GameConfig.UINT_INVAILD;
    public uint recordAngleId = GameConfig.UINT_INVAILD;

    public string Print()
    {
        string str = $"  [s] [StageSetting Id= {Id}] {Environment.NewLine}";

        if (!InvalidHelper.IsInvalid(name)) str += $"  [s] Name= {name}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(isIn)) str += $"  [s] isIn= {isIn}{Environment.NewLine}";
        if (type != TypeValue.None) str += $"  [s] Type= {type}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(obj)) str += $"  [s] Obj= {obj}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(ani)) str += $"  [s] Ani= {ani}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(dmg)) str += $"  [s] Dmg= {dmg}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(hp)) str += $"  [s] Hp= {hp}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(actTime)) str += $"  [s] ActTime= {actTime}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(restoreDistance)) str += $"  [s] RestoreDistance= {restoreDistance}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(speed)) str += $"  [s] Speed= {speed}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(addSpeed)) str += $"  [s] AddSpeed= {addSpeed}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(maxSpeed)) str += $"  [s] MaxSpeed= {maxSpeed}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(minSpeed)) str += $"  [s] MinSpeed= {minSpeed}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(timPosSpeedPoint)) str += $"  [s] timPosSpeedPoint= {timPosSpeedPoint}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(timPosStartSpeed)) str += $"  [s] timPosStartSpeed= {timPosStartSpeed}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(timPosEndSpeed)) str += $"  [s] timPosEndSpeed= {timPosEndSpeed}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(rotateIsMoveAngle)) str += $"  [s] RotateIsMoveAngle= {rotateIsMoveAngle}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(isThrough)) str += $"  [s] isThrough= {isThrough}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(sprite)) str += $"  [s] Sprite= {sprite}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(restoreTime)) str += $"  [s] RestoreTime= {restoreTime}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(deadTime)) str += $"  [s] DeadTime= {deadTime}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(timPosTime)) str += $"  [s] timPosTime= {timPosTime}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(isInvincible)) str += $"  [s] isInvincible= {isInvincible}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(callGameTime)) str += $"  [s] CallGameTime= {callGameTime}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(playerGameTime)) str += $"  [s] PlayerGameTime= {playerGameTime}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(playerShotHzTime)) str += $"  [s] PlayerShotHzTime= {playerShotHzTime}{Environment.NewLine}";

        void PrintList<T>(string label, System.Collections.Generic.List<T> list) where T : IPrintable
        {
            if (list != null)
            {
                str += $"  [s] {label} {Environment.NewLine}";
                foreach (var item in list)
                {
                    str += $"{item.Print()}{Environment.NewLine}";
                }
            }
        }

        PrintList("MovePos", movePos);
        PrintList("MoveAngle", moveAngle);
        PrintList("AddMoveAngle", addMoveAngle);
        PrintList("RotateZ", rotateZ);
        PrintList("AddRotateZ", addRotateZ);
        PrintList("RelatPos", relatPos);
        PrintList("timPosPos", timPosPos);

        return str;
    }

}

