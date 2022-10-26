using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigButtonBackScroll : MonoBehaviour
{
    public static Vector2 ScrollSpeed = new Vector2(-0.1f, -0.1f);
    private bool isScroll = false;
    private Material material;
    [SerializeField]
    private Vector2 firstOffset;

    void Start()
    {
        material = transform.GetComponent<Image>().material;
        material.SetTextureOffset("_MainTex", firstOffset);
    }

    void FixedUpdate()
    {
        if (isScroll)
        {
            Vector2 newTiling = material.GetTextureOffset("_MainTex") + ScrollSpeed * Time.deltaTime;
            material.SetTextureOffset("_MainTex", newTiling);
        }
    }

    public void StartScroll()
    {
        isScroll = true;
    }

    public void StopScroll()
    {
        isScroll = false;
    }

    private void OnApplicationQuit()
    {
        material.SetTextureOffset("_MainTex", firstOffset);
    }
}
