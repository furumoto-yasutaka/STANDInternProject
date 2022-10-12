using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    public enum MovePatternId
    {
        RoundTrip_Fixed = 0,  // ����_��葬
        Crawl_Fixed,          // ����_��葬
        RaundTrip_Tempo,      // ����_�ɋ}
        Crawl_Tempo,          // ����_�ɋ}
        Length,
    }

    private bool isMove = false;
    [SerializeField]
    private float moveSpeed = 0.0f;
    private Transform[] movePoint;
    [SerializeField]
    private int startPoint;         // �J�n�n�_
    private int nextPoint;          // ���̃|�C���g
    
    private System.Action[] MoveAction;
    private MovePatternId routePattern = MovePatternId.RoundTrip_Fixed;

    // �������̂ݎg�p
    [SerializeField]
    private bool isReturn = false;

    // �ɋ}���̂ݗL��
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
        // �l���s���łȂ����m�F
        if (startPoint < 0)
        {
            startPoint = 0;
        }
        else if (startPoint >= movePoint.Length)
        {
            startPoint = movePoint.Length - 1;
        }

        // �J�n�n�_�ɍ��W��������
        transform.position = movePoint[startPoint].transform.position;
    }

    void InitIsReturn()
    {
        // �X�^�[�g�n�_���I�[�������ꍇ�t���O��ύX����
        if (startPoint == movePoint.Length - 1)
        {
            isReturn = true;
        }
    }

    void InitLengthAll()
    {
        // �e�n�_�Ԃ̑��������v�Z
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

    // ����_��葬
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

    // ����_��葬
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

    // ����_�ɋ}
    void RoundTripTempo_Action()
    {
        float moveLength = CalcMoveLength_Tempo();
    }

    // ����_�ɋ}
    void CrawlTempo_Action()
    {
        float moveLength = CalcMoveLength_Tempo();
    }
}
