using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivalInfo
{
    public GameObject player;
    public bool death;
    public float revivalTimeCount;

    public RevivalInfo(GameObject player, bool death, float revivalTimeCount)
    {
        this.player = player;
        this.death = death;
        this.revivalTimeCount = revivalTimeCount;
    }
}

public class DeathRevivalManager : MonoBehaviour
{
    [SerializeField]
    private GameObject players;
    [SerializeField]
    private List<RevivalInfo> playerList = new List<RevivalInfo>();
    [SerializeField]
    private float revivalTime = 2.0f;

    void Start()
    {
        for (int i = 0; i < players.transform.childCount; i++)
        {
            playerList.Add(new RevivalInfo(players.transform.GetChild(i).gameObject, false, 0.0f));
        }
    }

    void Update()
    {
        foreach (RevivalInfo info in playerList)
        {
            if (info.death)
            {
                if (info.revivalTimeCount <= 0.0f)
                {
                    // •œŠˆˆ—
                    info.death = false;
                    info.revivalTimeCount = 0.0f;
                    info.player.transform.GetChild(0).localPosition = Vector3.zero;
                    info.player.SetActive(true);
                    info.player.GetComponent<PlayerInvincible>().SetInvincible();
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
            foreach (RevivalInfo info in playerList)
            {
                if (info.player == obj)
                {
                    info.player.SetActive(false);
                    info.death = true;
                    info.revivalTimeCount = revivalTime;
                    break;
                }
            }
        }
    }
}
