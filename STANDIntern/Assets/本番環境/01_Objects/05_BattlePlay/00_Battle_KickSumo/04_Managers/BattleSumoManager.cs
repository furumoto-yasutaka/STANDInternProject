using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleSumoManager : MonoBehaviour
{
    public static bool[] IsPlayerJoin;
    public static int[] IsPlayerSkinId;

    [SerializeField]
    private int stageId;
    [SerializeField]
    private Transform players;
    [SerializeField]
    private float fromKickMarkTime = 6.0f;
    [SerializeField]
    private float fromContactMarkTime = 3.0f;
    [SerializeField]
    private float contactMarkThreshold = 1.0f;
    [SerializeField]
    private Transform debugTextParent;
    [SerializeField]
    private Transform pointTextParent;

    public static int JoinPlayerCount = 0;
    private PlayerController[] player;
    [SerializeField]
    private int[] points;
    [SerializeField]
    private bool[] isMark;
    [SerializeField]
    private GameObject[] markPlayer;
    private float[] markTimeCount;
    private TextMeshProUGUI[] debugText;
    private TextMeshProUGUI[] pointText;
    private TextMeshProUGUI[] pointFrontText;

    public int StageId { get { return stageId; } }
    public PlayerController[] Player { get { return player; } }
    public int[] Points { get { return points; } }

    static BattleSumoManager()
    {
        IsPlayerJoin = new bool[DeviceManager.deviceNum] { false, false, false, false };
        IsPlayerSkinId = new int[DeviceManager.deviceNum] { 0, 0, 0, 0 };
    }

    public static void SetIsPlayerJoin(bool value, int index)
    {
        if (!IsPlayerJoin[index] && value)
        {
            JoinPlayerCount++;
        }
        else if (IsPlayerJoin[index] && !value)
        {
            JoinPlayerCount--;
        }

        IsPlayerJoin[index] = value;
    }

    public static void SetIsPlayerSkinId(int value, int index)
    {
        IsPlayerSkinId[index] = value;
    }

    void Awake()
    {
        player = new PlayerController[players.childCount];
        points = new int[players.childCount];
        isMark = new bool[players.childCount];
        markPlayer = new GameObject[players.childCount];
        markTimeCount = new float[players.childCount];
        debugText = new TextMeshProUGUI[players.childCount];
        pointText = new TextMeshProUGUI[players.childCount];
        pointFrontText = new TextMeshProUGUI[players.childCount];

        for (int i = 0; i < players.childCount; i++)
        {
            player[i] = players.GetChild(i).GetComponent<PlayerController>();
            points[i] = 0;
            isMark[i] = false;
            markPlayer[i] = null;
            markTimeCount[i] = 0;
            debugText[i] = debugTextParent.GetChild(i).GetComponent<TextMeshProUGUI>();
            pointText[i] = pointTextParent.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>();
            pointFrontText[i] = pointTextParent.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>();

            pointTextParent.GetChild(i).GetChild(3).GetComponent<Image>().sprite =
                player[i].GetComponent<PlayerFaceManager>().PlayerSkinDataBase.PlayerSkinInfos[IsPlayerSkinId[i]].Normal;
        }
    }

    void Update()
    {
        for (int i = 0; i < players.childCount; i++)
        {
            if (!IsPlayerJoin[i]) { continue; }

            if (isMark[i])
            {
                if (markTimeCount[i] <= 0.0f)
                {
                    isMark[i] = false;
                    markPlayer[i] = null;
                    markTimeCount[i] = 0.0f;
                }
                else
                {
                    markTimeCount[i] -= Time.deltaTime;
                }
            }
        }
    }

    public void AddPoint(int index, int point)
    {
        points[index] += point;
    }

    public void SubPoint(int index, int point)
    {
        points[index] -= point;
    }

    public void CalcPoint_DeathPlayer(int index)
    {
        SubPoint(index, 1);
        if (isMark[index])
        {
            int markIndex = markPlayer[index].GetComponent<PlayerId>().Id;
            AddPoint(markIndex, 1);
            EffectContainer.Instance.PlayEffect("ƒLƒ‹", player[markIndex].transform.GetChild(0));
            player[markIndex].GetComponent<PlayerFaceManager>().ChangeState((int)PlayerFaceManager.FaceState.Kill, 2.0f);
            debugText[markIndex].text = (markIndex + 1).ToString() + "P:" + points[markIndex];

            pointText[markIndex].text = points[markIndex].ToString() + "p";
            pointFrontText[markIndex].text = points[markIndex].ToString() + "p";
            CheckPointTextColor(pointFrontText[markIndex], points[markIndex]);
        }
        debugText[index].text = (index + 1).ToString() + "P:" + points[index];

        pointText[index].text = points[index].ToString() + "p";
        pointFrontText[index].text = points[index].ToString() + "p";
        CheckPointTextColor(pointFrontText[index], points[index]);
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

    public void RequestKickMark(int toIndex, int fromIndex)
    {
        isMark[toIndex] = true;
        markPlayer[toIndex] = players.GetChild(fromIndex).gameObject;
        markTimeCount[toIndex] = fromKickMarkTime;
    }

    public void RequestContactMark(int toIndex, int fromIndex, Vector2 vel)
    {
        if (vel.sqrMagnitude >= contactMarkThreshold * contactMarkThreshold &&
            markTimeCount[toIndex] <= 3.0f)
        {
            isMark[toIndex] = true;
            markPlayer[toIndex] = players.GetChild(fromIndex).gameObject;
            markTimeCount[toIndex] = fromContactMarkTime;
        }
    }

    public void RequestDeleteMark(int index)
    {
        if (markTimeCount[index] <= 3.0f)
        {
            isMark[index] = false;
            markPlayer[index] = null;
            markTimeCount[index] = 0.0f;
        }
    }

    public void PlayerStop()
    {
        for (int i = 0; i < player.Length; i++)
        {
            player[i].Stop();
        }
    }
}
