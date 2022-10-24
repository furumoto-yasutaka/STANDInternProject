using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigButtonBackScrollMessager : MonoBehaviour
{
    public static Vector2 ScrollSpeed = new Vector2(-0.1f, -0.1f);
    private BigButtonBackScroll back;

    void Start()
    {
        back = transform.GetChild(0).GetComponent<BigButtonBackScroll>();
    }

    public void StartScroll()
    {
        back.StartScroll();
    }

    public void StopScroll()
    {
        back.StopScroll();
    }
}
