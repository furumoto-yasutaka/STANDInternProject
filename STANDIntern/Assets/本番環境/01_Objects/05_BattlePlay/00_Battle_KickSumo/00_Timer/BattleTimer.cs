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
    [SerializeField]
    private GameObject timeUpObj;

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

        SetTimeText();
    }

    void Update()
    {
        if (!isActive) { return; }

        SetTimeText();

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
            
            timeUpObj.SetActive(true);
            AudioManager.Instance.PlaySe("ƒQ[ƒ€I—¹");
        }
    }

    private void SetTimeText()
    {
        int min = (int)timeCountSec / 60;
        int sec = (int)timeCountSec - min * 60;
        int msec = (int)((timeCountSec - (int)timeCountSec) * 100);

        minAndSecText.text = min + ":" + sec.ToString("00");
        msecText.text = "." + msec.ToString("00");
    }

    public void StartTimer()
    {
        isActive = true;
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1.0f;
    }
}
