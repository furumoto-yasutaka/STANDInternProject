using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class SkinSelectManager : InputLockElement
{
    [SerializeField]
    private PlayerSkinDataBase playerSkinDataBase;  // �f�[�^�x�[�X
    [SerializeField]
    private Animator promptAnimator;                // �Ñ��e�L�X�g�̃A�j���[�^�[
    [SerializeField]
    private UnityEvent specialSubmitCallBack;       // �m����͂̃R�[���o�b�N

    public static int[] PrevSkinId;                 // �O��m�肵���X�L���̏��

    private SkinSelectWindow[] skinSelectWindows;   // �e�v���C���[�̃X�L���E�B���h�E
    private bool isCanSubmit = false;               // �m����͂��\��
    private InputAction specialSubmitAction;        // �m�����

    private bool isFirstEnable = true;              // �A�v�����N�����čŏ��̏�����


    public PlayerSkinDataBase PlayerSkinDataBase { get { return playerSkinDataBase; } }


    static SkinSelectManager()
    {
        //=====�ÓI�ϐ���������
        PrevSkinId = new int[DeviceManager.DeviceNum];
        for (int i = 0; i < DeviceManager.DeviceNum; i++)
        {
            PrevSkinId[i] = -1;
        }
    }

    void Awake()
    {
        skinSelectWindows = new SkinSelectWindow[DeviceManager.DeviceNum];

        // �ǂ̃v���C���[�ɑΉ����Ă��邩�ԍ���o�^
        for (int i = 0; i < DeviceManager.DeviceNum; i++)
        {
            skinSelectWindows[i] = transform.GetChild(i).GetComponent<SkinSelectWindow>();
            skinSelectWindows[i].SetPlayerId(i);
        }

        PlayerInput input = myGroupParent.InputLockManager.transform.GetComponent<PlayerInput>();
        InputActionMap map = input.currentActionMap;
        specialSubmitAction = map["SpecialSubmit"];
    }

    void Start()
    {
        // �N������OnEnable�͎��s����邪�������̓s���ŃG���[�ƂȂ�̂�
        // ����̂ݖ��������Ă���s���ɂ��Start�̃^�C�~���O�Ɏ蓮�ŌĂ�
        OnEnable();
    }

    void Update()
    {
        if (!IsCanInput) { return; }

        if (isCanSubmit)
        {
            //=====�m�����
            if (specialSubmitAction.triggered)
            {
                SaveInfo();
                specialSubmitCallBack.Invoke();
                AudioManager.Instance.PlaySe("����(�^�C�g����ʂ̂�)");
            }
        }
    }

    /// <summary>
    /// �e�v���C���[�Ɍ����ăX�L�����肪�\���m�F��������
    /// </summary>
    public void CheckCanSubmitAll()
    {
        for (int i = 0; i < DeviceManager.DeviceNum; i++)
        {
            skinSelectWindows[i].CheckCanSubmit();
        }
    }

    /// <summary>
    /// �m����͂̉ۂ��m�F����
    /// </summary>
    public void CheckSkinAllSubmit()
    {
        for (int i = 0; i < DeviceManager.DeviceNum; i++)
        {
            if (DeviceManager.Instance.GetIsConnect(i))
            {
                if (!skinSelectWindows[i].IsSkinSubmit)
                {
                    promptAnimator.SetBool("IsCan", false);
                    isCanSubmit = false;
                    return;
                }
            }
        }

        if (!isCanSubmit)
        {
            AudioManager.Instance.PlaySe("���̉�ʂɐi�߂邱�Ƃ��\�ɂȂ鉹");
        }

        promptAnimator.SetBool("IsCan", true);
        isCanSubmit = true;
    }

    /// <summary>
    /// ���ݑI������Ă���X�L����ۑ�����
    /// </summary>
    private void SaveInfo()
    {
        for (int i = 0; i < DeviceManager.DeviceNum; i++)
        {
            if (skinSelectWindows[i].gameObject.activeInHierarchy)
            {
                PrevSkinId[i] = skinSelectWindows[i].SelectSkinId;
            }
        }
    }

    /// <summary>
    /// ���ݕۑ�����Ă���X�L�������폜����
    /// </summary>
    public void DeleteSaveInfo()
    {
        for (int i = 0; i < DeviceManager.DeviceNum; i++)
        {
            if (PrevSkinId[i] != -1)
            {
                PrevSkinId[i] = -1;
            }
            skinSelectWindows[i].ResetParam();
        }
    }

    private void OnEnable()
    {
        if (!isFirstEnable)
        {//=====����ȊO�͏������s��
            SetWindowParam();
        }
        else
        {//=====����͏������̓s���ŎQ�ƃG���[���N�����̂ŏ������������
            isFirstEnable = false;
        }
    }

    /// <summary>
    /// �ۑ�����Ă���X�L�������e�E�B���h�E�ɃZ�b�g����
    /// </summary>
    private void SetWindowParam()
    {
        for (int i = 0; i < DeviceManager.DeviceNum; i++)
        {
            skinSelectWindows[i].SetParam(PrevSkinId[i]);
        }
    }
}
