using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Linq;
using Cysharp.Threading.Tasks;


public class InputManager : MonoBehaviour
{
    static InputManager Input_Manager;

    InputData LatestInputData;

    [SerializeField] PlayerInput playerInput;


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static InputData GetInputData()
    {
        return Input_Manager.LatestInputData;
    }


    /// <summary>
    /// Load binding data(json)
    /// </summary>
    /// <param name="bindingJson"></param>
    public static void ImportBindings(string bindingJson)
    {
        Input_Manager.playerInput.actions.LoadBindingOverridesFromJson(bindingJson);
    }

    public static string ExportBindings()
    {
        return Input_Manager.playerInput.actions.SaveBindingOverridesAsJson();
    }


    public static List<string> GetIDfromPath(string path)
    {
        var result = Input_Manager.inputButtons.Where(x => x.hasPath(path));

        return result.Select(x => x.ID).ToList();
    }





    public enum Mode
    {
        Both,
        KeyBoard,
        GamePad
    }
    public static string GetBindings(string id, Mode mode)
    {
        try
        {
            var button = Input_Manager.inputButtons.First(x => x.ID == id);

            foreach (var item in button.inputAction.bindings)
            {
                print(item.effectivePath);
            }
        }
        catch
        {

        }
        return string.Empty;
    }

    public static void SetBindingsOverride(string id, string overridePath,Scheme scheme)
    {
        try
        {
            var inputAction = Input_Manager.inputButtons.First(x => x.ID == id).inputAction;

            for (int i = 0; i < inputAction.bindings.Count; i++)
            {
                if (Input_Utility.CheckPathScheme(inputAction.bindings[i].path, scheme))
                {
                    inputAction.ApplyBindingOverride(i, overridePath);
                }
            }
        }
        catch
        {
            Debug.LogError("invalid id");
        }

        Debug.Log(Input_Manager.playerInput.actions.SaveBindingOverridesAsJson());

    }

    public void ExchangeBinding(string id1,string id2, Scheme scheme)
    {
        try
        {
            var input1 = Input_Manager.inputButtons.First(x => x.ID == id1).inputAction;
            var input2 = Input_Manager.inputButtons.First(x => x.ID == id2).inputAction;


        }
        catch
        {
            Debug.LogError("invalid id");
        }
    }


    void Awake()
    {
        if (Input_Manager == null)
        {
            Input_Manager = this;
        }
        else
        {
            throw new Exception("awake twice error");
        }

        Init();
#pragma warning disable CS4014 // この呼び出しは待機されなかったため、現在のメソッドの実行は呼び出しの完了を待たずに続行されます
        Updater();
#pragma warning restore CS4014
    }


    //毎フレーム入力を更新する
    async UniTask Updater()
    {
        while (true)
        {
            UpdateFunction();

            //次のupdateの直前まで待機
            await UniTask.DelayFrame(1, PlayerLoopTiming.PreUpdate);
        }
    }

    void Init()
    {
        List<string> datas = GetInputList();

        inputButtons = new InputButton[datas.Count];

        for (int i = 0; i < datas.Count; i++)
        {
            inputButtons[i] = new InputButton(playerInput, datas[i]);
        }
    }



    List<string> GetInputList()
    {
        List<string> res = new List<string>();

        foreach (var item in playerInput.actions)
        {
            if (item.type == InputActionType.Button)
            {
                res.Add(item.name);
            }
        }

        return res;
    }

    InputButton[] inputButtons;


    void UpdateFunction()
    {

        DateTime datetime = DateTime.Now;
        float timer = Time.deltaTime;

        for (int i = 0; i < inputButtons.Length; i++)
        {
            inputButtons[i].FrameUpdate(timer);
        }

        var data = new InputData(inputButtons, Time.frameCount, datetime);
        LatestInputData = data;
    }

}


