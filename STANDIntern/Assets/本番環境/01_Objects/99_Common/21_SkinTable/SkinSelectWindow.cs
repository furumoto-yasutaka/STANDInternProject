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
    private Image skinPreview;              // 使用するスキンの画像
    [SerializeField]
    private Image skinPreviewBack;          // 使用するスキンのアニメーションに使用する画像
    [SerializeField]
    private GameObject notSelectIconObj;    // 選択不可アイコンオブジェクト

    private SkinSelectManager skinSelectManager;    // マネージャー
    private Animator animator;              // ウィンドウのアニメーター
    private int playerId = 0;               // 担当のプレイヤーID
    private int selectSkinId = 0;           // 選択したスキンのID
    private bool isSkinSubmit = false;      // スキンを確定したか
    private bool isCanInputMove = true;    // カーソル移動ができる状態か
    
    private InputState inputState = InputState.None;    // 連続入力検知用ステート
    private int inputPattern = (int)InputPattern.None;  // どのボタンが押されているかのビットパターン
    private float inputMoveThreshold = 0.7f;            // 反応する入力の強さのしきい値
    private float continueWaitTime = 0.5f;              // 連続入力開始待ち時間
    private float continueInterval = 0.2f;              // 連続入力待ち時間
    private float continueCount = 0.0f;                 // 残り時間


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

        // 入力取得
        CheckInput();
        // 入力に応じた処理を実行
        Execute();

        CheckSkinSelectWindowActive();
    }

    /// <summary>
    /// 入力確認
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
    /// 入力に応じた処理実行
    /// </summary>
    private void Execute()
    {
        if ((inputPattern & (int)InputPattern.Plus) > 0 ||
            (inputPattern & (int)InputPattern.Minus) > 0)
        {//=====カーソル移動
            switch (inputState)
            {
                case InputState.None:
                    // 連続入力待ちにする
                    inputState = InputState.Wait;
                    continueCount = continueWaitTime;

                    // カーソル移動
                    MoveCursor();
                    break;
                case InputState.Wait:
                    if (continueCount <= 0.0f)
                    {
                        // 連続入力を開始する
                        inputState = InputState.Interval;
                        continueCount = continueInterval;

                        // カーソル移動
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

                        // カーソル移動
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
        {//=====カーソル移動のボタンが押されていない場合連続入力系パラメータを初期化
            continueCount = 0.0f;
            inputState = InputState.None;
        }

        if ((inputPattern & (int)InputPattern.Submit) > 0)
        {//=====決定・解除処理
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
    /// カーソル移動
    /// </summary>
    private void MoveCursor()
    {
        int skinLength = skinSelectManager.PlayerSkinDataBase.PlayerSkinInfos.Length;

        if ((inputPattern & (int)InputPattern.Plus) > 0)
        {
            Select((selectSkinId + 1) % skinLength);

            // 効果音再生
            AudioManager.Instance.PlaySe("カーソル移動");
        }
        if ((inputPattern & (int)InputPattern.Minus) > 0)
        {
            Select((selectSkinId - 1 + skinLength) % skinLength);

            // 効果音再生
            AudioManager.Instance.PlaySe("カーソル移動");
        }
    }

    /// <summary>
    /// 決定処理
    /// </summary>
    private void Decition()
    {
        isSkinSubmit = true;
        isCanInputMove = false;
        skinSelectManager.PlayerSkinDataBase.PlayerSkinInfos[selectSkinId].isSelected = true;
        animator.SetBool("IsSubmit", true);
        skinSelectManager.CheckCanSubmitAll();
        skinSelectManager.CheckSkinAllSubmit();
        AudioManager.Instance.PlaySe("決定");
    }

    /// <summary>
    /// 決定解除処理
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
    /// 選択状態にする
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
    /// アニメーション終了の通知
    /// </summary>
    public void EndSelectAnimation()
    {
        isCanInputMove = true;
    }

    /// <summary>
    /// 選択したスキンが使用可能か確認する
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
    /// ウィンドウを非表示にするか確認
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
    /// プレイヤーIDをセットする
    /// </summary>
    public void SetPlayerId(int id)
    {
        playerId = id;
    }

    /// <summary>
    /// 初期値を外部からセットする
    /// </summary>
    public void SetParam(int skinId)
    {
        if (skinId != -1)
        {
            //=====選択処理
            selectSkinId = skinId;
            skinPreview.sprite = skinSelectManager.PlayerSkinDataBase.PlayerSkinInfos[selectSkinId].Normal;

            //=====決定処理
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
    /// パラメータを初期化する
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
