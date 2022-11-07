using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class SkinSelectWindow : InputLockElement
{
    public enum InputState
    {
        None = 0,
        Wait,
        Interval,
    }

    public enum InputPattern
    {
        None = 0,
        Plus = 1 << 0,
        Minus = 1 << 1,
        Submit = 1 << 2,
    }

    [SerializeField]
    private Image skinPreview;              // �g�p����X�L���̉摜
    [SerializeField]
    private Image skinPreviewBack;          // �g�p����X�L���̃A�j���[�V�����Ɏg�p����摜
    [SerializeField]
    private GameObject notSelectIconObj;    // �I��s�A�C�R���I�u�W�F�N�g

    private SkinSelectManager skinSelectManager;    // �}�l�[�W���[
    private Animator animator;              // �E�B���h�E�̃A�j���[�^�[
    private int playerId = 0;               // �S���̃v���C���[ID
    private int selectSkinId = 0;           // �I�������X�L����ID
    private bool isSkinSubmit = false;      // �X�L�����m�肵����
    private bool isCanInputMove = true;    // �J�[�\���ړ����ł����Ԃ�
    
    private InputState inputState = InputState.None;    // �A�����͌��m�p�X�e�[�g
    private int inputPattern = (int)InputPattern.None;  // �ǂ̃{�^����������Ă��邩�̃r�b�g�p�^�[��
    private float inputMoveThreshold = 0.7f;            // ����������͂̋����̂������l
    private float continueWaitTime = 0.5f;              // �A�����͊J�n�҂�����
    private float continueInterval = 0.2f;              // �A�����͑҂�����
    private float continueCount = 0.0f;                 // �c�莞��


    public int SelectSkinId { get { return selectSkinId; } }
    public bool IsSkinSubmit { get { return isSkinSubmit; } }


    void Awake()
    {
        skinSelectManager = transform.parent.GetComponent<SkinSelectManager>();
    }

    void Start()
    {
        animator = GetComponent<Animator>();

        CheckCanSubmit();
        CheckSkinSelectWindowActive();
    }

    void Update()
    {
        if (!IsCanInput) { return; }

        // ���͎擾
        CheckInput();
        // ���͂ɉ��������������s
        Execute();

        CheckSkinSelectWindowActive();
    }

    /// <summary>
    /// ���͊m�F
    /// </summary>
    private void CheckInput()
    {
        inputPattern = (int)InputPattern.None;

        if (!isCanInputMove) { return; }

        Gamepad pad = DeviceManager.Instance.GetDevice_FromPlayerIndex(playerId);
        float stickHorizontal = pad.leftStick.ReadValue().x;
        float dpadHorizontal = pad.dpad.ReadValue().x;

        if (stickHorizontal <= -inputMoveThreshold ||
            dpadHorizontal <= -inputMoveThreshold)
        {
            inputPattern |= (int)InputPattern.Minus;
        }
        if (stickHorizontal >= inputMoveThreshold ||
            dpadHorizontal >= inputMoveThreshold)
        {
            inputPattern |= (int)InputPattern.Plus;
        }
        if (pad.bButton.wasPressedThisFrame)
        {
            inputPattern |= (int)InputPattern.Submit;
        }
    }

    /// <summary>
    /// ���͂ɉ������������s
    /// </summary>
    private void Execute()
    {
        if ((inputPattern & (int)InputPattern.Plus) > 0 ||
            (inputPattern & (int)InputPattern.Minus) > 0)
        {//=====�J�[�\���ړ�
            switch (inputState)
            {
                case InputState.None:
                    // �A�����͑҂��ɂ���
                    inputState = InputState.Wait;
                    continueCount = continueWaitTime;

                    // �J�[�\���ړ�
                    MoveCursor();
                    break;
                case InputState.Wait:
                    if (continueCount <= 0.0f)
                    {
                        // �A�����͂��J�n����
                        inputState = InputState.Interval;
                        continueCount = continueInterval;

                        // �J�[�\���ړ�
                        MoveCursor();
                    }
                    else
                    {
                        continueCount -= Time.deltaTime;
                    }
                    break;
                case InputState.Interval:
                    if (continueCount <= 0.0f)
                    {
                        continueCount = continueInterval;

                        // �J�[�\���ړ�
                        MoveCursor();
                    }
                    else
                    {
                        continueCount -= Time.deltaTime;
                    }
                    break;
            }
        }
        else
        {//=====�J�[�\���ړ��̃{�^����������Ă��Ȃ��ꍇ�A�����͌n�p�����[�^��������
            continueCount = 0.0f;
            inputState = InputState.None;
        }

        if ((inputPattern & (int)InputPattern.Submit) > 0)
        {//=====����E��������
            if (isSkinSubmit)
            {
                Cancel();
            }
            else if (!skinSelectManager.PlayerSkinDataBase.PlayerSkinInfos[selectSkinId].isSelected)
            {
                Decition();
            }
        }
    }

    /// <summary>
    /// �J�[�\���ړ�
    /// </summary>
    private void MoveCursor()
    {
        int skinLength = skinSelectManager.PlayerSkinDataBase.PlayerSkinInfos.Length;

        if ((inputPattern & (int)InputPattern.Plus) > 0)
        {
            Select((selectSkinId + 1) % skinLength);

            // ���ʉ��Đ�
            AudioManager.Instance.PlaySe("�J�[�\���ړ�");
        }
        if ((inputPattern & (int)InputPattern.Minus) > 0)
        {
            Select((selectSkinId - 1 + skinLength) % skinLength);

            // ���ʉ��Đ�
            AudioManager.Instance.PlaySe("�J�[�\���ړ�");
        }
    }

    /// <summary>
    /// ���菈��
    /// </summary>
    private void Decition()
    {
        isSkinSubmit = true;
        isCanInputMove = false;
        skinSelectManager.PlayerSkinDataBase.PlayerSkinInfos[selectSkinId].isSelected = true;
        animator.SetBool("IsSubmit", true);
        skinSelectManager.CheckCanSubmitAll();
        skinSelectManager.CheckSkinAllSubmit();
        AudioManager.Instance.PlaySe("����");
    }

    /// <summary>
    /// �����������
    /// </summary>
    private void Cancel()
    {
        isSkinSubmit = false;
        isCanInputMove = false;
        skinSelectManager.PlayerSkinDataBase.PlayerSkinInfos[selectSkinId].isSelected = false;
        animator.SetBool("IsSubmit", false);
        skinSelectManager.CheckCanSubmitAll();
        skinSelectManager.CheckSkinAllSubmit();
    }

    /// <summary>
    /// �I����Ԃɂ���
    /// </summary>
    private void Select(int skinId)
    {
        selectSkinId = skinId;

        isCanInputMove = false;
        skinPreviewBack.sprite = skinPreview.sprite;
        skinPreview.sprite = skinSelectManager.PlayerSkinDataBase.PlayerSkinInfos[selectSkinId].Normal;

        animator.SetBool("IsChange", true);

        CheckCanSubmit();
    }

    /// <summary>
    /// �A�j���[�V�����I���̒ʒm
    /// </summary>
    public void EndSelectAnimation()
    {
        isCanInputMove = true;
    }

    /// <summary>
    /// �I�������X�L�����g�p�\���m�F����
    /// </summary>
    public void CheckCanSubmit()
    {
        if (skinSelectManager.PlayerSkinDataBase.PlayerSkinInfos[selectSkinId].isSelected &&
            !isSkinSubmit)
        {
            notSelectIconObj.SetActive(true);
        }
        else
        {
            notSelectIconObj.SetActive(false);
        }
    }

    /// <summary>
    /// �E�B���h�E���\���ɂ��邩�m�F
    /// </summary>
    private void CheckSkinSelectWindowActive()
    {
        if (!DeviceManager.Instance.GetIsConnect(playerId))
        {
            ResetParam();
            gameObject.SetActive(false);
            skinSelectManager.CheckSkinAllSubmit();
        }
    }

    /// <summary>
    /// �v���C���[ID���Z�b�g����
    /// </summary>
    public void SetPlayerId(int id)
    {
        playerId = id;
    }

    /// <summary>
    /// �����l���O������Z�b�g����
    /// </summary>
    public void SetParam(int skinId)
    {
        if (skinId != -1)
        {
            //=====�I������
            selectSkinId = skinId;
            skinPreview.sprite = skinSelectManager.PlayerSkinDataBase.PlayerSkinInfos[selectSkinId].Normal;

            //=====���菈��
            isSkinSubmit = true;
            isCanInputMove = false;
            skinSelectManager.PlayerSkinDataBase.PlayerSkinInfos[selectSkinId].isSelected = true;
            animator.SetBool("IsSubmit", true);
            skinSelectManager.CheckCanSubmitAll();
        }
        else
        {
            skinPreview.sprite = skinSelectManager.PlayerSkinDataBase.PlayerSkinInfos[selectSkinId].Normal;
        }
    }

    /// <summary>
    /// �p�����[�^������������
    /// </summary>
    public void ResetParam()
    {
        isSkinSubmit = false;
        isCanInputMove = true;
        skinSelectManager.PlayerSkinDataBase.PlayerSkinInfos[selectSkinId].isSelected = false;

        selectSkinId = 0;
        notSelectIconObj.SetActive(false);
    }
}
