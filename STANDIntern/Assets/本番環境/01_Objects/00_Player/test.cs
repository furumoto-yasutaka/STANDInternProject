using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField]
    private Vector2 angle;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.GetKeyDown(InputManager.KeyIdList.Mouse_LB))
        {
            rb.AddForceAtPosition(angle, transform.position + new Vector3(2.5f, 0.0f), ForceMode2D.Impulse);
        }
    }
}
