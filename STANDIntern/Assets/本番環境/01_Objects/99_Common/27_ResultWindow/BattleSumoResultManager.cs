using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleSumoResultManager : MonoBehaviour
{
    [System.Serializable]
    public class ResultPlayerInfo
    {
        public int playerId;
        public int Rank;
        public int Point;

        public ResultPlayerInfo(int id, int point)
        {
            playerId = id;
            Rank = 1;
            Point = point;
        }
    }

    [SerializeField]
    private Transform podiumParent;                     // �\����̐e�I�u�W�F�N�g
    [SerializeField]
    public Sprite[] PadiumSpriteSample;                 // �\���̃X�v���C�g
    [SerializeField]
    private Transform players;

    // ���ʂɉ������\���̃T�C�Y
    private static readonly float[] podiumHeightSize = new float[DeviceManager.DeviceNum] { 550.0f, 450.0f, 350.0f, 250.0f };
    
    private ResultPlayerInfo[] resultInfo = new ResultPlayerInfo[DeviceManager.DeviceNum];  // ���U���g�p���
    private List<int> rankOrderIndex = new List<int>(); // ���ʏ��Ƀv���C���[ID�̃��X�g

    private bool isStartTransition = false;             // ���o���I�������
    private float transitionTimeCount = 3.0f;           // ���o���I����Ă����ʂ�߂�܂ł̎���

    private int podiumEffectPlayCount = 4;              // �G�t�F�N�g�Đ��p�J�E���g
    private int showPlayerCount = 0;                    // �v���C���[�\���p�J�E���g

    void Start()
    {
        //=====���U���g�p���̐���
        int playerNum = 0;

        for (int i = 0; i < DeviceManager.DeviceNum; i++)
        {
            if (!DeviceManager.Instance.GetIsConnect(i))
            {
                podiumParent.GetChild(i).gameObject.SetActive(false);
                resultInfo[i] = new ResultPlayerInfo(i, 0);
            }
            else
            {
                resultInfo[i] = new ResultPlayerInfo(i, players.GetChild(i).GetComponent<PlayerBattleSumoPoint>().Point);
                playerNum++;
            }
        }

        InfoSort(playerNum);
        SetRank();
        SetPodium();
    }

    void Update()
    {
        if (isStartTransition)
        {
            transitionTimeCount -= Time.deltaTime;
            if (transitionTimeCount <= 0.0f)
            {
                GetComponent<SceneChange>().StartSceneChange();
            }
        }
    }

    /// <summary>
    /// �|�C���g�̑������ɕ��ѕς���
    /// </summary>
    /// <param name="playerNum"> �Q���v���C���[�� </param>
    private void InfoSort(int playerNum)
    {
        List<ResultPlayerInfo> tempInfo = new List<ResultPlayerInfo>(resultInfo);

        //=====�Z���N�g�\�[�g�ŕ��ѕς���
        for (int i = 0; i < playerNum; i++)
        {
            int maxElemIndex = 0;
            for (int j = 1; j < DeviceManager.DeviceNum - i; j++)
            {
                if (tempInfo[maxElemIndex].Point < tempInfo[j].Point)
                {
                    maxElemIndex = j;
                }
            }

            rankOrderIndex.Add(maxElemIndex);
            tempInfo.RemoveAt(maxElemIndex);
        }
    }

    /// <summary>
    /// ���ʕt��
    /// </summary>
    private void SetRank()
    {
        int rank = 1;
        for (int i = 1; i < rankOrderIndex.Count; i++)
        {
            if (resultInfo[rankOrderIndex[i - 1]].Point > resultInfo[rankOrderIndex[i]].Point)
            {
                rank = i + 1;
            }

            resultInfo[rankOrderIndex[i]].Rank = rank;
        }
    }

    /// <summary>
    /// �\����̐ݒ�
    /// </summary>
    private void SetPodium()
    {
        for (int i = 0; i < rankOrderIndex.Count; i++)
        {
            int index = rankOrderIndex[i];
            Transform podiumTrans = podiumParent.GetChild(i);
            RectTransform podiumImageTrans = podiumTrans.GetChild(1).GetComponent<RectTransform>();

            // ���ʂɂ���ĕ\����̃T�C�Y���Ⴄ�̂ō������l�ɕύX����
            Sprite podiumSprite = PadiumSpriteSample[resultInfo[index].Rank - 1];
            Vector2 size = podiumImageTrans.sizeDelta;
            size.y = podiumHeightSize[resultInfo[index].Rank - 1];
            podiumImageTrans.sizeDelta = size;


            // ���ʂɂ�����\����̉摜��ݒ肷��
            Sprite playerFaceSprite;
            PlayerSkinInfo skinInfo = players.GetChild(resultInfo[index].playerId).GetComponent<PlayerFace>().GetSkinInfo();
            switch (resultInfo[index].Rank)
            {
                case 1:
                    playerFaceSprite = skinInfo.No1;
                    break;
                case 2:
                    playerFaceSprite = skinInfo.No2;
                    break;
                case 3:
                    playerFaceSprite = skinInfo.No3;
                    break;
                case 4:
                    playerFaceSprite = skinInfo.No4;
                    break;
                default:
                    playerFaceSprite = null;
                    break;
            }

            // �摜�𔽉f
            podiumTrans.GetChild(0).GetComponent<Image>().sprite = playerFaceSprite;
            podiumTrans.GetChild(1).GetComponent<Image>().sprite = podiumSprite;

            podiumTrans.GetChild(2).GetComponent<TextMeshProUGUI>().text = resultInfo[index].Point.ToString() + "p";
            podiumTrans.GetChild(3).GetComponent<TextMeshProUGUI>().text = resultInfo[index].Point.ToString() + "p";
            CheckPointTextColor(podiumTrans.GetChild(3).GetComponent<TextMeshProUGUI>(), resultInfo[index].Point);
        }
    }

    /// <summary>
    /// �������w�i���~�肽�ۂ̉��o���Đ�
    /// </summary>
    public void PlayCoverStaging()
    {
        // �G�t�F�N�g�Đ�
        EffectContainer.Instance.PlayEffect("���G�t�F�N�g(���U���g)", transform.GetChild(0));

        // ���ʉ��Đ�
        AudioManager.Instance.PlaySe("���U���g�䗎��");
    }

    /// <summary>
    /// �\���䂪�~�肽�ۂ̉��o���Đ�
    /// </summary>
    public void PodiumSmokeEffect()
    {
        Transform trans = podiumParent.GetChild(podiumEffectPlayCount - 1);

        if (trans.gameObject.activeSelf)
        {
            // �G�t�F�N�g�Đ�
            EffectContainer.Instance.PlayEffect("�\����G�t�F�N�g(���U���g)", trans.GetChild(1));

            // ���ʉ��Đ�
            AudioManager.Instance.PlaySe("���U���g�䗎��");
        }
        podiumEffectPlayCount--;
    }

    /// <summary>
    /// �ŏ��̃v���C���[��\���䂩��o�ꂳ����
    /// </summary>
    public void StartPlayerShow()
    {
        int index = rankOrderIndex.Count - 1;

        // �ŏ��̃v���C���[�̉��o���J�n
        podiumParent.GetChild(index).GetComponent<Animator>().SetInteger("Rank", resultInfo[rankOrderIndex[index]].Rank);
        PlayDonSe(index);
        showPlayerCount++;
    }

    /// <summary>
    /// �ŏ��̃v���C���[��\���䂩��o�ꂳ����
    /// </summary>
    public void NextPlayerShow()
    {
        if (rankOrderIndex.Count > showPlayerCount)
        {
            int index = rankOrderIndex.Count - 1 - showPlayerCount;

            // ���̃v���C���[�̉��o���J�n
            podiumParent.GetChild(index).GetComponent<Animator>().
                SetInteger("Rank", resultInfo[rankOrderIndex[index]].Rank);
            PlayDonSe(index);
            showPlayerCount++;
        }
        else
        {
            isStartTransition = true;
        }
    }

    /// <summary>
    /// �v���C���[�o�ꎞ���ʉ����Đ�
    /// </summary>
    /// <param name="index"> �o�ꏇ�ԍ� </param>
    public void PlayDonSe(int index)
    {
        switch (resultInfo[rankOrderIndex[index]].Rank)
        {
            case 1:
                AudioManager.Instance.PlaySe("���U���g1��");
                break;
            case 2:
                AudioManager.Instance.PlaySe("���U���g2��");
                break;
            case 3:
                AudioManager.Instance.PlaySe("���U���g3��");
                break;
            case 4:
                AudioManager.Instance.PlaySe("���U���g4��");
                break;
        }
    }

    /// <summary>
    /// �|�C���g�\���e�L�X�g�̐F���w��
    /// </summary>
    private void CheckPointTextColor(TextMeshProUGUI tmp, int point)
    {
        if (point > 0)
        {
            tmp.fontMaterial.SetColor("_FaceColor", new Color(1.0f, 0.5f, 0.5f, 1.0f));
        }
        else if (point == 0)
        {
            tmp.fontMaterial.SetColor("_FaceColor", new Color(1.0f, 1.0f, 1.0f, 1.0f));
        }
        else
        {
            tmp.fontMaterial.SetColor("_FaceColor", new Color(0.5f, 0.5f, 1.0f, 1.0f));
        }
    }
}
