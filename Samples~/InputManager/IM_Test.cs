using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IM_Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InputManager.InputManager.Input_Manager.Init();

    }

    // Update is called once per frame
    void Update()
    {
        var data=InputManager.InputManager.Input_Manager.FrameUpdate();
    }
}
