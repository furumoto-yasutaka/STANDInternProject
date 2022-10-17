using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private Transform PlayerTrans;

    void Start()
    {
        //PlayerTrans = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        transform.position = PlayerTrans.position + new Vector3(0.0f, 0.0f, -10.0f);
    }
}
