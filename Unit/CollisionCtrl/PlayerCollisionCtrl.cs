using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Text;
using System.Linq;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static SaveJsonData;
using static GameConfig;

public class PlayerCollisionCtrl : CollisionCtrlBase
{
    ContactFilter2D filter;
    Collider2D myCollider;
    Collider2D[] results = new Collider2D[20];

    void Start()
    {
        myCollider = GetComponent<Collider2D>();
        filter = new ContactFilter2D();
        filter.useTriggers = true; // ✅ 啟用 Trigger 偵測
        filter.useLayerMask = true;
        filter.useDepth = false; // 不限制 Depth
        filter.SetLayerMask(Physics2D.GetLayerCollisionMask(myCollider.gameObject.layer));
    }

    // protected void OnTriggerStay2D(Collider2D collision)
    // {
    //     CollisionHandle(collision);
    // }
    public override void UpdateHandler()
    {
        int count = myCollider.OverlapCollider(filter, results);
        for (int i = 0; i < count; i++)
        {
            Collider2D opponentCollision = results[i];
            if (opponentCollision == null || opponentCollision == myCollider)
                continue;
            CollisionHandle(opponentCollision);
        }
    }
    protected override void Attacked(UnitPropBase opponentUnitProp)
    {
        if (unitProp.isInvincible != true && opponentUnitProp is EnemyShotUnitProp)
        {
            unitProp.isTriggerDead = true;
        }
        else if (opponentUnitProp is PowerUnitProp)
        {
            GamePlayer.GetPower();
        }
    }


}
