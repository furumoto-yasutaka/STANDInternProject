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

public class UnlockList
{
    public string Name;
    public bool ChildLock;

    public UnlockList(string name, bool childUnlock)
    {
        Name = name;
        ChildLock = childUnlock;
    }
}

public class InputLockManager : MonoBehaviour
{
    [SerializeField]
    private InputGroup[] inputGroupsInspector;
    private Dictionary<string, InputGroup> inputGroups = new Dictionary<string, InputGroup>();
    [SerializeField]
    private List<UnlockList> inputUnlockList = new List<UnlockList>();

    void Awake()
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

    void Update()
    {
        foreach (UnlockList unlock in inputUnlockList)
        {
            inputGroups[unlock.Name].IsCanInput = true;

            if (unlock.ChildLock)
            {
                foreach (InputLockElement elem in inputGroups[name].Elemants)
                {
                    elem.IsCanInput = true;
                }
            }
        }
        inputUnlockList.Clear();
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
        inputUnlockList.Add(new UnlockList(name, childUnlock));
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
            inputUnlockList.Add(new UnlockList(pair.Key, childUnlock));
        }
    }
}
