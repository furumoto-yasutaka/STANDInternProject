using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleCountDown : MonoBehaviour
{
    private TextMeshProUGUI second;
    private Animator animator;
    [SerializeField]
    private int count;

    public int Count { get { return count; } }

    void Start()
    {
        second = GetComponent<TextMeshProUGUI>();
        animator = GetComponent<Animator>();
        second.text = count.ToString();
    }

    public void NextNumber()
    {
        count--;
        second.text = count.ToString();
    }

    public void CheckStop()
    {
        if (count == 1)
        {
            animator.SetBool("Stop", true);
        }
    }
}
