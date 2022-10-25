using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorUnscaledSetting : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
    }
}
