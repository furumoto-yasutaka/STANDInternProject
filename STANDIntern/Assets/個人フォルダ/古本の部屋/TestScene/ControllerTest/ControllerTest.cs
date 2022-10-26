using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class ControllerTest : MonoBehaviour
{
    private int GamePadCount = 0;

    void Start()
    {
        
    }

    void Update()
    {
        GamePadCount = Gamepad.all.Count;
    }

    private void OnGUI()
    {
        GUILayout.Label("GamePadÅF" + GamePadCount);
    }
}
