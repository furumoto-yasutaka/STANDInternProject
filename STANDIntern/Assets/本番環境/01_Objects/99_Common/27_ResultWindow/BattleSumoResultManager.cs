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
    private Transform podiumParent;                     // 表彰台の親オブジェクト
    [SerializeField]
    public Sprite[] PadiumSpriteSample;                 // 表情台のスプライト
    [SerializeField]
    private Transform players;

    // 順位に応じた表情台のサイズ
    private static readonly float[] podiumHeightSize = new float[DeviceManager.DeviceNum] { 550.0f, 450.0f, 350.0f, 250.0f };
    
    private ResultPlayerInfo[] resultInfo = new ResultPlayerInfo[DeviceManager.DeviceNum];  // リザルト用情報
    private List<int> rankOrderIndex = new List<int>(); // 順位順にプレイヤーIDのリスト

    private bool isStartTransition = false;             // 演出が終わったか
    private float transitionTimeCount = 3.0f;           // 演出が終わってから画面を戻るまでの時間

    private int podiumEffectPlayCount = 4;              // エフェクト再生用カウント
    private int showPlayerCount = 0;                    // プレイヤー表示用カウント

    void Start()
    {
        //=====リザルト用情報の生成
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
    /// ポイントの多い順に並び変える
    /// </summary>
    /// <param name="playerNum"> 参加プレイヤー数 </param>
    private void InfoSort(int playerNum)
    {
        List<ResultPlayerInfo> tempInfo = new List<ResultPlayerInfo>(resultInfo);

        //=====セレクトソートで並び変える
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
    /// 順位付け
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
    /// 表彰台の設定
    /// </summary>
    private void SetPodium()
    {
        for (int i = 0; i < rankOrderIndex.Count; i++)
        {
            int index = rankOrderIndex[i];
            Transform podiumTrans = podiumParent.GetChild(i);
            RectTransform podiumImageTrans = podiumTrans.GetChild(1).GetComponent<RectTransform>();

            // 順位によって表彰台のサイズが違うので合った値に変更する
            Sprite podiumSprite = PadiumSpriteSample[resultInfo[index].Rank - 1];
            Vector2 size = podiumImageTrans.sizeDelta;
            size.y = podiumHeightSize[resultInfo[index].Rank - 1];
            podiumImageTrans.sizeDelta = size;


            // 順位によった表彰台の画像を設定する
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

            // 画像を反映
            podiumTrans.GetChild(0).GetComponent<Image>().sprite = playerFaceSprite;
            podiumTrans.GetChild(1).GetComponent<Image>().sprite = podiumSprite;

            podiumTrans.GetChild(2).GetComponent<TextMeshProUGUI>().text = resultInfo[index].Point.ToString() + "p";
            podiumTrans.GetChild(3).GetComponent<TextMeshProUGUI>().text = resultInfo[index].Point.ToString() + "p";
            CheckPointTextColor(podiumTrans.GetChild(3).GetComponent<TextMeshProUGUI>(), resultInfo[index].Point);
        }
    }

    /// <summary>
    /// 透明黒背景が降りた際の演出を再生
    /// </summary>
    public void PlayCoverStaging()
    {
        // エフェクト再生
        EffectContainer.Instance.PlayEffect("幕エフェクト(リザルト)", transform.GetChild(0));

        // 効果音再生
        AudioManager.Instance.PlaySe("リザルト台落下");
    }

    /// <summary>
    /// 表彰台が降りた際の演出を再生
    /// </summary>
    public void PodiumSmokeEffect()
    {
        Transform trans = podiumParent.GetChild(podiumEffectPlayCount - 1);

        if (trans.gameObject.activeSelf)
        {
            // エフェクト再生
            EffectContainer.Instance.PlayEffect("表彰台エフェクト(リザルト)", trans.GetChild(1));

            // 効果音再生
            AudioManager.Instance.PlaySe("リザルト台落下");
        }
        podiumEffectPlayCount--;
    }

    /// <summary>
    /// 最初のプレイヤーを表彰台から登場させる
    /// </summary>
    public void StartPlayerShow()
    {
        int index = rankOrderIndex.Count - 1;

        // 最初のプレイヤーの演出を開始
        podiumParent.GetChild(index).GetComponent<Animator>().SetInteger("Rank", resultInfo[rankOrderIndex[index]].Rank);
        PlayDonSe(index);
        showPlayerCount++;
    }

    /// <summary>
    /// 最初のプレイヤーを表彰台から登場させる
    /// </summary>
    public void NextPlayerShow()
    {
        if (rankOrderIndex.Count > showPlayerCount)
        {
            int index = rankOrderIndex.Count - 1 - showPlayerCount;

            // 次のプレイヤーの演出を開始
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
    /// プレイヤー登場時効果音を再生
    /// </summary>
    /// <param name="index"> 登場順番号 </param>
    public void PlayDonSe(int index)
    {
        switch (resultInfo[rankOrderIndex[index]].Rank)
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

    /// <summary>
    /// ポイント表示テキストの色を指定
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
