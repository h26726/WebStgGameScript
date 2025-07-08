using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CommonData;
using static CommonFunc;
using static PlayerKeyCtrl;

public static class GameConfig
{

    public const bool IS_OPEN_DIALOG = true;
    public const bool IS_OPEN_SPELLIMG = true;

    public const int TARGET_FRAME_RATE = 60;
    public const float Z_INDEX_TOP = 500f;
    public const float Z_INDEX_REDUCE = 0.001f;
    public readonly static Vector2 POOL_RESET_POS = new Vector2(0, 9999);

    //寬長 9 9
    public const float BORDER_LEFT = -8f;
    public const float BORDER_RIGHT = 1f;
    public const float BD_TOP = 4.5f;
    public const float BD_BOTTOM = -4.5f;
    public static Vector2 CENTER
    {
        get
        {
            return new Vector2((BORDER_LEFT + BORDER_RIGHT) / 2, (BD_TOP + BD_BOTTOM) / 2);
        }
    }
    public const float BORDER_PLAYER_MOVE_BEYOND = -0.55f; //自機移動與邊界間隔
    public const float PLAYER_MOVE_BORDER_TOP = BD_TOP + BORDER_PLAYER_MOVE_BEYOND; 
    public const float PLAYER_MOVE_BORDER_BOTTOM = BD_BOTTOM - BORDER_PLAYER_MOVE_BEYOND;
    public const float PLAYER_MOVE_BORDER_LEFT = BORDER_LEFT - BORDER_PLAYER_MOVE_BEYOND;
    public const float PLAYER_MOVE_BORDER_RIGHT = BORDER_RIGHT + BORDER_PLAYER_MOVE_BEYOND;

    public const float BOSS_HOMING_POS_X = 0f;
    public const float BOSS_HOMING_POS_Y = 3f;

    public static readonly Vector2 BOSS_HOMING_POS = new Vector2(BOSS_HOMING_POS_X, BOSS_HOMING_POS_Y);

    public readonly static Vector2 PLAYER_DEAD_TRACE_POS = new Vector2(0, -4f);
    public const float PLAYER_MOVE_SPEED = 0.06f;
    public const float PLAYER_SLOW_SPEED_RATE = 0.5f;

    public const uint PLAYER_BIRTH_BOOM = 3;
#if UNITY_WEBGL
    public const uint PLAYER_BIRTH_LIFE = 14;

#else
    public const uint PLAYER_BIRTH_LIFE = 2;
#endif
    public const float PLAYER_BIRTH_POWER = 1f;
    public const float PLAYER_MAX_POWER = 3f;
    public const float PLAYER_EVERY_POWER_GET = 0.1f;
    public const uint RECORD_TMP_ID_MIN = 99;

    public const string LUNATIC_STAGE_STR = "LunaticStage";
    public const string HARD_STAGE_STR = "HardStage";
    public const string NORMAL_STAGE_STR = "NormalStage";
    public const string EASY_STAGE_STR = "EasyStage";

    public const float DEFAULT_RESTORE_DIS = 0.2f;
    public const uint DEFAULT_SHOW_ANI_TIME = 10;
    public const float DEFAULT_SHOW_ANI_SPEED = 1f;
    public const float DEFAULT_SHOW_ANI_START = 0f;

    public const uint DEFAULT_ENEMY_HP = 1;
    public const uint DEFAULT_SPELL_TIME = 10;

    public const uint DEFAULT_DEADANI_KEY_TIME = 60;

    public const uint DIALOG_DELAY_KEY_TIME = 100;





    public static List<ConfigParam> CONFIG_PARAMS = new List<ConfigParam>()
    {

    };

    public static List<ConfigParam> LUNATIC_STAGES
    {
        get
        {
            return CONFIG_PARAMS.Where(r => r.key == LUNATIC_STAGE_STR).ToList();
        }
    }
    public static List<ConfigParam> HARD_STAGES
    {
        get
        {
            return CONFIG_PARAMS.Where(r => r.key == HARD_STAGE_STR).ToList();
        }
    }
    public static List<ConfigParam> NORMAL_STAGES
    {
        get
        {
            return CONFIG_PARAMS.Where(r => r.key == NORMAL_STAGE_STR).ToList();
        }
    }
    public static List<ConfigParam> EASY_STAGES
    {
        get
        {
            return CONFIG_PARAMS.Where(r => r.key == EASY_STAGE_STR).ToList();
        }
    }










}
