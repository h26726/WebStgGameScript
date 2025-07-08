using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Text;
using System.Linq;
using static CommonData;
using static CommonFunc;
using static PlayerKeyCtrl;
using static PlayerSaveData;
using static GameConfig;

public class ShotUnitCtrl : UnitCtrlBase
{
    public bool isSkipBirthAni = false;//透過Perfab設定是否跳過出生動畫
    public bool isThrough = false;//透過Perfab設定是否穿透
    protected uint showAniTime { get; set; }
    protected uint birthDurTime;
    protected float birthAniSpeed;
    protected float birthAniStart;


    protected override void CustomizeReset()
    {
        showAniTime = 0;
        birthAniSpeed = 0;
        birthAniStart = 0;
        birthDurTime = 0;
    }
    public override void OnActiveActCtrl(ActCtrl actCtrl, ActCtrl parentActCtrl = null)
    {
        if (animator == null)
        {
            base.OnActiveActCtrl(actCtrl, parentActCtrl);
            return;
        }
        if (!isSkipBirthAni && actCtrl.Id == coreSetting.Id  && AnimIsName("Idle", out var isEnd))
        {
            birthAniSpeed = coreSetting.birthAniSpeed == null ? DEFAULT_SHOW_ANI_SPEED : coreSetting.birthAniSpeed.Value;
            birthAniStart = coreSetting.birthAniStart == null ? DEFAULT_SHOW_ANI_START : coreSetting.birthAniStart.Value;
            birthDurTime = coreSetting.birthDurTime == null ? DEFAULT_SHOW_ANI_TIME : coreSetting.birthDurTime.Value;
            if (!Mathf.Approximately(birthDurTime, 0f))
                animator.Play("Idle", 0, birthAniStart);

            if (coreSetting.rotateIsMoveAngle == true && coreSetting.moveAngle != null)
            {
                var angle = GetAngle(coreSetting.moveAngle, out var isNewAngle);
                if (isNewAngle) SetRotateZ(angle);
                else SetRotateZ(angle);
            }
            else
            {
                if (coreSetting.addRotateZ != null)
                {
                    var angle = GetAngle(coreSetting.addRotateZ, out var isNewAngle);
                    if (isNewAngle) SetRotateZ(angle);
                    else SetRotateZ(GetRotateZ() + angle / 60);
                }
            }

            animator.speed = birthAniSpeed;
            eventMoveVectorCal += UpdateFadeIn;
        }
        else
        {
            ContinueActCtrl();
        }

        void UpdateFadeIn()
        {
            showAniTime++;
            AddPrintContent($"showAniTime:{showAniTime}");

            if (showAniTime < birthDurTime)
            {
                return;
            }
            eventMoveVectorCal -= UpdateFadeIn;
            ContinueActCtrl();
        }
        void ContinueActCtrl()
        {
            animator.speed = 1f;
            AddPrintContent($"ShotUnitCtrl FirstTriggerCustomize: animator is null or not Idle");
            animator.Play("Show", 0, 0f);
            base.OnActiveActCtrl(actCtrl, parentActCtrl);
        }
    }







    protected bool AnimIsName(string name, out bool isEnd)
    {
        var anim = animator.GetCurrentAnimatorStateInfo(0);
        isEnd = anim.normalizedTime >= 1;
        return anim.IsName(name);
    }

}
