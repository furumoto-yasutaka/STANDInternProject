using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdSetter : MonoBehaviour
{
    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<PlayerId>().SetId(i);
        }
    }
}
