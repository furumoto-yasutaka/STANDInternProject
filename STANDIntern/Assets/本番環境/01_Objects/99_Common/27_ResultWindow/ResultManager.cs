using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [System.Serializable]
    public class ResultPlayerInfo
    {
        public int playerId;
        public int Rank;
        public int Point;
        public Sprite PlayerFace = null;
        public RectTransform PadiumTransform;
        public Sprite PadiumSprite = null;

        public ResultPlayerInfo(int id, int v, int point)
        {
            playerId = id;
            Rank = v;
            Point = point;
        }
    }

    private int podiumEffectPlayCount = 4;
    private float[] podiumHeightSize;
    private BattleSumoManager battleSumoManager;
    private PlayerController[] player;

    private int showPlayerCount = 0;

    [SerializeField]
    public Sprite[] PadiumSpriteSumple;
    [SerializeField]
    private List<ResultPlayerInfo> resultInfo = new List<ResultPlayerInfo>();

    // 演出が終わってから画面を戻るまでの時間
    private float TransitionTime = 3.0f;
    private float TransitionTimeCount = 0.0f;

    void Start()
    {
        List<ResultPlayerInfo> tempInfos = new List<ResultPlayerInfo>();

        podiumHeightSize = new float[4]{ 550.0f, 450.0f, 350.0f, 250.0f };

        int index = (int)BattleSumoModeManagerList.BattleSumoModeManagerId.BattleSumoManager;
        battleSumoManager = GameObject.FindGameObjectWithTag("Managers").transform.GetChild(index).GetComponent<BattleSumoManager>();

        player = battleSumoManager.Player;

        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            if (!player[i].gameObject.activeSelf)
            {
                transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                tempInfos.Add(new ResultPlayerInfo(i, 1, battleSumoManager.Points[i]));
            }
        }

        while (tempInfos.Count != 0)
        {
            ResultPlayerInfo temp = tempInfos[0];
            for (int i = 1; i < tempInfos.Count; i++)
            {
                if (temp.Point < tempInfos[i].Point)
                {
                    temp = tempInfos[i];
                }
            }

            tempInfos.Remove(temp);
            resultInfo.Add(temp);
        }

        int rank = 1;
        for (int i = 1; i < resultInfo.Count; i++)
        {
            if (resultInfo[i - 1].Point > resultInfo[i].Point)
            {
                rank = i + 1;
                resultInfo[i].Rank = rank;
            }
            else
            {
                resultInfo[i].Rank = rank;
            }
        }

        

        for (int i = 0; i < resultInfo.Count; i++)
        {
            resultInfo[i].PadiumTransform = transform.GetChild(1).GetChild(i).GetChild(1).GetComponent<RectTransform>();

            resultInfo[i].PadiumSprite = PadiumSpriteSumple[resultInfo[i].Rank - 1];
            Vector2 size = resultInfo[i].PadiumTransform.sizeDelta;
            size.y = podiumHeightSize[resultInfo[i].Rank - 1];
            resultInfo[i].PadiumTransform.sizeDelta = size;

            switch (resultInfo[i].Rank)
            {
                case 1:
                    resultInfo[i].PlayerFace = player[resultInfo[i].playerId].GetComponent<PlayerFace>().SkinInfo.No1;
                    break;
                case 2:
                    resultInfo[i].PlayerFace = player[resultInfo[i].playerId].GetComponent<PlayerFace>().SkinInfo.No2;
                    break;
                case 3:
                    resultInfo[i].PlayerFace = player[resultInfo[i].playerId].GetComponent<PlayerFace>().SkinInfo.No3;
                    break;
                case 4:
                    resultInfo[i].PlayerFace = player[resultInfo[i].playerId].GetComponent<PlayerFace>().SkinInfo.No4;
                    break;
            }
            transform.GetChild(1).GetChild(i).GetChild(0).GetComponent<Image>().sprite = resultInfo[i].PlayerFace;
            transform.GetChild(1).GetChild(i).GetChild(1).GetComponent<Image>().sprite = resultInfo[i].PadiumSprite;

            transform.GetChild(1).GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>().text = resultInfo[i].Point.ToString() + "p";
            transform.GetChild(1).GetChild(i).GetChild(3).GetComponent<TextMeshProUGUI>().text = resultInfo[i].Point.ToString() + "p";
            CheckPointTextColor(transform.GetChild(1).GetChild(i).GetChild(3).GetComponent<TextMeshProUGUI>(), resultInfo[i].Point);
        }
    }

    void Update()
    {
        if (TransitionTimeCount > 0.0f)
        {
            TransitionTimeCount -= Time.deltaTime;
            if (TransitionTimeCount <= 0.0f)
            {
                GetComponent<SceneChange>().StartSceneChange();
            }
        }
    }

    public void CoverSmoveEffect()
    {
        EffectContainer.Instance.PlayEffect("幕エフェクト(リザルト)", transform.GetChild(0));
        AudioManager.Instance.PlaySe("リザルト台落下");
    }

    public void PodiumSmoveEffect()
    {
        Transform trans = transform.GetChild(1).GetChild(podiumEffectPlayCount - 1);
        if (trans.gameObject.activeSelf)
        {
            EffectContainer.Instance.PlayEffect("表彰台エフェクト(リザルト)", trans.GetChild(1));
            AudioManager.Instance.PlaySe("リザルト台落下");
        }
        podiumEffectPlayCount--;
    }

    public void StartPlayerShow()
    {
        // 最初のプレイヤーの演出を開始
        transform.GetChild(1).GetChild(resultInfo.Count - 1).GetComponent<Animator>().SetInteger("Rank", resultInfo[resultInfo.Count - 1].Rank);
        PlayDonSe(resultInfo.Count - 1);
        showPlayerCount++;
    }

    public void NextPlayerShow()
    {
        if (resultInfo.Count > showPlayerCount)
        {
            // 次のプレイヤーの演出を開始
            transform.GetChild(1).GetChild(resultInfo.Count - 1 - showPlayerCount).GetComponent<Animator>().
                SetInteger("Rank", resultInfo[resultInfo.Count - 1 - showPlayerCount].Rank);
            PlayDonSe(resultInfo.Count - 1 - showPlayerCount);
            showPlayerCount++;
        }
        else
        {
            TransitionTimeCount = TransitionTime;
        }
    }

    public void PlayDonSe(int index)
    {
        switch (resultInfo[index].Rank)
        {
            case 1:
                AudioManager.Instance.PlaySe("リザルト1位");
                break;
            case 2:
                AudioManager.Instance.PlaySe("リザルト2位");
                break;
            case 3:
                AudioManager.Instance.PlaySe("リザルト3位");
                break;
            case 4:
                AudioManager.Instance.PlaySe("リザルト4位");
                break;
        }
    }

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
