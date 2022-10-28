using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class BackWindowInput : InputLockElement
{
    private InputAction cancelAction;
    [SerializeField]
    private UnityEvent BackCallBack;

    void Start()
    {
        PlayerInput input = myGroupParent.InputLockManager.transform.GetComponent<PlayerInput>();
        InputActionMap map = input.currentActionMap;
        cancelAction = map["Cancel"];
    }

    void Update()
    {
        if (!IsCanInput) { return; }

        if (cancelAction.triggered)
        {
            BackCallBack.Invoke();
            AudioManager.Instance.PlaySe("–ß‚é");
        }
    }
}
