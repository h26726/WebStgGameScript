using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using System.Xml;

public static class GameConfig
{

    public const string VERSION = "v1.0";
    public const bool IS_OPEN_DIALOG = true;
    public const bool IS_OPEN_SPELLIMG = true;

    public const int TARGET_FRAME_RATE = 60;
    public const float Z_INDEX_TOP = 500f;
    public const float Z_INDEX_REDUCE = 0.001f;
    public readonly static Vector2 POOL_RESET_POS = new Vector2(0, 9999);
    public static readonly Vector2 DEFAULT_PLAYER_BIRTH_POS = new Vector2(0, 1f);

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

    public const string CONFIG_FILE_STR_CONFIG = "config";
    public const string CONFIG_FILE_STR_POWER = "power";
    public const string CONFIG_FILE_STR_DIALOG = "dialog1";
    public const string CONFIG_FILE_STR_PRACTICE = "practice";
    public const string CONFIG_SELECT_DIFFICULT_STR = "SelectDifficult";
    public const string CONFIG_SELECT_STAGE_KEY_STR = "SelectStageKey";


    public const string CONFIG_FILE_DIR_STR_PLAYER_LIST = "Player";
    public const string CONFIG_FILE_DIR_STR_LUNATIC_STAGE = "LunaticStage";
    public const string CONFIG_FILE_DIR_STR_HARD_STAGE = "HardStage";
    public const string CONFIG_FILE_DIR_STR_NORMAL_STAGE = "NormalStage";
    public const string CONFIG_FILE_DIR_STR_EASY_STAGE = "EasyStage";

    public const float DEFAULT_RESTORE_DIS = 0.2f;
    public const uint DEFAULT_SHOW_ANI_TIME = 10;
    public const float DEFAULT_SHOW_ANI_SPEED = 1f;
    public const float DEFAULT_SHOW_ANI_START = 0f;

    public const uint DEFAULT_ENEMY_HP = 1;
    public const uint DEFAULT_SPELL_TIME = 10;

    public const uint DEFAULT_DEADANI_KEY_TIME = 60;
    public const uint DEFAULT_PLAYER_DEADANI_KEY_TIME = 60;

    public const uint DIALOG_DELAY_KEY_TIME = 100;
    public const uint PRACTICE_DEAD_DELAY_KEY_TIME = 240;
    public const uint PAUSE_DELAY_TIME = 10;
    public const float RESTORE_DISTANCE_MAX = 100f;
    public const int SELECT_AUTO_CLOSE_TIME = 60;






    public static List<ConfigParam> CONFIG_PARAMS = new List<ConfigParam>()
    {

    };

    public static List<ConfigParam> PLAYER_LIST
    {
        get
        {
            return CONFIG_PARAMS.Where(r => r.key == CONFIG_FILE_DIR_STR_PLAYER_LIST).ToList();
        }
    }


    public static List<ConfigParam> LUNATIC_STAGES
    {
        get
        {
            return CONFIG_PARAMS.Where(r => r.key == CONFIG_FILE_DIR_STR_LUNATIC_STAGE).ToList();
        }
    }
    public static List<ConfigParam> HARD_STAGES
    {
        get
        {
            return CONFIG_PARAMS.Where(r => r.key == CONFIG_FILE_DIR_STR_HARD_STAGE).ToList();
        }
    }
    public static List<ConfigParam> NORMAL_STAGES
    {
        get
        {
            return CONFIG_PARAMS.Where(r => r.key == CONFIG_FILE_DIR_STR_NORMAL_STAGE).ToList();
        }
    }
    public static List<ConfigParam> EASY_STAGES
    {
        get
        {
            return CONFIG_PARAMS.Where(r => r.key == CONFIG_FILE_DIR_STR_EASY_STAGE).ToList();
        }
    }


    public static void SetConfigParamByXml()
    {
        TextAsset xmlAsset = Resources.Load<TextAsset>($"Setting/{GameConfig.CONFIG_FILE_STR_CONFIG}");
        
        CONFIG_PARAMS = new List<ConfigParam>();
        if (xmlAsset != null)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlAsset.text);

            XmlNodeList nodeList = xml.SelectSingleNode("root").ChildNodes;
            foreach (XmlElement xe in nodeList)
            {
                var str = xe.GetAttribute("Str");
                var preLayerNo = xe.GetAttribute("PreLayerNo");

                if (!string.IsNullOrEmpty(xe.GetAttribute("Float")))
                {
                    var floatVal = float.Parse(xe.GetAttribute("Float"));
                    CONFIG_PARAMS.Add(new ConfigParam
                    {
                        key = str,
                        PreLayerNo = preLayerNo,
                        floatVal = floatVal,
                    });
                }
                else if (!string.IsNullOrEmpty(xe.GetAttribute("Int")))
                {
                    var intVal = int.Parse(xe.GetAttribute("Int"));
                    CONFIG_PARAMS.Add(new ConfigParam
                    {
                        key = str,
                        PreLayerNo = preLayerNo,
                        intVal = intVal,
                    });
                }
                else if (!string.IsNullOrEmpty(xe.GetAttribute("Pos")))
                {
                    var strs = xe.GetAttribute("Pos").Split(',');
                    var pos = new Vector2(float.Parse(strs[0]), float.Parse(strs[1]));
                    CONFIG_PARAMS.Add(new ConfigParam
                    {
                        key = str,
                        PreLayerNo = preLayerNo,
                        pos = pos,
                    });
                }
                else if (!string.IsNullOrEmpty(xe.GetAttribute("Text")))
                {
                    var text = xe.GetAttribute("Text");
                    CONFIG_PARAMS.Add(new ConfigParam
                    {
                        key = str,
                        PreLayerNo = preLayerNo,
                        text = text,
                    });
                }
            }
        }
        else
        {
            Debug.LogError($"找不到 XML 檔案：Setting/{GameConfig.CONFIG_FILE_STR_CONFIG}");
        }
    }










}
