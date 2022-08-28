using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace InputManager
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Input_Manager;

        void Awake()
        {
            print("test");

            if (Input_Manager == null)
            {
                Input_Manager = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Init()
        {

            framecount = 0;
            List<string> datas = GetInputList();

            inputButtons = new InputButton[datas.Count];

            for (int i = 0; i < datas.Count; i++)
            {
                inputButtons[i] = new InputButton(playerInput, datas[i]);
            }
        }


        [SerializeField] PlayerInput playerInput;

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


        uint framecount;

        public InputData FrameUpdate()
        {
            DateTime datetime = DateTime.Now;
            float timer = Time.deltaTime;

            for (int i = 0; i < inputButtons.Length; i++)
            {
                inputButtons[i].FrameUpdate(timer);
            }

            framecount++;
            return new InputData(inputButtons, framecount, datetime);
        }



    }

}
