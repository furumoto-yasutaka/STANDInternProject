using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopPlayerLegCollision : MonoBehaviour
{
    [SerializeField]
    private CoopPlayerController playerController;

    void Start()
    {
        //playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        playerController.KickAddForce();
    }
}
