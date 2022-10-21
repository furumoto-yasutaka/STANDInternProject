using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleSumoManager : MonoBehaviour
{
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

    private int[] points;
    private bool[] isMark;
    private GameObject[] markPlayer;
    private float[] markTimeCount;
    private TextMeshProUGUI[] debugText;

    void Start()
    {
        points = new int[players.childCount];
        isMark = new bool[players.childCount];
        markPlayer = new GameObject[players.childCount];
        markTimeCount = new float[players.childCount];
        debugText = new TextMeshProUGUI[players.childCount];

        for (int i = 0; i < players.childCount; i++)
        {
            points[i] = 0;
            isMark[i] = false;
            markPlayer[i] = null;
            markTimeCount[i] = 0;
            debugText[i] = debugTextParent.GetChild(i).GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        for (int i = 0; i < players.childCount; i++)
        {
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
            debugText[markIndex].text = (markIndex + 1) + "P:" + points[markIndex];
        }
        debugText[index].text = (index + 1) + "P:" + points[index];
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
}
