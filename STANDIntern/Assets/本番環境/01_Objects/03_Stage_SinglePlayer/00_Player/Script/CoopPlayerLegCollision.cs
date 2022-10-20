using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopPlayerLegCollision : MonoBehaviour
{
    [SerializeField]
    private Transform playerParent;
    [SerializeField]
    private CoopPlayerController playerController;

    private void OnEnable()
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerParent.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerParent.GetChild(0).GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerParent.GetChild(1).GetComponent<Collider2D>());
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        playerController.KickAddForce();
    }
}
