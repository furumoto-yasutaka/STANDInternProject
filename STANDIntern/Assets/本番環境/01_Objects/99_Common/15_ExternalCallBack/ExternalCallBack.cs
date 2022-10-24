using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExternalCallBack : MonoBehaviour
{
    [SerializeField]
    private UnityEvent callback;

    public void ExternalCall()
    {
        callback.Invoke();
    }
}
