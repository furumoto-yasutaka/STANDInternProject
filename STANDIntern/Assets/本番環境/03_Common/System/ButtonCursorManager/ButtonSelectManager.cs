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

    protected Animator[] animators;
    protected ButtonSelecter[] selecters;

    [SerializeField]
    protected ButtonLine buttonLine = ButtonLine.Vertical;
    protected System.Action[] checkInput;
    protected int selectIndex = 0;
    protected float inputMoveThreshold = 0.7f;
    protected InputState inputState = InputState.None;
    protected int inputPattern = (int)InputPattern.None;
    protected float continueWaitTime = 0.5f;
    protected float continueInterval = 0.2f;
    protected float continueCount = 0.0f;
    protected bool isFirstExec = true;

    protected InputAction navigateAction;
    protected InputAction submitAction;
    [SerializeField]
    protected UnityEvent OnSelectCallBack;
    [SerializeField]
    protected UnityEvent OnUnselectCallBack;

    public int SelectIndex { get { return selectIndex; } }

    void Start()
    {
        animators = new Animator[transform.childCount];
        selecters = new ButtonSelecter[transform.childCount];
        checkInput = new System.Action[(int)ButtonLine.Length]
            { CheckInput_Vertical, CheckInput_Horizontal };

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform trans = transform.GetChild(i);
            animators[i] = trans.GetComponent<Animator>();
            selecters[i] = trans.GetComponent<ButtonSelecter>();
            selecters[i].Index = i;
        }

        if (transform.childCount == 0)
        {
            LockInput();
        }
        else
        {
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

        inputPattern = (int)InputPattern.None;
        checkInput[(int)buttonLine]();
        Execute();
    }

    protected void OnEnable()
    {
        if (!isFirstExec)
        {
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
        {
            isFirstExec = false;
        }
    }

    protected void CheckInput_Vertical()
    {
        float vertical = navigateAction.ReadValue<Vector2>().y;

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

    protected void CheckInput_Horizontal()
    {
        float horizontal = navigateAction.ReadValue<Vector2>().x;

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

    protected void Execute()
    {
        if ((inputPattern & (int)InputPattern.Plus) > 0 ||
                (inputPattern & (int)InputPattern.Minus) > 0)
        {
            switch (inputState)
            {
                case InputState.None:
                    inputState = InputState.Wait;
                    continueCount = continueWaitTime;
                    MoveCursor();
                    break;
                case InputState.Wait:
                    if (continueCount <= 0.0f)
                    {
                        inputState = InputState.Interval;
                        continueCount = continueInterval;
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
        {
            continueCount = 0.0f;
            inputState = InputState.None;
        }
        if ((inputPattern & (int)InputPattern.Submit) > 0)
        {
            Decition();
        }
    }

    protected void MoveCursor()
    {
        if ((inputPattern & (int)InputPattern.Plus) > 0)
        {
            Select((selectIndex + 1) % transform.childCount);
            AudioManager.Instance.PlaySe("カーソル移動");
        }
        if ((inputPattern & (int)InputPattern.Minus) > 0)
        {
            Select((selectIndex - 1 + transform.childCount) % transform.childCount);
            AudioManager.Instance.PlaySe("カーソル移動");
        }
    }

    public void Decition()
    {
        selecters[selectIndex].OnClickCallBack.Invoke();
        AudioManager.Instance.PlaySe("決定");
    }

    public virtual void Select(int index)
    {
        Unselect(selectIndex);
        selectIndex = index;
        animators[selectIndex].SetBool("Select", true);
        OnSelectCallBack.Invoke();
        selecters[selectIndex].OnSelectCallBack.Invoke();
    }

    public virtual void Unselect(int index)
    {
        animators[index].SetBool("Select", false);
        OnUnselectCallBack.Invoke();
        selecters[selectIndex].OnUnselectCallBack.Invoke();
    }
}
