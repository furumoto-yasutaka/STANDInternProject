using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class SpecialSubmitInput : InputLockElement
{
    private InputAction specialSubmitAction;
    [SerializeField]
    private UnityEvent specialSubmitCallBack;

    void Start()
    {
        PlayerInput input = myGroupParent.InputLockManager.transform.GetComponent<PlayerInput>();
        InputActionMap map = input.currentActionMap;
        specialSubmitAction = map["SpecialSubmit"];
    }

    void Update()
    {
        if (!IsCanInput) { return; }

        if (specialSubmitAction.triggered)
        {
            specialSubmitCallBack.Invoke();
        }
    }
}
