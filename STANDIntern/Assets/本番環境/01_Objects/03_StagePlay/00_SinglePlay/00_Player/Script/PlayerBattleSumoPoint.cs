using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerBattleSumoPoint : MonoBehaviour
{
    [SerializeField]
    private Transform pointTextParent;

    private PlayerInfo playerInfo;
    private int point = 0;
    private bool isMark = false;
    private PlayerBattleSumoPoint markPlayer = null;
    private float markTimeCount = 0.0f;
    private Transform pointTextTrans;
    private TextMeshProUGUI pointText;
    private TextMeshProUGUI pointFrontText;

    private static float fromKickMarkTime = 6.0f;
    private static float fromContactMarkTime = 3.0f;
    private static float contactMarkThreshold = 1.0f;


    public int Point { get { return point; } }


    void Start()
    {
        playerInfo = GetComponent<PlayerInfo>();
        pointTextTrans = pointTextParent.GetChild(playerInfo.Id);
        pointText = pointTextTrans.GetChild(1).GetComponent<TextMeshProUGUI>();
        pointFrontText = pointTextTrans.GetChild(2).GetComponent<TextMeshProUGUI>();

        pointTextTrans.GetChild(3).GetComponent<Image>().sprite =
            playerInfo.SkinInfo.Normal;
    }

    void Update()
    {
        if (isMark)
        {
            if (markTimeCount <= 0.0f)
            {
                markTimeCount = 0.0f;
                isMark = false;
                markPlayer = null;
            }
            else
            {
                markTimeCount -= Time.deltaTime;
            }
        }
    }

    public void AddPoint(int value)
    {
        point += value;
    }

    public void SubPoint(int value)
    {
        point -= value;
    }

    public void CalcPoint_Death()
    {
        SubPoint(1);

        if (isMark)
        {
            markPlayer.AddPoint(1);

            EffectContainer.Instance.PlayEffect("ƒLƒ‹", markPlayer.transform.GetChild(0));
            markPlayer.GetComponent<PlayerFace>().SetState((int)PlayerFace.FaceState.Kill, 2.0f);
            markPlayer.SetPointText();
        }

        SetPointText();
    }

    private void CheckPointTextColor()
    {
        if (point > 0)
        {
            pointFrontText.fontMaterial.SetColor("_FaceColor", new Color(1.0f, 0.5f, 0.5f, 1.0f));
        }
        else if (point == 0)
        {
            pointFrontText.fontMaterial.SetColor("_FaceColor", new Color(1.0f, 1.0f, 1.0f, 1.0f));
        }
        else
        {
            pointFrontText.fontMaterial.SetColor("_FaceColor", new Color(0.5f, 0.5f, 1.0f, 1.0f));
        }
    }

    public void SetPointText()
    {
        string text = point.ToString() + "p";

        pointText.text = text;
        pointFrontText.text = text;
        CheckPointTextColor();
    }

    public void RequestKickMark(PlayerBattleSumoPoint target)
    {
        isMark = true;
        markPlayer = target;
        markTimeCount = fromKickMarkTime;
    }

    public void RequestContactMark(PlayerBattleSumoPoint target, Vector2 vel)
    {
        if (vel.sqrMagnitude >= contactMarkThreshold * contactMarkThreshold &&
            markTimeCount <= 3.0f)
        {
            isMark = true;
            markPlayer = target;
            markTimeCount = fromContactMarkTime;
        }
    }

    public void RequestDeleteMark()
    {
        if (markTimeCount <= 3.0f)
        {
            isMark = false;
            markPlayer = null;
            markTimeCount = 0.0f;
        }
    }
}
