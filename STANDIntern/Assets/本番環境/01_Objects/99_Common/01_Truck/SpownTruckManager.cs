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
    [SerializeField]
    private float invincibleTime = 2.0f;
    
    private Vector3 firstTruckInitialPos;
    private Vector3 firstTruckTargetPos;
    private Vector3 respownTruckInitialPos;
    private Vector3 respownTruckTargetPos;
    private Vector3 moveAngle;
    private List<TruckInfo> truckList = new List<TruckInfo>();
    private List<TruckInfo> removeList = new List<TruckInfo>();

    void Start()
    {
        firstTruckInitialPos = transform.GetChild(0).position;
        firstTruckTargetPos = transform.GetChild(1).position;
        respownTruckInitialPos = transform.GetChild(2).position;
        respownTruckTargetPos = transform.GetChild(3).position;
        moveAngle = (firstTruckTargetPos - firstTruckInitialPos).normalized;

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
            float toHalfDistance;

            if (info.JumpParam.Count > 1)
            {
                toHalfDistance = ((firstTruckInitialPos + firstTruckTargetPos) * 0.5f - info.truck.position).sqrMagnitude;
            }
            else
            {
                toHalfDistance = ((respownTruckInitialPos + respownTruckTargetPos) * 0.5f - info.truck.position).sqrMagnitude;
            }

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
            float toEndDistance;

            if (info.JumpParam.Count > 1)
            {
                toEndDistance = (firstTruckTargetPos - info.truck.position).sqrMagnitude;
            }
            else
            {
                toEndDistance = (respownTruckTargetPos - info.truck.position).sqrMagnitude;
            }

            if (toEndDistance <= (moveSpeed * Time.deltaTime) * (moveSpeed * Time.deltaTime))
            {
                info.truck.position += moveAngle * toEndDistance;
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
            if (!BattleSumoManager.IsPlayerJoin[i]) { continue; }

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
        int playerCnt = 0;
        for (int i = 0; i < players.childCount; i++)
        {
            if (!BattleSumoManager.IsPlayerJoin[i]) { continue; }

            players.GetChild(i).GetChild(0).position = CalcLarpPoint(info.JumpParam[playerCnt].Start, info.JumpParam[playerCnt].Half, info.JumpParam[playerCnt].End, rate);
            playerCnt++;
        }
    }

    private void Revival(TruckInfo info)
    {
        Transform trans = players.GetChild(info.revivalPlayerIndex);
        trans.GetComponent<PlayerController>().Revival();
        trans.GetComponent<PlayerInvincible>().SetInvincible(invincibleTime);
        players.GetChild(info.revivalPlayerIndex).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = defaultSortLayer;
    }

    private void AllRevival(TruckInfo info)
    {
        for (int i = 0; i < players.childCount; i++)
        {
            if (!BattleSumoManager.IsPlayerJoin[i]) { continue; }

            Transform trans = players.GetChild(i);
            trans.GetComponent<PlayerController>().Revival();
            trans.GetComponent<PlayerInvincible>().SetInvincible(invincibleTime);
            players.GetChild(i).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = defaultSortLayer;
        }
    }

    private void StartJump(TruckInfo info)
    {
        int managerId = (int)BattleSumoModeManagerList.BattleSumoModeManagerId.BattleSumoManager;
        int stageId = GameObject.FindGameObjectWithTag("Managers").transform.GetChild(managerId).GetComponent<BattleSumoManager>().StageId;

        int playerNumId = players.childCount - 2;
        FirstPosition jumpTransInfo = firstPositionDataBase.BattleStageInfos[stageId].FirstPositions[playerNumId];

        if (info.JumpParam.Count > 1)
        {
            SetAllJumpParam(info, stageId, jumpTransInfo);
            EffectContainer.Instance.PlayEffect("スポーン", info.truck.position + new Vector3(-1.0f, 0.0f, 0.0f));
        }
        else
        {
            SetJumpParam(info, stageId, jumpTransInfo);
            EffectContainer.Instance.PlayEffect("リスポーン", info.truck.position + new Vector3(-1.0f, 0.0f, 0.0f));
            AudioManager.Instance.PlaySe("リスポーン時の音");
        }
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
        int playerCnt = 0;
        for (int i = 0; i < players.childCount; i++)
        {
            if (!BattleSumoManager.IsPlayerJoin[i]) { continue; }

            info.JumpParam[playerCnt].Start = players.GetChild(i).GetChild(0).position;
            info.JumpParam[playerCnt].End = jumpTransInfo.Position[playerCnt];
            info.JumpParam[playerCnt].Half = info.JumpParam[playerCnt].End - info.JumpParam[playerCnt].Start * 0.5f + info.JumpParam[playerCnt].Start;
            info.JumpParam[playerCnt].Half.y = info.JumpParam[playerCnt].End.y + firstPositionDataBase.BattleStageInfos[stageId].JumpRerativeHeight;

            playerCnt++;
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
            truck = Instantiate(firstTruckPrefab, transform.GetChild(4)).transform;
            truckList.Add(new TruckInfo(truck, -1));
            for (int i = 0; i < players.childCount; i++)
            {
                if (!BattleSumoManager.IsPlayerJoin[i]) { continue; }

                truckList[truckList.Count - 1].JumpParam.Add(new JumpInfo());
                players.GetChild(i).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = firstSpownSortLayer;
            }
            truck.position = firstTruckInitialPos;
        }
        else
        {
            truck = Instantiate(revivalTruckPrefab, transform.GetChild(4)).transform;
            truckList.Add(new TruckInfo(truck, playerIndex));
            truckList[truckList.Count - 1].JumpParam.Add(new JumpInfo());
            players.GetChild(playerIndex).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = revivalSpownSortLayer;
            truck.position = respownTruckInitialPos;
            AudioManager.Instance.PlaySe("クラクション");
        }
    }
}
