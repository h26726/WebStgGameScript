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

public class UpdateManager : SingletonBase<UpdateManager>
{
    public List<ISelectBaseUpdater> selectList = new List<ISelectBaseUpdater>();
    
    void Update()
    {
        foreach (var Select in selectList)
        {
            Select.UpdateHandle();
        }

        GameSystem.Instance.GameSystemUpdate();
    }

    
}
