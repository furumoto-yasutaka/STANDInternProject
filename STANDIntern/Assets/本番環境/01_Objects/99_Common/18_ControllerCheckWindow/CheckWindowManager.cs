using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class CheckWindowManager : InputLockElement
{
    [SerializeField]
    private Animator promptAnimator;
    private bool isCanSubmit = false;

    private InputAction specialSubmitAction;
    [SerializeField]
    private UnityEvent specialSubmitCallBack;

    private bool isQuit = false;
    private bool isFirstEnable = true;

    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<ControllerCheckWindow>().SetId(i);
        }
    }

    void Start()
    {
        PlayerInput input = myGroupParent.InputLockManager.transform.GetComponent<PlayerInput>();
        InputActionMap map = input.currentActionMap;
        specialSubmitAction = map["SpecialSubmit"];

        OnEnable();
    }

    void Update()
    {
        if (!IsCanInput) { return; }

        if (isCanSubmit)
        {
            if (specialSubmitAction.triggered)
            {
                specialSubmitCallBack.Invoke();
            }
        }
    }

    private void OnEnable()
    {
        if (!isFirstEnable)
        {
            DeviceManager.Instance.Add_AddDeviceCallBack(CheckDeviceSubmit);
            DeviceManager.Instance.Add_RemoveDeviceCallBack(CheckDeviceSubmit);

            CheckDeviceSubmit();
        }
        else
        {
            isFirstEnable = false;
        }
    }

    private void OnDisable()
    {
        if (!isQuit)
        {
            DeviceManager.Instance.Remove_AddDeviceCallBack(CheckDeviceSubmit);
            DeviceManager.Instance.Remove_RemoveDeviceCallBack(CheckDeviceSubmit);
        }
    }

    private void OnApplicationQuit()
    {
        isQuit = true;
    }

    public void CheckDeviceSubmit()
    {
        if (DeviceManager.Instance.deviceCount >= 2)
        {
            promptAnimator.SetBool("IsCan", true);
            isCanSubmit = true;
        }
        else
        {
            promptAnimator.SetBool("IsCan", false);
            isCanSubmit = false;
        }
    }
}
