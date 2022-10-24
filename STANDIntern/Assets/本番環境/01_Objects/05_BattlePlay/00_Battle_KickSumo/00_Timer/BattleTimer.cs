using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleTimer : MonoBehaviour
{
    [SerializeField]
    private float timeMin;
    [SerializeField, ReadOnly]
    private float timeCountSec;
    [SerializeField]
    private float timeupTimeScale = 0.1f;

    private bool isActive = false;
    private TextMeshProUGUI minAndSecText;
    private TextMeshProUGUI msecText;
    private BattleCountDown countDownStaging;

    void Start()
    {
        minAndSecText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        msecText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        countDownStaging = transform.GetChild(2).GetComponent<BattleCountDown>();

        if (timeMin <= 0)
        {
            timeMin = 1;
        }

        timeCountSec = timeMin * 60;
    }

    void Update()
    {
        if (!isActive) { return; }

        int min = (int)timeCountSec / 60;
        int sec = (int)timeCountSec - min * 60;
        int msec = (int)((timeCountSec - (int)timeCountSec) * 100);

        minAndSecText.text = min + ":" + sec.ToString("00");
        msecText.text = "." + msec.ToString("00");

        timeCountSec -= Time.deltaTime;

        if (timeCountSec <= countDownStaging.Count)
        {
            minAndSecText.gameObject.SetActive(false);
            msecText.gameObject.SetActive(false);
            countDownStaging.gameObject.SetActive(true);
            minAndSecText.enabled = false;
            msecText.enabled = false;
        }

        if (timeCountSec <= 0.0f)
        {
            isActive = false;
            timeCountSec = 0.0f;
            Time.timeScale = timeupTimeScale;
        }
    }

    public void StartTimer()
    {
        isActive = true;
    }
}
