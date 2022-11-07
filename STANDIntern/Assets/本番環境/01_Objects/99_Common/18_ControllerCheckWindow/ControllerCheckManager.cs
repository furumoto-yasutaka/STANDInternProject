using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class ControllerCheckManager : InputLockElement
{
    [SerializeField]
    private Animator promptAnimator;            // 催促テキストのアニメーター
    [SerializeField]
    private UnityEvent specialSubmitCallBack;   // 確定入力押下後のコールバック

    private bool isCanSubmit = false;           // 確定入力が有効か
    private InputAction specialSubmitAction;    // 確定入力取得用

    private bool isFirstEnable = true;          // アプリを起動して最初の処理か
    private bool isQuit = false;                // アプリを起動して最後の処理か

    void Awake()
    {
        // どのプレイヤーに対応しているか番号を登録
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

        // 起動時にOnEnableは実行されるが処理順の都合でエラーとなるので
        // 初回のみ無効化している都合によりStartのタイミングに手動で呼ぶ
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

                // 効果音再生
                AudioManager.Instance.PlaySe("決定(タイトル画面のみ)");
            }
        }
    }

    private void OnEnable()
    {
        if (!isFirstEnable)
        {//=====初回以外は処理を行う
            // コールバックを登録
            DeviceManager.Instance.Add_AddDeviceCallBack(CheckDeviceSubmit);
            DeviceManager.Instance.Add_RemoveDeviceCallBack(CheckDeviceSubmit);
            
            // 次の画面に進める状態になったか確認
            CheckDeviceSubmit();
        }
        else
        {//=====初回は初期化の都合で参照エラーを起こすので処理を回避する
            isFirstEnable = false;
        }
    }

    private void OnDisable()
    {
        if (!isQuit)
        {//=====アプリ終了時以外は処理を行う
            DeviceManager.Instance.Remove_AddDeviceCallBack(CheckDeviceSubmit);
            DeviceManager.Instance.Remove_RemoveDeviceCallBack(CheckDeviceSubmit);
        }
    }

    /// <summary>
    /// アプリ終了時に呼ばれるOnDisableにより参照エラーが出ることを防ぐためのフラグを立てる
    /// </summary>
    private void OnApplicationQuit()
    {
        isQuit = true;
    }

    /// <summary>
    /// デバイスが2台以上接続された状態になったか確認
    /// </summary>
    public void CheckDeviceSubmit()
    {
        // 2台以上接続されている場合次の画面に進むことを許可する
        if (DeviceManager.Instance.deviceCount >= 2)
        {
            isCanSubmit = true;
            promptAnimator.SetBool("IsCan", true);

            // 効果音再生
            AudioManager.Instance.PlaySe("次の画面に進めることが可能になる音");
        }
        else
        {
            isCanSubmit = false;
            promptAnimator.SetBool("IsCan", false);
        }
    }
}
