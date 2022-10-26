using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableAnimationCallBack : MonoBehaviour
{
    private Animator animator;
    private SkinSelectManager skinSelectManager;

    private void Start()
    {
        animator = GetComponent<Animator>();
        skinSelectManager = transform.parent.parent.GetComponent<SkinSelectManager>();
    }

    public void ChangeCompleteCallBack()
    {
        animator.SetBool("IsChange", false);
        skinSelectManager.EndSelectAnimation(transform.parent.GetComponent<PlayerId>().Id);
    }
}
