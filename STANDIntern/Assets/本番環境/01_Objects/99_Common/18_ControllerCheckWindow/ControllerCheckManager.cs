using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class ControllerCheckManager : InputLockElement
{
    [SerializeField]
    private Animator promptAnimator;            // �Ñ��e�L�X�g�̃A�j���[�^�[
    [SerializeField]
    private UnityEvent specialSubmitCallBack;   // �m����͉�����̃R�[���o�b�N

    private bool isCanSubmit = false;           // �m����͂��L����
    private InputAction specialSubmitAction;    // �m����͎擾�p

    private bool isFirstEnable = true;          // �A�v�����N�����čŏ��̏�����
    private bool isQuit = false;                // �A�v�����N�����čŌ�̏�����

    void Awake()
    {
        // �ǂ̃v���C���[�ɑΉ����Ă��邩�ԍ���o�^
        for (int i = 0; i < DeviceManager.DeviceNum; i++)
        {
            transform.GetChild(i).GetComponent<ControllerCheckWindow>().SetId(i);
        }
    }

    void Start()
    {
        PlayerInput input = myGroupParent.InputLockManager.transform.GetComponent<PlayerInput>();
        InputActionMap map = input.currentActionMap;
        specialSubmitAction = map["SpecialSubmit"];

        // �N������OnEnable�͎��s����邪�������̓s���ŃG���[�ƂȂ�̂�
        // ����̂ݖ��������Ă���s���ɂ��Start�̃^�C�~���O�Ɏ蓮�ŌĂ�
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

                // ���ʉ��Đ�
                AudioManager.Instance.PlaySe("����(�^�C�g����ʂ̂�)");
            }
        }
    }

    private void OnEnable()
    {
        if (!isFirstEnable)
        {//=====����ȊO�͏������s��
            // �R�[���o�b�N��o�^
            DeviceManager.Instance.Add_AddDeviceCallBack(CheckDeviceSubmit);
            DeviceManager.Instance.Add_RemoveDeviceCallBack(CheckDeviceSubmit);
            
            // ���̉�ʂɐi�߂��ԂɂȂ������m�F
            CheckDeviceSubmit();
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
            DeviceManager.Instance.Remove_AddDeviceCallBack(CheckDeviceSubmit);
            DeviceManager.Instance.Remove_RemoveDeviceCallBack(CheckDeviceSubmit);
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
    /// �f�o�C�X��2��ȏ�ڑ����ꂽ��ԂɂȂ������m�F
    /// </summary>
    public void CheckDeviceSubmit()
    {
        // 2��ȏ�ڑ�����Ă���ꍇ���̉�ʂɐi�ނ��Ƃ�������
        if (DeviceManager.Instance.deviceCount >= 2)
        {
            isCanSubmit = true;
            promptAnimator.SetBool("IsCan", true);

            // ���ʉ��Đ�
            AudioManager.Instance.PlaySe("���̉�ʂɐi�߂邱�Ƃ��\�ɂȂ鉹");
        }
        else
        {
            isCanSubmit = false;
            promptAnimator.SetBool("IsCan", false);
        }
    }
}
