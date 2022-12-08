using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class DeviceManager : SingletonMonoBehaviour<DeviceManager>
{
    public class DeviceInfo
    {
        public bool IsConnect = false;                  // プレイヤーと連携しているか
        public Gamepad Device = null;                   // デバイス
        public UnityEvent addDevicePartsCallBack = new UnityEvent();       // 該当プレイヤーデバイス接続時コールバック
        public UnityEvent removeDevicePartsCallBack = new UnityEvent();    // 該当プレイヤーデバイス切断時コールバック
    }

    public const int DeviceNum = 4;     // 最大デバイス数

    private DeviceInfo[] deviceInfos;   // プレイヤーごとのデバイス情報
    private List<Gamepad> tempDevice;   // コントローラー更新用リスト
    public int deviceCount = 0;         // 接続デバイス数

    private UnityEvent addDeviceCallBack = new UnityEvent();       // デバイス接続時コールバック
    private UnityEvent removeDeviceCallBack = new UnityEvent();    // デバイス切断時コールバック

    protected override void Awake()
    {
        base.Awake();

        deviceInfos = new DeviceInfo[DeviceNum];
        for (int i = 0; i < DeviceNum; i++)
        {
            deviceInfos[i] = new DeviceInfo();
        }

        tempDevice = new List<Gamepad>();

        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            AddDevice(Gamepad.all[i], i);
        }
    }

    void Update()
    {
        // 現在接続されているコントローラーのIDを仮リストに追加
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            tempDevice.Add(Gamepad.all[i]);
        }

        // プレイヤーと連携済みのコントローラーを全て調べる
        for (int i = 0; i < DeviceNum; i++)
        {
            if (deviceInfos[i].IsConnect)
            {
                Gamepad pad = deviceInfos[i].Device;
                // コントローラーがまだ存在しているか探索する
                int elemIndex = IndexOfDeviceId_FromTempDevice(pad.deviceId);

                if (elemIndex == -1)
                {//=====切断されている
                    // 該当のコントローラー情報を初期化する
                    RemoveDevice(i);
                }
                else
                {//=====接続が継続されている
                    // 既に連携済みのコントローラーなので仮リストから削除する
                    tempDevice.RemoveAt(elemIndex);
                }
            }
        }

        // tempDeviceに残った要素は新しく接続されたコントローラーなので登録する
        for (int i = 0; i < tempDevice.Count; i++)
        {
            int emptyIndex = IndexOfEmptyDevice();

            // プレイヤーに空きがあれば登録を認める
            if (emptyIndex != -1)
            {
                AddDevice(tempDevice[i], emptyIndex);
            }
            else
            {
                // 空きがない場合は続けても意味がないので
                break;
            }
        }

        tempDevice.Clear();
    }

    /// <summary>
    /// デバイス登録
    /// </summary>
    public void AddDevice(Gamepad device, int playerId)
    {
        deviceCount++;
        deviceInfos[playerId].IsConnect = true;
        deviceInfos[playerId].Device = device;
        addDeviceCallBack.Invoke();
        deviceInfos[playerId].addDevicePartsCallBack.Invoke();
    }

    /// <summary>
    /// デバイス削除
    /// </summary>
    public void RemoveDevice(int playerId)
    {
        deviceCount--;
        deviceInfos[playerId].IsConnect = false;
        deviceInfos[playerId].Device = null;
        removeDeviceCallBack.Invoke();
        deviceInfos[playerId].removeDevicePartsCallBack.Invoke();
    }


    public bool GetIsConnect(int playerId)
    {
        return deviceInfos[playerId].IsConnect;
    }

    public Gamepad GetDevice_FromPlayerIndex(int playerId)
    {
        if (deviceInfos[playerId].IsConnect)
        {
            return deviceInfos[playerId].Device;
        }
        else
        {
            return null;
        }
    }

    public Gamepad GetDevice_FromSystemInput(int index)
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


    /// <summary>
    /// デバイス登録時コールバック登録
    /// </summary>
    public void Add_AddDeviceCallBack(UnityAction action)
    {
        addDeviceCallBack.AddListener(action);
    }

    /// <summary>
    /// デバイス削除時コールバック登録
    /// </summary>
    public void Add_RemoveDeviceCallBack(UnityAction action)
    {
        removeDeviceCallBack.AddListener(action);
    }

    /// <summary>
    /// デバイス登録時コールバック削除
    /// </summary>
    public void Remove_AddDeviceCallBack(UnityAction action)
    {
        addDeviceCallBack.RemoveListener(action);
    }

    /// <summary>
    /// デバイス削除時コールバック削除
    /// </summary>
    public void Remove_RemoveDeviceCallBack(UnityAction action)
    {
        removeDeviceCallBack.RemoveListener(action);
    }

    /// <summary>
    /// 該当プレイヤーデバイス登録時コールバック登録
    /// </summary>
    public void Add_AddDevicePartsCallBack(UnityAction action, int playerId)
    {
        deviceInfos[playerId].addDevicePartsCallBack.AddListener(action);
    }

    /// <summary>
    /// 該当プレイヤーデバイス登録時コールバック削除
    /// </summary>
    public void Add_RemoveDevicePartsCallBack(UnityAction action, int playerId)
    {
        deviceInfos[playerId].removeDevicePartsCallBack.AddListener(action);
    }

    /// <summary>
    /// 該当プレイヤーデバイス削除時コールバック登録
    /// </summary>
    public void Remove_AddDevicePartsCallBack(UnityAction action, int playerId)
    {
        deviceInfos[playerId].addDevicePartsCallBack.RemoveListener(action);
    }

    /// <summary>
    /// 該当プレイヤーデバイス削除時コールバック削除
    /// </summary>
    public void Remove_RemoveDevicePartsCallBack(UnityAction action, int playerId)
    {
        deviceInfos[playerId].removeDevicePartsCallBack.RemoveListener(action);
    }


    /// <summary>
    /// 指定したデバイスIdが存在するかDeviceInfosから探索
    /// </summary>
    public int IndexOfDeviceId_FromDeviceInfos(int deviceId)
    {
        for (int i = 0; i < deviceInfos.Length; i++)
        {
            if (deviceInfos[i].Device.deviceId == deviceId)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// 指定したデバイスIdが存在するかTempDeviceから探索
    /// </summary>
    private int IndexOfDeviceId_FromTempDevice(int deviceId)
    {
        for (int i = 0; i < tempDevice.Count; i++)
        {
            if (tempDevice[i].deviceId == deviceId)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// 未登録の枠が空いているか探索
    /// </summary>
    private int IndexOfEmptyDevice()
    {
        for (int i = 0; i < deviceInfos.Length; i++)
        {
            if (!deviceInfos[i].IsConnect)
            {
                return i;
            }
        }

        return -1;
    }

    //private void OnGUI()
    //{
    //    var style = GUI.skin.GetStyle("label");
    //    style.fontSize = 40;

    //    GUILayout.BeginHorizontal(GUILayout.Width(1920));
    //    GUILayout.BeginVertical(GUILayout.Width(1080));
    //    foreach (int id in gameDeviceId)
    //    {
    //        GUILayout.Label(id.ToString());
    //    }
    //    GUILayout.EndVertical();
    //    GUILayout.EndHorizontal();
    //}
}
