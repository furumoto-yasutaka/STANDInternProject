using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class ButtonSelectManager : InputLockElement
{
    public enum ButtonLine
    {
        Vertical = 0,
        Horizontal,
        Length,
    }

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
    protected ButtonLine buttonLine = ButtonLine.Vertical;  // �{�^���̕��ѕ���
    [SerializeField]
    protected UnityEvent onSelectCallBack;                  // �I�����̃R�[���o�b�N
    [SerializeField]
    protected UnityEvent onUnselectCallBack;                // �I���������̃R�[���o�b�N

    protected Animator[] animators;                         // �q�I�u�W�F�N�g(�{�^��)�̃A�j���[�^�[
    protected ButtonSelecter[] selecters;                   // �{�^���X�N���v�g
    protected int selectCursorIndex = 0;                    // �I����Ԃ̃{�^��Id

    protected System.Action[] checkInput;                   // ���͊m�F�֐�(�{�^���̕��т��Ƃɗp��)
    protected InputAction navigateAction;                   // �J�[�\������
    protected InputAction submitAction;                     // �������
    protected InputState inputState = InputState.None;      // �A�����͌��m�p�X�e�[�g
    protected int inputPattern = (int)InputPattern.None;    // �ǂ̃{�^����������Ă��邩�̃r�b�g�p�^�[��
    protected float inputMoveThreshold = 0.7f;              // ����������͂̋����̂������l
    protected float continueWaitTime = 0.5f;                // �A�����͊J�n�҂�����
    protected float continueIntervalTime = 0.2f;            // �A�����͑҂�����
    protected float continueTimeCount = 0.0f;               // �c�莞��

    protected bool isFirstEnable = true;                    // �A�v�����N�����čŏ��̏�����
    

    public int SelectCursorIndex { get { return selectCursorIndex; } }


    void Start()
    {
        animators = new Animator[transform.childCount];
        selecters = new ButtonSelecter[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform trans = transform.GetChild(i);
            animators[i] = trans.GetComponent<Animator>();
            selecters[i] = trans.GetComponent<ButtonSelecter>();
            selecters[i].Index = i;
        }

        checkInput = new System.Action[(int)ButtonLine.Length]
            { CheckInput_Vertical, CheckInput_Horizontal };

        
        if (transform.childCount == 0)
        {//=====�{�^�������݂��Ȃ��ꍇ�͓��͂��󂯕t���Ȃ��悤�ɂ���
            LockInput();
        }
        else
        {//=====�{�^�������݂���ꍇ�͈�ԍŏ��̃{�^����I����Ԃɂ���
            Select(0);
        }

        PlayerInput input = myGroupParent.InputLockManager.transform.GetComponent<PlayerInput>();
        InputActionMap map = input.currentActionMap;
        navigateAction = map["Navigate"];
        submitAction = map["Submit"];
    }

    protected virtual void Update()
    {
        if (!IsCanInput) { return; }

        // ���͎擾
        checkInput[(int)buttonLine]();
        // ���͂ɉ��������������s
        Execute();
    }

    protected void OnEnable()
    {
        if (!isFirstEnable)
        {//=====����ȊO�͏������s��
            if (transform.childCount == 0)
            {
                LockInput();
            }
            else
            {
                Select(0);
            }
        }
        else
        {//=====����͏������̓s���ŎQ�ƃG���[���N�����̂ŏ������������
            isFirstEnable = false;
        }
    }

    /// <summary>
    /// ���͊m�F(�c����)
    /// </summary>
    protected void CheckInput_Vertical()
    {
        float vertical = navigateAction.ReadValue<Vector2>().y;

        inputPattern = (int)InputPattern.None;

        if (vertical >= inputMoveThreshold)
        {
            inputPattern |= (int)InputPattern.Minus;
        }
        else if (vertical <= -inputMoveThreshold)
        {
            inputPattern |= (int)InputPattern.Plus;
        }
        else if (submitAction.triggered)
        {
            inputPattern |= (int)InputPattern.Submit;
        }
    }

    /// <summary>
    /// ���͊m�F(������)
    /// </summary>
    protected void CheckInput_Horizontal()
    {
        float horizontal = navigateAction.ReadValue<Vector2>().x;

        inputPattern = (int)InputPattern.None;

        if (horizontal <= -inputMoveThreshold)
        {
            inputPattern |= (int)InputPattern.Minus;
        }
        if (horizontal >= inputMoveThreshold)
        {
            inputPattern |= (int)InputPattern.Plus;
        }
        if (submitAction.triggered)
        {
            inputPattern |= (int)InputPattern.Submit;
        }
    }

    /// <summary>
    /// ���͂ɉ������������s
    /// </summary>
    protected void Execute()
    {
        if ((inputPattern & (int)InputPattern.Plus) > 0 ||
            (inputPattern & (int)InputPattern.Minus) > 0)
        {//=====�J�[�\���ړ�
            switch (inputState)
            {
                case InputState.None:
                    // �A�����͑҂��ɂ���
                    inputState = InputState.Wait;
                    continueTimeCount = continueWaitTime;

                    // �J�[�\���ړ�
                    MoveCursor();
                    break;
                case InputState.Wait:
                    if (continueTimeCount <= 0.0f)
                    {
                        // �A�����͂��J�n����
                        inputState = InputState.Interval;
                        continueTimeCount = continueIntervalTime;

                        // �J�[�\���ړ�
                        MoveCursor();
                    }
                    else
                    {
                        continueTimeCount -= Time.deltaTime;
                    }
                    break;
                case InputState.Interval:
                    if (continueTimeCount <= 0.0f)
                    {
                        continueTimeCount = continueIntervalTime;

                        // �J�[�\���ړ�
                        MoveCursor();
                    }
                    else
                    {
                        continueTimeCount -= Time.deltaTime;
                    }
                    break;
            }
        }
        else
        {//=====�J�[�\���ړ��̃{�^����������Ă��Ȃ��ꍇ�A�����͌n�p�����[�^��������
            continueTimeCount = 0.0f;
            inputState = InputState.None;
        }

        if ((inputPattern & (int)InputPattern.Submit) > 0)
        {//=====���菈��
            Decition();
        }
    }

    /// <summary>
    /// �J�[�\���ړ�
    /// </summary>
    protected void MoveCursor()
    {
        if ((inputPattern & (int)InputPattern.Plus) > 0)
        {
            Select((selectCursorIndex + 1) % transform.childCount);

            // ���ʉ��Đ�
            AudioManager.Instance.PlaySe("�J�[�\���ړ�");
        }
        if ((inputPattern & (int)InputPattern.Minus) > 0)
        {
            Select((selectCursorIndex - 1 + transform.childCount) % transform.childCount);

            // ���ʉ��Đ�
            AudioManager.Instance.PlaySe("�J�[�\���ړ�");
        }
    }

    /// <summary>
    /// ���菈��
    /// </summary>
    public void Decition()
    {
        selecters[selectCursorIndex].OnClickCallBack.Invoke();

        // ���ʉ��Đ�
        AudioManager.Instance.PlaySe("����");
    }

    /// <summary>
    /// �I����Ԃɂ���
    /// </summary>
    public virtual void Select(int index)
    {
        // ���݂̑I��������
        Unselect(selectCursorIndex);

        selectCursorIndex = index;
        animators[selectCursorIndex].SetBool("Select", true);
        onSelectCallBack.Invoke();
        selecters[selectCursorIndex].OnSelectCallBack.Invoke();
    }

    /// <summary>
    /// �I����Ԃ���������
    /// </summary>
    public virtual void Unselect(int index)
    {
        animators[index].SetBool("Select", false);
        onUnselectCallBack.Invoke();
        selecters[selectCursorIndex].OnUnselectCallBack.Invoke();
    }
}
