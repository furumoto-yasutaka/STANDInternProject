using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleTimer : MonoBehaviour
{
    //=====内部から取得
    [SerializeField]
    private TextMeshProUGUI minAndSecText;
    [SerializeField]
    private TextMeshProUGUI msecText;
    [SerializeField]
    private BattleCountDown countDownStaging;

    [SerializeField]
    private float timeMin;
    [SerializeField]
    private GameObject timeUpObjPrefab;

    // タイマーが有効か
    private bool isActive = false;
    // タイマーの残り時間(秒)
    private float timeCountSec;

    // タイムアップ時のスロー演出の速度
    private static readonly float timeupTimeScale = 0.1f;

    void Start()
    {
        if (timeMin <= 0)
        {
            timeMin = 1;
        }

        // 分から秒に直す
        timeCountSec = timeMin * 60;

        SetTimeText();
    }

    void Update()
    {
        if (!isActive) { return; }

        SetTimeText();

        timeCountSec -= Time.deltaTime;

        // 残りの時間が最終カウントダウン以下になったら
        if (timeCountSec <= countDownStaging.Count)
        {
            //=====時間表示を終了し、カウントダウン演出を開始する
            countDownStaging.gameObject.SetActive(true);
            minAndSecText.enabled = false;
            msecText.enabled = false;
        }

        // 時間が終了したら
        if (timeCountSec <= 0.0f)
        {
            isActive = false;
            timeCountSec = 0.0f;

            // スロー演出
            Time.timeScale = timeupTimeScale;

            // タイムアップ演出開始
            timeUpObjPrefab.SetActive(true);

            // 効果音再生
            AudioManager.Instance.PlaySe("ゲーム終了");
        }
    }

    /// <summary>
    /// テキストを変更
    /// </summary>
    private void SetTimeText()
    {
        int min = (int)timeCountSec / 60;
        int sec = (int)timeCountSec - min * 60;
        int msec = (int)((timeCountSec - (int)timeCountSec) * 100);

        minAndSecText.text = min + ":" + sec.ToString("00");
        msecText.text = "." + msec.ToString("00");
    }

    /// <summary>
    /// タイマーの動作開始
    /// </summary>
    public void StartTimer()
    {
        isActive = true;
    }

    /// <summary>
    /// スロー状態を解除する
    /// </summary>
    public void ResetTimeScale()
    {
        Time.timeScale = 1.0f;
    }
}
