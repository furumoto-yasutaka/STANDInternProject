using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class SkinSelectManager : InputLockElement
{
    [SerializeField]
    private PlayerSkinDataBase playerSkinDataBase;  // データベース
    [SerializeField]
    private Animator promptAnimator;                // 催促テキストのアニメーター
    [SerializeField]
    private UnityEvent specialSubmitCallBack;       // 確定入力のコールバック

    public static int[] PrevSkinId;                 // 前回確定したスキンの情報

    private SkinSelectWindow[] skinSelectWindows;   // 各プレイヤーのスキンウィンドウ
    private bool isCanSubmit = false;               // 確定入力が可能か
    private InputAction specialSubmitAction;        // 確定入力

    private bool isFirstEnable = true;              // アプリを起動して最初の処理か


    public PlayerSkinDataBase PlayerSkinDataBase { get { return playerSkinDataBase; } }


    static SkinSelectManager()
    {
        //=====静的変数を初期化
        PrevSkinId = new int[DeviceManager.DeviceNum];
        for (int i = 0; i < DeviceManager.DeviceNum; i++)
        {
            PrevSkinId[i] = -1;
        }
    }

    void Awake()
    {
        skinSelectWindows = new SkinSelectWindow[DeviceManager.DeviceNum];

        // どのプレイヤーに対応しているか番号を登録
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
        // 起動時にOnEnableは実行されるが処理順の都合でエラーとなるので
        // 初回のみ無効化している都合によりStartのタイミングに手動で呼ぶ
        OnEnable();
    }

    void Update()
    {
        if (!IsCanInput) { return; }

        if (isCanSubmit)
        {
            //=====確定入力
            if (specialSubmitAction.triggered)
            {
                SaveInfo();
                specialSubmitCallBack.Invoke();
                AudioManager.Instance.PlaySe("決定(タイトル画面のみ)");
            }
        }
    }

    /// <summary>
    /// 各プレイヤーに向けてスキン決定が可能か確認をさせる
    /// </summary>
    public void CheckCanSubmitAll()
    {
        for (int i = 0; i < DeviceManager.DeviceNum; i++)
        {
            skinSelectWindows[i].CheckCanSubmit();
        }
    }

    /// <summary>
    /// 確定入力の可否を確認する
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
            AudioManager.Instance.PlaySe("次の画面に進めることが可能になる音");
        }

        promptAnimator.SetBool("IsCan", true);
        isCanSubmit = true;
    }

    /// <summary>
    /// 現在選択されているスキンを保存する
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
    /// 現在保存されているスキン情報を削除する
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
        {//=====初回以外は処理を行う
            SetWindowParam();
        }
        else
        {//=====初回は初期化の都合で参照エラーを起こすので処理を回避する
            isFirstEnable = false;
        }
    }

    /// <summary>
    /// 保存されているスキン情報を各ウィンドウにセットする
    /// </summary>
    private void SetWindowParam()
    {
        for (int i = 0; i < DeviceManager.DeviceNum; i++)
        {
            skinSelectWindows[i].SetParam(PrevSkinId[i]);
        }
    }
}
