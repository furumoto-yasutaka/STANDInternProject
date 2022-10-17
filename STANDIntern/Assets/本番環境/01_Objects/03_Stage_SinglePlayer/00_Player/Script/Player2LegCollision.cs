using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2LegCollision : MonoBehaviour
{
    [SerializeField]
    private Player2Controller playerController;

    void Start()
    {
        //playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        playerController.KickAddForce();
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.transform.parent.GetComponent<PlayerController>().KickAddForce(collision.transform.position - transform.position);
        }
    }
}
