using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleTimer : MonoBehaviour
{
    //=====��������擾
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

    // �^�C�}�[���L����
    private bool isActive = false;
    // �^�C�}�[�̎c�莞��(�b)
    private float timeCountSec;

    // �^�C���A�b�v���̃X���[���o�̑��x
    private static readonly float timeupTimeScale = 0.1f;

    void Start()
    {
        if (timeMin <= 0)
        {
            timeMin = 1;
        }

        // ������b�ɒ���
        timeCountSec = timeMin * 60;

        SetTimeText();
    }

    void Update()
    {
        if (!isActive) { return; }

        SetTimeText();

        timeCountSec -= Time.deltaTime;

        // �c��̎��Ԃ��ŏI�J�E���g�_�E���ȉ��ɂȂ�����
        if (timeCountSec <= countDownStaging.Count)
        {
            //=====���ԕ\�����I�����A�J�E���g�_�E�����o���J�n����
            countDownStaging.gameObject.SetActive(true);
            minAndSecText.enabled = false;
            msecText.enabled = false;
        }

        // ���Ԃ��I��������
        if (timeCountSec <= 0.0f)
        {
            isActive = false;
            timeCountSec = 0.0f;

            // �X���[���o
            Time.timeScale = timeupTimeScale;

            // �^�C���A�b�v���o�J�n
            timeUpObjPrefab.SetActive(true);

            // ���ʉ��Đ�
            AudioManager.Instance.PlaySe("�Q�[���I��");
        }
    }

    /// <summary>
    /// �e�L�X�g��ύX
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
    /// �^�C�}�[�̓���J�n
    /// </summary>
    public void StartTimer()
    {
        isActive = true;
    }

    /// <summary>
    /// �X���[��Ԃ���������
    /// </summary>
    public void ResetTimeScale()
    {
        Time.timeScale = 1.0f;
    }
}
