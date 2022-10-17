using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputGroup
{
    public string Name;
    public bool IsCanInput;
    public InputGroupParent Parent;
    public InputLockElement[] Elemants;
}

public class InputLockManager : MonoBehaviour
{
    [SerializeField]
    private InputGroup[] inputGroupsInspector;
    private Dictionary<string, InputGroup> inputGroups = new Dictionary<string, InputGroup>();

    void Start()
    {
        for (int i = 0; i < inputGroupsInspector.Length; i++)
        {
            inputGroups.Add(inputGroupsInspector[i].Name, inputGroupsInspector[i]);

            inputGroupsInspector[i].Parent.SetLockInputParam(this, inputGroupsInspector[i]);
            foreach (InputLockElement elem in inputGroupsInspector[i].Elemants)
            {
                elem.SetLockInputParam(inputGroupsInspector[i].Parent);
            }
        }
    }

    public void LockInputGroup(string name, bool childLock = false)
    {
        inputGroups[name].IsCanInput = false;

        if (childLock)
        {
            foreach (InputLockElement elem in inputGroups[name].Elemants)
            {
                elem.IsCanInput = false;
            }
        }
    }

    public void UnlockInputGroup(string name, bool childUnlock = false)
    {
        inputGroups[name].IsCanInput = true;

        if (childUnlock)
        {
            foreach (InputLockElement elem in inputGroups[name].Elemants)
            {
                elem.IsCanInput = true;
            }
        }
    }

    public void LockAll(bool childLock = false)
    {
        foreach (KeyValuePair<string, InputGroup> pair in inputGroups)
        {
            pair.Value.IsCanInput = false;

            if (childLock)
            {
                foreach (InputLockElement elem in pair.Value.Elemants)
                {
                    elem.IsCanInput = false;
                }
            }
        }
    }

    public void UnlockAll(bool childUnlock = false)
    {
        foreach (KeyValuePair<string, InputGroup> pair in inputGroups)
        {
            pair.Value.IsCanInput = true;

            if (childUnlock)
            {
                foreach (InputLockElement elem in pair.Value.Elemants)
                {
                    elem.IsCanInput = true;
                }
            }
        }
    }
}
