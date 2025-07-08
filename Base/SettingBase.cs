using UnityEngine;
using static CommonData;
using static CommonFunc;
using static GameConfig;
using static PlayerKeyCtrl;
using static PlayerSaveData;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingBase
{
    public uint Id = 0;
    public float? birthAniSpeed = null;
    public float? birthAniStart = null;
    public uint? birthDurTime = null;

    public List<uint> addIds = new List<uint>();
    public string name = null;
    public bool? isIn = null;
    public TypeValue type = TypeValue.None;
    public string obj = null;
    public string spellAni = null;
    public string ani = null;
    public uint? dmg = null;
    public uint? spellTime = null;
    public uint? hp = null;
    public uint? actTime = null;
    public float? restoreDistance = null;

    public float? speed = null;
    public float? addSpeed = null;
    public float? maxSpeed = null;
    public float? minSpeed = null;
    public float? timPosSpeedPoint = null;
    public float? timPosStartSpeed = null;
    public float? timPosEndSpeed = null;
    public uint? timPosTime = null;
    public List<Pos> timPosPos = null;
    public bool? rotateIsMoveAngle = null;
    public List<Pos> movePos = null;
    public List<AngleSet> moveAngle = null;
    public List<AngleSet> addMoveAngle = null;
    public List<AngleSet> rotateZ = null;
    public List<AngleSet> addRotateZ = null;

    public List<AngleSet> childRotateZ = null;
    public List<AngleSet> childAddRotateZ = null;

    public bool? isThrough = null;
    public string sprite = null;
    public uint? restoreTime = null;
    public uint? deadTime = null;

    public bool? isInvincible = null;

    public bool? bossDead = null;

    public uint? callGameTime = null;
    public uint? playerGameTime = null;
    public uint? playerShotHzTime = null;

    public uint? playerPowerNeed = null;
    public bool? playerShift = null;
    public uint? playerShotDelayTime = null;
    public uint? powerGive = null;

    public List<Pos> relatPos = new List<Pos>();
    public List<AngleSet> recordAngle = new List<AngleSet>();
    public List<Pos> recordPos = new List<Pos>();
    public uint? recordPosId = null;
    public uint? recordAngleId = null;

    public string Print()
    {
        string str = $"  [s] [StageSetting Id= {Id}] {Environment.NewLine}";

        if (name != null) str += $"  [s] Name= {name}{Environment.NewLine}";
        if (isIn != null) str += $"  [s] isIn= {isIn}{Environment.NewLine}";
        if (type != TypeValue.None) str += $"  [s] Type= {type}{Environment.NewLine}";
        if (obj != null) str += $"  [s] Obj= {obj}{Environment.NewLine}";
        if (ani != null) str += $"  [s] Ani= {ani}{Environment.NewLine}";
        if (dmg != null) str += $"  [s] Dmg= {dmg}{Environment.NewLine}";
        if (hp != null) str += $"  [s] Hp= {hp}{Environment.NewLine}";
        if (actTime != null) str += $"  [s] ActTime= {actTime}{Environment.NewLine}";
        if (restoreDistance != null) str += $"  [s] RestoreDistance= {restoreDistance}{Environment.NewLine}";
        if (speed != null) str += $"  [s] Speed= {speed}{Environment.NewLine}";
        if (addSpeed != null) str += $"  [s] AddSpeed= {addSpeed}{Environment.NewLine}";
        if (maxSpeed != null) str += $"  [s] MaxSpeed= {maxSpeed}{Environment.NewLine}";
        if (minSpeed != null) str += $"  [s] MinSpeed= {minSpeed}{Environment.NewLine}";
        if (timPosSpeedPoint != null) str += $"  [s] timPosSpeedPoint= {timPosSpeedPoint}{Environment.NewLine}";
        if (timPosStartSpeed != null) str += $"  [s] timPosStartSpeed= {timPosStartSpeed}{Environment.NewLine}";
        if (timPosEndSpeed != null) str += $"  [s] timPosEndSpeed= {timPosEndSpeed}{Environment.NewLine}";
        if (rotateIsMoveAngle != null) str += $"  [s] RotateIsMoveAngle= {rotateIsMoveAngle}{Environment.NewLine}";
        if (isThrough != null) str += $"  [s] isThrough= {isThrough}{Environment.NewLine}";
        if (sprite != null) str += $"  [s] Sprite= {sprite}{Environment.NewLine}";
        if (restoreTime != null) str += $"  [s] RestoreTime= {restoreTime}{Environment.NewLine}";
        if (deadTime != null) str += $"  [s] DeadTime= {deadTime}{Environment.NewLine}";
        if (timPosTime != null) str += $"  [s] timPosTime= {timPosTime}{Environment.NewLine}";
        if (isInvincible != null) str += $"  [s] isInvincible= {isInvincible}{Environment.NewLine}";
        if (bossDead != null) str += $"  [s] BossDead= {bossDead}{Environment.NewLine}";
        if (callGameTime != null) str += $"  [s] CallGameTime= {callGameTime}{Environment.NewLine}";
        if (playerGameTime != null) str += $"  [s] PlayerGameTime= {playerGameTime}{Environment.NewLine}";
        if (playerShotHzTime != null) str += $"  [s] PlayerShotHzTime= {playerShotHzTime}{Environment.NewLine}";

        if (movePos != null)
        {
            str += $"  [s] MovePos {Environment.NewLine}";
            foreach (var item in movePos)
            {
                str += $"{item.Print()}{Environment.NewLine}";
            }
        }

        if (moveAngle != null)
        {
            str += $"  [s] MoveAngle {Environment.NewLine}";
            foreach (var item in moveAngle)
            {
                str += $"{item.Print()}{Environment.NewLine}";
            }
        }

        if (addMoveAngle != null)
        {
            str += $"  [s] AddMoveAngle {Environment.NewLine}";
            foreach (var item in addMoveAngle)
            {
                str += $"{item.Print()}{Environment.NewLine}";
            }
        }

        if (rotateZ != null)
        {
            str += $"  [s] MoveAngle {Environment.NewLine}";
            foreach (var item in rotateZ)
            {
                str += $"{item.Print()}{Environment.NewLine}";
            }
        }

        if (addRotateZ != null)
        {
            str += $"  [s] AddRotateZ {Environment.NewLine}";
            foreach (var item in addRotateZ)
            {
                str += $"{item.Print()}{Environment.NewLine}";
            }
        }

        if (relatPos != null)
        {
            str += $"  [s] RelatPos {Environment.NewLine}";
            foreach (var item in relatPos)
            {
                str += $"{item.Print()}{Environment.NewLine}";
            }
        }

        if (timPosPos != null)
        {
            str += $"  [s] timPosPos {Environment.NewLine}";
            foreach (var item in timPosPos)
            {
                str += $"{item.Print()}{Environment.NewLine}";
            }
        }

        return str;
    }

}

