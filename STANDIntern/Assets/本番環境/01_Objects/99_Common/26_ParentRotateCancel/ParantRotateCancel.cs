using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParantRotateCancel : MonoBehaviour
{
    private Quaternion firstRotate;

    void Start()
    {
        firstRotate = transform.localRotation;
    }

    void Update()
    {
        transform.localRotation = firstRotate * Quaternion.Inverse(transform.parent.rotation);
    }
}
