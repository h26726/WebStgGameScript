using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;
using System.Xml;
using System.IO;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static SaveJsonData;
using static GameConfig;
using System.Linq;
using static LoadCtrl;

public static class AnimationHelper
{
    public static IEnumerator PlayAniCoroutine(Animator animator)
    {
        var obj = animator.transform.gameObject;
        obj.SetActive(true);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        obj.SetActive(false);
        animator.Rebind();
        animator.Update(0f);
    }
}