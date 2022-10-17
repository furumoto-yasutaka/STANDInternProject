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
        animator.SetBool("Open", true);
        animator.SetBool("Close", false);
    }

    public virtual void Close()
    {
        animator.SetBool("Open", false);
        animator.SetBool("Close", true);
    }

    public virtual void CloseComplete()
    {
        gameObject.SetActive(false);
        animator.SetBool("Close", false);

        if (closeCompleteCallBack != null)
        {
            closeCompleteCallBack.Invoke();
        }
    }
}
