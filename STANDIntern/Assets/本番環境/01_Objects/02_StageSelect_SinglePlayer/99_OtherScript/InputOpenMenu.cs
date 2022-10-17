using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputOpenMenu : InputLockElement
{
    [SerializeField]
    public UnityEvent onClickCallBack;

    void Update()
    {
        if (!IsCanInput) { return; }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            onClickCallBack.Invoke();
        }
    }
}
