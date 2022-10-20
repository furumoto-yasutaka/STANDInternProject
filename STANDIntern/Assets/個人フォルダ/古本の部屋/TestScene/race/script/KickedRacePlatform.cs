using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickedRacePlatform : MonoBehaviour
{
    [SerializeField]
    private float firstMoveSpeed = 2.0f;
    [SerializeField]
    private float addMoveSpeed = 1.0f;
    private new Rigidbody2D rigidbody2D;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 pos = transform.position;
        pos.x -= firstMoveSpeed * Time.deltaTime;
        rigidbody2D.MovePosition(pos);

        firstMoveSpeed += addMoveSpeed * Time.deltaTime;
    }
}
