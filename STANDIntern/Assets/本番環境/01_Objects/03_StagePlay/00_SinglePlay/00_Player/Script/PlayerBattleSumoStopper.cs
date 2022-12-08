using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleSumoStopper : MonoBehaviour
{
    private PlayerController[] playerControllers = new PlayerController[DeviceManager.DeviceNum];

    void Start()
    {
        for (int i = 0; i < DeviceManager.DeviceNum; i++)
        {
            playerControllers[i] = transform.GetChild(i).GetComponent<PlayerController>();
        }
    }

    public void PlayerAllStop()
    {
        for (int i = 0; i < DeviceManager.DeviceNum; i++)
        {
            playerControllers[i].Stop();
        }
    }
}
