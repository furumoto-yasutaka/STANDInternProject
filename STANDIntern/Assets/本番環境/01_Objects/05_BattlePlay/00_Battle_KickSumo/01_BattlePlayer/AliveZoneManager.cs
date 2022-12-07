using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliveZoneManager : MonoBehaviour
{
    [SerializeField]
    private SpownTruckManager spownTruckManager;

    void Start()
    {

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject playerParent = collision.transform.parent.parent.gameObject;
            int playerId = playerParent.GetComponent<PlayerInfo>().Id;
            playerParent.GetComponent<PlayerController>().Death();
            playerParent.GetComponent<PlayerBattleSumoPoint>().CalcPoint_Death();

            spownTruckManager.SetSpown(playerId);
        }
    }
}
