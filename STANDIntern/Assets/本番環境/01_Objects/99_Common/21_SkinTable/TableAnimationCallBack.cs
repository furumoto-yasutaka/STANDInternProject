using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableAnimationCallBack : MonoBehaviour
{
    private Animator animator;
    private SkinSelectWindow skinSelectWindow;

    private void Start()
    {
        animator = GetComponent<Animator>();
        skinSelectWindow = GetComponent<SkinSelectWindow>();
    }

    public void ChangeCompleteCallBack()
    {
        animator.SetBool("IsChange", false);
        skinSelectWindow.EndSelectAnimation();
    }
}
