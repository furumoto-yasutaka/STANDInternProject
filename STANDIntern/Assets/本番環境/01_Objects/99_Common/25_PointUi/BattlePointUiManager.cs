using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePointUiManager : MonoBehaviour
{
    private int playerId = 0;

    void Start()
    {
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

    void Update()
    {
        if (!BattleSumoManager.IsPlayerJoin[playerId]) { UiNotActive(); }
    }

    public void UiNotActive()
    {
        gameObject.SetActive(false);
    }
}
