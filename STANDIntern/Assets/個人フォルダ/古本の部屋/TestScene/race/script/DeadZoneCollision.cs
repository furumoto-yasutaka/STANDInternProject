using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZoneCollision : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent.parent.gameObject.SetActive(false);
        }
    }
}
