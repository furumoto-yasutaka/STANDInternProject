using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivalInfo
{
    public PlayerController playerController;
    public float revivalTimeCount;

    public RevivalInfo(PlayerController playerController, float revivalTimeCount)
    {
        this.playerController = playerController;
        this.revivalTimeCount = revivalTimeCount;
    }
}

public class RevivalManager : MonoBehaviour
{
    [SerializeField]
    private GameObject players;
    [SerializeField]
    private List<RevivalInfo> playerList = new List<RevivalInfo>();
    [SerializeField]
    private float revivalTime = 2.0f;
    private BattleSumoManager battleSumoManager;

    void Start()
    {
        for (int i = 0; i < players.transform.childCount; i++)
        {
            playerList.Add(new RevivalInfo(players.transform.GetChild(i).GetComponent<PlayerController>(), 0.0f));
        }
        battleSumoManager = transform.parent.GetChild((int)BattleSumoModeManagerList.BattleSumoModeManagerId.BattleSumoManager).GetComponent<BattleSumoManager>();
    }

    void Update()
    {
        foreach (RevivalInfo info in playerList)
        {
            if (info.playerController.IsDeath)
            {
                if (info.revivalTimeCount <= 0.0f)
                {
                    // •œŠˆˆ—
                    info.revivalTimeCount = 0.0f;
                    info.playerController.transform.GetChild(0).localPosition = Vector3.zero;
                    info.playerController.Revival();
                    info.playerController.GetComponent<PlayerInvincible>().SetInvincible();
                }
                else
                {
                    info.revivalTimeCount -= Time.deltaTime;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject obj = collision.transform.parent.parent.gameObject;
            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i].playerController.gameObject == obj)
                {
                    playerList[i].playerController.Death();
                    playerList[i].revivalTimeCount = revivalTime;
                    battleSumoManager.CalcPoint_DeathPlayer(i);
                    break;
                }
            }
        }
    }
}
