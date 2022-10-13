using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    public enum MovePatternId
    {
        RoundTrip_Fixed = 0,  // 往復_一定速
        Crawl_Fixed,          // 巡回_一定速
        RaundTrip_Tempo,      // 往復_緩急
        Crawl_Tempo,          // 巡回_緩急
        Length,
    }

    private bool isMove = false;
    [SerializeField]
    private float moveSpeed = 0.0f;
    private Transform[] movePoint;
    [SerializeField]
    private int startPoint;         // 開始地点
    private int nextPoint;          // 次のポイント
    
    private System.Action[] MoveAction;
    private MovePatternId routePattern = MovePatternId.RoundTrip_Fixed;

    // 往復時のみ使用
    [SerializeField]
    private bool isReturn = false;

    // 緩急時のみ有効
    private float lengthAll = 0.0f; // 
    private float nowLength = 0.0f;
    private float timeCount = 0.0f;

    void Start()
    {
        InitStartPoint();
        InitIsReturn();
        InitLengthAll();

        MoveAction = new System.Action[(int)MovePatternId.Length]
            { RoundTripFixed_Action, CrawlFixed_Action, RoundTripTempo_Action, CrawlTempo_Action };
    }
    
    void InitStartPoint()
    {
        // 値が不正でないか確認
        if (startPoint < 0)
        {
            startPoint = 0;
        }
        else if (startPoint >= movePoint.Length)
        {
            startPoint = movePoint.Length - 1;
        }

        // 開始地点に座標を初期化
        transform.position = movePoint[startPoint].transform.position;
    }

    void InitIsReturn()
    {
        // スタート地点が終端だった場合フラグを変更する
        if (startPoint == movePoint.Length - 1)
        {
            isReturn = true;
        }
    }

    void InitLengthAll()
    {
        // 各地点間の総距離を計算
        for (int i = 0; i < movePoint.Length - 1; i++)
        {
            lengthAll += (movePoint[i].position - movePoint[i + 1].position).magnitude;
        }
    }

    void Update()
    {
        if (isMove)
        {
            MoveAction[(int)routePattern]();
        }
    }

    private float CalcMoveLength_Fixed()
    {
        float moveLength = Mathf.Abs(Time.deltaTime * moveSpeed);

        return moveLength;
    }

    private float CalcMoveLength_Tempo()
    {
        timeCount += Time.deltaTime * moveSpeed;
        float nextLength = (Mathf.Sin(timeCount) + 1.0f) * lengthAll / 2;
        float moveLength = Mathf.Abs(nowLength - nextLength);

        return moveLength;
    }

    // 往復_一定速
    void RoundTripFixed_Action()
    {
        float moveLength = CalcMoveLength_Fixed();

        while (moveLength > 0.0f)
        {
            if (moveLength <= Vector2.Distance(transform.position, movePoint[nextPoint].transform.position))
            {
                transform.position = Vector3.MoveTowards(transform.position, movePoint[nextPoint].transform.position, moveLength);
                moveLength = 0.0f;
            }
            else
            {
                transform.position = movePoint[nextPoint].transform.position;
                moveLength -= Vector3.Distance(transform.position, movePoint[nextPoint].transform.position);

                if (nextPoint == 0)
                {
                    isReturn = false;
                    nowLength = 0.0f;
                }
                else if (nextPoint == movePoint.Length - 1)
                {
                    isReturn = true;
                    nowLength = lengthAll;
                }

                nextPoint += isReturn ? -1 : 1;
            }
        }
    }

    // 巡回_一定速
    void CrawlFixed_Action()
    {
        float moveLength = CalcMoveLength_Fixed();

        while (moveLength > 0.0f)
        {
            if (moveLength <= Vector2.Distance(transform.position, movePoint[nextPoint].transform.position))
            {
                transform.position = Vector3.MoveTowards(transform.position, movePoint[nextPoint].transform.position, moveLength);
                moveLength = 0.0f;
            }
            else
            {
                transform.position = movePoint[nextPoint].transform.position;
                moveLength -= Vector3.Distance(transform.position, movePoint[nextPoint].transform.position);

                if (nextPoint == 0)
                {
                    isReturn = false;
                    nowLength = 0.0f;
                }
                else if (nextPoint == movePoint.Length - 1)
                {
                    isReturn = true;
                    nowLength = lengthAll;
                }

                nextPoint += isReturn ? -1 : 1;
            }
        }
    }

    // 往復_緩急
    void RoundTripTempo_Action()
    {
        float moveLength = CalcMoveLength_Tempo();
    }

    // 巡回_緩急
    void CrawlTempo_Action()
    {
        float moveLength = CalcMoveLength_Tempo();
    }
}
