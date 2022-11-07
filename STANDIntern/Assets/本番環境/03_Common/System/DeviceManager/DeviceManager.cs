using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class DeviceManager : SingletonMonoBehaviour<DeviceManager>
{
    public class DeviceInfo
    {
        public bool IsConnect = false;                  // �v���C���[�ƘA�g���Ă��邩
        public Gamepad Device = null;                   // �f�o�C�X
        public UnityEvent addDevicePartsCallBack = new UnityEvent();       // �Y���v���C���[�f�o�C�X�ڑ����R�[���o�b�N
        public UnityEvent removeDevicePartsCallBack = new UnityEvent();    // �Y���v���C���[�f�o�C�X�ؒf���R�[���o�b�N
    }

    public const int DeviceNum = 4;     // �ő�f�o�C�X��

    private DeviceInfo[] deviceInfos;   // �v���C���[���Ƃ̃f�o�C�X���
    private List<Gamepad> tempDevice;   // �R���g���[���[�X�V�p���X�g
    public int deviceCount = 0;         // �ڑ��f�o�C�X��

    private UnityEvent addDeviceCallBack = new UnityEvent();       // �f�o�C�X�ڑ����R�[���o�b�N
    private UnityEvent removeDeviceCallBack = new UnityEvent();    // �f�o�C�X�ؒf���R�[���o�b�N

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
        // ���ݐڑ�����Ă���R���g���[���[��ID�������X�g�ɒǉ�
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            tempDevice.Add(Gamepad.all[i]);
        }

        // �v���C���[�ƘA�g�ς݂̃R���g���[���[��S�Ē��ׂ�
        for (int i = 0; i < DeviceNum; i++)
        {
            if (deviceInfos[i].IsConnect)
            {
                Gamepad pad = deviceInfos[i].Device;
                // �R���g���[���[���܂����݂��Ă��邩�T������
                int elemIndex = IndexOfDeviceId_FromTempDevice(pad.deviceId);

                if (elemIndex == -1)
                {//=====�ؒf����Ă���
                    // �Y���̃R���g���[���[��������������
                    RemoveDevice(i);
                }
                else
                {//=====�ڑ����p������Ă���
                    // ���ɘA�g�ς݂̃R���g���[���[�Ȃ̂ŉ����X�g����폜����
                    tempDevice.RemoveAt(elemIndex);
                }
            }
        }

        // tempDevice�Ɏc�����v�f�͐V�����ڑ����ꂽ�R���g���[���[�Ȃ̂œo�^����
        for (int i = 0; i < tempDevice.Count; i++)
        {
            int emptyIndex = IndexOfEmptyDevice();

            // �v���C���[�ɋ󂫂�����Γo�^��F�߂�
            if (emptyIndex != -1)
            {
                AddDevice(tempDevice[i], emptyIndex);
            }
            else
            {
                // �󂫂��Ȃ��ꍇ�͑����Ă��Ӗ����Ȃ��̂�
                break;
            }
        }

        tempDevice.Clear();
    }

    /// <summary>
    /// �f�o�C�X�o�^
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
    /// �f�o�C�X�폜
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
    /// �f�o�C�X�o�^���R�[���o�b�N�o�^
    /// </summary>
    public void Add_AddDeviceCallBack(UnityAction action)
    {
        addDeviceCallBack.AddListener(action);
    }

    /// <summary>
    /// �f�o�C�X�폜���R�[���o�b�N�o�^
    /// </summary>
    public void Add_RemoveDeviceCallBack(UnityAction action)
    {
        removeDeviceCallBack.AddListener(action);
    }

    /// <summary>
    /// �f�o�C�X�o�^���R�[���o�b�N�폜
    /// </summary>
    public void Remove_AddDeviceCallBack(UnityAction action)
    {
        addDeviceCallBack.RemoveListener(action);
    }

    /// <summary>
    /// �f�o�C�X�폜���R�[���o�b�N�폜
    /// </summary>
    public void Remove_RemoveDeviceCallBack(UnityAction action)
    {
        removeDeviceCallBack.RemoveListener(action);
    }

    /// <summary>
    /// �Y���v���C���[�f�o�C�X�o�^���R�[���o�b�N�o�^
    /// </summary>
    public void Add_AddDevicePartsCallBack(UnityAction action, int playerId)
    {
        deviceInfos[playerId].addDevicePartsCallBack.AddListener(action);
    }

    /// <summary>
    /// �Y���v���C���[�f�o�C�X�o�^���R�[���o�b�N�폜
    /// </summary>
    public void Add_RemoveDevicePartsCallBack(UnityAction action, int playerId)
    {
        deviceInfos[playerId].removeDevicePartsCallBack.AddListener(action);
    }

    /// <summary>
    /// �Y���v���C���[�f�o�C�X�폜���R�[���o�b�N�o�^
    /// </summary>
    public void Remove_AddDevicePartsCallBack(UnityAction action, int playerId)
    {
        deviceInfos[playerId].addDevicePartsCallBack.RemoveListener(action);
    }

    /// <summary>
    /// �Y���v���C���[�f�o�C�X�폜���R�[���o�b�N�폜
    /// </summary>
    public void Remove_RemoveDevicePartsCallBack(UnityAction action, int playerId)
    {
        deviceInfos[playerId].removeDevicePartsCallBack.RemoveListener(action);
    }


    /// <summary>
    /// �w�肵���f�o�C�XId�����݂��邩DeviceInfos����T��
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
    /// �w�肵���f�o�C�XId�����݂��邩TempDevice����T��
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
    /// ���o�^�̘g���󂢂Ă��邩�T��
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
