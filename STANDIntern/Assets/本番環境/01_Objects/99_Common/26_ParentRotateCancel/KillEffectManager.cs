using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEffectManager : MonoBehaviour
{
    void Start()
    {
        transform.localPosition = new Vector3(0.0f, -0.5f, 0.0f);
    }

    void Update()
    {
        transform.rotation = Quaternion.identity * Quaternion.AngleAxis(90.0f, Vector3.right);
        transform.position = transform.parent.position + new Vector3(0.0f, -0.5f, 0.0f);
    }
}
