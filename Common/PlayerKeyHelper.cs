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
using static PlayerSaveData;
using static GameConfig;

public static class PlayerKeyHelper
{
    public static bool[] IsKey_UpDown()
    {
        bool keyBoardUp = Input.GetKey(TransferToPlayerSetKey(KeyCode.UpArrow));
        if (keyBoardUp)
            return new bool[] { true, false };
        bool keyBoardDown = Input.GetKey(TransferToPlayerSetKey(KeyCode.DownArrow));
        if (keyBoardDown)
            return new bool[] { false, true };
        float vertical = Input.GetAxis("VerticalJoystick");// 上下
        if (vertical > 0.5f)
            return new bool[] { true, false };
        if (vertical < -0.5f)
            return new bool[] { false, true };
        return new bool[] { false, false };

    }

    public static bool[] IsKey_LeftRight()
    {
        bool keyBoardLeft = Input.GetKey(TransferToPlayerSetKey(KeyCode.LeftArrow));
        if (keyBoardLeft)
            return new bool[] { true, false };
        bool keyBoardRight = Input.GetKey(TransferToPlayerSetKey(KeyCode.RightArrow));
        if (keyBoardRight)
            return new bool[] { false, true };
        float horizontal = Input.GetAxis("HorizontalJoystick"); // 左右
        if (horizontal > 0.5f)
            return new bool[] { false, true };
        if (horizontal < -0.5f)
            return new bool[] { true, false };
        return new bool[] { false, false };

    }

    public static bool wasUp = false;
    public static bool wasDown = false;
    public static bool wasLeft = false;
    public static bool wasRight = false;

    public static bool[] IsKeyOne_UpDown(List<KeyBoardSaveData> keyBoardSaveData = null)
    {
        // 鍵盤（使用 GetKeyUp）
        if (keyBoardSaveData == null)
        {
            if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.DownArrow)))
                return new bool[] { false, true };
            if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.UpArrow)))
                return new bool[] { true, false };
        }
        else
        {
            if (Input.GetKeyDown(TransferToTmpSetKey(KeyCode.DownArrow, keyBoardSaveData)))
                return new bool[] { false, true };
            if (Input.GetKeyDown(TransferToTmpSetKey(KeyCode.UpArrow, keyBoardSaveData)))
                return new bool[] { true, false };
        }

        // 類比搖桿
        float vertical = Input.GetAxis("VerticalJoystick");
        bool upNow = vertical > 0.5f;
        bool downNow = vertical < -0.5f;

        bool upPressed = !wasUp && upNow;
        bool downPressed = !wasDown && downNow;

        wasUp = upNow;
        wasDown = downNow;

        return new bool[] { upPressed, downPressed };
    }

    public static bool[] IsKeyDown_LeftRight()
    {
        // 鍵盤（使用 GetKeyUp）
        if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.LeftArrow)))
            return new bool[] { true, false };
        if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.RightArrow)))
            return new bool[] { false, true };

        // 類比搖桿
        float horizontal = Input.GetAxis("HorizontalJoystick");
        bool leftNow = horizontal < -0.5f;
        bool rightNow = horizontal > 0.5f;

        bool leftPressed = !wasLeft && leftNow;
        bool rightPressed = !wasRight && rightNow;

        wasLeft = leftNow;
        wasRight = rightNow;

        return new bool[] { leftPressed, rightPressed };
    }

    public static int GetKeyOneVal_UpDown(List<KeyBoardSaveData> keyBoardSaveData = null)
    {
        var KeyOne_UpDown = IsKeyOne_UpDown(keyBoardSaveData);
        if (KeyOne_UpDown[1])
        {
            return 1;
        }
        else if (KeyOne_UpDown[0])
        {
            return -1;
        }
        return 0;
    }
}
