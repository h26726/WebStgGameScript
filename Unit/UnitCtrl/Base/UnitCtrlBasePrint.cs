using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static CommonData;
using static CommonFunc;
using static PlayerKeyCtrl;
using static GameConfig;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using System.IO;
public partial class UnitCtrlBase
{
    string printContent = "";
    public void PrintCreate()
    {
        // printContent += $"zCode: {zCode}  {Environment.NewLine}";
        // if (parentUnitCtrl != null)
        //     printContent += $"parentName: {parentUnitCtrl.zCode}  {Environment.NewLine}";
        // else
        //     printContent += $"parentName: null  {Environment.NewLine}";
        // printContent += createSetting.Print();
    }

    public void AddPrintContent(string content)
    {
        // printContent += content;
    }
    public string FileWriteContent()
    {
        File.WriteAllText(GameSystem.Instance.unitLogPath + zCode + ".txt", printContent);
        return printContent;
    }

}


