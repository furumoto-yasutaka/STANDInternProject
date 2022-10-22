using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputLockElement : MonoBehaviour
{
    private bool isCanInput = true;
    protected InputGroupParent myGroupParent;

    public bool IsCanInput
    {
        get { return myGroupParent.GetIsCanInput() && isCanInput; }
        set { isCanInput = value; }
    }

    public void SetLockInputParam(InputGroupParent parent)
    {
        myGroupParent = parent;
    }

    public virtual void LockInput()
    {
        isCanInput = false;
    }

    public virtual void UnlockInput()
    {
        isCanInput = true;
    }

    //public virtual void LockInputMyGroup()
    //{
    //    myGroupParent.LockInputMyGroup();
    //}

    //public virtual void UnlockInputMyGroup()
    //{
    //    myGroupParent.UnlockInputMyGroup();
    //}
}
