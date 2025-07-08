using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using static CommonData;
using static CommonFunc;
using static PlayerKeyCtrl;
using static PlayerSaveData;
using System.Linq;
using System.IO;

public class LoadingCtrl : SingletonBase<LoadingCtrl>
{
    public enum PageIndex
    {
        Title = 3,
        Game = 4,
        Init = 0,
        Exhibit = 1,
    }
    public ObjectPoolCtrl pool;

    public GameSystem gameSystem;

    public PracticeSelect practiceSelect;
    public OptionSelect optionSelect;
    public KeyBoardSelect keyBoardSelect;
    public AudioSource audioSource;

    public Camera gameCamera;

    public Camera titleCamera;


    public Animator animator;


    public enum GameSceneState
    {
        Sleep,
        Run,
        Stop,
    };
    public GameSceneState gameState = GameSceneState.Sleep;

    void Awake()
    {
        Time.captureFramerate = 0;
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = GameConfig.TARGET_FRAME_RATE;
    }

    void Start()
    {
        titleCamera.enabled = true;
        gameCamera.enabled = false;
        StartCoroutine(FirstLoadCoroutine());
    }

    IEnumerator FirstLoadCoroutine()
    {
        GameConfig.CONFIG_PARAMS.AddRange(ReadConfigXML("config"));
        dialogSettings = ReadDialogXML("dialog1");
        practiceSettings = ReadPracticeXML("practice");
        PracticeSettingHandle();

        if (GameConfig.CONFIG_PARAMS.First(r => r.key == "SelectDifficult") != null)
            GameSystem.Instance.selectDifficult = (Difficult)GameConfig.CONFIG_PARAMS.First(r => r.key == "SelectDifficult").intVal;

        keyBoardSaveData = LoadKeyBoardData();
        KeyBoardSelect.Instance.LoadData();
        configSaveDatas = PlayerSaveData.LoadConfigSaveData();
        ConfigSaveHandle();

        replaySaveDatas = PlayerSaveData.LoadReplayData();
        ReplaySelect.Instance.LoadPage();
        animator.Play("Show");
        yield return StartCoroutine(WaitLoadingAnimCoroutine());
        yield return StartCoroutine(pool.Init());
        yield return new WaitForSecondsRealtime(1f);
        yield return SwitchTitlePageCoroutine();
    }


    public void SwitchPage(PageIndex Index)
    {
        audioSource.Stop();
        if (gameState == GameSceneState.Run)
        {
            gameState = GameSceneState.Stop;
            GameSystem.Instance.CloseAll();
        }

        if (Index == PageIndex.Game)
        {
            StartCoroutine(SwitchGamePageCoroutine());
        }
        else
        {
            StartCoroutine(SwitchTitlePageCoroutine());
        }


    }

    IEnumerator SwitchGamePageCoroutine()
    {

        GameSystem.Instance.UnPause();
        animator.Play("Show");
        yield return StartCoroutine(WaitLoadingAnimCoroutine());
        titleCamera.enabled = false;
        gameCamera.enabled = true;
        if (gameState == GameSceneState.Stop)
        {
            GameSystem.Instance.ClearAll();
        }
        yield return StartCoroutine(GameSystem.Instance.GameStartCoroutine());
        yield return new WaitForSecondsRealtime(1f);
        animator.Play("Hide");
        yield return StartCoroutine(WaitLoadingAnimCoroutine());
        gameState = GameSceneState.Run;
    }

    IEnumerator SwitchTitlePageCoroutine()
    {
        GameSystem.Instance.UnPause();
        var anim = animator.GetCurrentAnimatorStateInfo(0);
        if (!anim.IsName("Show"))
        {
            animator.Play("Show");
            yield return StartCoroutine(WaitLoadingAnimCoroutine());
        }
        titleCamera.enabled = true;
        gameCamera.enabled = false;
        if (gameState == GameSceneState.Stop)
        {
            GameSystem.Instance.ClearAll();
        }
        yield return new WaitForSecondsRealtime(1f);
        animator.Play("Hide");
        yield return StartCoroutine(WaitLoadingAnimCoroutine());
        gameState = GameSceneState.Sleep;
        TitleSelect.Instance.Show();
        LoadingCtrl.Instance.pool.PlayBgm("Title");
    }



    IEnumerator WaitLoadingAnimCoroutine()
    {
        AnimatorStateInfo anim;

        do
        {
            anim = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }
        while ((!anim.IsName("Show") && !anim.IsName("Hide")) || animator.IsInTransition(0));

        while (true)
        {

            anim = animator.GetCurrentAnimatorStateInfo(0);
            if (anim.normalizedTime >= 1)
                break;
            yield return null;
        }
    }



    public Dictionary<uint, PracticeSetting> practiceSettings = new Dictionary<uint, PracticeSetting>();

    public void PracticeSettingHandle()
    {
        PracticeSelect.Instance.LoadData();
    }


    public List<DialogSetting> dialogSettings;

    public void ConfigSaveHandle()
    {

        audioSource.volume = PlayerSaveData.configSaveDatas.BGMVolume / 100f;
        if (PlayerSaveData.configSaveDatas.screenModeType == ScreenMode.FullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (PlayerSaveData.configSaveDatas.screenModeType == ScreenMode.Windowed)
        {
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
        }
        OptionSelect.Instance.LoadData();

    }
}
