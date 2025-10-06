using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static PlayerSaveData;
using System.Linq;
using System.IO;

public partial class LoadCtrl : SingletonBase<LoadCtrl>
{
    public List<VersionData> versionDatas = new List<VersionData>();
    public VersionData selectVersionData;
    public string unitLogPath { get; set; }


    public ObjectPoolCtrl pool;
    public AudioSource audioSource;

    public Camera gameCamera;

    public Camera titleCamera;


    public Animator animator;



    public GameSceneState gameState = GameSceneState.Stop;
    public List<ISelectBaseUpdater> selectList = new List<ISelectBaseUpdater>();

    void Start()
    {
        Debug.Log(nameof(Start));
        unitLogPath = Application.persistentDataPath + "/unitLog/";
        CreateUnitLogDirectory();
        ClearUnitLogDirectoryFile();
        ScreenSetup();
        FrameSetup();
        SwitchTitleCamera();

        //ConfigParam
        GameConfig.SetConfigParamByXml();
        GameSelect.DefaultSelectDifficultStageByConfigParam();

        //keyBoardSave
        keyBoardSaveDatas = PlayerSaveData.LoadKeyBoardSaveDatas();
        KeyBoardSelect.Instance.UseKeyBoardSaveDatas();

        //configSave
        configSaveDatas = PlayerSaveData.LoadConfigSaveData();
        OptionSelect.Instance.UseConfigSaveDatas();
        ChangeVolumeByConfigSave();
        ChangleScreenByConfigSave();

        //replaySaveDatas
        replaySaveDatas = PlayerSaveData.LoadReplaySaveDatas();
        ReplaySelect.Instance.UseReplaySaveDatas();

        //selectVersionData
        selectVersionData = new VersionData(GameConfig.VERSION, VersionGetType.InsideCreateByXml);
        InsideOverwriteVersionData();
        PracticeSelect.Instance.UseVersionData();

        StartCoroutine(CoroutineRun());
    }

    IEnumerator CoroutineRun()
    {
        Debug.Log(nameof(CoroutineRun));
        yield return StartCoroutine(TryPlayLoadingShowAni());
        yield return StartCoroutine(pool.Init());
        yield return SwitchTitlePageCoroutine();
    }

    void Update()
    {
        foreach (var Select in selectList)
        {
            Select.UpdateHandler();
        }

        GameMainCtrl.Instance.UpdateHandler();
    }



    void CreateUnitLogDirectory()
    {
        Debug.Log(nameof(CreateUnitLogDirectory));
        try
        {
            Directory.CreateDirectory(unitLogPath);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to create directory at {unitLogPath}: {ex.Message}");
        }
    }

    void ClearUnitLogDirectoryFile()
    {
        Debug.Log(nameof(ClearUnitLogDirectoryFile));
        DirectoryInfo direction = new DirectoryInfo(unitLogPath);
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            string FilePath = unitLogPath + "/" + files[i].Name;
            File.Delete(FilePath);
        }
    }

    void ScreenSetup()
    {
        Debug.Log(nameof(ScreenSetup));
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true);
        QualitySettings.vSyncCount = 0;
    }

    void FrameSetup()
    {
        Debug.Log(nameof(FrameSetup));
        Time.captureFramerate = 0;
        Application.targetFrameRate = GameConfig.TARGET_FRAME_RATE;
    }



    public void SwitchTitleCamera()
    {
        Debug.Log(nameof(SwitchTitleCamera));
        titleCamera.enabled = true;
        gameCamera.enabled = false;
    }

    public void SwitchGameCamera()
    {
        Debug.Log(nameof(SwitchGameCamera));
        gameCamera.enabled = true;
        titleCamera.enabled = false;
    }



    public void SwitchPage(PageIndex Index)
    {
        Debug.Log(nameof(SwitchPage));
        audioSource.Stop();
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
        Debug.Log(nameof(SwitchGamePageCoroutine));
        GameMainCtrl.Instance.UnPause();
        animator.Play("Show");
        yield return StartCoroutine(WaitLoadingAnimCoroutine());
        SwitchGameCamera();
        GameSelect.InitPlayerAndGameCtrlDatas();
        GameMainCtrl.Instance.Init();
        yield return new WaitForSecondsRealtime(1f);
        animator.Play("Hide");
        yield return StartCoroutine(WaitLoadingAnimCoroutine());
        gameState = GameSceneState.Run;
    }

    IEnumerator SwitchTitlePageCoroutine()
    {
        Debug.Log(nameof(SwitchTitlePageCoroutine));
        GameMainCtrl.Instance.UnPause();
        yield return StartCoroutine(TryPlayLoadingShowAni());
        SwitchTitleCamera();
        TryLeaveAndClearGameScene();
        yield return StartCoroutine(PlayLoadingHideAni());
        TitleSelect.Instance.Show();
        LoadCtrl.Instance.pool.PlayBgm("Title");
    }

    void TryLeaveAndClearGameScene()
    {
        Debug.Log(nameof(TryLeaveAndClearGameScene));
        if (gameState == GameSceneState.Run)
        {
            gameState = GameSceneState.Stop;
            GameDebut.ClearAll();
        }
    }

    IEnumerator TryPlayLoadingShowAni()
    {
        Debug.Log(nameof(TryPlayLoadingShowAni));
        var anim = animator.GetCurrentAnimatorStateInfo(0);
        if (!anim.IsName("Show"))
        {
            animator.Play("Show");
            yield return StartCoroutine(WaitLoadingAnimCoroutine());
        }
        yield return new WaitForSecondsRealtime(1f);

    }

    IEnumerator PlayLoadingHideAni()
    {
        Debug.Log(nameof(PlayLoadingHideAni));
        yield return new WaitForSecondsRealtime(1f);
        animator.Play("Hide");
        yield return StartCoroutine(WaitLoadingAnimCoroutine());
    }



    IEnumerator WaitLoadingAnimCoroutine()
    {
        Debug.Log(nameof(WaitLoadingAnimCoroutine));
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

    public void ChangeVolumeByConfigSave()
    {
        Debug.Log(nameof(ChangeVolumeByConfigSave));
        audioSource.volume = PlayerSaveData.configSaveDatas.BGMVolume / 100f;

    }

    void ChangleScreenByConfigSave()
    {
        Debug.Log(nameof(ChangleScreenByConfigSave));
        if (PlayerSaveData.configSaveDatas.screenModeType == ScreenMode.FullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (PlayerSaveData.configSaveDatas.screenModeType == ScreenMode.Windowed)
        {
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
        }
    }
    void InsideOverwriteVersionData()
    {
        Debug.Log(nameof(InsideOverwriteVersionData));
        int index = versionDatas.FindIndex(r => r.version == selectVersionData.version);
        if (index != -1)
        {
            versionDatas[index] = selectVersionData;
        }
        else
        {
            versionDatas.Add(selectVersionData);
        }
    }






    //POOL分出去建議
















}
