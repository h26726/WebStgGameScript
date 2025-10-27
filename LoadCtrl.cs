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
using static SaveJsonData;
using System.Linq;
using System.IO;

public partial class LoadCtrl : SingletonBase<LoadCtrl>
{
    public List<VersionData> versionDatas { get; set; } = new List<VersionData>();
    public VersionData selectVersionData { get; set; }
    public GameSceneState gameState { get; set; } = GameSceneState.Stop;
    public string unitLogPath { get; set; }

    public ObjectPoolCtrl pool;
    public AudioSource audioSource;
    public Camera gameCamera;
    public Camera titleCamera;
    public Animator animator;
    [SerializeField] List<GameObject> _selectList;
    public List<ISelectBaseUpdater> selectList = new List<ISelectBaseUpdater>();

    void Start()
    {
        Debug.Log(nameof(Start));
        unitLogPath = Application.persistentDataPath + "/unitLog/";

        foreach (var selectObj in _selectList)
        {
            var select = selectObj.GetComponent<ISelectBaseUpdater>();
            selectList.Add(select);
            select.Init();
        }

        CreateUnitLogDirectory();
        ClearUnitLogDirectoryFile();
        ScreenSetup();
        FrameSetup();
        SwitchTitleCamera();

        //ConfigParam
        GameConfig.SetConfigParamByXml();
        GameSelect.DefaultSelectDifficultStageByConfigParam();

        //keyBoardSave
        keyBoardSaveDatas = SaveJsonData.LoadKeyBoardSaveDatas();
        KeyBoardSelect.Instance.UseKeyBoardSaveDatas();

        //configSave
        configSaveDatas = SaveJsonData.LoadConfigSaveData();
        OptionSelect.Instance.UseConfigSaveDatas();
        ChangeVolumeByConfigSave();
        ChangleScreenByConfigSave();

        //replaySaveDatas
        replaySaveDatas = SaveJsonData.LoadReplaySaveDatas();
        ReplaySelect.Instance.UseReplaySaveDatas();

        //selectVersionData
        versionDatas = new List<VersionData>();
        foreach (var version in GameConfig.VERSIONS)
        {
            var versionData = new VersionData(version);
            versionDatas.Add(versionData);
        }
        selectVersionData = versionDatas.First(r => r.version == GameConfig.VERSION);
        PracticeSelect.Instance.UseVersionData();

        GameDebut.Init();
        GameMainCtrl.Instance.Reset();
        GameObjCtrl.Instance.Reset();
        GameProgressStageCtrl.Reset();
        GameReplay.Init();
        GameSelect.Init();
        GameBoss.Reset();
        GamePlayer.Reset();
        StartCoroutine(CoroutineRun());
    }

    IEnumerator CoroutineRun()
    {
        Debug.Log(nameof(CoroutineRun));
        yield return StartCoroutine(TryPlayLoadingShowAni());
        yield return StartCoroutine(pool.Init());
        yield return SwitchTitlePageCoroutine();
    }

    void ClearGameScene()
    {
        Debug.Log(nameof(ClearGameScene));
        if (gameState == GameSceneState.Run)
        {
            gameState = GameSceneState.Stop;
            GameDebut.ClearAll();
            GameDebut.Reset();
            GameMainCtrl.Instance.Reset();
            GameObjCtrl.Instance.Reset();
            GameProgressStageCtrl.Reset();
            GameReplay.ResetInput();
            GameBoss.Reset();
            GamePlayer.Reset();


            if (DialogCtrl.nowInstance != null)
            {
                DialogCtrl.nowInstance.Reset();
                DialogCtrl.nowInstance.Close();
            }
            ObjectPoolCtrl.Instance.LogNum();
        }
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
        if (gameState == GameSceneState.Run)
        {
            ClearGameScene();
        }

        if (Index == PageIndex.Game)
        {
            StartCoroutine(SwitchGamePageCoroutine());
        }
        else
        {
            ResetSelectContent();
            StartCoroutine(SwitchTitlePageCoroutine());
        }
    }

    void ResetSelectContent()
    {
        GameSelect.Reset();
        GameReplay.ResetRead();
    }

    IEnumerator SwitchGamePageCoroutine()
    {
        Debug.Log(nameof(SwitchGamePageCoroutine));
        GameMainCtrl.Instance.UnPause();
        animator.Play("Show");
        yield return StartCoroutine(WaitLoadingAnimCoroutine());
        SwitchGameCamera();
        GameSelect.InitPlayerAndGameCtrlDatas();
        GameMainCtrl.Instance.GameStartSet();
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
        System.GC.Collect();
        SwitchTitleCamera();
        yield return StartCoroutine(PlayLoadingHideAni());
        TitleSelect.Instance.Show();
        LoadCtrl.Instance.pool.PlayBgm("Title");
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
        audioSource.volume = SaveJsonData.configSaveDatas.BGMVolume / 100f;

    }

    void ChangleScreenByConfigSave()
    {
        Debug.Log(nameof(ChangleScreenByConfigSave));
        if (SaveJsonData.configSaveDatas.screenModeType == ScreenMode.FullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (SaveJsonData.configSaveDatas.screenModeType == ScreenMode.Windowed)
        {
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
        }
    }
    






    //POOL分出去建議
















}
