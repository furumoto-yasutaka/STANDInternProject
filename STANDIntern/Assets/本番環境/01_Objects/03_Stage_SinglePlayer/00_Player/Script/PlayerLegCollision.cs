using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegCollision : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;

    void Start()
    {
        //playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        playerController.KickAddForce();
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player2"))
        {
            collision.transform.parent.GetComponent<Player2Controller>().KickAddForce(collision.transform.position - transform.position);
        }
    }
}
