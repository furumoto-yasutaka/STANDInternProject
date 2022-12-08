using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCheckWindow : MonoBehaviour
{
    private int playerId;               // �S���̃v���C���[��ID
    private Animator animator;          // �A�j���[�^�[

    private bool isFirstEnable = true;  // �A�v�����N�����čŏ��̏�����
    private bool isQuit = false;        // �A�v�����N�����čŌ�̏�����

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // �N������OnEnable�͎��s����邪�������̓s���ŃG���[�ƂȂ�̂�
        // ����̂ݖ��������Ă���s���ɂ��Start�̃^�C�~���O�Ɏ蓮�ŌĂ�
        OnEnable();
    }

    private void OnEnable()
    {
        if (!isFirstEnable)
        {//=====����ȊO�͏������s��
            // �R�[���o�b�N��o�^
            DeviceManager.Instance.Add_AddDevicePartsCallBack(Connect, playerId);
            DeviceManager.Instance.Add_RemoveDevicePartsCallBack(Disconnect, playerId);

            // �ڑ���Ԃ��m�F
            CheckConnect();
        }
        else
        {//=====����͏������̓s���ŎQ�ƃG���[���N�����̂ŏ������������
            isFirstEnable = false;
        }
    }

    private void OnDisable()
    {
        if (!isQuit)
        {//=====�A�v���I�����ȊO�͏������s��
            DeviceManager.Instance.Remove_AddDevicePartsCallBack(Connect, playerId);
            DeviceManager.Instance.Remove_RemoveDevicePartsCallBack(Disconnect, playerId);
        }
    }

    /// <summary>
    /// �A�v���I�����ɌĂ΂��OnDisable�ɂ��Q�ƃG���[���o�邱�Ƃ�h�����߂̃t���O�𗧂Ă�
    /// </summary>
    private void OnApplicationQuit()
    {
        isQuit = true;
    }

    /// <summary>
    /// �R���g���[���[�ڑ��m�F
    /// </summary>
    public void CheckConnect()
    {
        if (DeviceManager.Instance.GetIsConnect(playerId))
        {
            Connect();
        }
        else
        {
            Disconnect();
        }
    }

    /// <summary>
    /// �R���g���[���[�ڑ��ʒm����
    /// </summary>
    public void Connect()
    {
        animator.SetBool("IsConnect", true);

        // ���ʉ��Đ�
        AudioManager.Instance.PlaySe("�R���g���[���[�ڑ�");
    }

    /// <summary>
    /// �R���g���[���[�ؒf�ʒm����
    /// </summary>
    public void Disconnect()
    {
        animator.SetBool("IsConnect", false);
    }

    /// <summary>
    /// �S���v���C���[ID��o�^
    /// </summary>
    public void SetId(int id)
    {
        playerId = id;
    }
}
