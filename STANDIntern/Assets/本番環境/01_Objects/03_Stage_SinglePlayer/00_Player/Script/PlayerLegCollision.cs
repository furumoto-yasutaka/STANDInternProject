using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegCollision : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    private List<Collider2D> playerList = new List<Collider2D>();

    private void OnEnable()
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerController.transform.GetChild(0).GetChild(0).GetComponent<Collider2D>());
    }

    private void OnDisable()
    {
        playerList.Clear();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        playerController.KickAddForce();
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") &&
            !playerList.Contains(collision))
        {
            playerList.Add(collision);
            if (!collision.transform.parent.parent.GetComponent<PlayerInvincible>().IsInvincible)
            {
                collision.transform.parent.parent.GetComponent<PlayerController>().KickedAddForce(
                    collision.transform.position - transform.position,
                    playerController.KickDirection);
            }
        }
    }
}
