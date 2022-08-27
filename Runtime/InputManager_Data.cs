using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using System.Linq;

namespace InputManager
{
    public struct InputData
    {
        internal InputData(InputButton[] sources, uint framecount, DateTime datetime)
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

        public uint Framecount { get; private set; }


        public SingleInputData GetSingleInputData(string id)
        {
            SingleInputData data;
            if (keyValues.TryGetValue(id,out data))
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

        InputAction inputAction;

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
    }

}

