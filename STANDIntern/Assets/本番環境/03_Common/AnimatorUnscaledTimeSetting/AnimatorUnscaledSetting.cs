using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorUnscaledSetting : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void SetBgm()
    {
        AudioManager.Instance.StopBgm();
        AudioManager.Instance.PlayBgm("ƒŠƒUƒ‹ƒg", true);
    }
}
