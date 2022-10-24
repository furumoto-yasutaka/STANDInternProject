using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NormalWindow : InputGroupParent
{
    private Animator animator;
    [SerializeField]
    private UnityEvent closeCompleteCallBack = null;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void Open()
    {
        gameObject.SetActive(true);
        animator.SetBool("IsClose", false);
    }

    public virtual void Close()
    {
        animator.SetBool("IsClose", true);
    }

    public virtual void CloseComplete()
    {
        gameObject.SetActive(false);

        if (closeCompleteCallBack != null)
        {
            closeCompleteCallBack.Invoke();
        }
    }
}
