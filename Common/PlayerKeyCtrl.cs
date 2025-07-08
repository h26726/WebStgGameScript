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

public static class PlayerKeyCtrl
{
    public static bool[] CheckUpDownKey()
    {
        bool keyBoardUp = Input.GetKey(GetSetKey(KeyCode.UpArrow));
        if (keyBoardUp)
            return new bool[] { true, false };
        bool keyBoardDown = Input.GetKey(GetSetKey(KeyCode.DownArrow));
        if (keyBoardDown)
            return new bool[] { false, true };
        float vertical = Input.GetAxis("VerticalJoystick");// 上下
        if (vertical > 0.5f)
            return new bool[] { true, false };
        if (vertical < -0.5f)
            return new bool[] { false, true };
        return new bool[] { false, false };

    }

    public static bool[] CheckLeftRightKey()
    {
        bool keyBoardLeft = Input.GetKey(GetSetKey(KeyCode.LeftArrow));
        if (keyBoardLeft)
            return new bool[] { true, false };
        bool keyBoardRight = Input.GetKey(GetSetKey(KeyCode.RightArrow));
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

    public static bool[] CheckDownUpKeyOne(List<KeyBoardSaveData> keyBoardSaveData = null)
    {
        // 鍵盤（使用 GetKeyUp）
        if (keyBoardSaveData == null)
        {
            if (Input.GetKeyDown(GetSetKey(KeyCode.DownArrow)))
                return new bool[] { true, false };
            if (Input.GetKeyDown(GetSetKey(KeyCode.UpArrow)))
                return new bool[] { false, true };
        }
        else
        {
            if (Input.GetKeyDown(GetSetKey(KeyCode.DownArrow, keyBoardSaveData)))
                return new bool[] { true, false };
            if (Input.GetKeyDown(GetSetKey(KeyCode.UpArrow, keyBoardSaveData)))
                return new bool[] { false, true };
        }

        // 類比搖桿
        float vertical = Input.GetAxis("VerticalJoystick");
        bool upNow = vertical > 0.5f;
        bool downNow = vertical < -0.5f;

        bool upPressed = !wasUp && upNow;
        bool downPressed = !wasDown && downNow;

        wasUp = upNow;
        wasDown = downNow;

        return new bool[] { downPressed, upPressed };
    }

    public static bool[] CheckLeftRightDown()
    {
        // 鍵盤（使用 GetKeyUp）
        if (Input.GetKeyDown(GetSetKey(KeyCode.LeftArrow)))
            return new bool[] { true, false };
        if (Input.GetKeyDown(GetSetKey(KeyCode.RightArrow)))
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

    public static int KeyBoardOnceToDirectVal(List<KeyBoardSaveData> keyBoardSaveData = null)
    {
        var downUpKeyOnce = CheckDownUpKeyOne(keyBoardSaveData);
        if (downUpKeyOnce[0])
        {
            return 1;
        }
        else if (downUpKeyOnce[1])
        {
            return -1;
        }
        return 0;
    }



    public static void AutoSkipNotBtnAndOutBtn(ref int _BtnKey, int upDown, OptionBase[] _Btns)
    {
        _Btns[_BtnKey].animator.Play("Idle");
        _BtnKey += upDown;
        uint Count = 0;
        while (true)
        {
            if (_Btns.Length <= _BtnKey)
            {
                _BtnKey = 0;
            }
            else if (_BtnKey < 0)
            {
                _BtnKey = _Btns.Length - 1;
            }


            if (!_Btns[_BtnKey].isHide && !_Btns[_BtnKey].isDisable)
            {
                break;
            }
            _BtnKey = _BtnKey + upDown;

            Count++;
            if (Count > 100)
            {
                Debug.LogError("Count > 100");
                break;
            }
        }
        _Btns[_BtnKey].animator.Play("Active");
    }


}
