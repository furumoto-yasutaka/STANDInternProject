using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpownTruckManager : MonoBehaviour
{
    public class TruckInfo
    {
        public bool IsPutOn;
        public bool IsJump;
        public int revivalPlayerIndex;
        public Transform truck;
        public float JumpTimeCount;
        public List<JumpInfo> JumpParam;

        public TruckInfo(Transform truck, int revivalPlayerIndex)
        {
            IsPutOn = true;
            IsJump = false;
            this.revivalPlayerIndex = revivalPlayerIndex;
            this.truck = truck;
            JumpTimeCount = 0.0f;
            JumpParam = new List<JumpInfo>();
        }
    }

    public class JumpInfo
    {
        public Vector2 Start;
        public Vector2 End;
        public Vector2 Half;

        public JumpInfo()
        {
            Start = Vector2.zero;
            End = Vector2.zero;
            Half = Vector2.zero;
        }
    }

    [SerializeField]
    private GameObject firstTruckPrefab;
    [SerializeField]
    private int firstSpownSortLayer;
    [SerializeField]
    private GameObject revivalTruckPrefab;
    [SerializeField]
    private int revivalSpownSortLayer;
    [SerializeField]
    private float moveSpeed = 1.0f;
    [SerializeField]
    private Transform players;
    [SerializeField]
    private int defaultSortLayer;
    [SerializeField]
    private float jumpTime;
    [SerializeField]
    private BattleFirstPositionDataBase firstPositionDataBase;
    
    private Vector3 firstPos;
    private Vector3 targetPos;
    private Vector3 moveAngle;
    private List<TruckInfo> truckList = new List<TruckInfo>();
    private List<TruckInfo> removeList = new List<TruckInfo>();

    void Start()
    {
        firstPos = transform.GetChild(0).position;
        targetPos = transform.GetChild(1).position;
        moveAngle = (targetPos - firstPos).normalized;

        Spown(-1);
    }

    void Update()
    {
        for (int i = 0; i < truckList.Count; i++)
        {
            DriveAction(truckList[i]);

            if (truckList[i].IsJump)
            {
                JumpAction(truckList[i]);
            }
        }

        for (int i = 0; i < removeList.Count; i++)
        {
            truckList.Remove(removeList[i]);
        }
        removeList.Clear();
    }

    private void DriveAction(TruckInfo info)
    {
        if (info.IsPutOn)
        {
            float toHalfDistance = ((firstPos + targetPos) * 0.5f - info.truck.position).sqrMagnitude;
            
            if (toHalfDistance <= (moveSpeed * Time.deltaTime) * (moveSpeed * Time.deltaTime))
            {
                info.truck.position += moveAngle * Mathf.Sqrt(toHalfDistance);
                StartJump(info);
                info.IsPutOn = false;
            }
            else
            {
                info.truck.position += moveAngle * moveSpeed * Time.deltaTime;
                if (info.JumpParam.Count > 1)   { AllPutOnMove(info); }
                else                            { PutOnMove(info); }
            }
        }
        else
        {
            float toEndDistance = (targetPos - info.truck.position).sqrMagnitude;

            if (toEndDistance <= (moveSpeed * Time.deltaTime) * (moveSpeed * Time.deltaTime))
            {
                info.truck.position = targetPos;
                Destroy(info.truck.gameObject);
                info.truck = null;
                removeList.Add(info);
            }
            else
            {
                info.truck.position += moveAngle * moveSpeed * Time.deltaTime;
            }
        }
    }

    private void PutOnMove(TruckInfo info)
    {
        players.GetChild(info.revivalPlayerIndex).GetChild(0).position = info.truck.position + new Vector3(-1.0f, 0.0f, 0.0f);
    }

    private void AllPutOnMove(TruckInfo info)
    {
        for (int i = 0; i < players.childCount; i++)
        {
            players.GetChild(i).GetChild(0).position = info.truck.position - new Vector3(i * 0.666f, 0.0f, 0.0f);
        }
    }

    private void JumpAction(TruckInfo info)
    {
        float rate = (jumpTime - info.JumpTimeCount) / jumpTime;
        if (info.JumpParam.Count > 1)   { AllJumpMove(info, rate); }
        else                            { JumpMove(info, rate); }
        info.JumpTimeCount -= Time.deltaTime;

        if (info.JumpTimeCount <= 0.0f)
        {
            info.JumpTimeCount = 0.0f;
            info.IsJump = false;
            if (info.JumpParam.Count > 1)   { AllRevival(info); }
            else                            { Revival(info); }
        }
    }

    private void JumpMove(TruckInfo info, float rate)
    {
        players.GetChild(info.revivalPlayerIndex).GetChild(0).position = CalcLarpPoint(info.JumpParam[0].Start, info.JumpParam[0].Half, info.JumpParam[0].End, rate);

    }

    private void AllJumpMove(TruckInfo info, float rate)
    {
        for (int i = 0; i < players.childCount; i++)
        {
            players.GetChild(i).GetChild(0).position = CalcLarpPoint(info.JumpParam[i].Start, info.JumpParam[i].Half, info.JumpParam[i].End, rate);
        }
    }

    private void Revival(TruckInfo info)
    {
        Transform trans = players.GetChild(info.revivalPlayerIndex);
        trans.GetComponent<PlayerController>().Revival();
        trans.GetComponent<PlayerInvincible>().SetInvincible();
        players.GetChild(info.revivalPlayerIndex).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = defaultSortLayer;
    }

    private void AllRevival(TruckInfo info)
    {
        for (int i = 0; i < players.childCount; i++)
        {
            Transform trans = players.GetChild(i);
            trans.GetComponent<PlayerController>().Revival();
            trans.GetComponent<PlayerInvincible>().SetInvincible();
            players.GetChild(i).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = defaultSortLayer;
        }
    }

    private void StartJump(TruckInfo info)
    {
        int managerId = (int)BattleSumoModeManagerList.BattleSumoModeManagerId.BattleSumoManager;
        int stageId = GameObject.FindGameObjectWithTag("Managers").transform.GetChild(managerId).GetComponent<BattleSumoManager>().StageId;
        int playerNumId = players.childCount - 2;
        FirstPosition jumpTransInfo = firstPositionDataBase.BattleStageInfos[stageId].FirstPositions[playerNumId];

        if (info.JumpParam.Count > 1)   { SetAllJumpParam(info, stageId, jumpTransInfo); }
        else                            { SetJumpParam(info, stageId, jumpTransInfo); }
    }

    private void SetJumpParam(TruckInfo info, int stageId, FirstPosition jumpTransInfo)
    {
        info.IsJump = true;
        info.JumpParam[0].Start = players.GetChild(info.revivalPlayerIndex).GetChild(0).position;
        info.JumpParam[0].End = jumpTransInfo.Position[info.revivalPlayerIndex];
        info.JumpParam[0].Half = info.JumpParam[0].End - info.JumpParam[0].Start * 0.5f + info.JumpParam[0].Start;
        info.JumpParam[0].Half.y = info.JumpParam[0].End.y + firstPositionDataBase.BattleStageInfos[stageId].JumpRerativeHeight;
        info.JumpTimeCount = jumpTime;
    }

    private void SetAllJumpParam(TruckInfo info, int stageId, FirstPosition jumpTransInfo)
    {
        info.IsJump = true;
        for (int i = 0; i < players.childCount; i++)
        {
            info.JumpParam[i].Start = players.GetChild(i).GetChild(0).position;
            info.JumpParam[i].End = jumpTransInfo.Position[i];
            info.JumpParam[i].Half = info.JumpParam[i].End - info.JumpParam[i].Start * 0.5f + info.JumpParam[i].Start;
            info.JumpParam[i].Half.y = info.JumpParam[i].End.y + firstPositionDataBase.BattleStageInfos[stageId].JumpRerativeHeight;
        }
        info.JumpTimeCount = jumpTime;
    }

    private Vector2 CalcLarpPoint(Vector2 start, Vector2 half, Vector2 end, float rate)
    {
        Vector2 a = Vector2.Lerp(start, half, rate);
        Vector2 b = Vector2.Lerp(half, end, rate);
        return Vector2.Lerp(a, b, rate);
    }

    public void Spown(int playerIndex)
    {
        Transform truck;
        if (playerIndex == -1)
        {
            truck = Instantiate(firstTruckPrefab, transform.GetChild(2)).transform;
            truckList.Add(new TruckInfo(truck, -1));
            for (int i = 0; i < players.childCount; i++)
            {
                truckList[truckList.Count - 1].JumpParam.Add(new JumpInfo());
                players.GetChild(i).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = firstSpownSortLayer;
            }
        }
        else
        {
            truck = Instantiate(revivalTruckPrefab, transform.GetChild(2)).transform;
            truckList.Add(new TruckInfo(truck, playerIndex));
            truckList[truckList.Count - 1].JumpParam.Add(new JumpInfo());
            players.GetChild(playerIndex).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = revivalSpownSortLayer;
        }
        truck.position = firstPos;
    }
}
