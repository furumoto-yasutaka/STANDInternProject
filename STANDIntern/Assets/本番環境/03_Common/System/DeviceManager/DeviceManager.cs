using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class DeviceManager : SingletonMonoBehaviour<DeviceManager>
{
    public const int deviceNum = 4;
    public Gamepad[] gameDevice = new Gamepad[deviceNum];
    public int[] gameDeviceId = new int[deviceNum];
    private int[] removeDeviceId = new int[deviceNum];
    public int deviceCount = 0;

    [SerializeField]
    private UnityEvent addDeviceCallBack;
    [SerializeField]
    private UnityEvent[] addDevicePartsCallBack = new UnityEvent[deviceNum];
    [SerializeField]
    private UnityEvent removeDeviceCallBack;
    [SerializeField]
    private UnityEvent[] removeDevicePartsCallBack = new UnityEvent[deviceNum];

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < deviceNum; i++)
        {
            gameDeviceId[i] = -1;
            removeDeviceId[i] = -1;
        }

        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            AddDevice(Gamepad.all[i], i);
        }
    }

    void Update()
    {
        gameDeviceId.CopyTo(removeDeviceId, 0);

        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            Gamepad pad = Gamepad.all[i];
            int elemIndex = IndexOfElement(removeDeviceId, pad.deviceId);

            if (elemIndex == -1)
            {
                int emptyIndex = IndexOfEmpty(gameDeviceId);

                if (emptyIndex != -1)
                {
                    AddDevice(pad, emptyIndex);
                }
            }
            else
            {
                removeDeviceId[elemIndex] = -1;
            }
        }

        for (int i = 0; i < removeDeviceId.Length; i++)
        {
            if (removeDeviceId[i] == gameDeviceId[i] && removeDeviceId[i] != -1)
            {
                RemoveDevice(i);
            }
        }
    }

    public void AddDevice(Gamepad device, int index)
    {
        gameDevice[index] = device;
        gameDeviceId[index] = device.deviceId;
        deviceCount++;
        addDeviceCallBack.Invoke();
        addDevicePartsCallBack[index].Invoke();
    }

    public void RemoveDevice(int index)
    {
        gameDevice[index] = null;
        gameDeviceId[index] = -1;
        deviceCount--;
        removeDeviceCallBack.Invoke();
        removeDevicePartsCallBack[index].Invoke();
    }


    public bool GetIsConnect(int index)
    {
        if (gameDeviceId[index] == -1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public Gamepad GetDeviceFromPlayerIndex(int index)
    {
        if (gameDeviceId[index] != -1)
        {
            return gameDevice[index];
        }
        else
        {
            return null;
        }
    }

    public Gamepad GetDeviceFromSystemInput(int index)
    {
        if (Gamepad.all.Count > index)
        {
            return Gamepad.all[index];
        }
        else
        {
            return null;
        }
    }


    public void Add_AddDeviceCallBack(UnityAction action)
    {
        addDeviceCallBack.AddListener(action);
    }

    public void Add_RemoveDeviceCallBack(UnityAction action)
    {
        removeDeviceCallBack.AddListener(action);
    }

    public void Remove_AddDeviceCallBack(UnityAction action)
    {
        addDeviceCallBack.RemoveListener(action);
    }

    public void Remove_RemoveDeviceCallBack(UnityAction action)
    {
        removeDeviceCallBack.RemoveListener(action);
    }


    public void Add_AddDevicePartsCallBack(UnityAction action, int index)
    {
        addDevicePartsCallBack[index].AddListener(action);
    }

    public void Add_RemoveDevicePartsCallBack(UnityAction action, int index)
    {
        removeDevicePartsCallBack[index].AddListener(action);
    }

    public void Remove_AddDevicePartsCallBack(UnityAction action, int index)
    {
        addDevicePartsCallBack[index].RemoveListener(action);
    }

    public void Remove_RemoveDevicePartsCallBack(UnityAction action, int index)
    {
        removeDevicePartsCallBack[index].RemoveListener(action);
    }


    private int IndexOfElement(int[] source, int elem)
    {
        for (int i = 0; i < source.Length; i++)
        {
            if (source[i] == elem)
            {
                return i;
            }
        }

        return -1;
    }

    private int IndexOfEmpty(int[] source)
    {
        for (int i = 0; i < source.Length; i++)
        {
            if (source[i] == -1)
            {
                return i;
            }
        }

        return -1;
    }

    public int IndexOfPlayerNum(int elem)
    {
        for (int i = 0; i < gameDeviceId.Length; i++)
        {
            if (gameDeviceId[i] == elem)
            {
                return i;
            }
        }

        return -1;
    }

    private void OnGUI()
    {
        var style = GUI.skin.GetStyle("label");
        style.fontSize = 40;

        GUILayout.BeginHorizontal(GUILayout.Width(1920));
        GUILayout.BeginVertical(GUILayout.Width(1080));
        foreach (int id in gameDeviceId)
        {
            GUILayout.Label(id.ToString());
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
}
