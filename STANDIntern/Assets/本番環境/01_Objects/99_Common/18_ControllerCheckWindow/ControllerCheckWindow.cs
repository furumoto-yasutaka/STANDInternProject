using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCheckWindow : MonoBehaviour
{
    private int playerId;
    private Animator animator;

    private bool isQuit = false;
    private bool isFirstEnable = true;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        OnEnable();
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        if (!isFirstEnable)
        {
            DeviceManager.Instance.Add_AddDevicePartsCallBack(CheckConnect, playerId);
            DeviceManager.Instance.Add_RemoveDevicePartsCallBack(CheckConnect, playerId);

            CheckConnect();
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
            DeviceManager.Instance.Remove_AddDevicePartsCallBack(CheckConnect, playerId);
            DeviceManager.Instance.Remove_RemoveDevicePartsCallBack(CheckConnect, playerId);
        }
    }

    private void OnApplicationQuit()
    {
        isQuit = true;
    }

    public void CheckConnect()
    {
        if (DeviceManager.Instance.GetIsConnect(playerId))
        {
            animator.SetBool("IsConnect", true);
            AudioManager.Instance.PlaySe("コントローラー接続");
        }
        else
        {
            animator.SetBool("IsConnect", false);
        }
    }

    public void SetId(int id)
    {
        playerId = id;
    }
}
