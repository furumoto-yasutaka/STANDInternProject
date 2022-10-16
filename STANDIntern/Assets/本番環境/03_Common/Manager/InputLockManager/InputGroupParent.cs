using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputGroupParent : MonoBehaviour
{
    protected InputGroup MyGroup;
    protected InputLockManager inputLockManager;

    public bool GetIsCanInput()
    {
        return MyGroup.IsCanInput;
    }

    public void SetLockInputParam(InputLockManager manager, InputGroup group)
    {
        inputLockManager = manager;
        MyGroup = group;
    }

    public void LockInputMyGroup()
    {
        inputLockManager.LockInputGroup(MyGroup.Name);
    }

    public void UnlockInputMyGroup()
    {
        inputLockManager.UnlockInputGroup(MyGroup.Name);
    }
}
