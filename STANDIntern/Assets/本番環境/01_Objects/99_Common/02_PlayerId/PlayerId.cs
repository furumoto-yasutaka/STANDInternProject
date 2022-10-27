using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerId : MonoBehaviour
{
    [SerializeField]
    private int id;
    public int Id { get { return id; } }

    public void SetId(int index)
    {
        id = index;
    }
}
