using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;
using System.Xml;
using System.IO;
using static CommonData;
using static CommonFunc;
using static PlayerKeyCtrl;
using static PlayerSaveData;
using static GameConfig;
using System.Linq;
using static LoadingCtrl;

public partial class GameSystem : SingletonBase<GameSystem>
{
    public Vector2 playerBirthPos { get; set; } = new Vector2(0, 0);
    public GameObject nowSpellBg { get; set; }
    public string sysLogPath { get; set; }
    public string unitLogPath { get; set; }
    public string stageLogPath { get; set; }

    public float zIndex { get; set; } = GameConfig.Z_INDEX_TOP;
    public bool isPause { get; set; } = false;
    public bool isFirst { get; set; } = true;


    public string stageXml { get; set; } = "";
    public Difficult selectDifficult { get; set; } = Difficult.Normal;
    public uint stageKey { get; set; } = 6;

    public uint selectPracticeId { get; set; } = 0;
    public bool isPractice
    {
        get
        {
            return selectPracticeId > 0 && LoadingCtrl.Instance.practiceSettings.ContainsKey(selectPracticeId);
        }
    }
    public PracticeSetting practiceSetting
    {
        get
        {
            if (isPractice)
            {
                return LoadingCtrl.Instance.practiceSettings[selectPracticeId];
            }
            return null;
        }
    }
    public string selectPlayer { get; set; } = "player1";
    float _playePower { get; set; } = 0f;
    uint _playerBoom { get; set; } = 0;
    uint _playerLife { get; set; } = 0;

    public float playePower
    {
        get { return _playePower; }
        set
        {
            if (value > GameConfig.PLAYER_MAX_POWER)
            {
                value = GameConfig.PLAYER_MAX_POWER;
            }
            _playePower = value;
            if (_playePower == GameConfig.PLAYER_MAX_POWER)
            {
                Instance.powerText.text = "MAX";
            }
            else
            {
                Instance.powerText.text = value.ToString();

            }
        }
    }
    public uint playerBoom
    {
        get { return _playerBoom; }
        set
        {
            _playerBoom = value;
            UpdatePlayerBoom();
        }
    }
    public uint playerLife
    {
        get { return _playerLife; }
        set
        {
            _playerLife = value;
            UpdatePlayerLife();
        }
    }
    public bool isContinue { get; set; } = false;
    public bool isSave { get; set; } = false;


    public Text scoreText;
    public Text powerText;
    public Text gameLog;
    public RectTransform playerLifeRect;
    public RectTransform playerBoomRect;
    public Animator spellCardNameAnimator;
    public Text spellCardNameText;
    public GameObject dialogBox;
    public Text dialogBoxText;
    public Animator dialogBoxAnimator;
    public RectTransform bossHpLine;
    public Text spellTime;



    public List<XmlStageSetting> player1XmlStageSettings = new List<XmlStageSetting>();
    public List<XmlStageSetting> powerXmlStageSettings = new List<XmlStageSetting>();
    public List<XmlStageSetting> xmlStageSettings = new List<XmlStageSetting>();

    // 載入過需保存的資料
    public CreateStageSetting playerCreateStageSetting = null;
    public List<CreateStageSetting> playerShotCreateStageSettings = new List<CreateStageSetting>();
    public List<CreateStageSetting> powerCreateStageSettings = new List<CreateStageSetting>();
    // 整合載入過需保存的資料
    public List<GameDataClass> gameDataClasses = new List<GameDataClass>();
    //依遊戲時間創造CallRuleScheme資料
    public List<CallRuleScheme> gTimeCreateCallRuleSchemes = new List<CallRuleScheme>();

    //依單位ID行動時創造或激活行動資料 
    public Dictionary<uint, List<CallRuleScheme>> callRuleSchemeById = new Dictionary<uint, List<CallRuleScheme>>();

    public uint nowGTimeCallRuleSchemeKey = 0;
    public uint gTime = 0;
    public uint keyTime = 0;

    public GameProgressState nowGameProgressState;
    public EnemyBossUnitCtrl nowEnemyBoss;
    public PlayerUnitCtrl playerUnitCtrl { get; set; }

    public uint takeDictKeyNo = 0;
    public Dictionary<uint, UnitCtrlBase> takeDict = new Dictionary<uint, UnitCtrlBase>();

    public List<ReplayKeyClass> replayKeyClasses = new List<ReplayKeyClass>();
    public ReplayDataClass replaySaveData = new ReplayDataClass();

    public List<ReplayKeyClass> playReplayKeys = new List<ReplayKeyClass>();
    public uint playReplayMaxKeyTime;
    public bool isReplay = false;

    public event Action waitRestores;
    public event Action waitDeads;
    public event Action waitCreates;
    public event Action waitCallActs;
    public event Action waitActStops;
    public event Action waitDeadAnis;
    public event Action waitPlayerKey;


    public void SetPlayerItem(bool NotSetPower = false)
    {

        playePower = NotSetPower ? playePower : GameConfig.PLAYER_BIRTH_POWER;
        playerBoom = isPractice ? 0 : GameConfig.PLAYER_BIRTH_BOOM;
        playerLife = isPractice ? 1 : GameConfig.PLAYER_BIRTH_LIFE;

    }


    void Awake()
    {
        sysLogPath = Application.persistentDataPath + "/sysLog/";
        unitLogPath = Application.persistentDataPath + "/unitLog/";
        stageLogPath = Application.persistentDataPath + "/stageIdLog/";
    }


    void Start()
    {

    }

    public IEnumerator GameStartCoroutine()
    {
        PlayerSaveData.score = 0;
        SetStartUTime();
        SetPlayerItem();
        UpdatePlayerLife();

        if (isFirst)
        {
            isFirst = false;
            GetPlayerPowerXml();
            yield return StartCoroutine(GetXmlStageSettings(player1XmlStageSettings));
            if (playerCreateStageSetting == null)
            {
                TestEnd("playerCreateStageSetting == null");
            }
            if (playerCreateStageSetting.coreSetting.movePos != null &&
                playerCreateStageSetting.coreSetting.movePos.Count > 0 &&
                playerCreateStageSetting.coreSetting.movePos[0].point != null)
            {
                playerBirthPos = playerCreateStageSetting.coreSetting.movePos[0].point.Value;
            }
            else
            {
                Debug.LogError("_playerCreateStageSetting.CoreSetting.MovePos is null or empty");
            }

            yield return StartCoroutine(GetXmlStageSettings(powerXmlStageSettings));
        }
        if (gameDataClasses.Any(r => r.selectDifficult == selectDifficult))
        {
            var gameData = gameDataClasses.FirstOrDefault(r => r.selectDifficult == selectDifficult);
            gTimeCreateCallRuleSchemes = gameData.callParGameTimes;
            callRuleSchemeById = gameData.callParamsById;
        }
        else
        {
            GetStage();
            yield return StartCoroutine(GetXmlStageSettings(xmlStageSettings));
            gameDataClasses.Add(new GameDataClass
            {
                selectDifficult = selectDifficult,
                callParGameTimes = gTimeCreateCallRuleSchemes,
                callParamsById = callRuleSchemeById,
            });
        }

        if (gTimeCreateCallRuleSchemes.Count > 0)
        {
            nowGTimeCallRuleSchemeKey = 0;
            while (gTime >= gTimeCreateCallRuleSchemes[(int)nowGTimeCallRuleSchemeKey].callGameTime)
            {
                Debug.Log($"skip CallGameTime:{gTimeCreateCallRuleSchemes[(int)nowGTimeCallRuleSchemeKey].callGameTime}");
                nowGTimeCallRuleSchemeKey++;
            }
        }
        waitCreates += () =>
        {
            CreateUnit(playerCreateStageSetting);
        };
        Directory.CreateDirectory(unitLogPath);
        // Directory.CreateDirectory(sysLogPath);
        // Directory.CreateDirectory(stageLogPath);
        DeleteFile(unitLogPath);
        // DeleteFile(sysLogPath);
        // DeleteFile(stageLogPath);
    }

    public void GetPlayerPowerXml()
    {
        ReadXMLToSetting(selectPlayer, ref player1XmlStageSettings);
        ReadXMLToSetting("power", ref powerXmlStageSettings);
    }

    public void GetStage()
    {
        Debug.Log($"StageKey:{stageKey}");
        Debug.Log($"SelectDifficult:{selectDifficult}");
        GetStage(stageKey, selectDifficult);
    }
    public void GetStage(uint stageKey, Difficult difficult)
    {
        stageXml = "";
        if (difficult == Difficult.Easy)
        {
            stageXml = GameConfig.EASY_STAGES[(int)stageKey].text;
        }
        else if (difficult == Difficult.Normal)
        {
            stageXml = GameConfig.NORMAL_STAGES[(int)stageKey].text;
        }
        else if (difficult == Difficult.Hard)
        {
            stageXml = GameConfig.HARD_STAGES[(int)stageKey].text;
        }
        else if (difficult == Difficult.Lunatic)
        {
            stageXml = GameConfig.LUNATIC_STAGES[(int)stageKey].text;
        }

        ReadPathXmlToSetting(stageXml, ref xmlStageSettings);
    }



    public void SetStartUTime()
    {
        zIndex = 500f;
        xmlStageSettings = new List<XmlStageSetting>();
        nowGTimeCallRuleSchemeKey = 0;
        gTimeCreateCallRuleSchemes = new List<CallRuleScheme>();
        callRuleSchemeById = new Dictionary<uint, List<CallRuleScheme>>();

        isSave = false;
        nowGameProgressState = GameProgressState.Stage;
        nowEnemyBoss = null;
        takeDictKeyNo = 0;
        takeDict = new Dictionary<uint, UnitCtrlBase>(); ;
        replaySaveData = new ReplayDataClass();
        replayKeyClasses = new List<ReplayKeyClass>();
        replaySaveData.replayKeys = replayKeyClasses;
        replaySaveData.selectDifficult = selectDifficult;
        replaySaveData.selectPracticeId = selectPracticeId;
        keyTime = 0;
        gTime = (uint)GameConfig.CONFIG_PARAMS.First(r => r.key == "GameTime").intVal;
        if (isPractice)
        {
            var PracticeSetting = LoadingCtrl.Instance.practiceSettings[selectPracticeId];
            gTime = PracticeSetting.bossEnterTime - 100;
            LoadingCtrl.Instance.pool.PlayBgm(PracticeSetting.music);
        }
        else if (gTime > 0)
        {
            var bgm = GameConfig.CONFIG_PARAMS.First(r => r.key == "BGM").text;
            var bgmStart = GameConfig.CONFIG_PARAMS.First(r => r.key == "BGMStart").intVal;
            LoadingCtrl.Instance.pool.PlayBgm(bgm, () =>
            {
                LoadingCtrl.Instance.audioSource.time = gTime / GameConfig.TARGET_FRAME_RATE;
                if (bgmStart != null)
                    LoadingCtrl.Instance.audioSource.time = bgmStart.Value / GameConfig.TARGET_FRAME_RATE;
            });

        }

    }
}
