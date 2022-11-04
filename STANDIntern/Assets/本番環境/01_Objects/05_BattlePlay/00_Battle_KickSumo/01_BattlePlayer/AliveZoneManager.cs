using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliveZoneManager : MonoBehaviour
{
    private SpownTruckManager spownTruckManager;

    void Start()
    {

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject playerParent = collision.transform.parent.parent.gameObject;
            int playerId = playerParent.GetComponent<PlayerId>().Id;

            spownTruckManager.SetSpown(playerId);
        }
    }
}
