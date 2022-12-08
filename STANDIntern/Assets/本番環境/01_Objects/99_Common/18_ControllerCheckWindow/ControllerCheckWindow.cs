using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCheckWindow : MonoBehaviour
{
    private int playerId;               // 担当のプレイヤーのID
    private Animator animator;          // アニメーター

    private bool isFirstEnable = true;  // アプリを起動して最初の処理か
    private bool isQuit = false;        // アプリを起動して最後の処理か

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // 起動時にOnEnableは実行されるが処理順の都合でエラーとなるので
        // 初回のみ無効化している都合によりStartのタイミングに手動で呼ぶ
        OnEnable();
    }

    private void OnEnable()
    {
        if (!isFirstEnable)
        {//=====初回以外は処理を行う
            // コールバックを登録
            DeviceManager.Instance.Add_AddDevicePartsCallBack(Connect, playerId);
            DeviceManager.Instance.Add_RemoveDevicePartsCallBack(Disconnect, playerId);

            // 接続状態を確認
            CheckConnect();
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
            DeviceManager.Instance.Remove_AddDevicePartsCallBack(Connect, playerId);
            DeviceManager.Instance.Remove_RemoveDevicePartsCallBack(Disconnect, playerId);
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
    /// コントローラー接続確認
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
    /// コントローラー接続通知処理
    /// </summary>
    public void Connect()
    {
        animator.SetBool("IsConnect", true);

        // 効果音再生
        AudioManager.Instance.PlaySe("コントローラー接続");
    }

    /// <summary>
    /// コントローラー切断通知処理
    /// </summary>
    public void Disconnect()
    {
        animator.SetBool("IsConnect", false);
    }

    /// <summary>
    /// 担当プレイヤーIDを登録
    /// </summary>
    public void SetId(int id)
    {
        playerId = id;
    }
}
