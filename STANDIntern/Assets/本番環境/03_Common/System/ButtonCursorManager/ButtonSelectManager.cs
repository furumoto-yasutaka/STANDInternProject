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
    protected ButtonLine buttonLine = ButtonLine.Vertical;  // ボタンの並び方向
    [SerializeField]
    protected UnityEvent onSelectCallBack;                  // 選択時のコールバック
    [SerializeField]
    protected UnityEvent onUnselectCallBack;                // 選択解除時のコールバック

    protected Animator[] animators;                         // 子オブジェクト(ボタン)のアニメーター
    protected ButtonSelecter[] selecters;                   // ボタンスクリプト
    protected int selectCursorIndex = 0;                    // 選択状態のボタンId

    protected System.Action[] checkInput;                   // 入力確認関数(ボタンの並びごとに用意)
    protected InputAction navigateAction;                   // カーソル入力
    protected InputAction submitAction;                     // 決定入力
    protected InputState inputState = InputState.None;      // 連続入力検知用ステート
    protected int inputPattern = (int)InputPattern.None;    // どのボタンが押されているかのビットパターン
    protected float inputMoveThreshold = 0.7f;              // 反応する入力の強さのしきい値
    protected float continueWaitTime = 0.5f;                // 連続入力開始待ち時間
    protected float continueIntervalTime = 0.2f;            // 連続入力待ち時間
    protected float continueTimeCount = 0.0f;               // 残り時間

    protected bool isFirstEnable = true;                    // アプリを起動して最初の処理か
    

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
        {//=====ボタンが存在しない場合は入力を受け付けないようにする
            LockInput();
        }
        else
        {//=====ボタンが存在する場合は一番最初のボタンを選択状態にする
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

        // 入力取得
        checkInput[(int)buttonLine]();
        // 入力に応じた処理を実行
        Execute();
    }

    protected void OnEnable()
    {
        if (!isFirstEnable)
        {//=====初回以外は処理を行う
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
        {//=====初回は初期化の都合で参照エラーを起こすので処理を回避する
            isFirstEnable = false;
        }
    }

    /// <summary>
    /// 入力確認(縦並び)
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
    /// 入力確認(横並び)
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
    /// 入力に応じた処理実行
    /// </summary>
    protected void Execute()
    {
        if ((inputPattern & (int)InputPattern.Plus) > 0 ||
            (inputPattern & (int)InputPattern.Minus) > 0)
        {//=====カーソル移動
            switch (inputState)
            {
                case InputState.None:
                    // 連続入力待ちにする
                    inputState = InputState.Wait;
                    continueTimeCount = continueWaitTime;

                    // カーソル移動
                    MoveCursor();
                    break;
                case InputState.Wait:
                    if (continueTimeCount <= 0.0f)
                    {
                        // 連続入力を開始する
                        inputState = InputState.Interval;
                        continueTimeCount = continueIntervalTime;

                        // カーソル移動
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

                        // カーソル移動
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
        {//=====カーソル移動のボタンが押されていない場合連続入力系パラメータを初期化
            continueTimeCount = 0.0f;
            inputState = InputState.None;
        }

        if ((inputPattern & (int)InputPattern.Submit) > 0)
        {//=====決定処理
            Decition();
        }
    }

    /// <summary>
    /// カーソル移動
    /// </summary>
    protected void MoveCursor()
    {
        if ((inputPattern & (int)InputPattern.Plus) > 0)
        {
            Select((selectCursorIndex + 1) % transform.childCount);

            // 効果音再生
            AudioManager.Instance.PlaySe("カーソル移動");
        }
        if ((inputPattern & (int)InputPattern.Minus) > 0)
        {
            Select((selectCursorIndex - 1 + transform.childCount) % transform.childCount);

            // 効果音再生
            AudioManager.Instance.PlaySe("カーソル移動");
        }
    }

    /// <summary>
    /// 決定処理
    /// </summary>
    public void Decition()
    {
        selecters[selectCursorIndex].OnClickCallBack.Invoke();

        // 効果音再生
        AudioManager.Instance.PlaySe("決定");
    }

    /// <summary>
    /// 選択状態にする
    /// </summary>
    public virtual void Select(int index)
    {
        // 現在の選択を解除
        Unselect(selectCursorIndex);

        selectCursorIndex = index;
        animators[selectCursorIndex].SetBool("Select", true);
        onSelectCallBack.Invoke();
        selecters[selectCursorIndex].OnSelectCallBack.Invoke();
    }

    /// <summary>
    /// 選択状態を解除する
    /// </summary>
    public virtual void Unselect(int index)
    {
        animators[index].SetBool("Select", false);
        onUnselectCallBack.Invoke();
        selecters[selectCursorIndex].OnUnselectCallBack.Invoke();
    }
}
