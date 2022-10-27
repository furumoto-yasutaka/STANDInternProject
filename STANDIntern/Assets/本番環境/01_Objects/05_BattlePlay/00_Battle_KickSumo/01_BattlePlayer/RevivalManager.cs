using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivalInfo
{
    public PlayerController playerController;
    public bool isSpown;
    public float revivalTimeCount;

    public RevivalInfo(PlayerController playerController, bool isSpown, float revivalTimeCount)
    {
        this.playerController = playerController;
        this.isSpown = isSpown;
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
            playerList.Add(new RevivalInfo(players.transform.GetChild(i).GetComponent<PlayerController>(), true, 0.0f));
        }
        battleSumoManager = transform.parent.GetChild((int)BattleSumoModeManagerList.BattleSumoModeManagerId.BattleSumoManager).GetComponent<BattleSumoManager>();
    }

    void Update()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (!BattleSumoManager.IsPlayerJoin[i]) { continue; }

            if (playerList[i].playerController.IsDeath && !playerList[i].isSpown)
            {
                if (playerList[i].revivalTimeCount <= 0.0f)
                {
                    // •œŠˆˆ—
                    playerList[i].revivalTimeCount = 0.0f;
                    int managerId = (int)BattleSumoModeManagerList.BattleSumoModeManagerId.SpownManager;
                    SpownTruckManager manager = GameObject.FindGameObjectWithTag("Managers").transform.GetChild(managerId).GetComponent<SpownTruckManager>();
                    manager.Spown(playerList[i].playerController.transform.GetComponent<PlayerId>().Id);
                    playerList[i].isSpown = true;
                }
                else
                {
                    playerList[i].revivalTimeCount -= Time.deltaTime;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject playerParent = collision.transform.parent.parent.gameObject;
            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i].playerController.gameObject == playerParent)
                {
                    playerList[i].playerController.Death();
                    playerList[i].isSpown = false;
                    playerList[i].revivalTimeCount = revivalTime;
                    battleSumoManager.CalcPoint_DeathPlayer(playerList[i].playerController.PlayerId);
                    break;
                }
            }
        }
    }
}
