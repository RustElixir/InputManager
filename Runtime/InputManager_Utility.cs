using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;



public enum Scheme
{
    All,
    KeyBoard,
    GamePad
}


public static class Input_Utility
{

    public static bool CheckPathScheme(string path,Scheme scheme)
    {
        switch (scheme)
        {
            case Scheme.All:
                return path.Contains("<Keyboard>") || path.Contains("<Gamepad>");
            case Scheme.KeyBoard:
                return path.Contains("<Keyboard>");
            case Scheme.GamePad:
                return path.Contains("<Gamepad>");
            default:
                return false;
        }
    }

    public static string Path2InputControlPath(string id)
    {
        return string.Empty;
    }

    public struct waitKeyData
    {
        public enum Result
        {
            Get,
            Canceled,
        }

        public Result result;
        public string name;
        public string keyPath;

    }

    public static async UniTask<waitKeyData> waitKeyInput(string cancelPath="")
    {

        Func<bool> func = () => {
            return Keyboard.current.anyKey.wasPressedThisFrame;
        };

        await UniTask.WaitUntil(func);


        var key = Keyboard.current.allKeys.First(x => x.wasPressedThisFrame);

        var keydata = new waitKeyData();

        if (key.path == cancelPath)
        {
            keydata.result = waitKeyData.Result.Canceled;
        }
        else
        {
            keydata.result = waitKeyData.Result.Get;
        }
        keydata.name = key.displayName;
        keydata.keyPath = key.path; 
        return keydata;
    }

    public static async UniTask<waitKeyData> waitGamepadInput(string cancelPath = "")
    {

        Func<bool> func = () => {
            return Gamepad.current.allControls.Any();
        };

        await UniTask.WaitUntil(func);


        var key = Keyboard.current.allKeys.First(x => x.wasPressedThisFrame);

        var keydata = new waitKeyData();

        if (key.path == cancelPath)
        {
            keydata.result = waitKeyData.Result.Canceled;
        }
        else
        {
            keydata.result = waitKeyData.Result.Get;
        }
        keydata.name = key.displayName;
        keydata.keyPath = key.path;
        return keydata;
    }

}



public struct InputData
{
    internal InputData(InputButton[] sources, int framecount, DateTime datetime)
    {
        DateTime = datetime;
        Framecount = framecount;
        keyValues = new Dictionary<string, SingleInputData>();
        for (int i = 0; i < sources.Length; i++)
        {
            keyValues.Add(sources[i].ID, new SingleInputData(sources[i]));
        }
    }

    Dictionary<string, SingleInputData> keyValues;

    public DateTime DateTime { get; private set; }

    public int Framecount { get; private set; }


    public SingleInputData GetSingleInputData(string id)
    {
        SingleInputData data;
        if (keyValues.TryGetValue(id, out data))
        {
            return data;
        }
        else
        {
            Debug.Log($"id:{id} is invalid");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
            return new SingleInputData();
        }

    }



    public Vector2 GetArrowInput(string up = "Up", string down = "Down", string right = "Right", string left = "Left")
    {
        var u = Vector2.up * (GetSingleInputData(up).isPressed ? 1 : 0);
        var d = Vector2.down * (GetSingleInputData(down).isPressed ? 1 : 0);
        var r = Vector2.right * (GetSingleInputData(right).isPressed ? 1 : 0);

        var l = Vector2.left * (GetSingleInputData(left).isPressed ? 1 : 0);

        return u + d + r + l;
    }




    public string KeyList()
    {
        string res = "Input ID List:\n";

        foreach (var item in keyValues.Keys)
        {
            string d = $"{item},";

            res += d;
        }
        return res;
    }

    public string InputLog()
    {
        string res = "InputLog:\n";

        foreach (var item in keyValues.Values)
        {
            string d = $" {item.ID}:   isP:{item.isPressed}   isPTF:{item.isPressedThisFrame}    isRTF:{item.isReleasedThisFrame}   PTimer:{item.PressTimer} HTimer:{item.HoldTimer} RTimer:{item.ReleaseTimer}\n";

            res += d;
        }
        return res;
    }
}

public struct SingleInputData
{
    internal SingleInputData(InputButton source)
    {
        ID = source.ID;
        isPressed = source.isPressed;
        isPressedThisFrame = source.isPressedThisFrame;
        isReleasedThisFrame = source.isReleasedThisFrame;
        PressTimer = source.PressTimer;
        HoldTimer = source.HoldTimer;
        ReleaseTimer = source.ReleaseTimer;
    }
    public string ID
    {
        get; private set;
    }
    public bool isPressed
    {
        get; private set;
    }
    public bool isPressedThisFrame
    {
        get; private set;
    }
    public bool isReleasedThisFrame
    {
        get; private set;
    }
    public float PressTimer
    {
        get; private set;
    }
    public float HoldTimer
    {
        get; private set;
    }
    public float ReleaseTimer

    {
        get; private set;
    }
}

internal struct InputButton
{
    internal InputButton(PlayerInput input, string id)
    {
        inputAction = input.actions[id];
        ID = id;

        isPressed = false;
        isPressedThisFrame = false;
        isReleasedThisFrame = false;
        PressTimer = 0;
        HoldTimer = 0;
        ReleaseTimer = 0;


    }
    internal string ID;

    internal InputAction inputAction;

    internal bool isPressed;
    internal bool isPressedThisFrame;
    internal bool isReleasedThisFrame;
    internal float PressTimer;
    internal float HoldTimer;
    internal float ReleaseTimer;


    internal void FrameUpdate(float time)
    {
        isPressed = inputAction.IsPressed();
        isPressedThisFrame = inputAction.WasPressedThisFrame();
        isReleasedThisFrame = inputAction.WasReleasedThisFrame();

        if (isPressedThisFrame)
        {
            PressTimer = 0;
        }
        else
        {
            PressTimer += time;
        }

        if (isReleasedThisFrame)
        {
            ReleaseTimer = 0;
        }
        else
        {
            ReleaseTimer += time;
        }

        if (isPressedThisFrame || isReleasedThisFrame)
        {
            HoldTimer = 0;
        }

        if (isPressed)
        {
            HoldTimer += time;
        }

    }

    public bool hasPath(string path)
    {
        foreach (var item in inputAction.bindings)
        {
            
            //Debug.Log($"path:{item.path} effectivepath:{InputControlPath.(item.effectivePath)}");
        }
        return inputAction.bindings.Any(x => x.path == path);
    }

    public List<string> GetPath()
    {
        return null;
    }

}
