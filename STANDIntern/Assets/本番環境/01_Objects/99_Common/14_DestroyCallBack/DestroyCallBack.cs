using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCallBack : MonoBehaviour
{
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
