using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMove : MonoBehaviour
{
    [SerializeField]
    private Vector2 moveRange;
    [SerializeField]
    private float moveSpeed;
    private Vector2 movePos = Vector2.zero;
    private float moveThreshold = 0.2f;

    void FixedUpdate()
    {
        Vector2 distance = moveRange - movePos;
        Vector2 move;

        if (distance.sqrMagnitude <= moveThreshold * moveThreshold)
        {
            move = distance;
            movePos = Vector2.zero;
            moveRange *= -1;
        }
        else
        {
            move = distance * moveSpeed;
            movePos += move;
        }

        transform.GetChild(0).localPosition += (Vector3)move;
        transform.GetChild(1).localPosition -= (Vector3)move;
    }
}
