using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopPlayer2LegCollision : MonoBehaviour
{
    [SerializeField]
    private CoopPlayer2Controller playerController;

    void Start()
    {
        //playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        playerController.KickAddForce();
    }
}
