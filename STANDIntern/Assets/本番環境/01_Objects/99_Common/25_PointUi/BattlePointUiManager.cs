using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePointUiManager : MonoBehaviour
{
    private int playerId = 0;

    private bool isFirstEnable = true;

    void Start()
    {
        if (!DeviceManager.Instance.GetIsConnect(playerId)) { UiNotActive(); }

        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if (transform == transform.parent.GetChild(i))
            {
                playerId = i;
                DeviceManager.Instance.Add_RemoveDevicePartsCallBack(UiNotActive, playerId);
                break;
            }
        }
    }

    private void OnEnable()
    {
        if (!isFirstEnable)
        {
            DeviceManager.Instance.Remove_RemoveDevicePartsCallBack(UiNotActive, playerId);
        }
        else
        {
            isFirstEnable = false;
        }
    }

    public void UiNotActive()
    {
        gameObject.SetActive(false);
    }
}
